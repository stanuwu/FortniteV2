using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using FortniteV2.Game;
using FortniteV2.Utils;
using SharpDX;
using WindowsInput;

namespace FortniteV2.Features
{
    public class AimBot : TickThread
    {
        public AimBot(GameProcess gameProcess, GameData gameData) : base(nameof(AimBot))
        {
            GameProcess = gameProcess;
            GameData = gameData;
        }

        public static bool IsOn { get; private set; }
        private Stopwatch Stopwatch { get; } = new Stopwatch();
        private bool Cooldown { get; set; }
        private GameProcess GameProcess { get; set; }
        private GameData GameData { get; set; }
        public static Vector2 CrosshairScreen { get; private set; } = Vector2.Zero;
        private static string TargetName { get; set; } = "";
        public static Vector2 Target { get; private set; } = Vector2.Zero;
        private InputSimulator InputSimulator { get; } = new InputSimulator();

        private static int SmoothSpeed(float distance)
        {
            return (int)(Config.AimSpeed + 0.0001 * (distance * distance));
        }

        protected override void Tick()
        {
            if (!Config.EnableAimBot) return;

            var max = Config.AimPixelFov;
            const int min = 3;
            var cooldownMs = Config.AimSwitchDelayMs;

            if (!GameProcess.IsValid) return;

            Application.Current.Dispatcher.Invoke(() => { IsOn = Config.AimBotKey != Key.None && Util.IsKeyDown(Config.AimBotKey); });

            if (!IsOn) return;

            var bonePos = new List<AimTarget>();

            foreach (var e in GameData.Entities)
            {
                if (!e.IsAlive() || e.PawnBase == GameData.Player.PawnBase) continue;
                if (e.Team == GameData.Player.Team) continue;
                var worldPos = e.BonePos["head"];
                var name = e.Name;
                var transformed = GameData.Player.MatrixViewProjectionViewport.Transform(worldPos);
                if (transformed.Z >= 1) continue;
                bonePos.Add(new AimTarget { Name = name, Position = new Vector2(transformed.X, transformed.Y) });
            }

            var crosshair = RecoilCrosshair.GetPositionScreen(GameProcess, GameData);
            var crosshairScreen = new Vector2(crosshair.X, crosshair.Y);

            var tName = "";
            var target = Vector2.Zero;
            var distance = float.MaxValue;
            foreach (var t in bonePos)
            {
                var newDist = (crosshairScreen - t.Position).Length();
                if (newDist < distance)
                {
                    tName = t.Name;
                    target = t.Position;
                    distance = newDist;
                }
            }

            if (distance > max)
            {
                Target = Vector2.Zero;
                return;
            }

            if (target != Vector2.Zero && Target != Vector2.Zero && tName != TargetName)
            {
                Stopwatch.Reset();
                Stopwatch.Start();
                Cooldown = true;
            }

            CrosshairScreen = crosshairScreen;
            TargetName = tName;
            Target = target;

            if (distance < min) return;

            var diff = Target - CrosshairScreen;
            diff.Normalize();
            diff *= SmoothSpeed(distance);

            if (Cooldown)
            {
                if (Stopwatch.Elapsed < TimeSpan.FromMilliseconds(cooldownMs)) return;
                Cooldown = false;
                Stopwatch.Stop();
            }
            else
            {
                InputSimulator.Mouse.MoveMouseBy((int)diff.X, (int)diff.Y);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            GameProcess = default;
            GameData = default;
        }

        private struct AimTarget
        {
            public Vector2 Position;
            public string Name;
        }
    }
}