using System.Windows.Input;

namespace FortniteV2
{
    public static class Config
    {
        public static string Watermark = $"Fortnite v{Fortnite.Version}";

        public static bool EnableAimBot = false;
        public static Key AimBotKey = Key.None;
        public static int AimSpeed = 2;
        public static int AimPixelFov = 300;
        public static int AimSwitchDelayMs = 500;

        public static bool EnableTriggerBot = false;
        public static Key TriggerBotKey = Key.None;
        public static int TriggerBotDelayMs = 10;
        public static int TriggerBotDelayRandomMs = 5;

        public static bool EnableRecoilCrosshair = true;

        public static bool EnableSkeletonEsp = true;
        public static bool EnableBoxEsp = true;
        public static int EspWidth = 2;
        public static bool EspRgb = true;

        public static bool EnableRecoilControl = true;
        public static int RecoilControlSpeed = 40;
    }
}