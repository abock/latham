//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.ProcessControl;

namespace Latham
{
    public sealed class Timelapse
    {
        public IReadOnlyList<string> InputFiles { get; }
        public TimeSpan? TotalInputDuration { get; }
        public string OutputFile { get; }
        public double? TimeScaleFactor { get; }
        public TimeSpan? DesiredDuration { get; }

        public Timelapse(
            IReadOnlyList<string> inputFiles,
            TimeSpan? totalInputDuration,
            string outputFile,
            double? timeScaleFactor,
            TimeSpan? desiredDuration)
        {
            InputFiles = inputFiles
                ?? throw new ArgumentNullException(nameof(inputFiles));

            TotalInputDuration = totalInputDuration;

            OutputFile = outputFile
                ?? throw new ArgumentNullException(nameof(outputFile));

            TimeScaleFactor = timeScaleFactor;
            DesiredDuration = desiredDuration;
        }

        public async Task InvokeAsync(CancellationToken cancellationToken = default)
        {
            double? speed = null;

            if (DesiredDuration.HasValue)
                speed = CalculateTotalInputDuration(cancellationToken) / DesiredDuration.Value;
            else if (TimeScaleFactor.HasValue)
                speed = TimeScaleFactor.Value;

            var inputListFilePath = Path.ChangeExtension(Path.GetTempFileName(), ".txt");

            try
            {
                using (var inputListFileWriter = new StreamWriter(inputListFilePath, append: false))
                {
                    foreach (var inputFile in InputFiles)
                    {
                        inputListFileWriter.Write("file ");
                        inputListFileWriter.WriteLine(inputFile);
                    }
                }

                var tmpOutputFile = OutputFile + ".tmp.mp4";

                File.Delete(tmpOutputFile);

                var execStatus = await Exec.RunAsync(
                    output => {
                        Console.Write(output.Data);
                    },
                    "ffmpeg",
                    "-f", "concat",
                    "-safe", "0",
                    "-i", inputListFilePath,
                    "-an",
                    "-filter:v",
                    $"setpts=PTS/{speed}",
                    tmpOutputFile);

                File.Delete(OutputFile);
                File.Move(tmpOutputFile, OutputFile, true);
            }
            finally
            {
                File.Delete(inputListFilePath);
            }
        }

        TimeSpan CalculateTotalInputDuration(CancellationToken cancellationToken)
        {
            if (TotalInputDuration.HasValue)
                return TotalInputDuration.Value;

            var totalDuration = TimeSpan.Zero;

            foreach (var inputFile in InputFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();
                totalDuration += FFMpeg.ParseDuration(inputFile);
            }

            return totalDuration;
        }
    }
}
