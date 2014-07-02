//http://www.dotnetperls.com/interface

using System;

interface IValue
{
    int Count { get; set; } // Property interface
    string Name { get; set; } // Property interface
}

class Image : IValue // Implements interface
{
    public int Count // Property implementation
    {
        get;
        set;
    }

    string _name;

    public string Name // Property implementation
    {
        get { return this._name; }
        set { this._name = value; }
    }
}

class Article : IValue // Implements interface
{
    public int Count // Property implementation
    {
        get;
        set;
    }

    string _name;

    public string Name // Property implementation
    {
        get { return this._name; }
        set { this._name = value; } // .ToUpper(); } TODO: ToUpper brings in a lot of other dependencies
    }
}

class Program
{
    static void Main()
    {
        IValue value1 = new Image();
        IValue value2 = new Article();

        value1.Count++; // Access int property on interface
        value2.Count++; // Increment

        value1.Name = "Mona Lisa"; // Use setter on interface
        value2.Name = "Resignation"; // Set

        Console.WriteLine(value1.Name); // Use getter on interface
        Console.WriteLine(value2.Name); // Get
    }
}