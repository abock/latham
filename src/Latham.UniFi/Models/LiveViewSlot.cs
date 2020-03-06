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
    public sealed class LiveViewSlot
    {
        public IReadOnlyList<string> Cameras { get; }
        public string CycleMode { get; }
        public long CycleInterval { get; }

        [JsonConstructor]
        public LiveViewSlot(
            IReadOnlyList<string>? cameras,
            string cycleMode,
            long cycleInterval)
        {
            Cameras = cameras ?? Array.Empty<string>();
            CycleMode = cycleMode;
            CycleInterval = cycleInterval;
        }
    }
}
