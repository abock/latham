//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Latham.UniFi.Models
{
    public sealed class TalkbackSettings
    {
        public string? TypeFmt { get; }
        public string? TypeIn { get; }
        public string? BindAddr { get; }
        public int BindPort { get; }
        public object? FilterAddr { get; }
        public object? FilterPort { get; }
        public int Channels { get; }
        public int SamplingRate { get; }
        public int BitsPerSample { get; }
        public int Quality { get; }

        [JsonConstructor]
        public TalkbackSettings(
            string? typeFmt,
            string? typeIn,
            string? bindAddr,
            int bindPort,
            object? filterAddr,
            object? filterPort,
            int channels,
            int samplingRate,
            int bitsPerSample,
            int quality)
        {
            TypeFmt = typeFmt;
            TypeIn = typeIn;
            BindAddr = bindAddr;
            BindPort = bindPort;
            FilterAddr = filterAddr;
            FilterPort = filterPort;
            Channels = channels;
            SamplingRate = samplingRate;
            BitsPerSample = bitsPerSample;
            Quality = quality;
        }
    }
}
