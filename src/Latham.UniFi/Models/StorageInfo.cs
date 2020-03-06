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
    public sealed class StorageInfo
    {
        public long TotalSize { get; }
        public long TotalSpaceUsed { get; }
        public IReadOnlyList<StorageUtilization> StorageUtilization { get; }
        public IReadOnlyList<HardDrive> HardDrives { get; }

        [JsonConstructor]
        public StorageInfo(
            long totalSize,
            long totalSpaceUsed,
            IReadOnlyList<StorageUtilization>? storageUtilization,
            IReadOnlyList<HardDrive>? hardDrives)
        {
            TotalSize = totalSize;
            TotalSpaceUsed = totalSpaceUsed;
            StorageUtilization = storageUtilization ?? Array.Empty<StorageUtilization>();
            HardDrives = hardDrives ?? Array.Empty<HardDrive>();
        }
    }
}
