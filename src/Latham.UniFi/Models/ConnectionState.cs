//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Latham.UniFi.Models
{
    public sealed class ConnectionState
    {
        public double PhyRate { get; }

        [JsonConstructor]
        public ConnectionState(double phyRate)
            => PhyRate = phyRate;
    }
}
