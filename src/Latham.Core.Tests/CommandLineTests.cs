// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace Latham
{
    public class CommandLineTests
    {
        [Theory]
        [InlineData("hello", "hello")]
        [InlineData("hello world", "\"hello world\"")]
        [InlineData("\"", "\"\\\"\"")]
        [InlineData("", "\"\"")]
        public void Quote(string unquoted, string quoted)
            => Assert.Equal(quoted, CommandLine.QuoteArgument(unquoted));

        [Theory]
        [InlineData("hello", "hello")]
        [InlineData("hello world", "hello", "world")]
        [InlineData("'hello world'", "hello world")]
        [InlineData("\"hello world\"", "hello world")]
        [InlineData("one 'two three' four", "one", "two three", "four")]
        public void ParseWithoutGlobs(string commandLine, params string[] expectedArguments)
            => Assert.Equal(
                expectedArguments,
                CommandLine.ParseArguments(commandLine));

        [Theory]
        [InlineData("hello", "hello")]
        [InlineData("hello world", "hello world")]
        [InlineData("'hello world'", "\"hello world\"")]
        [InlineData("\"hello world\"", "\"hello world\"")]
        [InlineData("one 'two three' four", "one \"two three\" four")]
        public void ParseAndToStringRoundTrip(string commandLine, string expectedToString)
            => Assert.Equal(
                expectedToString,
                CommandLine.ToString(CommandLine.ParseArguments(commandLine)));

        [Theory]
        [InlineData(
            "0 1 2 @ResponseFileTestData/depth.rsp 3 4 5",
            "0", "1", "2", "a", "b", "c", "d", "e", "f", "g", "h",
            "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s",
            "t", "u", "v", "w", "x", "y", "z", "3", "4", "5")]
        public void ParseResponseFiles(string commandLine, params string[] expectedArguments)
            => Assert.Equal(
                expectedArguments,
                CommandLine.ParseArguments(commandLine));

        [Theory]
        [InlineData("ResponseFileTestData/cycle.rsp")]
        [InlineData("ResponseFileTestData/cycle1.rsp")]
        public void DetectResponseFileCycle(string responseFile)
        {
            Assert.Throws<CommandLine.ResponseFileCycleException>(
                () => CommandLine.ParseResponseFile(responseFile));

            Assert.Throws<CommandLine.ResponseFileCycleException>(
                () => CommandLine.ParseArguments("@" + responseFile));
        }

        [Theory]
        [InlineData("a * b * c", "a", "1", "2", "3", "b", "1", "2", "3", "c")]
        [InlineData("a \"*\" b \"*\" c", "a", "1", "2", "3", "b", "1", "2", "3", "c")]
        [InlineData("a '*' b '*' c", "a", "*", "b", "*", "c")]
        [InlineData("a 3 e f 2 i", "a", "b", "c", "d", "e", "f", "g", "h", "i")]
        public void ArgumentHandler(string commandLine, params string[] expectedArguments)
        {
            Assert.Equal(
                expectedArguments,
                CommandLine.ParseArguments(
                    commandLine,
                    CommandLine.ParseOptions.Create(
                        argumentHandler: Handler)));

            static IEnumerable<string> Handler(CommandLine.ParseContext context, bool shellExpand, string arg)
            {
                if (arg == "*" && shellExpand)
                {
                    yield return "1";
                    yield return "2";
                    yield return "3";
                }
                else if (int.TryParse(arg, out var number))
                {
                    var lastChar = context.ParsedArguments.Last()[0];
                    for (int i = 0; i < number; i++)
                    {
                        lastChar = (char)((int)lastChar + 1);
                        yield return lastChar.ToString();
                    }
                }
                else
                {
                    yield return arg;
                }
            }
        }
    }
}
