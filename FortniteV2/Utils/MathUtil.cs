using System;
using System.Drawing;
using System.Numerics;
using FortniteV2.Game;
using OpenGL;

namespace FortniteV2.Utils
{
    public static class MathUtil
    {
        private const double _PI_Over_180 = Math.PI / 180.0;

        private const double _180_Over_PI = 180.0 / Math.PI;

        public static double DegreeToRadian(this double degree)
        {
            return degree * _PI_Over_180;
        }

        public static double RadianToDegree(this double radian)
        {
            return radian * _180_Over_PI;
        }

        public static float DegreeToRadian(this float degree)
        {
            return (float)(degree * _PI_Over_180);
        }

        public static float RadianToDegree(this float radian)
        {
            return (float)(radian * _180_Over_PI);
        }

        public static Matrix4x4f GetMatrixViewport(Size screenSize)
        {
            var matrix = new Matrix4x4f();
            matrix[0, 0] = screenSize.Width * 0.5f;
            matrix[1, 1] = -screenSize.Height * 0.5f;
            matrix[2, 2] = 1;
            matrix[3, 0] = screenSize.Width * 0.5f;
            matrix[3, 1] = screenSize.Height * 0.5f;
            matrix[3, 3] = 1;
            return matrix;
        }


        public static Vector3 Transform(this in Matrix4x4f matrix, Vector3 value)
        {
            var wInv = 1.0 / (matrix[0, 3] * (double)value.X + matrix[1, 3] * (double)value.Y + matrix[2, 3] * (double)value.Z + matrix[3, 3]);
            return new Vector3
            (
                (float)((matrix[0, 0] * (double)value.X + matrix[1, 0] * (double)value.Y + matrix[2, 0] * (double)value.Z + matrix[3, 0]) * wInv),
                (float)((matrix[0, 1] * (double)value.X + matrix[1, 1] * (double)value.Y + matrix[2, 1] * (double)value.Z + matrix[3, 1]) * wInv),
                (float)((matrix[0, 2] * (double)value.X + matrix[1, 2] * (double)value.Y + matrix[2, 2] * (double)value.Z + matrix[3, 2]) * wInv)
            );
        }

        public static Vector3 WorldToScreen(this Vector3 world, GameData gameData)
        {
            return gameData.Player.MatrixViewProjectionViewport.Transform(world);
        }
    }
}