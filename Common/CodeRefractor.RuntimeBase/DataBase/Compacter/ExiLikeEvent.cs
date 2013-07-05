namespace CodeRefractor.RuntimeBase.DataBase
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