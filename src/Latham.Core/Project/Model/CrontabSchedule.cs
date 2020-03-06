//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Latham.Project.Model
{
    [JsonConverter(typeof(CrontabSchedule.CrontabScheduleJsonConverter))]
    public readonly struct CrontabSchedule : IEquatable<CrontabSchedule>
    {
        static readonly NCrontab.CrontabSchedule.ParseOptions secondsParseOptions
            = new NCrontab.CrontabSchedule.ParseOptions
            {
                IncludingSeconds = true
            };

        readonly string originalString;
        readonly NCrontab.CrontabSchedule? schedule;

        CrontabSchedule(
            string originalString,
            NCrontab.CrontabSchedule? schedule)
        {
            this.originalString = originalString;
            this.schedule = schedule;
        }

        public override string ToString()
            => originalString;

        public DateTime? GetNextOccurrence(DateTime baseTime)
        {
            if (schedule is null)
                return null;

            return schedule.GetNextOccurrence(baseTime);
        }

        public DateTime? GetNextOccurrence(DateTime baseTime, DateTime endTime)
        {
            if (schedule is null)
                return null;

            return schedule.GetNextOccurrence(baseTime, endTime);
        }

        public IEnumerable<DateTime> GetNextOccurrences(DateTime baseTime, DateTime endTime)
        {
            if (schedule is null)
                return Array.Empty<DateTime>();

            return schedule.GetNextOccurrences(baseTime, endTime);
        }

        public static CrontabSchedule Parse(string schedule)
        {
            return new CrontabSchedule(
                schedule,
                NCrontab.CrontabSchedule.TryParse(schedule)
                    ?? NCrontab.CrontabSchedule.TryParse(schedule, secondsParseOptions));
        }

        public bool Equals(CrontabSchedule other)
            => other.schedule == schedule;

        public override bool Equals(object? obj)
            => obj is CrontabSchedule schedule && Equals(schedule);

        public override int GetHashCode()
            => schedule?.GetHashCode() ?? originalString?.GetHashCode() ?? 0;

        public static implicit operator NCrontab.CrontabSchedule?(CrontabSchedule schedule)
            => schedule.schedule;

        internal sealed class CrontabScheduleJsonConverter : JsonConverter<CrontabSchedule>
        {
            public override CrontabSchedule ReadJson(
                JsonReader reader,
                Type objectType,
                CrontabSchedule existingValue,
                bool hasExistingValue,
                JsonSerializer serializer)
            {
                if (reader.Value is string value)
                    return CrontabSchedule.Parse(value);

                throw new NotImplementedException();
            }

            public override void WriteJson(
                JsonWriter writer,
                CrontabSchedule value,
                JsonSerializer serializer)
            {
                if (value is CrontabSchedule crontabSchedule)
                    writer.WriteValue(crontabSchedule.ToString());
                else
                    writer.WriteNull();
            }
        }
    }
}
