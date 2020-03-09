using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace Latham
{
    sealed class PosixDaemon : Daemon
    {
        readonly Dictionary<int, sig_t> previousSignalHandlers = new Dictionary<int, sig_t>();
        readonly sig_t signalHandler;
        readonly string pidFile;
        readonly Func<string> statusHandler;

        public PosixDaemon(string pidFile, Func<string> statusHandler)
        {
            this.pidFile = pidFile;
            this.statusHandler = statusHandler;
            this.signalHandler = new sig_t(SignalHandler);
        }

        public override bool IsSupported => true;

        void SignalHandler(int signal)
        {
            if (previousSignalHandlers.TryGetValue(signal, out var previousHandler))
            {
                previousSignalHandlers.Remove(signal);
                previousHandler(signal);
            }

            if (signal == SIGHUP)
            {
                Console.WriteLine("HUP IGNORE");
                return;
            }

            if (signal == SIGINT)
            {
                if (pidFile is string)
                    File.Delete(pidFile);

                Environment.Exit(0);
            }
        }

        public override Process? GetExistingProcess()
        {
            if (File.Exists(pidFile) && int.TryParse(File.ReadAllText(pidFile), out var existingPid))
            {
                try
                {
                    return Process.GetProcessById(existingPid);
                }
                catch
                {
                }
            }

            return null;
        }

        protected override void OnProcessStarted(Process process)
        {
            File.Delete(pidFile);
            File.WriteAllText(pidFile, process.Id.ToString(CultureInfo.InvariantCulture));
        }

        public override DaemonStartAction Start(out Process process)
        {
            #nullable disable
            process = GetExistingProcess();
            #nullable restore

            if (process is null)
            {
                process = CurrentProcess;
                return DaemonStartAction.StartDaemonProcess;
            }

            if (CurrentProcess.Id != process.Id)
                return DaemonStartAction.AlreadyRunning;

            if (signal(SIGINT, signalHandler) is sig_t previousInt)
                previousSignalHandlers.Add(SIGINT, previousInt);

            if (signal(SIGHUP, signalHandler) is sig_t previousHup)
                previousSignalHandlers.Add(SIGHUP, previousHup);

            return DaemonStartAction.StartNormally;
        }

        public override bool Stop(out Process? process)
        {
            process = GetExistingProcess();
            if (process is null)
                return false;

            kill(process.Id, SIGINT);

            process.WaitForExit();

            return true;
        }

        class PosixException : Exception
        {
            public PosixException(string message) : base(message)
            {
            }
        }

        sealed class PosixErrnoException : PosixException
        {
            public int ErrorNumber { get; }
            public string ErrorNumberMessage { get; }

            public PosixErrnoException(string contextMessage)
                : this(contextMessage, Marshal.GetLastWin32Error())
            {
            }

            public PosixErrnoException(string contextMessage, int errorNumber)
                : this(contextMessage, errorNumber, GetErrorNumberMessage(errorNumber))
            {
            }

            public PosixErrnoException(string contextMessage, int errorNumber, string errorNumberMessage)
                : base($"{contextMessage}: ({errorNumber}) {errorNumberMessage}")
            {
                ErrorNumber = errorNumber;
                ErrorNumberMessage = errorNumberMessage;
            }

            static string GetErrorNumberMessage(int errorNumber)
                #nullable disable
                => Marshal.PtrToStringUTF8(strerror(errorNumber));
                #nullable restore
        }

        [DllImport("libc")]
        static extern IntPtr strerror(int errno);

        [DllImport("libc", EntryPoint = nameof(kill), SetLastError = true)]
        static extern int _kill(int pid, int signal);

        static void kill(int pid, int signal)
        {
            if (_kill(pid, signal) != 0)
                throw new PosixErrnoException($"{nameof(kill)}({pid}, {signal})");
        }

        delegate void sig_t(int signal);

        [DllImport("libc", EntryPoint = nameof(signal), SetLastError = true)]
        static extern IntPtr _signal(int signal, sig_t handler);

        static sig_t? signal(int signal, sig_t handler)
        {
            var previousHandler = _signal(signal, handler);
            var error = (long)previousHandler;
            if (error < 0)
            {
                var message = $"{nameof(signal)}({signal}, <handler>)";
                if (error == -1) // SIG_ERR on Linux and macOS
                    throw new PosixErrnoException(message);

                throw new PosixException($"{message}: returned unknown value '{error}'");
            }

            if (previousHandler == IntPtr.Zero)
                return null;

            return (sig_t)Marshal.GetDelegateForFunctionPointer(previousHandler, typeof(sig_t));
        }

        const int SIGHUP = 1;
        const int SIGINT = 2;
        const int SIGKILL = 9;
        const int SIGINFO = 29;
    }
}
