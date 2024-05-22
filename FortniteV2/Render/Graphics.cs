﻿using System.Drawing;
using System.Windows.Forms;
using FortniteV2.Features;
using FortniteV2.Game;
using FortniteV2.Utils;
using HijackOverlay;
using HijackOverlay.Render.Font;

namespace FortniteV2.Render
{
    public class Graphics : TickThread
    {
        public Graphics(GameProcess gameProcess, GameData gameData) : base(nameof(Graphics))
        {
            GameProcess = gameProcess;
            GameData = gameData;
            FpsCounter = new FpsCounter();
        }

        private bool HasInitialized { get; set; }
        public Color[] Rainbow { get; } = { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo };

        public GameProcess GameProcess { get; private set; }
        public GameData GameData { get; private set; }
        public Overlay GameOverlay { get; set; }
        public FontRenderer FontAzonix64 { get; private set; }
        public FontRenderer FontConsolas32 { get; private set; }
        public FpsCounter FpsCounter { get; private set; }

        public override void Dispose()
        {
            base.Dispose();

            FpsCounter = default;
            GameData = default;
            GameProcess = default;
        }

        private void InitDevice()
        {
            // TODO: make this not garbage
            GameOverlay = new Overlay(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            // var handle = (IntPtr)typeof(Overlay).GetProperty("WindowHandle", BindingFlags.Instance | BindingFlags.NonPublic).GetMethod
            //     .Invoke(GameOverlay, new object[0]);
            // User32.SetWindowPos(handle, new IntPtr(0), 0, 0, 1920 * 2, 1080 * 2, 0U);
            FontAzonix64 = new FontRenderer("Azonix", 64);
            FontConsolas32 = new FontRenderer("Consolas", 32);
        }

        protected override void End()
        {
            GameOverlay.Clear();
            GameOverlay.Dispose();
        }

        protected override void Tick()
        {
            if (!HasInitialized)
            {
                HasInitialized = true;
                InitDevice();
            }

            if (!GameProcess.IsValid)
            {
                GameOverlay.Clear();
                return;
            }

            FpsCounter.Update();

            var gameWindow = GameProcess.WindowRectangle;
            GameOverlay.StartDraw(gameWindow.X, Screen.PrimaryScreen.Bounds.Height - gameWindow.Y - gameWindow.Height, gameWindow.Width, gameWindow.Height);

            Draw();

            GameOverlay.EndDraw();
        }

        private void Draw()
        {
            // draw here
            SkeletonEsp.Draw(this);
            RecoilCrosshair.Draw(this);
            Hud.Draw(GameProcess, this);
        }
    }
}