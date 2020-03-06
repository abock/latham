//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;

using Newtonsoft.Json;

namespace Latham.Project.Model
{
    [JsonConverter(typeof(TimelapsePredicate.JsonConverter))]
    public sealed class TimelapsePredicate
    {
        public string OriginalString { get; }
        public DayOfWeekRange? DayOfWeekRange { get; }
        public DateTimeOffsetRange? DateTimeRange { get; }
        public TimeSpanRange? TimeSpanRange { get; }

        public TimelapsePredicate(
            string originalString,
            DayOfWeekRange? dayOfWeekRange)
        {
            OriginalString = originalString;
            DayOfWeekRange = dayOfWeekRange;
        }

        public TimelapsePredicate(
            string originalString,
            DateTimeOffsetRange? dateTimeRange)
        {
            OriginalString = originalString;
            DateTimeRange = dateTimeRange;
        }

        public TimelapsePredicate(
            string originalString,
            TimeSpanRange? timeSpanRange)
        {
            OriginalString = originalString;
            TimeSpanRange = timeSpanRange;
        }

        public bool Evaluate(DateTimeOffset dateTime)
        {
            if (DayOfWeekRange.HasValue)
                return DayOfWeekRange.Value.Includes(dateTime.DayOfWeek);
            else if (DateTimeRange.HasValue)
                return DateTimeRange.Value.Includes(dateTime);
            else if (TimeSpanRange.HasValue)
                return TimeSpanRange.Value.Includes(dateTime.TimeOfDay);

            return false;
        }

        public static TimelapsePredicate Parse(
            string predicateString,
            CultureInfo? culture = null)
        {
            culture = culture ?? CultureInfo.CurrentCulture;

            if (predicateString is null)
                throw new ArgumentNullException(nameof(predicateString));

            var parts = predicateString.Split("..", 2, StringSplitOptions.RemoveEmptyEntries);
            var start = Parse(parts[0], culture);
            var end = parts.Length == 1
                ? start
                : Parse(parts[1], culture);

            if (start is null || end is null)
                throw new FormatException("Unable to parse predicate (should not be reached)");

            if (start.GetType () != end.GetType())
                throw new FormatException(
                    "Unable to parse predicate - incompatible range types " +
                    $"({start.GetType()} vs {end.GetType()})");

            if (start is DayOfWeek startDayOfWeek)
            {
                return new TimelapsePredicate(
                    predicateString,
                    new DayOfWeekRange(startDayOfWeek, (DayOfWeek)end));
            }
            else if (start is DateTime startDateTime)
            {
                var endDateTime = (DateTime)end;

                if (startDateTime.Date == DateTime.MinValue && endDateTime.Date == DateTime.MinValue)
                    return new TimelapsePredicate(
                        predicateString,
                        new TimeSpanRange(startDateTime.TimeOfDay, endDateTime.TimeOfDay));

                return new TimelapsePredicate(
                    predicateString,
                    new DateTimeOffsetRange(startDateTime, endDateTime));
            }

            throw new NotImplementedException();

            static object Parse(string str, CultureInfo culture)
            {
                if (TryParseDayOfWeek(str, culture, out var dayOfWeek))
                    return dayOfWeek;

                if (DateTime.TryParse(str, culture, DefaultDateTimeStyles, out var dateTime))
                    return dateTime;

                throw new FormatException($"Unable to parse predicate part '{str}'");
            }
        }

        const DateTimeStyles DefaultDateTimeStyles =
            DateTimeStyles.NoCurrentDateDefault |
            DateTimeStyles.AssumeLocal |
            DateTimeStyles.AllowLeadingWhite |
            DateTimeStyles.AllowTrailingWhite |
            DateTimeStyles.AllowWhiteSpaces;

        static readonly Dictionary<CultureInfo, Dictionary<string, DayOfWeek>> dayOfWeekNames
            = new Dictionary<CultureInfo, Dictionary<string, DayOfWeek>>();

        static bool TryParseDayOfWeek(string dayOfWeekName, CultureInfo cultureInfo, out DayOfWeek dayOfWeek)
        {
            if (dayOfWeekName is null)
                throw new ArgumentNullException(nameof(dayOfWeekName));

            if (cultureInfo is null)
                throw new ArgumentNullException(nameof(cultureInfo));

            if (!dayOfWeekNames.TryGetValue(cultureInfo, out var names))
            {
                names = new Dictionary<string, DayOfWeek>();

                for (int i = (int)DayOfWeek.Sunday, n = (int)DayOfWeek.Saturday; i <= n; i++)
                {
                    var day = (DayOfWeek)i;
                    names[cultureInfo.DateTimeFormat.DayNames[i].ToLower(cultureInfo)] = day;
                    names[cultureInfo.DateTimeFormat.AbbreviatedDayNames[i].ToLower(cultureInfo)] = day;
                    names[cultureInfo.DateTimeFormat.ShortestDayNames[i].ToLower(cultureInfo)] = day;
                }

                dayOfWeekNames.Add(cultureInfo, names);
            }

            return names.TryGetValue(
                dayOfWeekName.ToLower(cultureInfo).Trim(),
                out dayOfWeek);
        }

        internal class JsonConverter : JsonConverter<TimelapsePredicate>
        {
            public override TimelapsePredicate ReadJson(
                JsonReader reader,
                Type objectType,
                TimelapsePredicate existingValue,
                bool hasExistingValue,
                JsonSerializer serializer)
            {
                if (reader.Value is string value)
                    return TimelapsePredicate.Parse(value, serializer.Culture);

                throw new NotImplementedException();
            }

            public override void WriteJson(
                JsonWriter writer,
                TimelapsePredicate value,
                JsonSerializer serializer)
            {
                if (value is TimelapsePredicate timelapsePredicate)
                    writer.WriteValue(timelapsePredicate.OriginalString);
                else
                    writer.WriteNull();
            }
        }
    }
}
