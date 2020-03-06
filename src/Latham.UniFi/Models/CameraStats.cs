//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Latham.UniFi.Models
{
    public sealed class CameraStats
    {
        public long RxBytes { get; }
        public long TxBytes { get; }
        public WifiStats? Wifi { get; }
        public BatteryStats? Battery { get; }
        public VideoStats? Video { get; }
        public long WifiQuality { get; }
        public long WifiStrength { get; }

        [JsonConstructor]
        public CameraStats(
            long rxBytes,
            long txBytes,
            WifiStats? wifi,
            BatteryStats? battery,
            VideoStats? video,
            long wifiQuality,
            long wifiStrength)
        {
            RxBytes = rxBytes;
            TxBytes = txBytes;
            Wifi = wifi;
            Battery = battery;
            Video = video;
            WifiQuality = wifiQuality;
            WifiStrength = wifiStrength;
        }
    }
}
