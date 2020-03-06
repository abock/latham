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
    public sealed class Channel
    {
        public int Id { get; }
        public string? Name { get; }
        public bool Enabled { get; }
        public bool IsRtspEnabled { get; }
        public string? RtspAlias { get; }
        public int Width { get; }
        public int Height { get; }
        public int Fps { get; }
        public int Bitrate { get; }
        public int MinBitrate { get; }
        public int MaxBitrate { get; }
        public IReadOnlyList<int> FpsValues { get; }
        public int IdrInterval { get; }

        [JsonConstructor]
        public Channel(
            int id,
            string? name,
            bool enabled,
            bool isRtspEnabled,
            string? rtspAlias,
            int width,
            int height,
            int fps,
            int bitrate,
            int minBitrate,
            int maxBitrate,
            IReadOnlyList<int>? fpsValues,
            int idrInterval)
        {
            Id = id;
            Name = name;
            Enabled = enabled;
            IsRtspEnabled = isRtspEnabled;
            RtspAlias = rtspAlias;
            Width = width;
            Height = height;
            Fps = fps;
            Bitrate = bitrate;
            MinBitrate = minBitrate;
            MaxBitrate = maxBitrate;
            FpsValues = fpsValues ?? Array.Empty<int>();
            IdrInterval = idrInterval;
        }
    }
}
