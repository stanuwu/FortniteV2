using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using FortniteV2.Data;
using FortniteV2.Utils;
using HijackOverlay.Render;
using HijackOverlay.Render.Buffer;
using Graphics = FortniteV2.Render.Graphics;

namespace FortniteV2.Features
{
    public static class SkeletonEsp
    {
        private static readonly Color[] ColorFriendly = { Color.Aqua, Color.Blue, Color.DodgerBlue };

        public static void Draw(Graphics graphics)
        {
            if (!Config.EnableSkeletonEsp) return;

            var bufferBuilder = Renderer.StartPositionColorLines();
            foreach (var entity in graphics.GameData.Entities)
            {
                if (!entity.IsAlive() || entity.PawnBase == graphics.GameData.Player.PawnBase) continue;

                DrawBones(graphics, bufferBuilder, entity, entity.Team == graphics.GameData.Player.Team);
            }

            Renderer.End(bufferBuilder);
        }

        private static void DrawBones(Graphics graphics, BufferBuilder bufferBuilder, Entity entity, bool friendly)
        {
            var height = graphics.GameProcess.WindowRectangle.Height;
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
            for (var i = 0; i < skeleton.Count; i++)
            {
                var bone = skeleton[i];
                var pos1 = bone.pos1.WorldToScreen(graphics.GameData);
                var pos2 = bone.pos2.WorldToScreen(graphics.GameData);
                if (pos1.Z >= 1 || pos2.Z >= 1) return;
                ;
                xs[i] = pos1.X;
                ys[i] = height - pos1.Y;
                x2s[i] = pos2.X;
                y2s[i] = height - pos2.Y;
            }

            if (friendly) Renderer.BufferColorGradientLineGroup(bufferBuilder, xs, ys, x2s, y2s, ColorFriendly);
            else Renderer.BufferColorGradientLineGroup(bufferBuilder, xs, ys, x2s, y2s, graphics.Rainbow);
        }

        private record struct Bone(Vector3 pos1, Vector3 pos2);
    }
}