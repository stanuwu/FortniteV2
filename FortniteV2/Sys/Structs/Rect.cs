using System.Runtime.InteropServices;

namespace FortniteV2.Sys.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int Left, Top, Right, Bottom;
    }
}