using System.Drawing;
using System.Numerics;
using FortniteV2.Game;
using FortniteV2.Utils;
using HijackOverlay.Render;
using HijackOverlay.Render.Texture;
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
        private static readonly GlTexture Logo = new("1.ico");

        public static void Draw(GameProcess gameProcess, Graphics graphics)
        {
            var width = gameProcess.WindowRectangle.Width;
            var height = gameProcess.WindowRectangle.Height;

            // window border
            var bufferBuilder = Renderer.StartPositionColorLines();
            Renderer.BufferColorGradientLineGroup(bufferBuilder,
                new float[] { 1, width, 1, 1 },
                new float[] { height, 1, 1, 1 },
                new float[] { width - 1, width - 1, 1, width },
                new float[] { height - 1, height - 1, height, 1 },
                graphics.Rainbow
            );

            // logo
            Renderer.DrawRoundedTextureRect(XOffset, height - 170 - YOffset, 170, 170, 25, Logo);

            // watermark
            graphics.FontAzonix64.DrawString(Config.Watermark, XOffset * 2 + 170, height - 64 - YOffset, HudColor);

            // fps count
            graphics.FontConsolas32.DrawString($"{(int)graphics.FpsCounter.Fps} FPS", XOffset * 2 + 170, height - 32 - YOffset * 2 - 64, HudColor);

            // triggerbot status
            if (Config.EnableTriggerBot)
                graphics.FontConsolas32.DrawString("TriggerBot", XOffset * 2 + 170, height - 32 - YOffset * 3 - 64 - 32, TriggerBot.IsOn ? OnColor : OffColor);

            // aimbot status
            if (Config.EnableAimBot)
                graphics.FontConsolas32.DrawString("AimBot", XOffset * 2 + 170,
                    height - 32 - YOffset * 3 - 64 - 32 - (Config.EnableTriggerBot ? YOffset + 32 : 0),
                    AimBot.IsOn ? OnColor : OffColor);

            // aimbot line and fov
            if (Config.EnableAimBot)
            {
                if (AimBot.IsOn && AimBot.Target != Vector2.Zero)
                    Renderer.BufferColorLine(bufferBuilder,
                        AimBot.Target.X, height - AimBot.Target.Y,
                        AimBot.CrosshairScreen.X, height - AimBot.CrosshairScreen.Y,
                        graphics.Rainbow);

                Renderer.DrawOutlineColorCircle(RecoilCrosshair.CurrentX - Config.AimPixelFov, height - RecoilCrosshair.CurrentY - Config.AimPixelFov,
                    Config.AimPixelFov * 2, 2,
                    Color.Red.WithAlpha(128), Color.Yellow.WithAlpha(128), Color.Cyan.WithAlpha(128), Color.Lime.WithAlpha(128));
            }

            Renderer.End(bufferBuilder);
        }
    }
}