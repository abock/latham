//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

using Mono.Options;
using Mono.Options.Reflection;

namespace Latham.Commands
{
    sealed class TimelapseCommand : ReflectionCommand
    {
        [Option("d|duration=", "Create a timelapse of a specific duration")]
        public TimeSpan? DesiredDuration { get; set; }

        [Option("s|speed=", "Create a timelapse speed up (or down) by a factor of VALUE")]
        public double? TimeScaleFactor { get; set; }

        [Option("o|output=", "Output file for the timelapse", Required = true)]
        public string? OutputFile { get; set; }

        [Option("<>", "Input files for the timelapse", Hidden = true)]
        public List<string> InputFiles { get; } = new List<string>();

        public TimelapseCommand() : base("timelapse", "Some help")
        {
        }

        bool outputCulledFromInput;

        protected override int? BeforeInvoke(string directive)
        {
            outputCulledFromInput = false;

            if (OutputFile is null && InputFiles.Count > 0)
            {
                OutputFile = InputFiles[InputFiles.Count - 1];
                InputFiles.RemoveAt(InputFiles.Count - 1);
                outputCulledFromInput = true;
            }

            return base.BeforeInvoke(directive);
        }

        protected override int Invoke()
        {
            if (InputFiles.Count <= 0)
                throw outputCulledFromInput
                    ? new Exception(
                        $"At least one input file must be specified. " +
                        $"'{OutputFile}' was interpreted as an output file.")
                    : new Exception("At least one input file must be specified.");

            if (OutputFile is null)
                throw new Exception("Output file must be specified.");

            var timelapse = new Timelapse(
                InputFiles,
                OutputFile,
                TimeScaleFactor,
                DesiredDuration);

            timelapse
                .InvokeAsync()
                .GetAwaiter()
                .GetResult();

            return 0;
        }
    }
}
