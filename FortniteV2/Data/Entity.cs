using System;
using System.Collections.Generic;
using FortniteV2.Game;
using FortniteV2.Utils;
using SharpDX;

namespace FortniteV2.Data
{
    public class Entity : EntityBase
    {
        public Entity(int index)
        {
            Index = index;
        }

        public int Index { get; }
        public bool Dormant { get; private set; } = true;

        public Dictionary<string, Vector3> BonePos { get; } = new Dictionary<string, Vector3>
        {
            { "head", Vector3.Zero },
            { "neck_0", Vector3.Zero },
            { "spine_1", Vector3.Zero },
            { "spine_2", Vector3.Zero },
            { "pelvis", Vector3.Zero },
            { "arm_upper_L", Vector3.Zero },
            { "arm_lower_L", Vector3.Zero },
            { "hand_L", Vector3.Zero },
            { "arm_upper_R", Vector3.Zero },
            { "arm_lower_R", Vector3.Zero },
            { "hand_R", Vector3.Zero },
            { "leg_upper_L", Vector3.Zero },
            { "leg_lower_L", Vector3.Zero },
            { "ankle_L", Vector3.Zero },
            { "leg_upper_R", Vector3.Zero },
            { "leg_lower_R", Vector3.Zero },
            { "ankle_R", Vector3.Zero }
        };

        public override bool IsAlive()
        {
            return base.IsAlive() && !Dormant;
        }

        protected override IntPtr ReadControllerBase(GameProcess gameProcess)
        {
            var listEntry = gameProcess.Process.Read<IntPtr>(EntityList + ((8 * (Index & 0x7FFF)) >> 9) + 16);
            if (listEntry == IntPtr.Zero) return IntPtr.Zero;
            return gameProcess.Process.Read<IntPtr>(listEntry + 120 * (Index & 0x1FF));
        }

        protected override IntPtr ReadPawnBase(GameProcess gameProcess)
        {
            var playerPawn = gameProcess.Process.Read<int>(ControllerBase + Offsets.m_hPawn);
            var listEntry2 = gameProcess.Process.Read<IntPtr>(EntityList + 0x8 * ((playerPawn & 0x7FFF) >> 9) + 16);
            if (listEntry2 == IntPtr.Zero) return IntPtr.Zero;
            return gameProcess.Process.Read<IntPtr>(listEntry2 + 120 * (playerPawn & 0x1FF));
        }

        public override bool Update(GameProcess gameProcess)
        {
            if (!base.Update(gameProcess)) return false;

            Dormant = gameProcess.Process.Read<bool>(PawnBase + Offsets.m_bDormant);
            if (!IsAlive()) return true;

            UpdateBonePos(gameProcess);

            return true;
        }

        private void UpdateBonePos(GameProcess gameProcess)
        {
            var gameSceneNode = gameProcess.Process.Read<IntPtr>(PawnBase + Offsets.m_pGameSceneNode);
            var boneArray = gameProcess.Process.Read<IntPtr>(gameSceneNode + Offsets.m_modelState + 128);
            foreach (var data in Offsets.BONES)
            {
                var name = data.Key;
                var index = data.Value;

                var boneAddress = boneArray + index * 32;
                var bonePos = gameProcess.Process.Read<Vector3>(boneAddress);
                BonePos[name] = bonePos;
            }
        }
    }
}