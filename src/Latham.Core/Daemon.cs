using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Latham
{
    public enum DaemonStartAction
    {
        None,
        AlreadyRunning,
        StartDaemonProcess,
        StartNormally
    }

    public abstract class Daemon
    {
        public static Daemon Create(string pidFile, Func<string> statusHandler)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return new WindowsDaemon(pidFile, statusHandler);
            else
                return new PosixDaemon(pidFile, statusHandler);
        }

        private protected Daemon()
        {
        }

        public Process CurrentProcess { get; } = Process.GetCurrentProcess();

        public abstract bool IsSupported { get; }
        public abstract DaemonStartAction Start(out Process process);
        public abstract bool Stop(out Process? process);
        public abstract Process? GetExistingProcess();

        protected virtual void OnProcessStarted(Process process)
        {
        }

        public virtual Process StartProcess(string command, IEnumerable<string> arguments)
                => Process.Start(command, CommandLine.ToString(arguments));

        public Process StartProcess(IEnumerable<string>? appendExtraArguments = null)
        {
            var arguments = Environment.GetCommandLineArgs().Skip(1);
            if (appendExtraArguments is object)
                arguments = arguments.Concat(appendExtraArguments);

            var process = StartProcess(
                CurrentProcess.MainModule.FileName,
                arguments);

            OnProcessStarted(process);

            return process;
        }
    }
}
