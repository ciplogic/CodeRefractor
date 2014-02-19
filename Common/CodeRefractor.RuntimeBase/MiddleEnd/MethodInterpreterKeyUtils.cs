namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public static class MethodInterpreterKeyUtils
    {
        public static MethodInterpreterKey ToKey(this MethodInterpreter methodInterpreter)
        {
            var result = new MethodInterpreterKey(methodInterpreter);
            return result;
        }

    }
}