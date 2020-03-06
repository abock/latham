//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Latham.UniFi.Models
{
    public sealed class SpeakerSettings
    {
        public bool IsEnabled { get; }
        public bool AreSystemSoundsEnabled { get; }
        public double Volume { get; }

        [JsonConstructor]
        public SpeakerSettings(
            bool isEnabled,
            bool areSystemSoundsEnabled,
            double volume)
        {
            IsEnabled = isEnabled;
            AreSystemSoundsEnabled = areSystemSoundsEnabled;
            Volume = volume;
        }
    }
}
