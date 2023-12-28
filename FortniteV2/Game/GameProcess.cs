using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using FortniteV2.Sys;
using FortniteV2.Utils;

namespace FortniteV2.Game
{
    public class GameProcess : TickThread
    {
        private const string PROCESS_NAME = "cs2";

        private const string CLIENT_MODULE_NAME = "client.dll";

        private const string WINDOW_NAME = "Counter-Strike 2";

        public GameProcess() : base(nameof(GameProcess), 500)
        {
        }

        public Process Process { get; set; }
        public Module ModuleClient { get; set; }
        private IntPtr WindowHwnd { get; set; }
        public Rectangle WindowRectangle { get; private set; }
        private bool WindowActive { get; set; }

        public bool IsValid => WindowActive && !(Process is null) && !(ModuleClient is null);

        private void DisposeProcess()
        {
            ModuleClient?.Dispose();
            ModuleClient = default;

            Process?.Dispose();
            Process = default;
        }

        private void DisposeWindow()
        {
            WindowHwnd = IntPtr.Zero;
            WindowRectangle = Rectangle.Empty;
            WindowActive = false;
        }

        public override void Dispose()
        {
            DisposeWindow();
            DisposeProcess();

            base.Dispose();
        }

        protected override void Tick()
        {
            if (!EnsureProcess()) DisposeProcess();
            if (!EnsureWindow()) DisposeWindow();
        }

        private bool EnsureProcess()
        {
            if (Process is null) Process = Process.GetProcessesByName(PROCESS_NAME).FirstOrDefault();

            if (Process is null || !Process.IsRunning()) return false;

            if (ModuleClient is null) ModuleClient = Process.GetModule(CLIENT_MODULE_NAME);

            if (ModuleClient is null) return false;

            return true;
        }

        private bool EnsureWindow()
        {
            WindowHwnd = User32.FindWindow(null, WINDOW_NAME);
            if (WindowHwnd == IntPtr.Zero) return false;

            WindowRectangle = Util.GetClientRectangle(WindowHwnd);
            if (WindowRectangle.Width <= 0 || WindowRectangle.Height <= 0) return false;

            WindowActive = WindowHwnd == User32.GetForegroundWindow();

            return WindowActive;
        }
    }
}