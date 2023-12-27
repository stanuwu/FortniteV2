using System;
using System.Diagnostics;
using FortniteV2.Features;
using FortniteV2.Game;
using FortniteV2.Render;
using OpenGL;

namespace FortniteV2
{
    // IDisposable
    // Disposes when closed to avoid memory leaks
    // All IDisposable classes used by this program should be stored and disposed in here
    public class Fortnite : IDisposable
    {
        // Client Version
        public const int Version = 2;

        private GameProcess GameProcess { get; set; }
        private GameData GameData { get; set; }
        private Graphics Graphics { get; set; }
        private AimBot AimBot { get; set; }
        private TriggerBot TriggerBot { get; set; }

        public void Dispose()
        {
            GameData.Dispose();
            GameData = default;

            GameProcess.Dispose();
            GameProcess = default;

            Graphics.Dispose();
            Graphics = default;

            AimBot.Dispose();
            AimBot = default;

            TriggerBot.Dispose();
            TriggerBot = default;
        }

        public void OnStartup()
        {
            // set process priority
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            GameProcess = new GameProcess();
            GameData = new GameData(GameProcess);
            Graphics = new Graphics(GameProcess, GameData);
            AimBot = new AimBot(GameProcess, GameData);
            TriggerBot = new TriggerBot(GameProcess, GameData);

            Wgl.MakeCurrent(IntPtr.Zero, IntPtr.Zero);

            GameProcess.Start();
            GameData.Start();
            Graphics.Start();
            AimBot.Start();
            TriggerBot.Start();
        }
    }
}