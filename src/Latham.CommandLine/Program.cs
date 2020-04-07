//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;

using Mono.Options;
using Mono.Options.Reflection;

using Serilog;
using Serilog.Events;

using Latham.Commands;

namespace Latham
{
    static class Program
    {
        public static string? ProjectFilePath { get; private set; }

        static int Main(string[] args)
        {
            FriendlyTimeSpan.RegisterTypeConverter();

            ReflectionCommand.ProgramName = "latham";
            ReflectionCommand.UsageLine = (command, fullCommandName) =>
            {
                var usageLine = $"usage: {ReflectionCommand.ProgramName} [GLOBAL OPTIONS] ";
                if (command is ProjectCommand)
                    usageLine += "-p PROJECT_FILE ";
                return usageLine + $"{fullCommandName} [OPTIONS]";
            };

            string? logFilePath = null;
            int consoleVerbosity = (int)LogEventLevel.Fatal - (int)LogEventLevel.Information;
            int logFileVerbosity = (int)LogEventLevel.Fatal - (int)LogEventLevel.Debug;

            void UpdateLogging()
            {
                LogEventLevel GetLevel(int verbosity)
                {
                    var min = (int)LogEventLevel.Verbose;
                    var max = (int)LogEventLevel.Fatal;
                    verbosity = max - Math.Min(max, Math.Max(verbosity, min));
                    return (LogEventLevel)verbosity;
                }

                var loggerConfiguration = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console(restrictedToMinimumLevel: GetLevel(consoleVerbosity));

                if (logFilePath is string)
                {
                    loggerConfiguration = loggerConfiguration
                        .WriteTo
                        .File(
                            logFilePath,
                            rollingInterval: RollingInterval.Day,
                            restrictedToMinimumLevel: GetLevel(logFileVerbosity));
                }

                Log.Logger = loggerConfiguration.CreateLogger();
            }

            UpdateLogging();

            var commandSet = new CommandSet(ReflectionCommand.ProgramName)
            {
                { $"usage: {ReflectionCommand.ProgramName} [GLOBAL OPTIONS] COMMAND [COMMAND OPTIONS]"},
                { "" },
                { "Global Options:"},
                { "" },
                {
                    "p|project=",
                    "Path to a Latham project file for commands that require one.",
                    v => ProjectFilePath = v
                },
                {
                    "l|log=",
                    "Path to where logs should be written. The file name will be " +
                    "used as a template for creating a rolling log. Defaults to a " +
                    "'logs' directory relative to the provided project file.",
                    v =>
                    {
                        logFilePath = v;
                        UpdateLogging();
                    }
                },
                {
                    "v|verbosity",
                    "Increase or decrease logging verbosity. If --log is specified " +
                    "before this argument, " +"verbosity applies to the log file. If " +
                    "there is no preceding --log argument, verbosity applies to the console.",
                    v =>
                    {
                        ref int verbosity = ref consoleVerbosity;
                        if (logFilePath is string)
                            verbosity = ref logFileVerbosity;
                        verbosity += v is null ? -1 : 1;
                        UpdateLogging();
                    }
                },
                {
                    "ffmpeg=",
                    "Path to the preferred ffmpeg binary. ffmpeg will be used based on PATH if unset.",
                    v => FFMpeg.Path = v
                },
                { "" },
                { "Commands:" },
                { "" },
                new RecordCommandSet(),
                { "" },
                new TimelapseCommand(),
                { "" },
                new IndexCommandSet(),
                { "" },
                new ProjectCommandSet()
            };

            if (args.Length == 0)
                args = new [] { "help" };

            return commandSet.Run(args);
        }
    }
}
