using System.Drawing;
using System.Numerics;
using FortniteV2.Game;
using FortniteV2.Utils;
using GameBarOverlay.Render;
using Graphics = FortniteV2.Render.Graphics;

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
            if (!Config.EnableRecoilCrosshair) return;

            var pointScreen = GetPositionScreen(graphics.GameProcess, graphics.GameData);
            Draw(graphics, new Vector2(pointScreen.X, pointScreen.Y));
        }

        private static void Draw(Graphics graphics, Vector2 pointScreen)
        {
            var height = graphics.GameProcess.WindowRectangle.Height;
            const int radius = 12;
            var bufferBuilder = Renderer.StartPositionColorLines();
            var p1 = pointScreen - new Vector2(radius, 0);
            var p2 = pointScreen + new Vector2(radius, 0);
            var p3 = pointScreen - new Vector2(0, radius);
            var p4 = pointScreen + new Vector2(0, radius);
            Renderer.BufferColorLine(bufferBuilder, p1.X, height - p1.Y, p2.X, height - p2.Y, CrosshairColor);
            Renderer.BufferColorLine(bufferBuilder, p3.X, height - p3.Y, p4.X, height - p4.Y, CrosshairColor);
            Renderer.End(bufferBuilder);
        }
    }
}