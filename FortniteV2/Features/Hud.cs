using FortniteV2.Game;
using FortniteV2.Render;
using SharpDX;

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
            // window border
            graphics.DrawLine(HudColor,
                new Vector2(0, 0),
                new Vector2(gameProcess.WindowRectangle.Width - 1, 0),
                new Vector2(gameProcess.WindowRectangle.Width - 1, 0),
                new Vector2(gameProcess.WindowRectangle.Width - 1, gameProcess.WindowRectangle.Height - 1),
                new Vector2(gameProcess.WindowRectangle.Width - 1, gameProcess.WindowRectangle.Height - 1),
                new Vector2(0, gameProcess.WindowRectangle.Height - 1),
                new Vector2(0, gameProcess.WindowRectangle.Height - 1),
                new Vector2(0, 0)
            );

            // watermark
            graphics.FontAzonix64.DrawText(default, Config.Watermark, XOffset, YOffset, HudColor);

            // fps count
            graphics.FontConsolas32.DrawText(default, $"{(int)graphics.FpsCounter.Fps} FPS", XOffset, YOffset * 2 + 64, HudColor);

            // triggerbot status
            if (Config.EnableTriggerBot)
                graphics.FontConsolas32.DrawText(default, "TriggerBot", XOffset, YOffset * 3 + 64 + 32, TriggerBot.IsOn ? OnColor : OffColor);

            // aimbot status
            if (Config.EnableAimBot)
                graphics.FontConsolas32.DrawText(default, "AimBot", XOffset, YOffset * 3 + 64 + 32 + (Config.EnableTriggerBot ? YOffset + 32 : 0),
                    AimBot.IsOn ? OnColor : OffColor);

            // aimbot line
            if (Config.EnableAimBot)
                if (AimBot.IsOn && AimBot.Target != Vector2.Zero)
                    graphics.DrawLine(AimBotColor, AimBot.CrosshairScreen, AimBot.Target);
        }
    }
}