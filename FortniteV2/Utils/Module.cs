using System;
using System.Diagnostics;

namespace FortniteV2.Utils
{
    public class Module : IDisposable
    {
        public Module(Process process, ProcessModule processModule)
        {
            Process = process;
            ProcessModule = processModule;
        }

        public Process Process { get; private set; }

        public ProcessModule ProcessModule { get; private set; }

        public void Dispose()
        {
            Process = default;

            ProcessModule.Dispose();
            ProcessModule = default;
        }
    }
}