//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Latham.Project.Model
{
    public sealed class TimelapseInfo : IProjectInfoNode<TimelapseInfo, ProjectInfo>
    {
        [JsonIgnore]
        public ProjectInfo? Parent { get; internal set; }

        [JsonIgnore]
        public ProjectInfo? Project => Parent;

        [JsonProperty("tagMatch", Order = 0)]
        public string TagMatch { get; }

        [JsonProperty("include", Order = 10)]
        readonly IReadOnlyList<TimelapsePredicate>? include;

        [JsonIgnore]
        public IReadOnlyList<TimelapsePredicate> Include
            => include ?? Array.Empty<TimelapsePredicate>();

        [JsonProperty("exclude", Order = 20)]
        readonly IReadOnlyList<TimelapsePredicate>? exclude;

        [JsonIgnore]
        public IReadOnlyList<TimelapsePredicate> Exclude
            => exclude ?? Array.Empty<TimelapsePredicate>();

        [JsonConstructor]
        public TimelapseInfo(
            string tagMatch,
            IReadOnlyList<TimelapsePredicate>? include,
            IReadOnlyList<TimelapsePredicate>? exclude)
        {
            TagMatch = tagMatch;
            this.include = include;
            this.exclude = exclude;
        }

        public TimelapseInfo Evaluate(bool expandPaths)
            => new TimelapseInfo(
                TagMatch,
                Include,
                Exclude);

        public bool EvaluatePredicates(DateTimeOffset dateTime)
        {
            foreach (var includePredicate in Include)
            {
                if (!includePredicate.Evaluate(dateTime))
                    return false;
            }

            foreach (var excludePredicate in Exclude)
            {
                if (excludePredicate.Evaluate(dateTime))
                    return false;
            }

            return true;
        }
    }
}
