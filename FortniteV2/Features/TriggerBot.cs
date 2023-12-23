using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FortniteV2.Game;
using FortniteV2.Sys;
using FortniteV2.Utils;

namespace FortniteV2.Features
{
    public class TriggerBot : TickThread
    {
        public TriggerBot(GameProcess gameProcess, GameData gameData) : base(nameof(TriggerBot))
        {
            GameProcess = gameProcess;
            GameData = gameData;
            Random = new Random();
        }

        public static bool IsOn { get; private set; }
        private GameProcess GameProcess { get; set; }
        private GameData GameData { get; set; }
        private Random Random { get; }
        private bool State { get; set; }
        private bool LastState { get; set; }

        protected override void Tick()
        {
            if (!Config.EnableTriggerBot) return;
            if (!GameProcess.IsValid) return;

            var delayConfig = Config.TriggerBotDelayMs;
            var randomConfig = Config.TriggerBotDelayRandomMs / 2;

            var delay = Random.Next(Math.Max(delayConfig - randomConfig, 0), Math.Max(delayConfig + randomConfig, 0));

            Application.Current.Dispatcher.Invoke(() => { IsOn = Config.TriggerBotKey != Key.None && Keyboard.IsKeyDown(Config.TriggerBotKey); });

            if (State != LastState && !State)
                Task.Run(() =>
                {
                    Thread.Sleep(delay + 10);
                    Util.MouseEvent(User32.MouseEventFlags.LeftUp);
                });

            if (State != LastState && State)
                Task.Run(() =>
                {
                    Thread.Sleep(delay);
                    Util.MouseEvent(User32.MouseEventFlags.LeftDown);
                });

            LastState = State;

            if (!IsOn)
            {
                State = false;
                return;
            }

            var entityId = GameData.Player.TargetedEntityIndex;
            if (entityId < 0)
            {
                State = false;
                return;
            }

            var targetEntry = GameProcess.Process.Read<IntPtr>(GameData.Player.EntityList + 8 * (entityId >> 9) + 16);
            var target = GameProcess.Process.Read<IntPtr>(targetEntry + 120 * (entityId & 0x1FF));

            var targetTeam = GameProcess.Process.Read<int>(target + Offsets.m_iTeamNum).ToTeam();
            var targetHealth = GameProcess.Process.Read<int>(target + Offsets.m_iHealth);

            State = targetTeam != GameData.Player.Team && targetHealth > 0;
        }

        public override void Dispose()
        {
            base.Dispose();

            GameProcess = default;
            GameData = default;
        }
    }
}