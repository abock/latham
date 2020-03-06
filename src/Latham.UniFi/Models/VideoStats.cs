//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;

using Newtonsoft.Json;

namespace Latham.UniFi.Models
{
    public sealed class VideoStats
    {
        public DateTimeOffset? RecordingStart { get; }
        public DateTimeOffset? RecordingEnd { get; }
        public DateTimeOffset? RecordingStartLq { get; }
        public DateTimeOffset? RecordingEndLq { get; }
        public DateTimeOffset? TimelapseStart { get; }
        public DateTimeOffset? TimelapseEnd { get; }
        public DateTimeOffset? TimelapseStartLq { get; }
        public DateTimeOffset? TimelapseEndLq { get; }

        [JsonConstructor]
        public VideoStats(
            DateTimeOffset? recordingStart,
            DateTimeOffset? recordingEnd,
            DateTimeOffset? recordingStartLq,
            DateTimeOffset? recordingEndLq,
            DateTimeOffset? timelapseStart,
            DateTimeOffset? timelapseEnd,
            DateTimeOffset? timelapseStartLq,
            DateTimeOffset? timelapseEndLq)
        {
            RecordingStart = recordingStart;
            RecordingEnd = recordingEnd;
            RecordingStartLq = recordingStartLq;
            RecordingEndLq = recordingEndLq;
            TimelapseStart = timelapseStart;
            TimelapseEnd = timelapseEnd;
            TimelapseStartLq = timelapseStartLq;
            TimelapseEndLq = timelapseEndLq;
        }
    }
}
