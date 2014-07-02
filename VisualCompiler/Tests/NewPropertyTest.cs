//Multiple issues with this test, TODO: Ciprian can you have a look at this ?
public class Employee
{
    private string name;
    public string Name
    {
        get { return name; }
        set { name = value; }
    }
}

public class Manager : Employee
{
    private string name;

    // Notice the use of the new modifier: 
    public new string Name
    {
        get { return name; }
        set { name = ", Manager"; } //This is very problematic
    }
}

class TestHiding
{
    static void Main()
    {
        Manager m1 = new Manager();

        // Derived class property.
        m1.Name = "John";

        // Base class property.
        ((Employee)m1).Name = "Mary";

        System.Console.Write("Name in the derived class is: {0}");
        System.Console.WriteLine(m1.Name);
        System.Console.Write("Name in the base class is: {0}");
        System.Console.WriteLine(((Employee)m1).Name);
    }
}