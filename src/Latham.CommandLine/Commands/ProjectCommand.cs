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

using Newtonsoft.Json;

using Latham.Project.Model;

namespace Latham.Commands
{
    sealed class ProjectCommandSet : CommandSet
    {
        public ProjectCommandSet() : base("project")
        {
            Add(new ProjectCommand.DumpCommand());
            Add(new ProjectCommand.ScheduleCommand());
        }
    }

    abstract class ProjectCommand : ReflectionCommand
    {
        [Option("p|project=", "A latham.json project file.", Required = true)]
        public string? ProjectFilePath { get; set; }

        [Option("<>", "Project files", Hidden = true)]
        public List<string> InputFiles { get; } = new List<string>();

        protected ProjectCommand(string name, string? help) : base(name, help)
        {
        }

        protected override int? BeforeInvoke(string directive)
        {
            if (ProjectFilePath is null && InputFiles.Count >= 1)
            {
                ProjectFilePath = InputFiles[0];
                InputFiles.RemoveAt(0);
            }

            return base.BeforeInvoke(directive);
        }

        protected sealed override int Invoke()
        {
            if (ProjectFilePath is null)
                throw new OptionException("A project file is required", "project");

            var projectInfo = ProjectInfo.FromFile(ProjectFilePath);

            return Invoke(projectInfo);
        }

        protected abstract int Invoke(ProjectInfo project);

        public sealed class DumpCommand : ProjectCommand
        {
            [Option("e|evaluate", "Evaluate the project file.")]
            public bool Evaluate { get; set; }

            public DumpCommand() : base(
                "dump",
                "Dump project file's JSON representations.")
            {
            }

            protected override int Invoke(ProjectInfo project)
            {
                var serializerSettings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore,

                };

                if (Evaluate)
                {
                    project = project.Evaluate();
                    serializerSettings.NullValueHandling = NullValueHandling.Include;
                }

                Console.WriteLine(JsonConvert.SerializeObject(
                    project,
                    serializerSettings));

                return 0;
            }
        }

        public sealed class ScheduleCommand : ProjectCommand
        {
            [Option("s|start=", "The start time for the schedule. Defaults to current time.")]
            public DateTime StartTime { get; set; } = DateTime.Now;

            [Option("e|end=", "The end time for the schedule. If specified, --count is ignored.")]
            public DateTime? EndTime { get; set; }

            [Option("c|count=", "Show up to VALUE number of timings starting at the provided --start time. Defaults to 100.")]
            public int Count { get; set; } = 100;

            public ScheduleCommand() : base(
                "schedule",
                "Show the timelapse schedule for a project.")
            {
            }

            protected override int Invoke(ProjectInfo project)
            {
                project = project.Evaluate();

                if (project.Recordings is null || project.Recordings.Sources.Count == 0)
                {
                    Console.Error.WriteLine("No recording sources are configured on this project.");
                    return 1;
                }

                if (!EndTime.HasValue)
                    EndTime = DateTime.MaxValue;

                foreach (var recordingSource in project.Recordings.Sources)
                {
                    if (!recordingSource.Schedule.HasValue)
                        continue;

                    var timelapse = project.GetTimelapse(recordingSource);

                    int n = 0;

                    foreach (var time in recordingSource.Schedule.Value.GetNextOccurrences(StartTime, EndTime.Value))
                    {
                        if (timelapse is object && !timelapse.EvaluatePredicates(time))
                            continue;

                        Console.WriteLine($"{n} {recordingSource.Tag} @ {time}");

                        if (++n >= Count && EndTime == DateTime.MaxValue)
                            break;
                    }
                }

                return 0;
            }
        }
    }
}
