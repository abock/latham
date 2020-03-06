//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;

using Newtonsoft.Json;

namespace Latham.Project.Model
{
    public sealed class RecordingSourceInfo : IProjectInfoNode<RecordingSourceInfo, RecordingInfo>
    {
        [JsonIgnore]
        public RecordingInfo? Parent { get; internal set; }

        [JsonIgnore]
        public ProjectInfo? Project => Parent?.Parent;

        [JsonProperty("tag", Order = 0)]
        public string Tag { get; }

        [JsonProperty("uri", Order = 10)]
        public Uri Uri { get; }

        [JsonProperty("outputPath", Order = 20)]
        public string? OutputPath { get; }

        [JsonProperty("schedule", Order = 30)]
        public CrontabSchedule? Schedule { get; }

        [JsonProperty("duration", Order = 40)]
        [JsonConverter(typeof(JsonFriendlyTimeSpanConverter))]
        public FriendlyTimeSpan? Duration { get; }

        [JsonProperty("unifiProtectEndpoint")]
        public Uri? UnifiProtectEndpoint { get; }

        [JsonConstructor]
        public RecordingSourceInfo(
            string tag,
            Uri uri,
            string? outputPath,
            CrontabSchedule? schedule,
            FriendlyTimeSpan? duration,
            Uri? unifiProtectEndpoint)
        {
            Tag = tag
                ?? throw new ArgumentNullException(nameof(tag));
            Uri = uri
                ?? throw new ArgumentNullException(nameof(uri));
            OutputPath = outputPath;
            Schedule = schedule;
            Duration = duration;
            UnifiProtectEndpoint = unifiProtectEndpoint;
        }

        public RecordingSourceInfo Evaluate(bool expandPaths)
        {
            return new RecordingSourceInfo(
                Tag,
                Uri,
                expandPaths ?
                    PathCombine(Project?.BasePath, OutputPath ?? Parent?.OutputPath)
                    : OutputPath ?? Parent?.OutputPath,
                Schedule ?? Parent?.Schedule,
                Duration ?? Parent?.Duration,
                GetUnifiProtectEndpoint());

            static string? PathCombine(string? a, string? b)
            {
                if (a is null)
                    return b;
                else if (b is null)
                    return a;
                return Path.Combine(a, b);
            }

            Uri? GetUnifiProtectEndpoint()
            {
                if (UnifiProtectEndpoint is Uri)
                    return UnifiProtectEndpoint;

                if (Uri is null)
                    return null;

                if (Parent is RecordingInfo)
                {
                    foreach (var endpoint in Parent.UnifiProtectEndpoints)
                    {
                        if (endpoint.Host == Uri.Host)
                            return endpoint;
                    }
                }

                return null;
            }
        }

        public string? CreateOutputPath(DateTimeOffset? localNowTimeOpt = null)
        {
            if (Project is null)
                throw new NullReferenceException(
                    $"Instance is not associated with a {nameof(ProjectInfo)}");

            var outputFile = Project.Interpolate(
                OutputPath,
                interpolation => interpolation.VariableLowerInvariant switch
                {
                    "tag" => Tag,
                    _ => null
                },
                localNowTimeOpt)?.ToString();

            return Project.SanitizePath(outputFile);
        }
    }
}
