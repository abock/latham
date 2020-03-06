//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

namespace Latham.Project.Model
{
    public sealed class ProjectInfo : IProjectInfoNode<ProjectInfo, ProjectInfo>
    {
        ProjectInfo? IProjectInfoNode<ProjectInfo, ProjectInfo>.Parent => null;
        ProjectInfo? IProjectInfoNode<ProjectInfo, ProjectInfo>.Project => this;

        [JsonProperty("name", Order = 0)]
        public string? Name { get; }

        [JsonProperty("description", Order = 10)]
        public string? Description { get; }

        [JsonProperty(Order = 20)]
        readonly string? basePath;

        string? computedBasePath;

        [JsonIgnore]
        public string? BasePath {
            get => computedBasePath ?? basePath;
            set => computedBasePath = value;
        }

        [JsonProperty("ingestionIndexFilePath", Order = 30)]
        public string? IngestionIndexFilePath { get; }

        [JsonProperty("recordings", Order = 40)]
        public RecordingInfo? Recordings { get; }

        [JsonProperty("ingestions", Order = 50)]
        readonly IReadOnlyList<IngestionInfo>? ingestions;

        [JsonIgnore]
        public IReadOnlyList<IngestionInfo> Ingestions
            => ingestions ?? Array.Empty<IngestionInfo>();

        [JsonProperty("timelapses", Order = 60)]
        readonly IReadOnlyList<TimelapseInfo>? timelapses;

        [JsonIgnore]
        public IReadOnlyList<TimelapseInfo> Timelapses
            => timelapses ?? Array.Empty<TimelapseInfo>();

        [JsonIgnore]
        public string? FilePath { get; private set; }

        [JsonConstructor]
        public ProjectInfo(
            string? name,
            string? description,
            string? basePath,
            string? ingestionIndexFilePath,
            RecordingInfo? recordings,
            IReadOnlyList<IngestionInfo>? ingestions,
            IReadOnlyList<TimelapseInfo>? timelapses)
        {
            Name = name;
            Description = description;
            this.basePath = basePath;
            IngestionIndexFilePath = ingestionIndexFilePath;
            Recordings = recordings;
            this.ingestions = ingestions;
            this.timelapses = timelapses;

            if (recordings is object)
                recordings.Parent = this;

            if (ingestions is object)
            {
                foreach (var ingestion in ingestions)
                    ingestion.Parent = this;
            }

            if (timelapses is object)
            {
                foreach (var timelapse in timelapses)
                    timelapse.Parent = this;
            }
        }

        public static ProjectInfo FromFile(string projectFilePath)
        {
            using var streamReader = new StreamReader(projectFilePath);
            using var jsonTextReader = new JsonTextReader(streamReader);
            var projectInfo = new JsonSerializer().Deserialize<ProjectInfo>(jsonTextReader);

            if (projectInfo is null)
                throw new Exception(
                    $"Unable to deserialize a {nameof(ProjectInfo)} " +
                    $"instance from {projectFilePath}.");

            projectInfo.FilePath = projectFilePath;

            var projectFileDirectory = Path.GetDirectoryName(projectFilePath) ?? ".";

            projectInfo.BasePath = string.IsNullOrEmpty(projectInfo.basePath)
                ? projectFileDirectory
                : Path.Combine(projectInfo.basePath, projectFileDirectory);

            return projectInfo;
        }

        public ProjectInfo Evaluate(bool expandPaths = true)
            => new ProjectInfo(
                Name,
                Description,
                BasePath,
                IngestionIndexFilePath,
                Recordings?.Evaluate(expandPaths),
                Ingestions?.Select(ingestion => ingestion.Evaluate(expandPaths)).ToList(),
                Timelapses?.Select(timelapse => timelapse.Evaluate(expandPaths)).ToList());

        public FormattableString? Interpolate(
            string? str,
            Func<TemplateString.Interpolation, object?>? interpolationHandler = null,
            DateTimeOffset? localNowTimeOpt = null)
        {
            var localNowTime = localNowTimeOpt.HasValue
                ? localNowTimeOpt.Value
                : DateTimeOffset.Now;

            return TemplateString.Interpolate(
                str,
                interpolation =>
                {
                    var replacement = interpolationHandler?.Invoke(interpolation);
                    if (replacement is object)
                        return replacement;

                    return interpolation.VariableLowerInvariant switch
                    {
                        "datetime.now" => (object)localNowTime.LocalDateTime,
                        "datetime.utcnow" => (object)localNowTime.UtcDateTime,
                        "datetimeoffset.now" => (object)localNowTime,
                        "datetimeoffset.utcnow" => (object)localNowTime.ToUniversalTime(),
                        _ => throw new Exception($"Unknown interpolation variable '{interpolation.Variable}'")
                    };
                });
        }

        public string? SanitizePath(string? path)
        {
            if (path is string &&
                Path.IsPathRooted(path) &&
                Path.GetPathRoot(path) is string root)
                return root + Clean(path.Substring(root.Length));

            return Clean(path);

            static string? Clean(string? path)
                => path?.Replace(":", string.Empty);
        }

        public TimelapseInfo? GetTimelapse(string tag)
            => Timelapses.SingleOrDefault(timelapse => Regex.IsMatch(tag, timelapse.TagMatch));

        public TimelapseInfo? GetTimelapse(RecordingSourceInfo recordingSource)
            => GetTimelapse(recordingSource.Tag);

        public bool IncludeInTimelapse(IngestionItem item)
        {
            if (!item.Timestamp.HasValue || item.Tag is null)
                return false;

            if (GetTimelapse(item.Tag) is TimelapseInfo timelapse)
                return timelapse.EvaluatePredicates(item.Timestamp.Value);

            return false;
        }
    }
}
