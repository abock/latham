//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Latham.UniFi.Models
{
    public sealed class RecordingSettings
    {
        public double PrePaddingSecs { get; }
        public double PostPaddingSecs { get; }
        public double MinMotionEventTrigger { get; }
        public double EndMotionEventDelay { get; }
        public bool SuppressIlluminationSurge { get; }
        public string? Mode { get; }
        public string? Geofencing { get; }
        public bool UseNewMotionAlgorithm { get; }
        public bool EnablePirTimelapse { get; }

        [JsonConstructor]
        public RecordingSettings(
            double prePaddingSecs,
            double postPaddingSecs,
            double minMotionEventTrigger,
            double endMotionEventDelay,
            bool suppressIlluminationSurge,
            string? mode,
            string? geofencing,
            bool useNewMotionAlgorithm,
            bool enablePirTimelapse)
        {
            PrePaddingSecs = prePaddingSecs;
            PostPaddingSecs = postPaddingSecs;
            MinMotionEventTrigger = minMotionEventTrigger;
            EndMotionEventDelay = endMotionEventDelay;
            SuppressIlluminationSurge = suppressIlluminationSurge;
            Mode = mode;
            Geofencing = geofencing;
            UseNewMotionAlgorithm = useNewMotionAlgorithm;
            EnablePirTimelapse = enablePirTimelapse;
        }
    }
}
