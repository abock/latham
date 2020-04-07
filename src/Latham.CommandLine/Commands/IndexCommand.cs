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
                int totalItems = 0;
                TimeSpan totalDuration = default;
                long totalSize = 0;

                IEnumerable<IngestionItem> YieldAndLog()
                {
                    foreach (var item in Ingestion.EnumerateIngestionFiles(project, parseMetadataFromFile: !NoMetadata))
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

                if (DryRun)
                {
                    var enumerator = YieldAndLog().GetEnumerator();
                    while (enumerator.MoveNext());
                }
                else
                {
                    index.Reset();
                    index.Insert(YieldAndLog());
                }

                Log.Information("Count = {TotalItems}", totalItems);
                Log.Information("Duration = {TotalDuration}", totalDuration);
                Log.Information("Size = {TotalSize} GB", totalSize / 1024.0 / 1024.0 / 1024.0);

                return 0;
            }
        }

        public sealed class AddCommand : IndexCommand
        {
            public AddCommand(CommandSet commandSet) : base(
                commandSet,
                "add",
                "Add or update files to the index for the provided project.")
            {
            }

            protected override int Invoke(ProjectInfo project, IngestionIndex index)
            {
                index.Insert(Ingestion.EnumerateIngestionFiles(project, InputFiles));
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
