//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Mono.Options.Reflection
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class OptionAttribute : Attribute
    {
        public string Prototype { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Hidden { get; set; }

        public OptionAttribute(string prototype, string description)
        {
            Prototype = prototype;
            Description = description;
        }
    }
}
