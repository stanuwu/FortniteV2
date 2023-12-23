using System.Drawing;
using System.Windows;
using System.Windows.Threading;
using FortniteV2.Game;
using FortniteV2.Utils;
using GameOverlay.Windows;

namespace FortniteV2.Render
{
    public class WindowOverlay : TickThread
    {
        public WindowOverlay(GameProcess gameProcess) : base(nameof(WindowOverlay), 100)


        {
            GameProcess = gameProcess;

            Window = new OverlayWindow
            {
                Title = "Fortnite Overlay",
                IsTopmost = true,
                IsVisible = true,
                X = -32000,
                Y = -32000,
                Width = 16,
                Height = 16
            };

            Window.Create();
        }

        private GameProcess GameProcess { get; set; }

        public OverlayWindow Window { get; private set; }

        public override void Dispose()
        {
            base.Dispose();

            Window.Dispose();
            Window = default;

            GameProcess = default;
        }

        protected override void Tick()
        {
            Update(GameProcess.WindowRectangle);
        }

        private void Update(Rectangle windowRectangle)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Window.X != windowRectangle.Location.X || Window.Y != windowRectangle.Location.Y ||
                    Window.Width != windowRectangle.Size.Width || Window.Height != windowRectangle.Size.Height)
                {
                    if (windowRectangle.Width > 0 && windowRectangle.Height > 0)
                    {
                        Window.X = windowRectangle.Location.X;
                        Window.Y = windowRectangle.Location.Y;
                        Window.Width = windowRectangle.Size.Width;
                        Window.Height = windowRectangle.Size.Height;
                    }
                    else
                    {
                        Window.X = -32000;
                        Window.Y = -32000;
                        Window.Width = 16;
                        Window.Height = 16;
                    }
                }
            }, DispatcherPriority.Normal);
        }
    }
}