//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Latham
{
    public static class CommandLine
    {
        public delegate IEnumerable<string> ArgumentHandler(
            ParseContext parseContext,
            bool shellExpand,
            string argument);

        public enum ResponseFileHandling
        {
            None,
            OneArgumentPerLine,
            MultipleArgumentsPerLine
        }

        public sealed class ParseContext
        {
            public ParseOptions BaseOptions { get; }

            public List<string> ParsedArguments { get; } = new List<string>();

            internal Stack<(string FilePath, ParseOptions ParseOptions)> ResponseFileStack { get; }
                = new Stack<(string, ParseOptions)>();

            public ParseOptions CurrentOptions => ResponseFileStack.Count > 0
                ? ResponseFileStack.Peek().ParseOptions
                : BaseOptions;

            public string? ResponseFilePath => ResponseFileStack.Count > 0
                ? ResponseFileStack.Peek().FilePath
                : null;

            internal ParseContext(ParseOptions baseOptions)
                => BaseOptions = baseOptions;
        }

        public readonly struct ParseOptions
        {
            public static ParseOptions Create(
                string basePath = ".",
                ResponseFileHandling responseFileHandling = ResponseFileHandling.MultipleArgumentsPerLine,
                ArgumentHandler? argumentHandler = null)
                => new ParseOptions(
                    basePath,
                    responseFileHandling,
                    argumentHandler);

            public string BasePath { get; }
            public ResponseFileHandling ResponseFileHandling { get; }
            public ArgumentHandler? ArgumentHandler { get; }

            ParseOptions(
                string basePath,
                ResponseFileHandling responseFileHandling,
                ArgumentHandler? argumentHandler)
            {
                BasePath = basePath ?? ".";
                ResponseFileHandling = responseFileHandling;
                ArgumentHandler = argumentHandler;
            }

            public ParseOptions WithBasePath(string basePath)
                => new ParseOptions(
                    basePath,
                    ResponseFileHandling,
                    ArgumentHandler);

            public ParseOptions WithResponseFileHandling(ResponseFileHandling responseFileHandling)
                => new ParseOptions(
                    BasePath,
                    responseFileHandling,
                    ArgumentHandler);

            public ParseOptions WithArgumentHandler(ArgumentHandler? argumentHandler)
                => new ParseOptions(
                    BasePath,
                    ResponseFileHandling,
                    argumentHandler);
        }

        public static IReadOnlyList<string> ParseArguments(
            string? commandLine,
            ParseOptions? parseOptions = null)
            => new Parser(parseOptions)
                .ParseArguments(commandLine)
                .ParsedArguments;

        public static IReadOnlyList<string> ParseResponseFile(
            string responseFilePath,
            ParseOptions? parseOptions = null)
        {
            if (responseFilePath is null)
                throw new ArgumentNullException(nameof(responseFilePath));

            return new Parser(parseOptions)
                .ParseResponseFile(responseFilePath)
                .ParsedArguments;
        }

        sealed class Parser
        {
            readonly ParseContext context;

            public IReadOnlyList<string> ParsedArguments => context.ParsedArguments;

            public Parser(ParseOptions? parseOptions)
                => context = new ParseContext(parseOptions ?? ParseOptions.Create());

            void AppendArgument(
                StringBuilder argumentBuilder,
                char quoteChar = char.MinValue)
            {
                if (quoteChar == char.MinValue && argumentBuilder.Length == 0)
                    return;

                var parseOptions = context.CurrentOptions;

                if (parseOptions.ArgumentHandler is null)
                {
                    Add(argumentBuilder.ToString());
                }
                else
                {
                    foreach (var handledArgument in parseOptions.ArgumentHandler(
                        parseContext: context,
                        shellExpand: quoteChar != '\'',
                        argument: argumentBuilder.ToString()))
                        Add(handledArgument);
                }

                void Add(string argument)
                {
                    if (parseOptions.ResponseFileHandling != ResponseFileHandling.None &&
                        IsResponseFileArgument(argument))
                    {
                        ParseResponseFile(argument);
                        return;
                    }

                    context.ParsedArguments.Add(argument);
                }
            }

            public Parser ParseArguments(string? commandLine)
            {
                if (string.IsNullOrEmpty(commandLine))
                    return this;

                var builder = new StringBuilder();
                var quote = char.MinValue;

                for (int i = 0; i < commandLine.Length; i++)
                {
                    var c = commandLine[i];
                    var n = char.MinValue;
                    if (i < commandLine.Length - 1)
                        n = commandLine[i + 1];

                    if (char.IsWhiteSpace(c) && quote == char.MinValue)
                    {
                        AppendArgument(builder);
                        builder.Length = 0;
                    }
                    else if (quote == char.MinValue && (c == '\'' || c == '"'))
                    {
                        quote = c;
                    }
                    else if (c == '\\' && n == quote)
                    {
                        builder.Append(n);
                        i++;
                    }
                    else if (quote != char.MinValue && c == quote)
                    {
                        AppendArgument(builder, quote);
                        builder.Length = 0;
                        quote = char.MinValue;
                    }
                    else
                    {
                        builder.Append(c);
                    }
                }

                if (builder.Length > 0)
                    AppendArgument(builder);

                return this;
            }

            public Parser ParseResponseFile(string responseFilePath)
            {
                var parseOptions = context.CurrentOptions;

                if (responseFilePath.Length > 0 && responseFilePath[0] == '@')
                    responseFilePath = responseFilePath.Substring(1);

                responseFilePath = Path.GetFullPath(Path.Combine(
                    parseOptions.BasePath,
                    responseFilePath));

                if (context.ResponseFileStack.Any(r => r.FilePath == responseFilePath))
                    throw new ResponseFileCycleException(
                        responseFilePath,
                        context.ResponseFileStack.Select(r => r.FilePath).ToList());

                context.ResponseFileStack.Push((
                    FilePath: responseFilePath,
                    ParseOptions: parseOptions.WithBasePath(
                        Path.GetDirectoryName(responseFilePath) ?? ".")
                ));

                foreach (var line in File.ReadLines(responseFilePath))
                    ParseArguments(line);

                context.ResponseFileStack.Pop();

                return this;
            }
        }

        public sealed class ResponseFileCycleException : Exception
        {
            public string ResponseFileWithCycle { get; }
            public IReadOnlyList<string> ResponseFileStack { get; }

            internal ResponseFileCycleException(
                string responseFileWithCycle,
                IReadOnlyList<string> responseFileStack)
                : base(
                    $"Response file creates a cycle: '{responseFileWithCycle}' " +
                    $"(referencing file: '{responseFileStack.Last()})'")
            {
                ResponseFileWithCycle = responseFileWithCycle;
                ResponseFileStack = responseFileStack;
            }
        }

        /// <summary>
        /// Quote a single argument if necessary.
        /// </summary>
        public static string QuoteArgument(string? argument)
        {
            if (string.IsNullOrEmpty(argument))
                return "\"\"";

            StringBuilder? builder = null;

            for (int i = 0; i < argument.Length; i++)
            {
                var c = argument[i];
                if (char.IsWhiteSpace(c) || c == '\b' || c == '"' || c == '\\')
                {
                    if (builder is null)
                    {
                        builder = new StringBuilder(argument.Length + 8);
                        builder.Append('"');
                        builder.Append(argument.Substring(0, i));
                    }

                    if (c == '"' || c == '\\')
                        builder.Append('\\');
                }

                if (builder != null)
                    builder.Append(c);
            }

            if (builder is null)
                return argument;

            builder.Append('"');
            return builder.ToString();
        }

        public static bool IsResponseFileArgument(string? argument)
        {
            if (string.IsNullOrEmpty(argument))
                return false;

            return argument[0] == '@';
        }

        public static string ToString(IEnumerable<string>? arguments)
        {
            if (arguments is null)
                return string.Empty;

            return string.Join(" ", arguments.Select(argument => QuoteArgument(argument)));
        }
    }
}
