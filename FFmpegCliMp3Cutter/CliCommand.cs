using FFmpegCliMp3Cutter.FFmpeg;
using FFmpegCliMp3Cutter.Spectre;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace FFmpegCliMp3Cutter;
internal sealed class CliCommand : Command<CliCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [Description("Path to target file.")]
        [CommandArgument(0, "[filePath]")]
        public string? FilePath { get; init; }
        [CommandOption("-f|--front")]
        public string? FrontCut { get; init; }
        [CommandOption("-b|--back")]
        public string? BackCut { get; init; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {

        //Console.WriteLine("We are executing in CLI mode");
        //Debug prints
        //Console.WriteLine($"Path = [{settings.FilePath}]");
        //Console.WriteLine($"Front = [{settings.FrontCut}]");
        //Console.WriteLine($"Back = [{settings.BackCut}]");
        //Note: While this code does its purpose it is not of good Quality and needs work
        //TODO: this entire section needs rework
        var pathValidation = ValidatePath(settings.FilePath);
        int fronterrorCode = 0;
        int backerrorCode = 0;
        TimeStampWrap frontStamp;
        TimeStampWrap backStamp;
        try
        {
            frontStamp = new TimeStampWrap(settings.FrontCut);
        }
        catch (ArgumentException)
        {
            frontStamp = new TimeStampWrap();
            fronterrorCode = 3;
        }

        try
        {
            backStamp = new TimeStampWrap(settings.BackCut);
        }
        catch (ArgumentException)
        {
            backStamp = new TimeStampWrap();
            backerrorCode = 3;
        }

        int errorValue = ExecutionErrorDecider(pathValidation.error, fronterrorCode, backerrorCode);
        if (errorValue != 0)
        {
            //replace return with a function that still returns the errorcode but also prints a message 
            ErrorPrettyPrinter printer = new ErrorPrettyPrinter(errorValue);
            return printer.PrettyPrint();
        }
        if (frontStamp.Empty && backStamp.Empty)
        {
            ErrorPrettyPrinter printer = new ErrorPrettyPrinter(6);
            return printer.PrettyPrint();
        }
        FFmpegCutTask task = new FFmpegCutTask(pathValidation.result, frontStamp, backStamp);
        //Start execution
        //Run ffmpeg on selected file
        FFmpegTaskRunSpinner spin = new FFmpegTaskRunSpinner();
        spin.Run(task);
        return 0;
    }

    private int ExecutionErrorDecider(int path, int front, int back)
    {
        // 1 = path is null or empty
        // 2 = inputfile does not exist
        // 3 = inputfile is not supported
        // 4 = frontcut does not represent timevalue
        // 5 = backcut does not represent timevalue
        // 6 = front and back not specified impedes execution.
        if (path != 0)
        {
            if (path == 2)
            {
                return 1;
            }

            if (path == 3)
            {
                return 2;
            }

            if (path == 4)
            {
                return 3;
            }
        }

        if (front == 3)
        {
            return 4;
        }

        if (back == 3)
        {
            return 5;
        }

        if (front == 2 && back == 2)
        {
            return 6;
        }

        return 0;
    }

    private int PrettyPrintError(int error)
    {
        int err = error switch
        {
            1 => 1,
            2 => 2,
            3 => 3,
            4 => 4,
            5 => 5,
            6 => 6,
            _ => error
        };
        return error;
    }

    private (string result, int error) ValidatePath(string? input)
    {
        // Completed successfully = error 0
        // input is null or empty = error 2
        // input does not point to a valid file = error 3
        // file is not of supported extension = error 4
        int errorValue = 0;
        string path;
        if (string.IsNullOrEmpty(input))
        {
            errorValue = 2;
            return (string.Empty, errorValue);
        }
        else
        {
            path = input;
        }

        FileInfo info = new FileInfo(path);
        if (!info.Exists)
        {
            errorValue = 3;
            return (string.Empty, errorValue);
        }
        string extension = info.Extension[1..];
        var audios = Enum.GetNames(typeof(SupportedFiles)).ToList();
        if (!audios.Contains(extension))
        {
            errorValue = 4;
            return (string.Empty, errorValue);
        }
        return (path, errorValue);
    }

    private (string result, int error) ValidateTimeStamp(string? input)
    {
        // Completed successfully = error 0
        // input does not represent a valid time format = error 2
        int errorValue = 0;
        string result;

        if (string.IsNullOrEmpty(input))
        {
            result = string.Empty;
            errorValue = 2;
            return (string.Empty, errorValue);
        }
        else
        {
            result = input;
        }

        if (!ValidateStamp(result))
        {
            errorValue = 3;
            return (string.Empty, errorValue);
        }
        else
        {
            return (result, errorValue);
        }
    }

    private bool ValidateStamp(string input)
    {
        // hh:mm:ss.ffffff
        Regex hhmmssffffff = new Regex(@"^\d{2}:\d{2}:\d{2}\.\d{6}$", RegexOptions.Compiled);

        // mm:ss.ffffff
        Regex mmssffffff = new Regex(@"^\d{2}:\d{2}\.\d{6}$", RegexOptions.Compiled);

        // ss.ffffff
        Regex ssffffff = new Regex(@"^\d{2}\.\d{6}$", RegexOptions.Compiled);

        // hh:mm:ss
        Regex hhmmss = new Regex(@"^\d{2}:\d{2}:\d{2}$", RegexOptions.Compiled);

        // mm:ss
        Regex mmss = new Regex(@"^\d{2}:\d{2}$", RegexOptions.Compiled);

        // ss
        Regex ss = new Regex(@"^\d{2}$", RegexOptions.Compiled);

        Regex[] filters = [hhmmssffffff, mmssffffff, ssffffff, hhmmss, mmss, ss];

        foreach (Regex filter in filters)
        {
            if (filter.IsMatch(input))
            {
                return true;
            }
        }
        return false;

    }
}
