//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Latham
{
    public static class TemplateString
    {
        enum ParseState
        {
            Verbatim,
            Variable,
            Alignment,
            Format
        }

        public readonly struct Interpolation : IEquatable<Interpolation>
        {
            public string? Variable { get; }
            public string? VariableLowerInvariant { get; }
            public string? Alignment { get; }
            public string? Format { get; }

            public Interpolation(
                string? variable,
                string? alignment,
                string? format)
            {
                Variable = variable;
                VariableLowerInvariant = variable?.ToLowerInvariant();
                Alignment = alignment;
                Format = format;
            }

            public Interpolation WithExpression(string? expression)
                => new Interpolation(expression, Alignment, Format);

            public Interpolation WithAlignment(string? alignment)
                => new Interpolation(Variable, alignment, Format);

            public Interpolation WithFormat(string? format)
                => new Interpolation(Variable, Alignment, format);

            public string ToString(object? value)
                => string.Format(WithExpression("0").ToString(), value);

            public string ToString(object? value, IFormatProvider formatProvider)
                => string.Format(formatProvider, WithExpression("0").ToString(), value);

            public override string ToString()
            {
                if (Alignment is string && Format is string)
                    return $"{{{Variable},{Alignment}:{Format}}}";
                else if (Alignment is string)
                    return $"{{{Variable},{Alignment}}}";
                else if (Format is string)
                    return $"{{{Variable}:{Format}}}";
                else
                    return $"{{{Variable}}}";
            }

            public bool Equals(Interpolation other)
                => other.Variable == Variable &&
                    other.Alignment == Alignment &&
                    other.Format == Format;

            public override bool Equals(object? obj)
                => obj is Interpolation interpolation && Equals(interpolation);

            public override int GetHashCode()
            {
                const int factor = unchecked((int)0xa5555529);
                return
                    (factor + (Variable is string variable ? variable.GetHashCode() : 0)) *
                    (factor + (Alignment is string alignment ? alignment.GetHashCode() : 0)) *
                    (factor + (Format is string format ? format.GetHashCode() : 0));
            }
        }

        public static FormattableString? Interpolate(
            string? template,
            Func<Interpolation, object?> interpolationHandler)
        {
            if (interpolationHandler is null)
                throw new ArgumentNullException(nameof(interpolationHandler));

            if (template is null)
                return null;

            if (template.Length == 0)
                return FormattableStringFactory.Create(string.Empty);

            var arguments = new List<object?>();
            var builder = new StringBuilder(template.Length * 2);
            var state = ParseState.Verbatim;
            string? variable = null;
            string? alignment = null;
            string? format = null;
            (int start, int length) part = default;

            for (int i = 0; i < template.Length; i++)
            {
                var c = template[i];

                if (state == ParseState.Verbatim)
                {
                    if ((c == '{' || c == '}') && i < template.Length - 1)
                    {
                        if (template[i + 1] == c)
                        {
                            builder
                                .Append(c)
                                .Append(c);
                            i++;
                        }
                        else if (c == '{')
                        {
                            state = ParseState.Variable;
                            part = (i + 1, 0);
                        }
                    }
                    else
                    {
                        builder.Append(c);
                    }
                }
                else if (state == ParseState.Variable)
                {
                    if (c == ',')
                    {
                        CommitPart(ref variable);
                        state = ParseState.Alignment;
                    }
                    else if (c == ':')
                    {
                        CommitPart(ref variable);
                        state = ParseState.Format;
                    }
                    else if (c == '}')
                    {
                        CommitPart(ref variable, true);
                        state = ParseState.Verbatim;
                    }
                    else
                    {
                        part.length++;
                    }
                }
                else if (state == ParseState.Alignment)
                {
                    if (c == ':')
                    {
                        CommitPart(ref alignment);
                        state = ParseState.Format;
                    }
                    else if (c == '}')
                    {
                        CommitPart(ref alignment, true);
                        state = ParseState.Verbatim;
                    }
                    else
                    {
                        part.length++;
                    }
                }
                else if (state == ParseState.Format)
                {
                    if (c == '}')
                    {
                        CommitPart(ref format, true);
                        state = ParseState.Verbatim;
                    }
                    else
                    {
                        part.length++;
                    }
                }
                else
                {
                    throw new NotImplementedException($"state: {state}");
                }
            }

            void CommitPart(ref string? partBuffer, bool closed = false)
            {
                partBuffer = template is string && part.length > 0
                    ? template.Substring(part.start, part.length)
                    : null;

                part.start += part.length + 1;
                part.length = 0;

                if (closed)
                {
                    builder.Append('{').Append(arguments.Count.ToString(CultureInfo.InvariantCulture));

                    if (!string.IsNullOrEmpty(alignment))
                        builder.Append(',').Append(alignment);

                    if (!string.IsNullOrEmpty(format))
                        builder.Append(':').Append(format);

                    builder.Append('}');

                    arguments.Add(interpolationHandler(new Interpolation(
                        variable,
                        alignment,
                        format)));
                }
            }

            return FormattableStringFactory.Create(
                builder.ToString(),
                arguments.ToArray());
        }
    }
}
