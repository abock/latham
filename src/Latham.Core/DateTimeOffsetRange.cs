//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Latham
{
    public readonly struct DateTimeOffsetRange
    {
        public DateTimeOffset Start { get; }
        public DateTimeOffset End { get; }

        public DateTimeOffsetRange(DateTimeOffset start, DateTimeOffset end)
        {
            if (end < start)
                throw new ArgumentOutOfRangeException(
                    nameof(end),
                    $"{nameof(end)} < {nameof(start)}");

            Start = start;
            End = end;
        }

        public bool Includes(DateTimeOffset value)
            => Start <= value && value <= End;

        public bool Includes(DateTimeOffsetRange range)
            => Start <= range.Start && range.End <= End;
    }
}
