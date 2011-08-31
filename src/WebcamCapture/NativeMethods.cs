using System;
using System.Runtime.InteropServices;

namespace WebcamCapture
{
    public static class NativeMethods
    {
        [DllImport("oleaut32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int OleCreatePropertyFrame(
           IntPtr hwndOwner,
           int x,
           int y,
           [MarshalAs(UnmanagedType.LPWStr)] string lpszCaption,
           int cObjects,
           [MarshalAs(UnmanagedType.Interface, ArraySubType = UnmanagedType.IUnknown)]
           ref object ppUnk,
           int cPages,
           IntPtr lpPageClsID,
           int lcid,
           int dwReserved,
           IntPtr lpvReserved);
    }
}
