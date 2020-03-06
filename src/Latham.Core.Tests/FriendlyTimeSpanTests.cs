using System;
using System.Collections.Generic;

using Xunit;

namespace Latham
{
    public sealed class FriendlyTimeSpanTests
    {
        [Theory]
        [MemberData(nameof(TestData))]
        public void Parse(string timeSpanDescription, TimeSpan? expectedTimeSpan)
        {
            if (expectedTimeSpan is null)
                Assert.Throws<FormatException>(() => FriendlyTimeSpan.Parse(timeSpanDescription));
            else
                Assert.Equal(expectedTimeSpan, FriendlyTimeSpan.Parse(timeSpanDescription));
        }

        public static IEnumerable<object?[]> TestData => new List<object?[]>
        {
            new object?[] { "", null },
            new object?[] { "garbage", null },
            new object?[] { "123.456", null },
            new object?[] { "123.456 years", null },
            new object?[] { "1s", TimeSpan.FromSeconds(1) },
            new object?[] { "1s ", TimeSpan.FromSeconds(1) },
            new object?[] { " 1s", TimeSpan.FromSeconds(1) },
            new object?[] { " 1s ", TimeSpan.FromSeconds(1) },
            new object?[] { "1 second", TimeSpan.FromSeconds(1) },
            new object?[] { "22sec", TimeSpan.FromSeconds(22) },
            new object?[] { "123hrs", TimeSpan.FromHours(123) },
            new object?[] { "5 weeks", TimeSpan.FromDays(5 * 7) },
            new object?[] { "1 minute 30 seconds", TimeSpan.FromSeconds(90) },
            new object?[] { "1m30s", TimeSpan.FromSeconds(90) },
            new object?[] { "1:05:01.5", TimeSpan.FromHours(1) + TimeSpan.FromMinutes(5) + TimeSpan.FromSeconds(1.5) },
            new object?[] { "6h3m12.25s", TimeSpan.FromHours(6) + TimeSpan.FromMinutes(3) + TimeSpan.FromSeconds(12.25) }
        };
    }
}
