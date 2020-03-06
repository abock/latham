//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Latham.UniFi.Models
{
    public sealed class Location
    {
        public bool IsAway { get; }
        public double? Latitude { get; }
        public double? Longitude { get; }

        [JsonConstructor]
        public Location(
            bool isAway,
            double? latitude,
            double? longitude)
        {
            IsAway = isAway;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
