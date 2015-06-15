//Taken From Microsoft Documentation on Virtual Methods
using System;
using System.Runtime.InteropServices;

/**
 * Display a message box imported from an external source.
 */

class SingletonClass
{
    private static SingletonClass singletonInstance = null;

    public static SingletonClass GetInstance()
    {
        if (singletonInstance == null)
        {
            SingletonClass value = new SingletonClass();
            CompareExchange(ref singletonInstance, value);
        }
        return singletonInstance;
    }

    private static void CompareExchange(ref SingletonClass singletonClass, SingletonClass value)
    {
        singletonClass = value;
    }

    public int Value;
}
public class Test
{
    // this must be MessageBoxW, since in CR characters are char_w, and not char.
    [DllImport("User32.dll")]
    public static extern int MessageBoxW(int handle, String message, String title, uint type);

    [STAThread]
    public static void Main()
    {
        var inst = SingletonClass.GetInstance();
        inst.Value = 3;
        Console.WriteLine(inst.Value);
    }
}
