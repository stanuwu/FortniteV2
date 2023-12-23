using System;
using FortniteV2.Game;
using FortniteV2.Utils;
using SharpDX;
using MathUtil = FortniteV2.Utils.MathUtil;

namespace FortniteV2.Data
{
    public class Player : EntityBase
    {
        public Matrix MatrixViewProjection { get; private set; }
        public Matrix MatrixViewport { get; private set; }
        public Matrix MatrixViewProjectionViewport { get; private set; }
        public Vector3 ViewOffset { get; private set; }
        public Vector3 EyePosition { get; private set; }
        public Vector3 ViewAngles { get; private set; }
        public Vector3 AimPunchAngle { get; private set; }
        public Vector3 AimDirection { get; private set; }
        public int TargetedEntityIndex { get; private set; }

        protected override IntPtr ReadControllerBase(GameProcess gameProcess)
        {
            return gameProcess.ModuleClient.Read<IntPtr>(Offsets.dwLocalPlayerController);
        }

        protected override IntPtr ReadPawnBase(GameProcess gameProcess)
        {
            return gameProcess.ModuleClient.Read<IntPtr>(Offsets.dwLocalPlayerPawn);
        }

        public override bool Update(GameProcess gameProcess)
        {
            if (!base.Update(gameProcess)) return false;

            MatrixViewProjection = Matrix.Transpose(gameProcess.ModuleClient.Read<Matrix>(Offsets.dwViewMatrix));
            MatrixViewport = MathUtil.GetMatrixViewport(gameProcess.WindowRectangle.Size);
            MatrixViewProjectionViewport = MatrixViewProjection * MatrixViewport;

            ViewOffset = gameProcess.Process.Read<Vector3>(PawnBase + Offsets.m_vecViewOffset);
            EyePosition = Origin + ViewOffset;
            ViewAngles = gameProcess.ModuleClient.Read<Vector3>(Offsets.dwViewAngles);
            AimPunchAngle = gameProcess.Process.Read<Vector3>(PawnBase + Offsets.m_aimPunchAngle);
            TargetedEntityIndex = gameProcess.Process.Read<int>(PawnBase + Offsets.m_iIDEntIndex);

            AimDirection = GetAimDirection(ViewAngles, AimPunchAngle);

            return true;
        }

        private static Vector3 GetAimDirection(Vector3 viewAngles, Vector3 aimPunchAngle)
        {
            var phi = (viewAngles.X + aimPunchAngle.X * Offsets.RECOIL_SCALE).DegreeToRadian();
            var theta = (viewAngles.Y + aimPunchAngle.Y * Offsets.RECOIL_SCALE).DegreeToRadian();

            return Vector3.Normalize(new Vector3
            (
                (float)(Math.Cos(phi) * Math.Cos(theta)),
                (float)(Math.Cos(phi) * Math.Sin(theta)),
                (float)-Math.Sin(phi)
            ));
        }
    }
}