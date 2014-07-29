#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CodeRefractor.ClosureCompute;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.RuntimeBase.TypeInfoWriter
{
    public class TypeDescriptionTable : IComparer<Type>
    {
        private readonly ClosureEntities _closureEntities;
        private readonly List<Type> _typeClosure;

        private readonly Dictionary<Type, HashSet<Type>> _dictionary = new Dictionary<Type, HashSet<Type>>();

        private readonly Dictionary<Type, int> _result = new Dictionary<Type, int>();

        public TypeDescriptionTable(List<Type> typeClosure, ClosureEntities closureEntities)
        {
            _closureEntities = closureEntities;
            _typeClosure = typeClosure.Where(
                FilterType)
                .ToList();
           

            for (int index = 0; index < _typeClosure.Count; index++)
            {
                var type = _typeClosure[index];
                // Mapped Types should share typeId with the mapping type
                var mapped = type.GetReversedMappedType(closureEntities);
                if (mapped != type)
                {
                    _result[mapped] = index;
                }
                _result[type] = index;
                
            }
        }

        private static bool FilterType(Type type)
        {
            var isStatic = (type.IsAbstract && type.IsSealed);
            return !isStatic;
        }

        private void BuildDependantTypeDictionary()
        {
            foreach (var type in _typeClosure)
            {
                var typeDesc = new TypeDescription(type);

                var layout = typeDesc.Layout
                    .Select(field => field.TypeDescription.GetClrType(_closureEntities))
                    .ToArray();
                var hashSet = new HashSet<Type>(layout);
                _dictionary[type] = hashSet;
            }
        }

        public Dictionary<Type, int> ExtractInformation()
        {
            BuildDependantTypeDictionary();
            _typeClosure.Sort(this);

            _result.Clear();
            var index = 0;
            foreach (var type in _typeClosure)
            {
                _result[type] = index++;
            }
            return _result;
        }

        public int Compare(Type left, Type right)
        {
            var leftLayout = _dictionary[left];
            var rightLayout = _dictionary[right];
            if (leftLayout.Count == 0 && rightLayout.Count == 0)
                return 0;
            if (rightLayout.Contains(left))
                return -1;
            if (leftLayout.Contains(right))
                return 1;
            var countLeft = leftLayout.Count;
            var countRight = rightLayout.Count;
            if (countLeft == countRight) return 0;
            var compare = countLeft - countRight;
            return compare;
        }

        public void SetIdOfInstance(StringBuilder sb, LocalVariable variable, Type type, bool isStack)
        {
            int typeId;
            if (!_result.TryGetValue(type, out typeId))
                throw new InvalidDataException(
                    string.Format(
                        "Type id for type: '{0}' is not defined ", type.ToCppMangling()
                        ));
            if (isStack)
            {
                sb.AppendFormat("{0}._typeId = {1};", variable.Name, typeId);
            }
            else
            {
                sb.AppendFormat("{0}->_typeId = {1};", variable.Name, typeId);
            }
        }

        public bool HasType(Type type)
        {
            
            
            return _dictionary.ContainsKey(type);
            
        }

        public object GetTypeId(Type implementation)
        {
            int result;
            if (!_result.TryGetValue(implementation, out result))
                return -1;
            return result;
        }
    }
}