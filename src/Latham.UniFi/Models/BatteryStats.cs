//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Latham.UniFi.Models
{
    public sealed class BatteryStats
    {
        public double? Percentage { get; }
        public bool IsCharging { get; }
        public string? SleepState { get; }

        [JsonConstructor]
        public BatteryStats(
            double? percentage,
            bool isCharging,
            string? sleepState)
        {
            Percentage = percentage;
            IsCharging = isCharging;
            SleepState = sleepState;
        }
    }
}
