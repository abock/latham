using System;
using System.Collections.Generic;

using Xunit;

namespace Latham
{
    using Interpolation = TemplateString.Interpolation;

    public sealed class TemplateStringTests
    {
        [Theory]
        [MemberData(nameof(InterpolationTestData))]
        public void InterpolationTest(
            string? templateString,
            string? interpolatedString)
            => Assert.Equal(
                interpolatedString,
                TemplateString.Interpolate(
                    templateString,
                    interpolation => interpolation.ToString(interpolation.Variable switch
                    {
                        nameof(Now) => Now,
                        null => null,
                        _ => double.Parse(interpolation.Variable)
                    }))?.ToString());

        static readonly DateTime Now = new DateTime(2020, 3, 25, 11, 12, 13, 123, DateTimeKind.Local);

        public static IEnumerable<object?[]> InterpolationTestData => new List<object?[]>
        {
            new object?[] { "{Now}", $"{Now}" },
            new object?[] { "{Now:yyyy-MM-dd}", $"{Now:yyyy-MM-dd}" },
            new object?[] { "{100}", $"{100}" },
            new object?[] { "[{3,-5}]", $"[{3,-5}]" },
            new object?[] { "[{30,5}]", $"[{30,5}]" },
            new object?[] { "{.32928:P1} {Now:HH:mm:ss}", $"{.32928:P1} {Now:HH:mm:ss}" },
            new object?[] { "[{.32928,10:P1}] [{Now,-20:HH:mm:ss}]", $"[{.32928,10:P1}] [{Now,-20:HH:mm:ss}]" }
        };

        [Theory]
        [MemberData(nameof(ParserTestData))]
        public void ParserTest(
            string templateString,
            Interpolation[] interpolations,
            string? interpolatedString)
        {
            int interpolationIndex = 0;

            Assert.Equal(
                interpolatedString,
                TemplateString.Interpolate(
                    templateString,
                    interpolation =>
                    {
                        Assert.Equal(
                            interpolations[interpolationIndex++],
                            interpolation);

                        return interpolation.ToString();
                    })?.ToString());
        }

        public static IEnumerable<object?[]> ParserTestData => new List<object?[]>
        {
            new object?[]
            {
                null,
                null,
                null
            },
            new object?[]
            {
                "",
                Array.Empty<Interpolation>(),
                ""
            },
            new object?[]
            {
                "a",
                Array.Empty<Interpolation>(),
                "a"
            },
            new object?[]
            {
                "ab",
                Array.Empty<Interpolation>(),
                "ab"
            },
            new object?[]
            {
                "{}",
                new[]
                {
                    new Interpolation(null, null, null)
                },
                "{}"
            },
            new object?[]
            {
                "{}{}",
                new[]
                {
                    new Interpolation(null, null, null),
                    new Interpolation(null, null, null)
                },
                "{}{}"
            },
            new object?[]
            {
                "w{}x{}y{}z",
                new[]
                {
                    new Interpolation(null, null, null),
                    new Interpolation(null, null, null),
                    new Interpolation(null, null, null)
                },
                "w{}x{}y{}z"
            },
            new object?[]
            {
                "ww{}xx{}yy{}zz",
                new[]
                {
                    new Interpolation(null, null, null),
                    new Interpolation(null, null, null),
                    new Interpolation(null, null, null)
                },
                "ww{}xx{}yy{}zz"
            },
            new object?[]
            {
                "{}ww{}xx{}yy{}zz{}",
                new[]
                {
                    new Interpolation(null, null, null),
                    new Interpolation(null, null, null),
                    new Interpolation(null, null, null),
                    new Interpolation(null, null, null),
                    new Interpolation(null, null, null)
                },
                "{}ww{}xx{}yy{}zz{}"
            },
            new object?[]
            {
                "{a}ww{b}xx{c}yy{d}zz{e}",
                new[]
                {
                    new Interpolation("a", null, null),
                    new Interpolation("b", null, null),
                    new Interpolation("c", null, null),
                    new Interpolation("d", null, null),
                    new Interpolation("e", null, null)
                },
                "{a}ww{b}xx{c}yy{d}zz{e}"
            },
            new object?[]
            {
                "{a,1:c}ww{b,2:d}xx{c,3:e}yy{d,4:f}zz{e,5:g}",
                new[]
                {
                    new Interpolation("a", "1", "c"),
                    new Interpolation("b", "2", "d"),
                    new Interpolation("c", "3", "e"),
                    new Interpolation("d", "4", "f"),
                    new Interpolation("e", "5", "g")
                },
                "{a,1:c}ww{b,2:d}xx{c,3:e}yy{d,4:f}zz{e,5:g}"
            },
            new object?[]
            {
                "{variable}",
                new[]
                {
                    new Interpolation("variable", null, null)
                },
                "{variable}"
            },
            new object?[]
            {
                "{variable,10}",
                new[]
                {
                    new Interpolation("variable", "10", null)
                },
                "{variable,10}"
            },
            new object?[]
            {
                "{variable:format}",
                new[]
                {
                    new Interpolation("variable", null, "format")
                },
                "{variable:format}"
            },
            new object?[]
            {
                "{variable,-10:format}",
                new[]
                {
                    new Interpolation("variable", "-10", "format")
                },
                "{variable,-10:format}"
            },
            new object?[]
            {
                "{{not an interpolation}}",
                Array.Empty<Interpolation>(),
                "{not an interpolation}"
            },
            new object?[]
            {
                "}}",
                Array.Empty<Interpolation>(),
                "}"
            },
            new object?[]
            {
                "{{",
                Array.Empty<Interpolation>(),
                "{"
            },
            new object?[]
            {
                "{{",
                Array.Empty<Interpolation>(),
                "{"
            },
            new object?[]
            {
                "}}",
                Array.Empty<Interpolation>(),
                "}"
            },
            new object?[]
            {
                "}}{{",
                Array.Empty<Interpolation>(),
                "}{"
            },
            new object?[]
            {
                "}}{{{{",
                Array.Empty<Interpolation>(),
                "}{{"
            },
            new object?[]
            {
                "}}{{{}",
                new[]
                {
                    new Interpolation(null, null, null)
                },
                "}{{}"
            }
        };
    }
}
