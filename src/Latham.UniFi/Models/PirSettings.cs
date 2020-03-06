//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Latham.UniFi.Models
{
    public sealed class PirSettings
    {
        public double PirSensitivity { get; }
        public double PirMotionClipLength { get; }
        public double TimelapseFrameInterval { get; }
        public double TimelapseTransferInterval { get; }

        [JsonConstructor]
        public PirSettings(
            double pirSensitivity,
            double pirMotionClipLength,
            double timelapseFrameInterval,
            double timelapseTransferInterval)
        {
            PirSensitivity = pirSensitivity;
            PirMotionClipLength = pirMotionClipLength;
            TimelapseFrameInterval = timelapseFrameInterval;
            TimelapseTransferInterval = timelapseTransferInterval;
        }
    }
}
