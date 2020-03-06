//
// Author:
//   Aaron Bockover <abock@microsoft.com>
//
// Copyright (c) Aaron Bockover. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using NCrontab;

namespace Latham
{
    public sealed class CrontabRunner
    {
        public sealed class Job
        {
            public CrontabSchedule Schedule { get; }
            public Func<DateTime, CancellationToken, Task> Action { get; }

            internal Job(CrontabSchedule schedule, Func<DateTime, CancellationToken, Task> action)
            {
                Schedule = schedule;
                Action = action;
            }
        }

        readonly List<Job> jobs = new List<Job>();

        public CrontabSchedule.ParseOptions ScheduleParseOptions { get; }
        public DateTime BaseTime { get; } = DateTime.Now;

        public CrontabRunner(CrontabSchedule.ParseOptions? scheduleParseOptions = null)
        {
            ScheduleParseOptions = scheduleParseOptions ?? new CrontabSchedule.ParseOptions
            {
                IncludingSeconds = true
            };
        }

        public Job AddJob(string schedule, Func<DateTime, CancellationToken, Task> action)
            => AddJob(
                CrontabSchedule.Parse(schedule, ScheduleParseOptions),
                action);

        public Job AddJob(CrontabSchedule schedule, Func<DateTime, CancellationToken, Task> action)
        {
            var job = new Job(schedule, action);
            jobs.Add(job);
            return job;
        }

        public void Run(CancellationToken cancellationToken = default)
        {
            var tasks = new Task[jobs.Count];

            for (int i = 0; i < tasks.Length; i++)
            {
                var job = jobs[i];
                tasks[i] = Task.Run(async () =>
                {
                    var baseTime = BaseTime;

                    while (true)
                    {
                        var nextOccurrence = job.Schedule.GetNextOccurrence(baseTime);
                        var waitTime = nextOccurrence - DateTime.Now;

                        if (waitTime > TimeSpan.Zero)
                        {
                            await Task.Delay(waitTime);
                        }

                        baseTime = DateTime.Now;

                        await job.Action(nextOccurrence, cancellationToken);
                    }
                });
            }

            Task.WaitAll(tasks);
        }
    }
}
