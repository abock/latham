//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

namespace Latham.Project.Model
{
    public sealed class RecordingInfo : IProjectInfoNode<RecordingInfo, ProjectInfo>
    {
        [JsonIgnore]
        public ProjectInfo? Parent { get; internal set; }

        [JsonIgnore]
        public ProjectInfo? Project => Parent;

        [JsonProperty("outputPath", Order = 0)]
        public string? OutputPath { get; }

        [JsonProperty("schedule", Order = 10)]
        public CrontabSchedule? Schedule { get; }

        [JsonProperty("duration", Order = 20)]
        [JsonConverter(typeof(JsonFriendlyTimeSpanConverter))]
        public FriendlyTimeSpan? Duration { get; }

        [JsonProperty("unifiProtectEndpoints", Order = 30)]
        IReadOnlyList<Uri>? unifiProtectEndpoints;

        [JsonIgnore]
        public IReadOnlyList<Uri> UnifiProtectEndpoints
            => unifiProtectEndpoints ?? Array.Empty<Uri>();

        [JsonProperty("sources", Order = 40)]
        readonly IReadOnlyList<RecordingSourceInfo>? sources;

        [JsonIgnore]
        public IReadOnlyList<RecordingSourceInfo> Sources
            => sources ?? Array.Empty<RecordingSourceInfo>();

        [JsonConstructor]
        public RecordingInfo(
            string? outputPath,
            CrontabSchedule? schedule,
            FriendlyTimeSpan? duration,
            IReadOnlyList<Uri>? unifiProtectEndpoints,
            IReadOnlyList<RecordingSourceInfo>? sources)
        {
            OutputPath = outputPath;
            Schedule = schedule;
            Duration = duration;
            this.unifiProtectEndpoints = unifiProtectEndpoints;
            this.sources = sources;

            foreach (var source in Sources)
                source.Parent = this;
        }

        public RecordingInfo Evaluate(bool expandPaths)
            => new RecordingInfo(
                OutputPath,
                Schedule,
                Duration,
                UnifiProtectEndpoints,
                Sources?
                    .Select(source => source.Evaluate(expandPaths))
                    .ToList());
    }
}
