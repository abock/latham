//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Latham.UniFi.Models
{
    public sealed class HardDrive
    {
        public string? Status { get; }
        public string? Name { get; }
        public string? Serial { get; }
        public string? Firmware { get; }
        public long Size { get; }
        public long Rpm { get; }
        public string? AtaVersion { get; }
        public string? SataVersion { get; }
        public string? Health { get; }

        [JsonConstructor]
        public HardDrive(
            string? status,
            string? name,
            string? serial,
            string? firmware,
            long size,
            long rpm,
            string? ataVersion,
            string? sataVersion,
            string? health)
        {
            Status = status;
            Name = name;
            Serial = serial;
            Firmware = firmware;
            Size = size;
            Rpm = rpm;
            AtaVersion = ataVersion;
            SataVersion = sataVersion;
            Health = health;
        }
    }
}
