using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using FortniteV2.Game;
using FortniteV2.Utils;
using HijackOverlay.Render;
using Graphics = FortniteV2.Render.Graphics;

namespace FortniteV2.Features
{
    public static class SkeletonEsp
    {
        private static readonly Color[] ColorFriendly = { Color.Aqua, Color.Blue, Color.DodgerBlue };

        private static List<int> _bones = new();
        private static List<bool> _friendly = new();
        private static List<float[]> _xs = new();
        private static List<float[]> _ys = new();
        private static List<float[]> _x2s = new();
        private static List<float[]> _y2s = new();

        public static void Draw(Graphics graphics)
        {
            if (!Config.EnableSkeletonEsp) return;

            var bufferBuilder = Renderer.StartPositionColorLines(2f);
            for (var i = 0; i < _bones.Count; i++)
                try
                {
                    if (_bones[i] < 1) continue;
                    Renderer.BufferColorGradientLineGroup(bufferBuilder, _xs[i], _ys[i], _x2s[i], _y2s[i], _friendly[i] ? ColorFriendly : graphics.Rainbow);
                }
                catch (IndexOutOfRangeException)
                {
                    // player amount changed while loading in, pass
                }

            Renderer.End(bufferBuilder);
        }

        public static void CalculateBones(GameProcess gameProcess, GameData gameData)
        {
            if (!Config.EnableSkeletonEsp) return;

            var tbones = new List<int>();
            var tfriendly = new List<bool>();
            var txs = new List<float[]>();
            var tys = new List<float[]>();
            var tx2s = new List<float[]>();
            var ty2s = new List<float[]>();
            foreach (var entity in gameData.Entities)
            {
                if (!entity.IsAlive() || entity.PawnBase == gameData.Player.PawnBase) continue;
                var friendly = entity.Team == gameData.Player.Team;
                var height = gameProcess.WindowRectangle.Height;
                var skeleton = new List<Bone>();
                skeleton.Add(new Bone(entity.BonePos["head"], entity.BonePos["neck_0"]));
                skeleton.Add(new Bone(entity.BonePos["neck_0"], entity.BonePos["spine_1"]));
                skeleton.Add(new Bone(entity.BonePos["spine_1"], entity.BonePos["spine_2"]));
                skeleton.Add(new Bone(entity.BonePos["spine_2"], entity.BonePos["pelvis"]));
                skeleton.Add(new Bone(entity.BonePos["spine_1"], entity.BonePos["arm_upper_L"]));
                skeleton.Add(new Bone(entity.BonePos["arm_upper_L"], entity.BonePos["arm_lower_L"]));
                skeleton.Add(new Bone(entity.BonePos["arm_lower_L"], entity.BonePos["hand_L"]));
                skeleton.Add(new Bone(entity.BonePos["spine_1"], entity.BonePos["arm_upper_R"]));
                skeleton.Add(new Bone(entity.BonePos["arm_upper_R"], entity.BonePos["arm_lower_R"]));
                skeleton.Add(new Bone(entity.BonePos["arm_lower_R"], entity.BonePos["hand_R"]));
                skeleton.Add(new Bone(entity.BonePos["pelvis"], entity.BonePos["leg_upper_L"]));
                skeleton.Add(new Bone(entity.BonePos["leg_upper_L"], entity.BonePos["leg_lower_L"]));
                skeleton.Add(new Bone(entity.BonePos["leg_lower_L"], entity.BonePos["ankle_L"]));
                skeleton.Add(new Bone(entity.BonePos["pelvis"], entity.BonePos["leg_upper_R"]));
                skeleton.Add(new Bone(entity.BonePos["leg_upper_R"], entity.BonePos["leg_lower_R"]));
                skeleton.Add(new Bone(entity.BonePos["leg_lower_R"], entity.BonePos["ankle_R"]));

                var xs = new float[skeleton.Count];
                var ys = new float[skeleton.Count];
                var x2s = new float[skeleton.Count];
                var y2s = new float[skeleton.Count];
                var bones = 0;
                for (var i = 0; i < skeleton.Count; i++)
                {
                    var bone = skeleton[i];
                    var pos1 = bone.pos1.WorldToScreen(gameData);
                    var pos2 = bone.pos2.WorldToScreen(gameData);
                    if (pos1.Z >= 1 || pos2.Z >= 1) continue;
                    if (pos1.X == pos2.X && pos1.Y == pos2.Y) continue;
                    bones++;
                    xs[i] = pos1.X;
                    ys[i] = height - pos1.Y;
                    x2s[i] = pos2.X;
                    y2s[i] = height - pos2.Y;
                }

                tbones.Add(bones);
                tfriendly.Add(friendly);
                txs.Add(xs);
                tys.Add(ys);
                tx2s.Add(x2s);
                ty2s.Add(y2s);
            }

            _bones = tbones;
            _friendly = tfriendly;
            _xs = txs;
            _ys = tys;
            _x2s = tx2s;
            _y2s = ty2s;
        }

        private record struct Bone(Vector3 pos1, Vector3 pos2);
    }
}