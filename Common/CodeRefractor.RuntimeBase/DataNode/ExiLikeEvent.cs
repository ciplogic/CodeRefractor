namespace CodeRefractor.DataNode
{
    public enum ExiLikeEvent
    {
        None = 0,
        CreateElement,
        ExistingElement,
        CreateAttribute,
        ExistingAttribute,
        ExistingAttributeValue,
        CreateString,
        CreateExistingString,
        EndElement,
        CreateAttributeValue,
        CreateText,
        ExistingText
    }
}