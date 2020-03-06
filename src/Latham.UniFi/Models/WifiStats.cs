//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Latham.UniFi.Models
{
    public sealed class WifiStats
    {
        public object? Channel { get; }
        public object? Frequency { get; }
        public object? LinkSpeedMbps { get; }
        public object? SignalQuality { get; }
        public object? SignalStrength { get; }
        public object? PhyRate { get; }

        [JsonConstructor]
        public WifiStats(
            object? channel,
            object? frequency,
            object? linkSpeedMbps,
            object? signalQuality,
            object? signalStrength,
            object? phyRate)
        {
            Channel = channel;
            Frequency = frequency;
            LinkSpeedMbps = linkSpeedMbps;
            SignalQuality = signalQuality;
            SignalStrength = signalStrength;
            PhyRate = phyRate;
        }
    }
}
