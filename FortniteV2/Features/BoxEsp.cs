using System;
using System.Collections.Generic;
using System.Numerics;
using FortniteV2.Game;
using FortniteV2.Render;
using FortniteV2.Utils;
using HijackOverlay.Render;

namespace FortniteV2.Features
{
    public static class BoxEsp
    {
        private static List<bool> _friendly = new();
        private static List<float> _xs = new();
        private static List<float> _ys = new();
        private static List<float> _x2s = new();
        private static List<float> _y2s = new();

        public static void Draw(Graphics graphics)
        {
            if (!Config.EnableBoxEsp) return;

            var bufferBuilder = Renderer.StartPositionColorLines(Config.EspWidth);
            for (var i = 0; i < _friendly.Count; i++)
                try
                {
                    Renderer.BufferColorGradientLineGroup(
                        bufferBuilder,
                        new[] { _xs[i], _xs[i], _x2s[i], _x2s[i] },
                        new[] { _ys[i], _ys[i], _y2s[i], _y2s[i] },
                        new[] { _xs[i], _x2s[i], _xs[i], _x2s[i] },
                        new[] { _y2s[i], _ys[i], _y2s[i], _ys[i] },
                        _friendly[i] ? SkeletonEsp.ColorFriendly : Config.EspRgb ? graphics.Rainbow : SkeletonEsp.ColorEnemy
                    );
                }
                catch (IndexOutOfRangeException)
                {
                    // player amount changed while loading in, pass
                }

            Renderer.End(bufferBuilder);
        }

        public static void CalculateBox(GameProcess gameProcess, GameData gameData)
        {
            if (!Config.EnableBoxEsp) return;

            var tfriendly = new List<bool>();
            var txs = new List<float>();
            var tys = new List<float>();
            var tx2s = new List<float>();
            var ty2s = new List<float>();
            foreach (var entity in gameData.Entities)
            {
                if (!entity.IsAlive() || entity.PawnBase == gameData.Player.PawnBase) continue;
                var friendly = entity.Team == gameData.Player.Team;
                var height = gameProcess.WindowRectangle.Height;
                var top = entity.BonePos["head"] + new Vector3(0f, 0f, 10f);
                var bottom = new Vector3(entity.BonePos["head"].X, entity.BonePos["head"].Y, entity.BonePos["ankle_R"].Z - 10f);
                var width = Vector3.Distance(entity.BonePos["head"].WorldToScreen(gameData), entity.BonePos["spine_1"].WorldToScreen(gameData)) *
                            2f;

                var topScreen = top.WorldToScreen(gameData);
                var bottomScreen = bottom.WorldToScreen(gameData);

                if (topScreen.Z >= 1 || bottomScreen.Z >= 1) continue;
                if (width > gameProcess.WindowRectangle.Width) continue;
                if (topScreen.X == bottomScreen.X && topScreen.Y == bottomScreen.Y) continue;

                tfriendly.Add(friendly);
                txs.Add(topScreen.X - width);
                tys.Add(height - topScreen.Y);
                tx2s.Add(topScreen.X + width);
                ty2s.Add(height - bottomScreen.Y);
            }

            _friendly = tfriendly;
            _xs = txs;
            _ys = tys;
            _x2s = tx2s;
            _y2s = ty2s;
        }
    }
}