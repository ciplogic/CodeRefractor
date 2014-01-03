using CodeRefractor.RuntimeBase.MiddleEnd;

namespace CodeRefractor.CompilerBackend.Optimizations.Purity
{
    public static class AnalyzeUtils
    {
        private const string PropString = "AnalyzeProperties";
        public static AnalyzeProperties GetProperties(this MetaMidRepresentation intermediateCode)
        {

            if (intermediateCode == null)
                return null;
            var additionalData = intermediateCode.AuxiliaryObjects;

            object isPureData;
            if (additionalData.TryGetValue(PropString, out isPureData))
            {
                return (AnalyzeProperties) isPureData;
            }
            var result = new AnalyzeProperties();
            additionalData[PropString] = result;
            return result;
        }
    }
}