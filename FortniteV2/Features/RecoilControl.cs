using System.Numerics;
using FortniteV2.Game;
using FortniteV2.Utils;
using WindowsInput;

namespace FortniteV2.Features
{
    public class RecoilControl : TickThread
    {
        public RecoilControl(GameProcess gameProcess, GameData gameData) : base(nameof(RecoilControl))
        {
            GameProcess = gameProcess;
            GameData = gameData;
        }

        private GameProcess GameProcess { get; set; }
        private GameData GameData { get; set; }
        private InputSimulator InputSimulator { get; } = new();
        private Vector2 LastRecoil { get; set; } = Vector2.Zero;

        protected override void Tick()
        {
            if (!Config.EnableRecoilControl) return;

            if (!GameProcess.IsValid) return;

            // only run when aimbot is not running already
            if (AimBot.IsOn) return;

            var center = new Vector2(
                GameProcess.WindowRectangle.Width / 2f,
                GameProcess.WindowRectangle.Height / 2f
            );

            var target = Vector2.Zero;
            if (GameData.Player.ShotsFired > 0 && LastRecoil != Vector2.Zero) target = LastRecoil;

            var crosshair = RecoilCrosshair.GetPositionScreen(GameProcess, GameData);
            var crosshairScreen = new Vector2(crosshair.X, crosshair.Y);
            var distance = Vector2.Distance(target, crosshairScreen);
            var distanceC = Vector2.Distance(center, target);

            LastRecoil = crosshairScreen;

            if (target == Vector2.Zero) return;

            if (distance < 1 || distanceC < 1) return;

            var diff = target - crosshairScreen;
            diff *= Config.RecoilControlSpeed / 10f;

            InputSimulator.Mouse.MoveMouseBy((int)diff.X, (int)diff.Y);
        }

        public override void Dispose()
        {
            base.Dispose();

            GameProcess = default;
            GameData = default;
        }
    }
}