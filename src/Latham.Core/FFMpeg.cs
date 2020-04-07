using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Serilog;

using Xamarin.ProcessControl;

namespace Latham
{
    public static class FFMpeg
    {
        sealed class Native
        {
            readonly IntPtr handle;

            [DllImport("libc")]
            static extern IntPtr dlopen(string path, int mode);

            [DllImport("libc")]
            static extern IntPtr dlsym(IntPtr handle, string symbol);

            public delegate IntPtr avformat_alloc_context_delegate();

            public readonly avformat_alloc_context_delegate avformat_alloc_context;

            public delegate void avformat_open_input_delgate(ref IntPtr ctx, string filename, IntPtr a, IntPtr b);

            public readonly avformat_open_input_delgate avformat_open_input;

            public delegate void avformat_close_input_delegate(ref IntPtr ctx);

            public readonly avformat_close_input_delegate avformat_close_input;

            public delegate void avformat_free_context_delegate(IntPtr ctx);

            public readonly avformat_free_context_delegate avformat_free_context;

            public Native(string ffmpegExecutablePath)
            {
                var programPath = Xamarin.PathHelpers.FindProgramPath(ffmpegExecutablePath);
                if (programPath is null)
                    throw new Exception($"Unable to resolve full program path for '{ffmpegExecutablePath}' in PATH");

                handle = dlopen(programPath, 1);
                if (handle == IntPtr.Zero)
                    throw new Exception($"Unable to open '{programPath}'");

                avformat_alloc_context = dlsymfn<avformat_alloc_context_delegate>(nameof(avformat_alloc_context));
                if (avformat_alloc_context is null)
                    throw new Exception($"Unable to resolve '{nameof(avformat_alloc_context)}' in '{programPath}'");

                avformat_open_input = dlsymfn<avformat_open_input_delgate>(nameof(avformat_open_input));
                if (avformat_open_input is null)
                    throw new Exception($"Unable to resolve '{nameof(avformat_open_input)}' in '{programPath}'");

                avformat_close_input = dlsymfn<avformat_close_input_delegate>(nameof(avformat_close_input));
                if (avformat_close_input is null)
                    throw new Exception($"Unable to resolve '{nameof(avformat_close_input)}' in '{programPath}'");

                avformat_free_context = dlsymfn<avformat_free_context_delegate>(nameof(avformat_free_context));
                if (avformat_free_context is null)
                    throw new Exception($"Unable to resolve '{nameof(avformat_free_context)}' in '{programPath}'");

                T dlsymfn<T>(string symbol)
                    => Marshal.GetDelegateForFunctionPointer<T>(dlsym(handle, symbol));
            }
        }

        static Native? _native;
        static Native native => _native ?? (_native = new Native(Path));

        static readonly Regex timespanRegex = new Regex(@"time=(?<timespan>[\d\.\:]+)");

        public static string Path { get; set; } = "ffmpeg";

        public static Task<int> RunAsync(
            CancellationToken cancellationToken,
            params string[] arguments)
            => RunAsync(
                cancellationToken,
                null,
                arguments);

        public static async Task<int> RunAsync(
            CancellationToken cancellationToken,
            Action<TimeSpan>? progressHandler,
            params string[] arguments)
        {
            var commandLine = ProcessArguments.FromCommandAndArguments(Path, arguments);

            Log.Debug("Exec {Path} {Arguments}", Path, arguments);

            TimeSpan progress = default;
            progressHandler?.Invoke(progress);

            return (await Exec.RunAsync(output =>
            {
                switch (output.FileDescriptor)
                {
                    case ConsoleRedirection.FileDescriptor.Output:
                        Log.Debug("ffmpeg: {stdout}", output.Data.TrimEnd('\r', '\n'));
                        break;
                    case ConsoleRedirection.FileDescriptor.Error:
                        Log.Debug("ffmpeg: {stderr}", output.Data.TrimEnd('\r', '\n'));
                        var match = timespanRegex.Match(output.Data);
                        if (match.Success &&
                            match.Groups.TryGetValue("timespan", out var timespanGroup) &&
                            TimeSpan.TryParse(timespanGroup.Value, out var timespan) &&
                            timespan != progress)
                        {
                            progress = timespan;
                            progressHandler?.Invoke(progress);
                        }
                        break;
                }
            }, Path, arguments).ConfigureAwait(false)).ExitCode ?? -1;
        }

        public static TimeSpan ParseDuration(string path)
        {
            var ctx = native.avformat_alloc_context();
            native.avformat_open_input(ref ctx, path, default, default);

            var durationOffset =
                IntPtr.Size + // const AVClass * av_class
                IntPtr.Size + // struct AVInputFormat * iformat
                IntPtr.Size + // struct AVOutputFormat * oformat
                IntPtr.Size + // void * priv_data
                IntPtr.Size + // AVIOContext * pb
                4 +           // int ctx_flags
                4 +           // unsigned int nb_streams
                IntPtr.Size + // AVStream ** streams
                1024 +        // char filename [1024]
                8 +           // int64_t start_time
                8;            // int64_t duration

            var time = TimeSpan.FromTicks(Marshal.ReadInt64(ctx, durationOffset) * 10);

            native.avformat_close_input(ref ctx);
            native.avformat_free_context(ctx);

            return time;
        }
    }
}
