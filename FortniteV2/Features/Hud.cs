using System.Drawing;
using System.Numerics;
using FortniteV2.Game;
using GameBarOverlay.Render;
using Graphics = FortniteV2.Render.Graphics;

namespace FortniteV2.Features
{
    public static class Hud
    {
        private const int XOffset = 5;
        private const int YOffset = 5;
        private static readonly Color HudColor = Color.Cyan;
        private static readonly Color OnColor = Color.Lime;
        private static readonly Color OffColor = Color.Red;
        private static readonly Color AimBotColor = Color.Magenta;

        public static void Draw(GameProcess gameProcess, Graphics graphics)
        {
            var height = gameProcess.WindowRectangle.Height;

            // window border
            var bufferBuilder = Renderer.StartPositionColorLines();
            Renderer.BufferColorGradientLineGroup(bufferBuilder,
                new float[] { 1, gameProcess.WindowRectangle.Width, 1, 1 },
                new float[] { gameProcess.WindowRectangle.Height, 1, 1, 1 },
                new float[] { gameProcess.WindowRectangle.Width - 1, gameProcess.WindowRectangle.Width - 1, 1, gameProcess.WindowRectangle.Width },
                new float[] { gameProcess.WindowRectangle.Height - 1, gameProcess.WindowRectangle.Height - 1, gameProcess.WindowRectangle.Height, 1 },
                graphics.Rainbow
            );

            // watermark
            graphics.FontAzonix64.DrawString(Config.Watermark, XOffset, height - 64 - YOffset, graphics.Rainbow);

            // fps count
            graphics.FontConsolas32.DrawString($"{(int)graphics.FpsCounter.Fps} FPS", XOffset, height - 32 - YOffset * 2 - 64, HudColor);

            // triggerbot status
            if (Config.EnableTriggerBot)
                graphics.FontConsolas32.DrawString("TriggerBot", XOffset, height - 32 - YOffset * 3 - 64 - 32, TriggerBot.IsOn ? OnColor : OffColor);

            // aimbot status
            if (Config.EnableAimBot)
                graphics.FontConsolas32.DrawString("AimBot", XOffset, height - 32 - YOffset * 3 - 64 - 32 - (Config.EnableTriggerBot ? YOffset + 32 : 0),
                    AimBot.IsOn ? OnColor : OffColor);

            // aimbot line
            if (Config.EnableAimBot)
                if (AimBot.IsOn && AimBot.Target != Vector2.Zero)
                    Renderer.BufferColorLine(bufferBuilder, AimBot.CrosshairScreen.X, height - AimBot.CrosshairScreen.Y, AimBot.Target.X,
                        height - AimBot.Target.Y,
                        AimBotColor);
            Renderer.End(bufferBuilder);
        }
    }
}