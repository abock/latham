//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using Mono.Options;

using Latham.Commands;

namespace Latham
{
    class Program
    {
        static int Main(string[] args)
        {
            FriendlyTimeSpan.RegisterTypeConverter();

            var commandSet = new CommandSet("latham")
            {
                new RecordCommandSet(),
                new IndexCommandSet(),
                new TimelapseCommand(),
                new ProjectCommandSet()
            };

            return commandSet.Run(args);
        }
    }
}
