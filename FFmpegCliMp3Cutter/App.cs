using Spectre.Console.Cli;
using System.Text;

namespace FFmpegCliMp3Cutter;
internal class App
{
    //remove file extension checking
    private string[] args;

    public App(string[] args)
    {
        this.args = args;
    }

    public int Run()
    {
        var app = new CommandApp();

        app.Configure(config =>
        {
            config.AddCommand<CliCommand>("cut")
            .WithDescription(CLIDesc())
            .WithExample("cut", @"C:\user\selected.mp3", "-f", "01:30", "-b", "02:00")
            .WithExample("cut", @"C:\user\selected.mp3", "-f", "01:30")
            .WithExample("cut", @"C:\user\selected.mp3", "-b", "02:00");
        });

        return app.Run(args);
    }

    private string BatchDesc()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Opens the specified file and reads each line as a list of Arguments")
        .AppendLine(@"Any line starting with # or // are ignored as Comments");
        return sb.ToString();
    }

    private string CLIDesc()
    {
        StringBuilder sb = new StringBuilder();
        _ = sb
            .AppendLine("This C# app processes a file by cutting it between specified start and end points.")
            .AppendLine("If only an end point is given, it cuts from the beginning to that point.")
            .AppendLine("If only a start point is provided, it cuts from that point to the end.")
            .AppendLine("At least one parameter must be specified.");
        return sb.ToString();
    }
}
