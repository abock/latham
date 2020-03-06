//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Latham.UniFi.Models
{
    public sealed class LedSettings
    {
        public bool IsEnabled { get; }
        public double BlinkRate { get; }

        [JsonConstructor]
        public LedSettings(
            bool isEnabled,
            double blinkRate)
        {
            IsEnabled = isEnabled;
            BlinkRate = blinkRate;
        }
    }
}
