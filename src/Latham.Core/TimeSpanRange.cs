//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Latham
{
    public readonly struct TimeSpanRange
    {
        public TimeSpan Start { get; }
        public TimeSpan End { get; }

        public TimeSpanRange(TimeSpan start, TimeSpan end)
        {
            if (end < start)
                throw new ArgumentOutOfRangeException(
                    nameof(end),
                    $"{nameof(end)} < {nameof(start)}");

            Start = start;
            End = end;
        }

        public bool Includes(TimeSpan value)
            => Start <= value && value <= End;

        public bool Includes(TimeSpanRange range)
            => Start <= range.Start && range.End <= End;
    }
}
