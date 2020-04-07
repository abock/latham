//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

using CommandLine = Latham.CommandLine;

namespace Mono.Options.Reflection
{
    abstract class ReflectionCommand : Command
    {
        public static string? ProgramName { get; set; }

        public static Func<ReflectionCommand, string, string> UsageLine { get; set; }
            = (command, fullCommandName) => $"usage: {ProgramName} [GLOBAL OPTIONS] {fullCommandName} [OPTIONS]";

        static readonly Regex DirectiveRegex = new Regex(@"^\[(?<directive>\w+)\]$");

        protected ReflectionCommand(
            CommandSet? commandSet,
            string name,
            string? help = null)
            : base(name, help)
        {
            var fullCommandName = name;
            if (commandSet?.Suite is string suite)
                fullCommandName = $"{suite} {name}";

            Options = new OptionSet
            {
                UsageLine(this, fullCommandName),
                "",
                Help,
                "",
                "Options:",
                ""
            };

            foreach (var property in GetType().GetProperties())
            {
                var optionAttribute = property.GetCustomAttribute<OptionAttribute>();
                if (optionAttribute is null)
                    continue;

                Options.Add(new ReflectionOption(this, optionAttribute, property));
            }
        }

        public sealed override int Invoke(IEnumerable<string> arguments)
        {
            var directive = arguments.FirstOrDefault();
            if (directive is string)
            {
                var directiveMatch = DirectiveRegex.Match(directive);
                if (directiveMatch.Success)
                {
                    arguments = arguments.Skip(1);
                    directive = directiveMatch.Groups["directive"].Value;
                }
            }

            base.Invoke(ExpandResponseFiles(arguments));

            var result = BeforeInvoke(directive);

            if (result is null)
            {
                result = Invoke();
                result = AfterInvoke((int)result, directive);
            }

            return result.Value;

            static IEnumerable<string> ExpandResponseFiles(IEnumerable<string> arguments)
            {
                foreach (var argument in arguments)
                {
                    if (CommandLine.IsResponseFileArgument(argument))
                    {
                        foreach (var responseArgument in CommandLine.ParseResponseFile(
                            argument,
                            CommandLine.ParseOptions.Create(argumentHandler: ArgumentGlobber)))
                            yield return responseArgument;
                    }
                    else
                    {
                        yield return argument;
                    }
                }
            }
        }

        static IEnumerable<string> ArgumentGlobber(
            CommandLine.ParseContext context,
            bool shellExpand,
            string argument)
        {
            if (shellExpand)
            {
                foreach (var expandedArgument in Xamarin.PathHelpers.Glob.ShellExpand(
                    context.CurrentOptions.BasePath,
                    argument))
                    yield return expandedArgument;
            }
            else
            {
                yield return argument;
            }
        }

        protected virtual int? BeforeInvoke(string directive)
        {
            if (string.Equals(directive, "explain", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(directive, "parse", StringComparison.OrdinalIgnoreCase))
            {
                Explain();
                return 0;
            }

            return null;
        }

        protected abstract int Invoke();

        protected virtual int AfterInvoke(int invokeResultCode, string directive)
            => invokeResultCode;

        protected void Explain()
        {
            var json = JsonConvert.SerializeObject(
                Options
                    .OfType<ReflectionOption>()
                    .ToDictionary(
                        o =>
                        {
                            var optionName = o.GetNames().OrderByDescending(n => n.Length).First();
                            if (optionName == "<>")
                                optionName = "POSITIONAL_ARGS";
                            else if (optionName.Length == 1)
                                optionName = "-" + optionName;
                            else
                                optionName = "--" + optionName;
                            return optionName;
                        },
                        o => o.Property.GetValue(o.Command)),
                Formatting.Indented);
            Console.WriteLine(json);
        }
    }
}
