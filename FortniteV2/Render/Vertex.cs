using System.Runtime.InteropServices;
using SharpDX;

namespace FortniteV2.Render
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vertex
    {
        public Vector4 Position;
        public ColorBGRA Color;
    }
}