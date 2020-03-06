//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Latham
{
    public readonly struct DayOfWeekRange
    {
        public DayOfWeek Start { get; }
        public DayOfWeek End { get; }

        public DayOfWeekRange(DayOfWeek start, DayOfWeek end)
        {
            if (end < start)
                throw new ArgumentOutOfRangeException(
                    nameof(end),
                    $"{nameof(end)} < {nameof(start)}");

            Start = start;
            End = end;
        }

        public bool Includes(DayOfWeek value)
            => Start <= value && value <= End;

        public bool Includes(DayOfWeekRange range)
            => Start <= range.Start && range.End <= End;
    }
}
