//Taken From Microsoft Documentation on Virtual Methods
using System;
using System.Runtime.InteropServices;

/**
 * Display a message box imported from an external source.
 */
public class Test
{
    [DllImport("User32.dll")]
    public static extern int MessageBox(int handle, String message, String title, uint type);

    [STAThread]
    public static void Main()
    {
        MessageBox(0, "Message", "Title", 0);
    }
}
