using System.Drawing;
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
        public FontRenderer FontAzonix32 { get; private set; }
        public FontRenderer FontConsolas32 { get; private set; }
        public FontRenderer FontConsolas16 { get; private set; }
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
            GameOverlay = new Overlay(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, "Create", "Create");
            FontAzonix64 = new FontRenderer("Azonix", 64);
            FontAzonix32 = new FontRenderer("Azonix", 32);
            FontConsolas32 = new FontRenderer("Consolas", 32);
            FontConsolas16 = new FontRenderer("Consolas", 16);
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
            BoxEsp.Draw(this);
            RecoilCrosshair.Draw(this);
            Hud.Draw(GameProcess, this);
        }
    }
}