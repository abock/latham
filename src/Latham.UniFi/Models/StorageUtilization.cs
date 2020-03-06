//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Latham.UniFi.Models
{
    public sealed class StorageUtilization
    {
        public string? Type { get; }
        public long SpaceUsed { get; }

        [JsonConstructor]
        public StorageUtilization(
            string? type,
            long spaceUsed)
        {
            Type = type;
            SpaceUsed = spaceUsed;
        }
    }
}
