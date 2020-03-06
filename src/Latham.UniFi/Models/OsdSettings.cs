//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Latham.UniFi.Models
{
    public sealed class OsdSettings
    {
        public bool IsNameEnabled { get; }
        public bool IsDateEnabled { get; }
        public bool IsLogoEnabled { get; }
        public bool IsDebugEnabled { get; }

        [JsonConstructor]
        public OsdSettings(
            bool isNameEnabled,
            bool isDateEnabled,
            bool isLogoEnabled,
            bool isDebugEnabled)
        {
            IsNameEnabled = isNameEnabled;
            IsDateEnabled = isDateEnabled;
            IsLogoEnabled = isLogoEnabled;
            IsDebugEnabled = isDebugEnabled;
        }
    }
}
