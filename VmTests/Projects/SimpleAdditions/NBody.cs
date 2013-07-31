#region Usings

using System;

#endregion

class Item
{
    private int _itemType;

    public Item(int itemType)
    {
        _itemType = itemType;
    }

    public void SetItem(ref int item)
    {
        item = _itemType;
    }
}

internal class NBody
{
    public static void Main()
    {
        var item = new Item(8);
        int data = 3;
        item.SetItem(ref data);
        Console.WriteLine(data);
    }
}