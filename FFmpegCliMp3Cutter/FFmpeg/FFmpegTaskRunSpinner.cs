using FFmpegCliMp3Cutter.Spectre;
using FFmpegCliMp3Cutter.Styling;
using Spectre.Console;
using System.Diagnostics;

namespace FFmpegCliMp3Cutter.FFmpeg;

//TODO: Have a better way for specifying styles
//something like an colour palette object that is read at startup
//or an command that sets an palette.toml file as the style
internal class FFmpegTaskRunSpinner
{
    private readonly SpinnerStyle _initStyle;
    public FFmpegTaskRunSpinner(SpinnerStyle init)
    {
        _initStyle = init;
    }
    public void Run(FFmpegCutTask task)
    {
        // Create a status indicator
        AnsiConsole.Status()
            .Spinner(_initStyle.Type)
            .SpinnerStyle(_initStyle.StyleSpinner)
            .Start(_initStyle.StatusMessage, ctx =>
            {
                try
                {
                    // Execute your long-running task asynchronously and wait for it
                    Task executionTask = task.Execute();
                    executionTask.Wait();
                }
                catch (AggregateException ex)
                {
                    foreach (var innerException in ex.InnerExceptions)
                    {
                        AnsiConsole.MarkupLine($"[red]Error:[/] {innerException.Message}");
                    }
                }
            });
    }


    //COde usage example
    //List<FFmpegCutTask> tasks = new List<FFmpegCutTask>();
    //tasks.Add(new FFmpegCutTask(@"C:\FFmpegTest\target.mp4", new TimeStampWrap(), new TimeStampWrap("01:30")));
    //tasks.Add(new FFmpegCutTask(@"C:\FFmpegTest\DREAMOIR_-_Worth_It.mp3", new TimeStampWrap("30"), new TimeStampWrap("01:30")));
    //tasks.Add(new FFmpegCutTask(@"C:\FFmpegTest\Calmani_Grey_-_Tattoo_ft._Pearl_Andersson.mp3", new TimeStampWrap("30"), new TimeStampWrap()));
    //tasks.Add(new FFmpegCutTask(@"C:\FFmpegTest\target.mp3", new TimeStampWrap("01:00"), new TimeStampWrap("01:30")));
    //var init = new SpinnerStyle("Starting", Spinner.Known.Ascii, new Style(Color.Cyan1), new Style(Color.CornflowerBlue, Color.DeepPink4));
    //FFmpegTaskRunSpinner spin = new FFmpegTaskRunSpinner(init);
    //spin.RunFFmpegCutTasks(tasks, 10);
    public void RunFFmpegCutTasks(List<FFmpegCutTask> tasks)
    {
        var stopwatch = Stopwatch.StartNew();
        (int completed, int failed) = SpinToWin(tasks);
        stopwatch.Stop();
        var totalExecutionTime = stopwatch.Elapsed;
        var averageTimePerTask = totalExecutionTime / tasks.Count;

        HexStyle success = new HexStyle("00C800");
        HexStyle failure = new HexStyle("CE2029");
        HexStyle accent = new HexStyle("7755FF");

        // Display completion status with total execution time and average time per task
        AnsiConsole.MarkupLine($"[{success.ToMarkup()}]{completed}[/] completed, [{failure.ToMarkup()}]{failed}[/] failed.");

        AnsiConsole.MarkupLine($"[{accent.ToMarkup()}]{FormatTimeSpan(totalExecutionTime)}[/] elapsed. ");
        AnsiConsole.MarkupLine($"Average time per task: [{accent.ToMarkup()}]{FormatTimeSpan(averageTimePerTask)}[/] per task.");
    }

    private (int completed, int failed) SpinToWin(List<FFmpegCutTask> tasks)
    {
        (int, int) vals = (0, 0);
        AnsiConsole.Status()
            .Spinner(_initStyle.Type)
            .SpinnerStyle(_initStyle.StyleSpinner)
            .Start(_initStyle.StatusMessage, ctx =>
            {
                TaskStatusPrinter stat = new TaskStatusPrinter(ctx, new HexStyle("00C800"), new HexStyle("7755FF"), new HexStyle("CE2029"));
                var tup = RunAsPool(tasks, stat, 5);
                vals = (tup.completed, tup.failed);
            });
        return vals;
    }


    private (int completed, int failed, int total) RunAsPool(List<FFmpegCutTask> tasks, TaskStatusPrinter stat, int maxConcurrency)
    {
        Tracker tracker = new Tracker();
        tracker.Total = tasks.Count;
        var semaphore = new SemaphoreSlim(maxConcurrency);
        var taskList = new List<Task>();
        foreach (var task in tasks)
        {
            ExecuteTask(task, semaphore, ref tracker, taskList);
            stat.GetStatus(tracker.Completed, tracker.Total, tracker.Failed);
        }
        Task.WaitAll(taskList.ToArray());
        semaphore.Release();
        return (tracker.Completed, tracker.Failed, tracker.Total);
    }

    private void ExecuteTask(FFmpegCutTask task, SemaphoreSlim semaphore,
        ref Tracker tracker, List<Task> taskList)
    {
        bool success = true;
        semaphore.Wait();

        var t = Task.Run(async () =>
        {
            try
            {
                await task.Execute();
            }
            catch (Exception ex)
            {
                success = false;
                // TODO: log the exception instead of throwing
            }
            finally
            {
                semaphore.Release();
            }
        });

        taskList.Add(t);
        if (success)
        {
            tracker.IncrementCompleted();
        }
        else
        {
            tracker.IncrementFailed();
        }
    }

    private string FormatTimeSpan(TimeSpan timeSpan)
    {
        if (timeSpan.TotalDays >= 1)
        {
            return $"{timeSpan.TotalDays:0} day(s), {timeSpan.Hours:0} hour(s), {timeSpan.Minutes:0} minute(s)";
        }
        if (timeSpan.TotalHours >= 1)
        {
            return $"{timeSpan.TotalHours:0} hour(s), {timeSpan.Minutes:0} minute(s), {timeSpan.Seconds:0} second(s)";
        }
        if (timeSpan.TotalMinutes >= 1)
        {
            return $"{timeSpan.TotalMinutes:0} minute(s), {timeSpan.Seconds:0} second(s)";
        }

        return $"{timeSpan.TotalSeconds:0} second(s)";
    }

    private ref struct Tracker
    {
        private ref int _completed;
        private ref int _failed;

        public int Completed => Volatile.Read(ref _completed);
        public int Failed => Volatile.Read(ref _failed);
        public int Total { get; set; }

        public static Tracker Create()
        {
            Tracker tracker = new Tracker();
            tracker._completed = 0;
            tracker._failed = 0;
            return tracker;
        }

        public void IncrementCompleted()
        {
            Interlocked.Increment(ref _completed);
        }

        public void DecrementCompleted()
        {
            Interlocked.Decrement(ref _completed);
        }

        public void IncrementFailed()
        {
            Interlocked.Increment(ref _failed);
        }

        public void DecrementFailed()
        {
            Interlocked.Decrement(ref _failed);
        }
    }

    private class TaskStatusPrinter
    {
        private readonly HexStyle _completedStyle;
        private readonly HexStyle _totalStyle;
        private readonly HexStyle _failedStyle;
        private StatusContext _statusContext;

        public TaskStatusPrinter(StatusContext statusContext, HexStyle completedStyle, HexStyle totalStyle, HexStyle failedStyle)
        {
            _statusContext = statusContext;
            _completedStyle = completedStyle;
            _totalStyle = totalStyle;
            _failedStyle = failedStyle;
        }

        public void GetStatus(int completedTasks, int totalTasks, int failedTasks)
        {
            string message = $"[{_completedStyle.ToMarkup()}]{completedTasks}[/] / [{_totalStyle.ToMarkup()}]{totalTasks}[/] tasks with [{_failedStyle.ToMarkup()}]{failedTasks}[/] failures...";
            _statusContext.Status(message);
        }
    }
}
