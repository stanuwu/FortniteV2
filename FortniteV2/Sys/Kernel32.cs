using System;
using System.Runtime.InteropServices;

namespace FortniteV2.Sys
{
    public static class Kernel32
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] [MarshalAs(UnmanagedType.AsAny)] object lpBuffer, int dwSize,
            out int lpNumberOfBytesRead);
    }
}