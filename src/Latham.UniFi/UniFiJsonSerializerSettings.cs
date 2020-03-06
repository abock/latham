//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Latham.UniFi
{
    public sealed class UniFiJsonSerializerSettings : JsonSerializerSettings
    {
        public static JsonSerializerSettings Instance { get; } = new UniFiJsonSerializerSettings();

        public static JsonSerializer CreateSerializer()
            => JsonSerializer.Create(Instance);

        UniFiJsonSerializerSettings()
        {
            Formatting = Formatting.Indented;
            ContractResolver = new CamelCasePropertyNamesContractResolver();
            Converters = new List<JsonConverter>
            {
                new UnixTimeDateTimeOffsetConverter()
            };
        }

        sealed class UnixTimeDateTimeOffsetConverter : JsonConverter<DateTimeOffset?>
        {
            public override DateTimeOffset? ReadJson(
                JsonReader reader,
                Type objectType,
                DateTimeOffset? existingValue,
                bool hasExistingValue,
                JsonSerializer serializer)
                => reader.Value switch
                    {
                        null => null,
                        long value => DateTimeOffset.FromUnixTimeMilliseconds(value),
                        object o => DateTimeOffset.FromUnixTimeMilliseconds(
                            (long)Convert.ChangeType(o, typeof(long)))
                    };

            public override void WriteJson(
                JsonWriter writer,
                DateTimeOffset? value,
                JsonSerializer serializer)
            {
                if (value.HasValue)
                    writer.WriteValue(value.Value.ToUnixTimeMilliseconds());
                else
                    writer.WriteNull();
            }
        }
    }
}
