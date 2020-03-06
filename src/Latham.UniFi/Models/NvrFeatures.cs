//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Latham.UniFi.Models
{
    public sealed class NvrFeatures
    {
        public bool Dev { get; }
        public bool Beta { get; }

        [JsonConstructor]
        public NvrFeatures(
            bool dev,
            bool beta)
        {
            Dev = dev;
            Beta = beta;
        }
    }
}
