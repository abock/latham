//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Latham
{
    public sealed class IngestionItem
    {
        public string FilePath { get; }
        public long? FileSize { get; }
        public string? Tag { get; }
        public DateTimeOffset? Timestamp { get; }
        public TimeSpan? Duration { get; }

        public IngestionItem(
            string filePath,
            long? fileSize,
            string? tag = null,
            DateTimeOffset? timestamp = null,
            TimeSpan? duration = null)
        {
            FilePath = filePath;
            FileSize = fileSize;
            Tag = tag;
            Timestamp = timestamp;
            Duration = duration;
        }

        public IngestionItem WithFilePath(string filePath)
            => new IngestionItem(
                filePath,
                FileSize,
                Tag,
                Timestamp,
                Duration);

        public IngestionItem WithDurationAndFileSizeByReadingFile(string? basePath = null)
        {
            var fullPath = basePath is null
                ? FilePath
                : Path.Combine(basePath, FilePath);

            return new IngestionItem(
                FilePath,
                new FileInfo(fullPath).Length,
                Tag,
                Timestamp,
                FFMpeg.ParseDuration(fullPath));
        }

        public static IngestionItem FromFileSystem(
            string? basePath,
            string fullPath,
            Match? metadataMatch,
            bool parseDuration = true)
        {
            var path = basePath is null
                ? fullPath
                : fullPath.Substring(basePath.Length + 1);

            var fileSize = new FileInfo(fullPath).Length;

            if (metadataMatch is null)
                return new IngestionItem(path, fileSize);

            var tzString = Match("zzzz", "zzz", "zz", "z");
            var tzOffset = TimeZoneInfo.Local.BaseUtcOffset;

            if (tzString is string)
            {
                tzString = tzString.Replace(":", string.Empty);
                if (int.TryParse(tzString, out var tzInt))
                    tzOffset = TimeSpan
                        .FromHours(tzInt / 100)
                        .Add(TimeSpan.FromMinutes(Math.Abs(tzInt % 100)));
                else
                    tzOffset = TimeZoneInfo
                        .FindSystemTimeZoneById(tzString)
                        .BaseUtcOffset;
            }

            var timestamp = new DateTimeOffset(
                MatchInt("yyyyy", "yyyy", "yyy", "yy") ?? 1,
                MatchInt("MM", "M") ?? 1,
                MatchInt("dd", "d") ?? 1,
                MatchInt("HH", "H") ?? 0,
                MatchInt("mm", "m") ?? 0,
                MatchInt("ss", "s") ?? 0,
                tzOffset);

            TimeSpan? duration = null;
            if (parseDuration)
                duration = FFMpeg.ParseDuration(fullPath);

            return new IngestionItem(
                path,
                fileSize,
                Match("tag"),
                timestamp,
                duration);

            string? Match(params string[] keys)
            {
                // FIXME: Roslyn: this should never be null
                if (metadataMatch is null)
                    return null;

                foreach (var key in keys)
                {
                    if (metadataMatch.Groups.TryGetValue(key, out var group) && group.Success)
                        return group.Value;
                }

                return null;
            }

            int? MatchInt(params string[] keys)
            {
                // FIXME: Roslyn: this should never be null
                if (metadataMatch is null)
                    return null;

                foreach (var key in keys)
                {
                    if (metadataMatch.Groups.TryGetValue(key, out var group) &&
                        group.Success &&
                        int.TryParse(group.Value, out var intValue))
                        return intValue;
                }
                return null;
            }
        }
    }
}
