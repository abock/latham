//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Mono.Options;
using Mono.Options.Reflection;

using Serilog;

using Latham.Project;
using Latham.Project.Model;

namespace Latham.Commands
{
    sealed class IndexCommandSet : CommandSet
    {
        public IndexCommandSet() : base("index")
        {
            Add(new IndexCommand.RebuildCommand(this));
            Add(new IndexCommand.AddCommand(this));
            Add(new IndexCommand.QueryCommand(this));
        }
    }

    abstract class IndexCommand : ProjectCommand
    {
        [Option("i|index=", "The index file path to use.")]
        public string? IndexFilePath { get; set; }

        protected IndexCommand(
            CommandSet commandSet,
            string name,
            string? help)
            : base(
                commandSet,
                name,
                help)
        {
        }

        protected sealed override int Invoke(ProjectInfo projectInfo)
        {
            using var index = IndexFilePath is null
                ? new IngestionIndex(projectInfo)
                : new IngestionIndex(IndexFilePath);

            return Invoke(projectInfo, index);
        }

        protected abstract int Invoke(ProjectInfo project, IngestionIndex index);

        void InsertItems(IngestionIndex index, IEnumerable<IngestionItem> items, bool dryRun, bool resetIndex = false)
        {
            int totalItems = 0;
            TimeSpan totalDuration = default;
            long totalSize = 0;

            IEnumerable<IngestionItem> YieldAndLog()
            {
                foreach (var item in items)
                {
                    Log.Information(
                        "[{TotalItems}] {FilePath} {Duration} @ {Timestamp}",
                        ++totalItems,
                        item.FilePath,
                        item.Duration,
                        item.Timestamp);

                    if (item.Duration.HasValue)
                        totalDuration += item.Duration.Value;

                    if (item.FileSize.HasValue)
                        totalSize += item.FileSize.Value;

                    yield return item;
                }
            }

            if (dryRun)
            {
                var enumerator = YieldAndLog().GetEnumerator();
                while (enumerator.MoveNext());
            }
            else
            {
                if (resetIndex)
                    index.Reset();
                index.Insert(YieldAndLog());
            }

            Log.Information("Count = {TotalItems}", totalItems);
            Log.Information("Duration = {TotalDuration}", totalDuration);
            Log.Information("Size = {TotalSize} GB", totalSize / 1024.0 / 1024.0 / 1024.0);
        }

        public sealed class RebuildCommand : IndexCommand
        {
            [Option("dry-run", "Do not update the index on disk, just walk the file system.")]
            public bool DryRun { get; set; }

            [Option("no-metadata", "Do not read metadata (e.g. duration) from files.")]
            public bool NoMetadata { get; set; }

            public RebuildCommand(CommandSet commandSet) : base(
                commandSet,
                "rebuild",
                "Rebuild the ingestion index from disk for the provided project.")
            {
            }

            protected override int Invoke(ProjectInfo project, IngestionIndex index)
            {
                InsertItems(
                    index,
                    Ingestion.EnumerateIngestionFiles(
                        project,
                        parseMetadataFromFile: !NoMetadata),
                    DryRun,
                    resetIndex: true);
                return 0;
            }
        }

        public sealed class AddCommand : IndexCommand
        {
            [Option("dry-run", "Do not update the index on disk, just walk the file system.")]
            public bool DryRun { get; set; }

            [Option("no-metadata", "Do not read metadata (e.g. duration) from files.")]
            public bool NoMetadata { get; set; }

            public AddCommand(CommandSet commandSet) : base(
                commandSet,
                "add",
                "Add or update files to the index for the provided project. " +
                "If no files are provided, this command will attempt to add missing" +
                "files that match the ingestion filters since the last index addition.")
            {
            }

            protected override int Invoke(ProjectInfo project, IngestionIndex index)
            {
                if (InputFiles.Count > 0)
                {
                    InsertItems(
                        index,
                        Ingestion.EnumerateIngestionFiles(
                            project,
                            parseMetadataFromFile:
                            !NoMetadata,
                            InputFiles),
                        DryRun);
                    return 0;
                }

                project = project.Evaluate(expandPaths: false);

                if (project.Recordings is null || project.Recordings.Sources.Count == 0)
                {
                    Console.Error.WriteLine("No recording sources are configured on this project.");
                    return 1;
                }

                InsertItems(
                    index,
                    FindNewIngestionItems(),
                    DryRun);

                IEnumerable<IngestionItem> FindNewIngestionItems()
                {
                    var newestTimestamp = index.SelectNewestTimestamp();

                    #nullable disable
                    foreach (var recordingSource in project.Recordings.Sources)
                    #nullable restore
                    {
                        if (!recordingSource.Schedule.HasValue)
                            continue;

                        // Unfortunately NCrontab doesn't work with DateTimeOffset, so normalize to UTC.
                        var expectedRecordingTimes = recordingSource
                            .Schedule
                            .Value
                            .GetNextOccurrences(
                                newestTimestamp.UtcDateTime,
                                DateTime.UtcNow);

                        var basePaths = new List<string>();

                        foreach (var expectedRecordingTime in expectedRecordingTimes)
                        {
                            if (Path.GetDirectoryName(
                                recordingSource.CreateOutputPath(
                                    expectedRecordingTime)) is string expectedDirectory)

                                if (!basePaths.Contains(expectedDirectory))
                                    basePaths.Add(expectedDirectory);
                        }

                        foreach (var ingestion in project.Ingestions)
                        {
                            foreach (var basePath in basePaths)
                            {
                                var adjustedIngestion = new IngestionInfo(
                                    Path.GetFileName(ingestion.PathGlob),
                                    basePath,
                                    ingestion.PathFilter);

                                foreach (var item in Ingestion.EnumerateIngestionFiles(
                                    adjustedIngestion,
                                    parseMetadataFromFile: !NoMetadata,
                                    basePath: ingestion.BasePath))
                                    yield return item.WithFilePath(Path.Combine(basePath, item.FilePath));
                            }
                        }
                    }
                }

                return 0;
            }
        }

        public sealed class QueryCommand : IndexCommand
        {
            [Option("f|timelapse-filter", "Apply any timelapse filter predicates")]
            public bool ApplyTimelapseFilters { get; set; }

            [Option("t|tag=", "Select items only matching the given tag")]
            public string? Tag { get; set; }

            public QueryCommand(CommandSet commandSet) : base(
                commandSet,
                "query",
                "Query the index for the provided project.")
            {
            }

            protected override int Invoke(ProjectInfo project, IngestionIndex index)
            {
                var totalDuration = TimeSpan.Zero;

                var items = index
                    .SelectAll()
                    .Where(item => Tag is null || item.Tag == Tag)
                    .Where(item => !ApplyTimelapseFilters || project.IncludeInTimelapse(item))
                    .OrderBy(item => item.Tag)
                    .ThenBy(item => item.Timestamp)
                    .Select(item =>
                    {
                        item = item.WithFilePath(Path.Combine(project.BasePath ?? ".", item.FilePath));
                        if (!item.Duration.HasValue)
                            item = item.WithDurationAndFileSizeByReadingFile();
                        return item;
                    });

                foreach (var item in items)
                {
                    if (item.Duration.HasValue)
                        totalDuration += item.Duration.Value;

                    Console.WriteLine(item.FilePath);
                }

                Console.WriteLine($"--total-input-duration {totalDuration}");

                return 0;
            }
        }
    }
}
