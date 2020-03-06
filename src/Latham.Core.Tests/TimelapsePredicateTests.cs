using System;
using System.Collections.Generic;

using Xunit;

namespace Latham.Project.Model
{
    public sealed class TimelapsePredicateTests
    {
        public static IEnumerable<object?[]> EvaluateTestData => new List<object?[]>
        {
            new object?[] { "2020-02-5 .. 2020-2-10", DateTime.Parse("2020-02-04"), false },
            new object?[] { "2020-02-5 .. 2020-2-10", DateTime.Parse("2020-02-05"), true },
            new object?[] { "2020-02-5 .. 2020-2-10", DateTime.Parse("2020-02-06"), true },
            new object?[] { "2020-02-5 .. 2020-2-10", DateTime.Parse("2020-02-07"), true },
            new object?[] { "2020-02-5 .. 2020-2-10", DateTime.Parse("2020-02-08"), true },
            new object?[] { "2020-02-5 .. 2020-2-10", DateTime.Parse("2020-02-09"), true },
            new object?[] { "2020-02-5 .. 2020-2-10", DateTime.Parse("2020-02-10"), true },
            new object?[] { "2020-02-5 .. 2020-2-10", DateTime.Parse("2020-02-11"), false },

            new object?[] { "5pm .. 7pm", DateTime.Parse("2020-10-3 16:59:59"), false },
            new object?[] { "5pm .. 7pm", DateTime.Parse("2020-10-3 17:00:00"), true },
            new object?[] { "5pm .. 7pm", DateTime.Parse("2020-10-3 18:00:00"), true },
            new object?[] { "5pm .. 7pm", DateTime.Parse("2020-10-3 19:00:00"), true },
            new object?[] { "5pm .. 7pm", DateTime.Parse("2020-10-3 19:00:01"), false },

            new object?[] { "Sunday", new DateTime(2020, 3, 1), true },
            new object?[] { "Monday", new DateTime(2020, 3, 2), true },
            new object?[] { "Tuesday", new DateTime(2020, 3, 3), true },
            new object?[] { "Wednesday", new DateTime(2020, 3, 4), true },
            new object?[] { "Thursday", new DateTime(2020, 3, 5), true },
            new object?[] { "Friday", new DateTime(2020, 3, 6), true },
            new object?[] { "Saturday", new DateTime(2020, 3, 7), true },

            new object?[] { "Sunday", new DateTime(2020, 3, 2), false },
            new object?[] { "Monday", new DateTime(2020, 3, 3), false },
            new object?[] { "Tuesday", new DateTime(2020, 3, 4), false },
            new object?[] { "Wednesday", new DateTime(2020, 3, 5), false },
            new object?[] { "Thursday", new DateTime(2020, 3, 6), false },
            new object?[] { "Friday", new DateTime(2020, 3, 7), false },
            new object?[] { "Saturday", new DateTime(2020, 3, 8), false },
        };

        [Theory]
        [MemberData(nameof(EvaluateTestData))]
        public void Evaluate(string predicate, DateTime testTime, bool expected)
            => Assert.Equal(expected, TimelapsePredicate.Parse(predicate).Evaluate(testTime));
    }
}
