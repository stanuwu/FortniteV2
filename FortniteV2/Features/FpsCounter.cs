using System;
using System.Diagnostics;

namespace FortniteV2.Features
{
    public class FpsCounter
    {
        private static readonly TimeSpan TimeSpanFpsUpdate = TimeSpan.FromSeconds(1);

        private Stopwatch FpsTimer { get; } = Stopwatch.StartNew();

        private int FpsFrameCount { get; set; }

        public double Fps { get; private set; }

        public void Update()
        {
            var fpsTimerElapsed = FpsTimer.Elapsed;
            if (fpsTimerElapsed > TimeSpanFpsUpdate)
            {
                Fps = FpsFrameCount / fpsTimerElapsed.TotalSeconds;
                FpsTimer.Restart();
                FpsFrameCount = 0;
            }

            FpsFrameCount++;
        }
    }
}