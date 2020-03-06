//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Latham.UniFi.Models
{
    public sealed class WifiSettings
    {
        public bool UseThirdPartyWifi { get; }
        public string? Ssid { get; }
        public string? Password { get; }

        [JsonConstructor]
        public WifiSettings(
            bool useThirdPartyWifi,
            string? ssid,
            string? password)
        {
            UseThirdPartyWifi = useThirdPartyWifi;
            Ssid = ssid;
            Password = password;
        }
    }
}
