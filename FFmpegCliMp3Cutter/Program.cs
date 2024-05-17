using FFmpegCliMp3Cutter.FFmpeg;
using FFmpegCliMp3Cutter.Spectre;
using Spectre.Console;
using static FFmpegCliMp3Cutter.GlobalValues;

namespace FFmpegCliMp3Cutter;

internal class Program
{
    static void Main(string[] args)
    {

        List<FFmpegCutTask> tasks = new List<FFmpegCutTask>();
        tasks.Add(new FFmpegCutTask(@"C:\FFmpegTest\target.mp4", new TimeStampWrap(), new TimeStampWrap("01:30")));
        tasks.Add(new FFmpegCutTask(@"C:\FFmpegTest\DREAMOIR_-_Worth_It.mp3", new TimeStampWrap("30"), new TimeStampWrap("01:30")));
        tasks.Add(new FFmpegCutTask(@"C:\FFmpegTest\Calmani_Grey_-_Tattoo_ft._Pearl_Andersson.mp3", new TimeStampWrap("30"), new TimeStampWrap()));
        tasks.Add(new FFmpegCutTask(@"C:\FFmpegTest\target.mp3", new TimeStampWrap("01:00"), new TimeStampWrap("01:30")));
        var init = new SpinnerStyle("Starting",Spinner.Known.Ascii, new Style(Color.Cyan1), new Style(Color.CornflowerBlue, Color.DeepPink4));
        FFmpegTaskRunSpinner spin = new FFmpegTaskRunSpinner(init);
        spin.RunFFmpegCutTasks(tasks, 10);

        return;
        if (args.Length == 0)
        {
            //print help message
            string usage = "Usage:";
            string appname = DyeString(ApplicationName, "yellow");
            string command = WrapInSquare("command", "bold blue", "green");
            string options = WrapInSquare("option", "bold blue", "green");
            string line1 = $"{usage} {appname} {command} {options}";
            string help = WrapInSquare("help", "bold blue", "green");
            string line2 = $"To see a full list of available {DyeString("commands", "yellow")} and their {DyeString("options", "yellow")} use the {help} command";
            AnsiConsole.Markup(line1);
            Console.WriteLine();
            AnsiConsole.Markup(line2);
        }
        else
        {
            //construct args for spectrecli
        }
    }

    private static string WrapInSquare(string input, string textStyling, string bracketStyling)
    {
        return $"[{bracketStyling}][[[/][{textStyling}]{input}[/][{bracketStyling}]]][/]";
    }

    private static string DyeString(string input, string styling)
    {
        return $"[{styling}]{input}[/]";
    }
}
