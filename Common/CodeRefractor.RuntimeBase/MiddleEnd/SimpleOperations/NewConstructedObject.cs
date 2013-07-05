using System.Reflection;

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