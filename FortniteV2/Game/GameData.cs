using System.Linq;
using FortniteV2.Data;
using FortniteV2.Features;
using FortniteV2.Utils;

namespace FortniteV2.Game
{
    public class GameData : TickThread
    {
        public GameData(GameProcess gameProcess) : base(nameof(GameData))
        {
            GameProcess = gameProcess;
            Player = new Player();
            Entities = Enumerable.Range(0, 64).Select(index => new Entity(index)).ToArray();
        }

        public GameProcess GameProcess { get; private set; }
        public Player Player { get; private set; }
        public Entity[] Entities { get; private set; }

        public override void Dispose()
        {
            base.Dispose();

            Entities = default;
            Player = default;
            GameProcess = default;
        }

        protected override void Tick()
        {
            if (!GameProcess.IsValid) return;

            Player.Update(GameProcess);
            foreach (var entity in Entities) entity.Update(GameProcess);
            SkeletonEsp.CalculateBones(GameProcess, this);
            BoxEsp.CalculateBox(GameProcess, this);
        }
    }
}