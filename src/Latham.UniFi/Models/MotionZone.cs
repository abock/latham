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
    public sealed class MotionZone
    {
        public string? Name { get; }

        public string? Color { get; }

        public IReadOnlyList<IReadOnlyList<double>> Points { get; }

        public double Sensitivity { get; }

        [JsonConstructor]
        public MotionZone(
            string? name,
            string? color,
            IReadOnlyList<IReadOnlyList<double>>? points,
            double sensitivity)
        {
            Name = name;
            Color = color;
            Points = points ?? Array.Empty<IReadOnlyList<double>>();
            Sensitivity = sensitivity;
        }
    }
}
