using FortniteV2.Data;
using FortniteV2.Data.Raw;
using FortniteV2.Render;
using SharpDX;

namespace FortniteV2.Features
{
    public static class SkeletonEsp
    {
        private static readonly Color ColorT = Color.Gold;
        private static readonly Color ColorCt = Color.DodgerBlue;

        public static void Draw(Graphics graphics)
        {
            if(!Config.EnableSkeletonEsp) return;
            
            foreach (var entity in graphics.GameData.Entities)
            {
                if (!entity.IsAlive() || entity.PawnBase == graphics.GameData.Player.PawnBase) continue;

                var color = entity.Team == Team.Terrorists ? ColorT : ColorCt;
                DrawBones(graphics, entity, color);
            }
        }

        private static void DrawBones(Graphics graphics, Entity entity, Color color)
        {
            graphics.DrawLineWorld(color, entity.BonePos["head"], entity.BonePos["neck_0"]);
            graphics.DrawLineWorld(color, entity.BonePos["neck_0"], entity.BonePos["spine_1"]);
            graphics.DrawLineWorld(color, entity.BonePos["spine_1"], entity.BonePos["spine_2"]);
            graphics.DrawLineWorld(color, entity.BonePos["spine_2"], entity.BonePos["pelvis"]);
            graphics.DrawLineWorld(color, entity.BonePos["spine_1"], entity.BonePos["arm_upper_L"]);
            graphics.DrawLineWorld(color, entity.BonePos["arm_upper_L"], entity.BonePos["arm_lower_L"]);
            graphics.DrawLineWorld(color, entity.BonePos["arm_lower_L"], entity.BonePos["hand_L"]);
            graphics.DrawLineWorld(color, entity.BonePos["spine_1"], entity.BonePos["arm_upper_R"]);
            graphics.DrawLineWorld(color, entity.BonePos["arm_upper_R"], entity.BonePos["arm_lower_R"]);
            graphics.DrawLineWorld(color, entity.BonePos["arm_lower_R"], entity.BonePos["hand_R"]);
            graphics.DrawLineWorld(color, entity.BonePos["pelvis"], entity.BonePos["leg_upper_L"]);
            graphics.DrawLineWorld(color, entity.BonePos["leg_upper_L"], entity.BonePos["leg_lower_L"]);
            graphics.DrawLineWorld(color, entity.BonePos["leg_lower_L"], entity.BonePos["ankle_L"]);
            graphics.DrawLineWorld(color, entity.BonePos["pelvis"], entity.BonePos["leg_upper_R"]);
            graphics.DrawLineWorld(color, entity.BonePos["leg_upper_R"], entity.BonePos["leg_lower_R"]);
            graphics.DrawLineWorld(color, entity.BonePos["leg_lower_R"], entity.BonePos["ankle_R"]);
        }
    }
}