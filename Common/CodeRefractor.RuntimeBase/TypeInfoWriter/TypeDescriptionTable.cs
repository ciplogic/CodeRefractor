#region Uses

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeRefractor.Analyze;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CodeWriter.Output;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.TypeInfoWriter
{
    public class TypeDescriptionTable : IComparer<Type>
    {
        readonly ClosureEntities _closureEntities;
        readonly Dictionary<Type, HashSet<Type>> _dictionary = new Dictionary<Type, HashSet<Type>>();
        readonly Dictionary<Type, int> _result = new Dictionary<Type, int>();
        readonly List<Type> _typeClosure;

        public TypeDescriptionTable(List<Type> typeClosure, ClosureEntities closureEntities)
        {
            _closureEntities = closureEntities;
            _typeClosure = typeClosure.Where(
                FilterType)
                .ToList();


            for (var index = 0; index < _typeClosure.Count; index++)
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

        static bool FilterType(Type type)
        {
            var isStatic = (type.IsAbstract && type.IsSealed);
            return !isStatic;
        }

        void BuildDependantTypeDictionary()
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

        public void SetIdOfInstance(CodeOutput sb, LocalVariable variable, Type type, bool isStack)
        {
            int typeId;
            if (!_result.TryGetValue(type, out typeId))
                throw new InvalidDataException(
                    $"Type id for type: '{type.ToCppMangling()}' is not defined ");
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

        public int GetTypeId(Type implementation)
        {
            int result;
            if (!_result.TryGetValue(implementation, out result))
                return -1;
            return result;
        }
    }
}