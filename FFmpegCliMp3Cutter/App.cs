using Spectre.Console.Cli;
using System.Text;

namespace FFmpegCliMp3Cutter;
internal class App
{
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
            config.AddCommand<CliCommand>("cli")
            .WithDescription(CLIDesc())
            .WithExample("cli", @"C:\user\selected.mp3", "-f", "01:30", "-b", "02:00")
            .WithExample("cli", @"C:\user\selected.mp3", "-f", "01:30")
            .WithExample("cli", @"C:\user\selected.mp3", "-b", "02:00");

            config.AddCommand<TuiCommand>("tui")
            .WithDescription(TUIDesc())
            .WithExample("tui")
            .WithExample("tui", @"C:\user\selected");
        });

        return app.Run(args);
    }

    private string TUIDesc()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Opens this App in its TUI mode")
        .AppendLine("Uses the provided path if specified, otherwise defaults to the working directory.");
        return sb.ToString();
    }

    private string CLIDesc()
    {
        StringBuilder sb = new StringBuilder();
        _ = sb.AppendLine("Opens this App in its CLI mode")
            .AppendLine("This C# app processes a file by cutting it between specified start and end points.")
            .AppendLine("If only an end point is given, it cuts from the beginning to that point.")
            .AppendLine("If only a start point is provided, it cuts from that point to the end.")
            .AppendLine("At least one parameter must be specified.");
        return sb.ToString();
    }
}
