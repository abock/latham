//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Latham
{
    public readonly struct FriendlyTimeSpan
    {
        public TimeSpan TimeSpan { get; }
        public string OriginalString { get; }
        public CultureInfo? OriginalCulture { get; }

        FriendlyTimeSpan(
            TimeSpan timeSpan,
            string originalString,
            CultureInfo? originalCulture)
        {
            TimeSpan = timeSpan;
            OriginalString = originalString;
            OriginalCulture = originalCulture;
        }

        public static implicit operator FriendlyTimeSpan(TimeSpan timeSpan)
            => new FriendlyTimeSpan(
                timeSpan,
                timeSpan.ToString(),
                null);

        public static implicit operator TimeSpan(FriendlyTimeSpan friendlyTimeSpan)
            => friendlyTimeSpan.TimeSpan;

        sealed class Converter : TimeSpanConverter
        {
            public override object ConvertFrom(
                ITypeDescriptorContext context,
                CultureInfo culture,
                object value)
            {
                if (value is string stringValue)
                    return Parse(stringValue, culture).TimeSpan;

                return base.ConvertFrom(context, culture, value);
            }
        }

        static bool registeredTypeConverter;

        public static void RegisterTypeConverter()
        {
            if (!registeredTypeConverter)
            {
                registeredTypeConverter = true;
                TypeDescriptor.AddAttributes(
                    typeof(TimeSpan),
                    new TypeConverterAttribute(typeof(Converter)));
            }
        }

        public static FriendlyTimeSpan Parse(string timeDescription, CultureInfo? culture = null)
        {
            var regex = new Regex(@"^(?:\s*(?<factor>\d+(?:\.\d+)?)\s*(?<unit>[A-Za-z]+)\s*)+$");
            var match = regex.Match(timeDescription);
            if (!match.Success)
                return new FriendlyTimeSpan(
                    TimeSpan.Parse(timeDescription, culture),
                    timeDescription,
                    culture);

            TimeSpan time = default;
            var factors = match.Groups["factor"].Captures;
            var units = match.Groups["unit"].Captures;

            for (int i = 0; i < factors.Count; i++)
            {
                var factor = double.Parse(factors[i].Value);
                var unit = units[i].Value;

                switch (unit.ToLowerInvariant())
                {
                    case "w":
                    case "wk":
                    case "wks":
                    case "week":
                    case "weeks":
                        time += TimeSpan.FromDays(factor * 7);
                        break;
                    case "d":
                    case "day":
                    case "days":
                        time += TimeSpan.FromDays(factor);
                        break;
                    case "h":
                    case "hr":
                    case "hrs":
                    case "hour":
                    case "hours":
                        time += TimeSpan.FromHours(factor);
                        break;
                    case "m":
                    case "mn":
                    case "mm":
                    case "min":
                    case "mins":
                    case "minute":
                    case "minutes":
                        time += TimeSpan.FromMinutes(factor);
                        break;
                    case "s":
                    case "ss":
                    case "sec":
                    case "secs":
                    case "second":
                    case "seconds":
                        time += TimeSpan.FromSeconds(factor);
                        break;
                    case "ms":
                    case "msec":
                    case "msecs":
                    case "mil":
                    case "mils":
                    case "mill":
                    case "mills":
                    case "milli":
                    case "millis":
                    case "millisec":
                    case "millisecs":
                    case "millisecond":
                    case "milliseconds":
                        time += TimeSpan.FromMilliseconds(factor);
                        break;
                    default:
                        throw new FormatException($"Unsupported unit of time '{unit}'");
                }
            }

            return new FriendlyTimeSpan(
                time,
                timeDescription,
                culture);
        }
    }
}
