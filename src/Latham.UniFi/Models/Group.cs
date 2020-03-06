//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Latham.UniFi.Models
{
    public sealed class Group
    {
        public string? Name { get; }
        public IReadOnlyList<string> Permissions { get; }
        public string? Type { get; }
        public bool IsDefault { get; }
        public string? Id { get; }
        public string? ModelKey { get; }

        [JsonConstructor]
        public Group(
            string? name,
            IReadOnlyList<string>? permissions,
            string? type,
            bool isDefault,
            string? id,
            string? modelKey)
        {
            Name = name;
            Permissions = permissions ?? Array.Empty<string>();
            Type = type;
            IsDefault = isDefault;
            Id = id;
            ModelKey = modelKey;
        }
    }
}
