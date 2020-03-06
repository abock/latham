//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;

using Newtonsoft.Json;

namespace Latham.Project.Model
{
    sealed class JsonFriendlyTimeSpanConverter : JsonConverter<FriendlyTimeSpan>
    {
        public override FriendlyTimeSpan ReadJson(
            JsonReader reader,
            Type objectType,
            FriendlyTimeSpan existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.Value is string value)
                return FriendlyTimeSpan.Parse(value, serializer.Culture);

            throw new NotImplementedException();
        }

        public override void WriteJson(
            JsonWriter writer,
            FriendlyTimeSpan value,
            JsonSerializer serializer)
        {
            if (value is FriendlyTimeSpan friendlyTimeSpan)
                writer.WriteValue(friendlyTimeSpan.OriginalString);
            else
                writer.WriteNull();
        }
    }
}
