#region Usings

using System.Reflection;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class NewConstructedObject : IdentifierValue
    {
        private readonly ConstructorInfo _constructorInfo;

        public ConstructorInfo Info
        {
            get { return _constructorInfo; }
        }

        public NewConstructedObject(ConstructorInfo constructorInfo)
        {
            _constructorInfo = constructorInfo;
        }
    }
}