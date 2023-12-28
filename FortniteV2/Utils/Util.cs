using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using FortniteV2.Data.Raw;
using FortniteV2.Sys;
using Point = FortniteV2.Sys.Structs.Point;

namespace FortniteV2.Utils
{
    public static class Util
    {
        public static Rectangle GetClientRectangle(IntPtr handle)
        {
            if (User32.ClientToScreen(handle, out var point) && User32.GetClientRect(handle, out var rect))
                return new Rectangle(point.X, point.Y, rect.Right - rect.Left, rect.Bottom - rect.Top);

            return default;
        }

        public static Module GetModule(this Process process, string moduleName)
        {
            var processModule = process.GetProcessModule(moduleName);
            if (processModule is null || processModule.BaseAddress == IntPtr.Zero) return default;

            return new Module(process, processModule);
        }

        public static ProcessModule GetProcessModule(this Process process, string moduleName)
        {
            return process?.Modules.OfType<ProcessModule>().FirstOrDefault(a => string.Equals(a.ModuleName.ToLower(), moduleName.ToLower()));
        }

        public static bool IsRunning(this Process process)
        {
            try
            {
                Process.GetProcessById(process.Id);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        public static T Read<T>(this Process process, IntPtr lpBaseAddress) where T : unmanaged
        {
            return Read<T>(process.Handle, lpBaseAddress);
        }

        public static T Read<T>(this Module module, int offset) where T : unmanaged
        {
            return Read<T>(module.Process.Handle, module.ProcessModule.BaseAddress + offset);
        }

        private static T Read<T>(IntPtr hProcess, IntPtr lpBaseAddress) where T : unmanaged
        {
            var size = Marshal.SizeOf<T>();
            object buffer = default(T);
            Kernel32.ReadProcessMemory(hProcess, lpBaseAddress, buffer, size, out var lpNumberOfBytesRead);
            return lpNumberOfBytesRead == size ? (T)buffer : default;
        }

        public static void ReadRaw<T>(this Process process, IntPtr lpBaseAddress, in T[] buffer)
        {
            ReadRaw(process.Handle, lpBaseAddress, buffer);
        }

        public static void ReadRaw<T>(this Module module, int offset, in T[] buffer)
        {
            ReadRaw(module.Process.Handle, module.ProcessModule.BaseAddress + offset, buffer);
        }

        private static void ReadRaw<T>(IntPtr hProcess, IntPtr lpBaseAddress, in T[] buffer)
        {
            Kernel32.ReadProcessMemory(hProcess, lpBaseAddress, buffer, Marshal.SizeOf<T>() * buffer.Length, out _);
        }

        public static Vector2 GetCursorPosition()
        {
            var gotPoint = User32.GetCursorPos(out var currentMousePoint);
            if (!gotPoint) currentMousePoint = new Point { X = 0, Y = 0 };
            return new Vector2(currentMousePoint.X, currentMousePoint.Y);
        }

        public static void MouseEvent(User32.MouseEventFlags value)
        {
            var position = GetCursorPosition();

            User32.mouse_event
                ((int)value,
                    (int)position.X,
                    (int)position.Y,
                    0,
                    0)
                ;
        }

        public static Team ToTeam(this int teamNum)
        {
            switch (teamNum)
            {
                case 1:
                    return Team.Spectator;
                case 2:
                    return Team.Terrorists;
                case 3:
                    return Team.CounterTerrorists;
                default:
                    return Team.Unknown;
            }
        }

        public static bool IsKeyDown(Key key)
        {
            var keyState = User32.GetAsyncKeyState(KeyInterop.VirtualKeyFromKey(key));
            Console.WriteLine(keyState);
            return ((keyState >> 15) & 0x0001) == 0x0001;
        }

        public static Color WithAlpha(this Color color, int alpha)
        {
            return Color.FromArgb(alpha, color.R, color.G, color.B);
        }
    }
}