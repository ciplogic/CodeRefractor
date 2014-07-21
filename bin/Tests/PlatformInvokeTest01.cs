using System;
using System.Runtime.InteropServices;
class PlatformInvokeTest01 {
    
    [DllImport("user32.dll")]
    private static extern int MessageBoxW(System.IntPtr hWnd, string lpText, string lpCaption, System.UInt32 uType);
    //We use wide chars only
    static void Main() {
        MessageBoxW(System.IntPtr.Zero, "Hello, world!", "Test", 64);
    }
}