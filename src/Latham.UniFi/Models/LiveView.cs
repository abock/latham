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
    public sealed class LiveView
    {
        public string? Name { get; }
        public bool IsGlobal { get; }
        public long Layout { get; }
        public IReadOnlyList<LiveViewSlot> Slots { get; }
        public string? Owner { get; }
        public string? Id { get; }
        public string? ModelKey { get; }

        [JsonConstructor]
        public LiveView(
            string? name,
            bool isGlobal,
            long layout,
            IReadOnlyList<LiveViewSlot>? slots,
            string? owner,
            string? id,
            string? modelKey)
        {
            Name = name;
            IsGlobal = isGlobal;
            Layout = layout;
            Slots = slots ?? Array.Empty<LiveViewSlot>();
            Owner = owner;
            Id = id;
            ModelKey = modelKey;
        }
    }
}
