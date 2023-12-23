﻿using System;
using System.Drawing;
using SharpDX;

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

        public static Matrix GetMatrixViewport(Size screenSize)
        {
            return GetMatrixViewport(new Viewport
            {
                X = 0,
                Y = 0,
                Width = screenSize.Width,
                Height = screenSize.Height,
                MinDepth = 0,
                MaxDepth = 1
            });
        }

        public static Matrix GetMatrixViewport(in Viewport viewport)
        {
            return new Matrix
            {
                M11 = viewport.Width * 0.5f,
                M12 = 0,
                M13 = 0,
                M14 = 0,

                M21 = 0,
                M22 = -viewport.Height * 0.5f,
                M23 = 0,
                M24 = 0,

                M31 = 0,
                M32 = 0,
                M33 = viewport.MaxDepth - viewport.MinDepth,
                M34 = 0,

                M41 = viewport.X + viewport.Width * 0.5f,
                M42 = viewport.Y + viewport.Height * 0.5f,
                M43 = viewport.MinDepth,
                M44 = 1
            };
        }

        public static Vector3 Transform(this in Matrix matrix, Vector3 value)
        {
            var wInv = 1.0 / (matrix.M14 * (double)value.X + matrix.M24 * (double)value.Y + matrix.M34 * (double)value.Z + matrix.M44);
            return new Vector3
            (
                (float)((matrix.M11 * (double)value.X + matrix.M21 * (double)value.Y + matrix.M31 * (double)value.Z + matrix.M41) * wInv),
                (float)((matrix.M12 * (double)value.X + matrix.M22 * (double)value.Y + matrix.M32 * (double)value.Z + matrix.M42) * wInv),
                (float)((matrix.M13 * (double)value.X + matrix.M23 * (double)value.Y + matrix.M33 * (double)value.Z + matrix.M43) * wInv)
            );
        }
    }
}