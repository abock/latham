//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Latham.UniFi.Models
{
    public sealed class LocationSettings
    {
        public bool IsAway { get; }
        public bool IsGeofencingEnabled { get; }
        public double? Latitude { get; }
        public double? Longitude { get; }
        public double? Radius { get; }

        [JsonConstructor]
        public LocationSettings(
            bool isAway,
            bool isGeofencingEnabled,
            double? latitude,
            double? longitude,
            double? radius)
        {
            IsAway = isAway;
            IsGeofencingEnabled = isGeofencingEnabled;
            Latitude = latitude;
            Longitude = longitude;
            Radius = radius;
        }
    }
}
