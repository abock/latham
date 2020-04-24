//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System.Text.RegularExpressions;

using Newtonsoft.Json;

namespace Latham.Project.Model
{
    public sealed class IngestionInfo : IProjectInfoNode<IngestionInfo, ProjectInfo>
    {
        [JsonIgnore]
        public ProjectInfo? Parent { get; internal set; }

        [JsonIgnore]
        public ProjectInfo? Project => Parent;

        [JsonProperty("basePath", Order = 0)]
        public string? BasePath { get; }

        [JsonProperty("pathGlob", Order = 10)]
        public string PathGlob { get; }

        [JsonProperty(Order = 20)]
        readonly string? pathFilter;

        [JsonIgnore]
        public Regex? PathFilter { get; }

        [JsonConstructor]
        public IngestionInfo(
            string pathGlob,
            string? basePath = null,
            string? pathFilter = null)
        {
            PathGlob = pathGlob;
            BasePath = basePath;
            this.pathFilter = pathFilter;
            if (pathFilter is string)
                PathFilter = new Regex(pathFilter);
        }

        public IngestionInfo(
            string pathGlob,
            string? basePath = null,
            Regex? pathFilter = null)
        {
            PathGlob = pathGlob;
            BasePath = basePath;
            PathFilter = pathFilter;
        }

        public IngestionInfo Evaluate(bool expandPaths)
            => new IngestionInfo(
                PathGlob,
                BasePath,
                pathFilter);
    }
}
