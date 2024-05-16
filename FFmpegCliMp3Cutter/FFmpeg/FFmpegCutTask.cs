using CliWrap.Builders;
using CliWrap;
using CliWrap.Buffered;
using Spectre.Console;

namespace FFmpegCliMp3Cutter.FFmpeg;
internal class FFmpegCutTask
{
    private FileInfo _file;
    private TimeStampWrap _front;
    private TimeStampWrap _back;
    public bool InPath { get; set; }
    public FFmpegCutTask(string input, TimeStampWrap front, TimeStampWrap back)
    {
        _front = front;
        _back = back;
        _file = new FileInfo(input);
    }

    public async Task<FileInfo?> Execute()
    {
        string outputPath = GetOutputFileName(_file);
        ArgumentsBuilder args = new ArgumentsBuilder();
        bool invalid = false;
        // Check if ffmpeg path should be included in the command
        string ffmpegPath = "ffmpeg"; // Default ffmpeg path
        if (!InPath)
        {
            // Use custom ffmpeg path
            ffmpegPath = @"path\to\ffmpeg.exe"; // Update with the custom ffmpeg path
        }
        //in switch assemble the command for ffmpeg
        switch ((_front.Empty, _back.Empty))
        {
            case (true, false):
                // From start of file to specified end point
                args = new ArgumentsBuilder()
                    .Add("-i")
                    .Add(_file.FullName)
                    .Add("-to")
                    .Add(_back.Value)
                    .Add(outputPath);
                break;
            case (false, true):
                // From specified start point to end of file
                args = new ArgumentsBuilder()
                    .Add("-i")
                    .Add(_file.FullName)
                    .Add("-ss")
                    .Add(_front.Value)
                    .Add(outputPath);
                break;
            case (false, false):
                // From specified start to specified end
                args = new ArgumentsBuilder()
                    .Add("-i")
                    .Add(_file.FullName)
                    .Add("-ss")
                    .Add(_front.Value)
                    .Add("-to")
                    .Add(_back.Value)
                    .Add(outputPath);
                break;
            case (true, true):
                //invalid state execution cannot be done
                invalid = true;
                break;
        }

        if (invalid)
        {
            return null;
        }

        try
        {
            //add stdout and stderr redirect
            var result = await Cli.Wrap(ffmpegPath)
                .WithArguments(args.Build())
                .ExecuteAsync();
        }catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }

        return new FileInfo(outputPath);
    }


    private string GetOutputFileName(FileInfo inputFile)
    {
        string? directory = inputFile.DirectoryName;
        string dir;
        if (directory is null)
        {
            throw new ArgumentException("Invalid input file path.", nameof(inputFile));
        }
        else
        {
            dir = directory;
        }
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputFile.Name);
        string extension = inputFile.Extension;
        string outputFileName = $"{dir}\\{fileNameWithoutExtension}[cut]{extension}";

        return outputFileName;
    }
}
