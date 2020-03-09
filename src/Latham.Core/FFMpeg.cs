using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.ProcessControl;

namespace Latham
{
    public static class FFMpeg
    {
        public static string Path { get; set; } = "ffmpeg";

        public static Task<int> RunAsync(TextWriter outputWriter, params string[] arguments)
            => RunAsync(outputWriter, outputWriter, default, arguments);

        public static Task<int> RunAsync(TextWriter stdout, TextWriter stderr, params string[] arguments)
            => RunAsync(stdout, stderr, default, arguments);

        public static async Task<int> RunAsync(
            TextWriter stdout,
            TextWriter stderr,
            CancellationToken cancellationToken,
            params string[] arguments)
        {
            var commandLine = ProcessArguments.FromCommandAndArguments(Path, arguments);

            Console.WriteLine($"[{DateTime.Now:O}] {commandLine}");

            stdout.WriteLine(commandLine);
            stdout.Flush();

            return (await Exec.RunAsync(output =>
            {
                switch (output.FileDescriptor)
                {
                    case ConsoleRedirection.FileDescriptor.Output:
                        stdout.Write(output.Data);
                        stdout.Flush();
                        break;
                    case ConsoleRedirection.FileDescriptor.Error:
                        stderr.Write(output.Data);
                        stdout.Flush();
                        break;
                }
            }, Path, arguments).ConfigureAwait(false)).ExitCode ?? -1;
        }
    }
}
