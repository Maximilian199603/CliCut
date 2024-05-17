using FFmpegCliMp3Cutter.Spectre;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public void RunFFmpegCutTasks(List<FFmpegCutTask> tasks, int maximumConcurrency)
    {
        var stopwatch = Stopwatch.StartNew();
        var semaphore = new SemaphoreSlim(maximumConcurrency);
        var taskList = new List<Task>();
        int completedTasks = 0;

        AnsiConsole.Status()
            .Spinner(_initStyle.Type)
            .SpinnerStyle(_initStyle.StyleSpinner)
            .Start(_initStyle.StatusMessage, ctx =>
            {
                foreach (var task in tasks)
                {
                    semaphore.Wait();

                    var t = Task.Run(() =>
                    {
                        try
                        {
                            task.Execute().Wait();
                            Interlocked.Increment(ref completedTasks);
                            ctx.Status($"Completed [{_initStyle.StyleSpinner.ToMarkup()}]{completedTasks}[/]/[{new Style(Color.HotPink2).ToMarkup()}]{tasks.Count}[/] tasks...");
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"[red]Error:[/] {ex.Message}");
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    });

                    taskList.Add(t);
                }

                Task.WaitAll(taskList.ToArray());

                stopwatch.Stop();
                var totalExecutionTime = stopwatch.Elapsed;
                var averageTimePerTask = totalExecutionTime / tasks.Count;

                // Display completion status with total execution time and average time per task
                AnsiConsole.MarkupLine($"Completed [yellow]{completedTasks}/{tasks.Count}[/] tasks in [yellow]" +
                    $"{FormatTimeSpan(totalExecutionTime)}[/] " +
                    $"(Average: [yellow]{FormatTimeSpan(averageTimePerTask)}[/] per task)");
            });
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


}
