using FortniteV2.Game;
using FortniteV2.Render;
using FortniteV2.Utils;
using SharpDX;

namespace FortniteV2.Features
{
    public static class RecoilCrosshair
    {
        private const int Fov = 90;
        private static readonly Color CrosshairColor = Color.Red;

        public static Vector3 GetPositionScreen(GameProcess gameProcess, GameData gameData)
        {
            var screenSize = gameProcess.WindowRectangle.Size;
            var aspectRatio = (double)screenSize.Width / screenSize.Height;
            var player = gameData.Player;
            var fovY = ((double)Fov).DegreeToRadian();
            var fovX = fovY * aspectRatio;

            var doPunch = gameData.Player.ShotsFired > 0;

            var punchX = doPunch ? ((double)player.AimPunchAngle.X * Offsets.RECOIL_SCALE).DegreeToRadian() : 0;
            var punchY = doPunch ? ((double)player.AimPunchAngle.Y * Offsets.RECOIL_SCALE).DegreeToRadian() : 0;
            var pointClip = new Vector3(
                (float)(-punchY / fovX),
                (float)(-punchX / fovY),
                0
            );
            return player.MatrixViewport.Transform(pointClip);
        }

        public static void Draw(Graphics graphics)
        {
            if(!Config.EnableRecoilCrosshair) return;

            var pointScreen = GetPositionScreen(graphics.GameProcess, graphics.GameData);
            Draw(graphics, new Vector2(pointScreen.X, pointScreen.Y));
        }

        private static void Draw(Graphics graphics, Vector2 pointScreen)
        {
            const int radius = 12;
            graphics.DrawLine(CrosshairColor, pointScreen - new Vector2(radius, 0), pointScreen + new Vector2(radius, 0));
            graphics.DrawLine(CrosshairColor, pointScreen - new Vector2(0, radius), pointScreen + new Vector2(0, radius));
        }
    }
}