namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public static class MidRepresentationUtils
    {
        public static object GetAdditionalProperty(this MetaMidRepresentation intermediateCode, string itemName)
        {
            if (intermediateCode == null)
                return null;
            var additionalData = intermediateCode.AuxiliaryObjects;

            object itemValue;
            return !additionalData.TryGetValue(itemName, out itemValue) ? null : itemValue;
        }

        public static bool ReadAdditionalBool(this MetaMidRepresentation intermediateCode, string itemName)
        {
            var result = intermediateCode.GetAdditionalProperty(itemName);
            return result != null && (bool) result;
        }

        public static void SetAdditionalValue(this MetaMidRepresentation intermediateCode, string itemName,
            object valueToSet)
        {
            var additionalData = intermediateCode.AuxiliaryObjects;
            additionalData[itemName] = valueToSet;
        }
    }
}