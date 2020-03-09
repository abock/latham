using System;
using System.Diagnostics;

namespace Latham
{
    sealed class WindowsDaemon : Daemon
    {
        readonly string pidFile;
        readonly Func<string> statusHandler;

        public WindowsDaemon(string pidFile, Func<string> statusHandler)
        {
            this.pidFile = pidFile;
            this.statusHandler = statusHandler;
        }

        public override bool IsSupported => false;

        public override DaemonStartAction Start(out Process process)
            => throw new NotImplementedException();

        public override bool Stop(out Process? process)
            => throw new NotImplementedException();

        public override Process? GetExistingProcess()
            => throw new NotImplementedException();
    }
}
