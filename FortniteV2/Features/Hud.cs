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
            Renderer.DrawRoundedTextureRect(XOffset, height - 100 - YOffset, 100, 100, 15, Logo);

            // watermark
            graphics.FontAzonix32.DrawString(Config.Watermark, XOffset * 2 + 100, height - 32 - YOffset, HudColor);

            // fps count
            graphics.FontConsolas16.DrawString($"{(int)graphics.FpsCounter.Fps} FPS", XOffset * 2 + 100, height - 16 - YOffset * 2 - 32, HudColor);

            // triggerbot status
            if (Config.EnableTriggerBot)
                graphics.FontConsolas16.DrawString("TriggerBot", XOffset * 2 + 100, height - 16 - YOffset * 3 - 32 - 16, TriggerBot.IsOn ? OnColor : OffColor);

            // aimbot status
            if (Config.EnableAimBot)
                graphics.FontConsolas16.DrawString("AimBot", XOffset * 2 + 100,
                    height - 16 - YOffset * 3 - 32 - 16 - (Config.EnableTriggerBot ? YOffset + 16 : 0),
                    AimBot.IsOn ? OnColor : OffColor);

            // aimbot line and fov
            if (Config.EnableAimBot)
            {
                if (AimBot.IsOn && AimBot.Target != Vector2.Zero)
                    Renderer.BufferColorLine(bufferBuilder,
                        AimBot.Target.X, height - AimBot.Target.Y,
                        AimBot.CrosshairScreen.X, height - AimBot.CrosshairScreen.Y,
                        Config.EspRgb ? graphics.Rainbow : new[] { Color.Red });

                // yes this is a bit goofy
                Renderer.DrawOutlineColorCircle(RecoilCrosshair.CurrentX - Config.AimPixelFov, height - RecoilCrosshair.CurrentY - Config.AimPixelFov,
                    Config.AimPixelFov * 2, 3,
                    Config.EspRgb ? Color.Red.WithAlpha(128) : Color.Red.WithAlpha(128), Config.EspRgb ? Color.Yellow.WithAlpha(128) : Color.Red.WithAlpha(128),
                    Config.EspRgb ? Color.Cyan.WithAlpha(128) : Color.Red.WithAlpha(128), Config.EspRgb ? Color.Lime.WithAlpha(128) : Color.Red.WithAlpha(128));
            }

            Renderer.End(bufferBuilder);
        }
    }
}