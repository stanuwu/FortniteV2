using System;
using System.Numerics;
using FortniteV2.Data.Raw;
using FortniteV2.Game;
using FortniteV2.Utils;

namespace FortniteV2.Data
{
    public abstract class EntityBase
    {
        public IntPtr EntityList { get; protected set; }
        public IntPtr ControllerBase { get; protected set; }
        public IntPtr PawnBase { get; protected set; }

        public string Name { get; set; }
        public bool LifeState { get; protected set; }
        public int Health { get; protected set; }
        public int Armor { get; protected set; }
        public Team Team { get; protected set; }
        public Vector3 Origin { get; private set; }
        public int ShotsFired { get; private set; }

        public virtual bool IsAlive()
        {
            return ControllerBase != IntPtr.Zero &&
                   PawnBase != IntPtr.Zero &&
                   LifeState &&
                   Health > 0 &&
                   (Team == Team.Terrorists || Team == Team.CounterTerrorists);
        }

        protected abstract IntPtr ReadControllerBase(GameProcess gameProcess);
        protected abstract IntPtr ReadPawnBase(GameProcess gameProcess);

        public virtual bool Update(GameProcess gameProcess)
        {
            EntityList = gameProcess.ModuleClient.Read<IntPtr>(Offsets.dwEntityList);
            ControllerBase = ReadControllerBase(gameProcess);
            PawnBase = ReadPawnBase(gameProcess);
            if (ControllerBase == IntPtr.Zero || PawnBase == IntPtr.Zero) return false;

            var nameChars = new char[128];
            gameProcess.Process.ReadRaw(ControllerBase + Offsets.m_iszPlayerName, nameChars);
            Name = new string(nameChars);

            LifeState = gameProcess.Process.Read<bool>(PawnBase + Offsets.m_lifeState);
            Health = gameProcess.Process.Read<int>(PawnBase + Offsets.m_iHealth);
            Armor = gameProcess.Process.Read<int>(PawnBase + Offsets.m_ArmorValue);
            Team = gameProcess.Process.Read<int>(PawnBase + Offsets.m_iTeamNum).ToTeam();
            Origin = gameProcess.Process.Read<Vector3>(PawnBase + Offsets.m_vOldOrigin);
            ShotsFired = gameProcess.Process.Read<int>(PawnBase + Offsets.m_iShotsFired);

            return true;
        }
    }
}