using Spectre.Console.Cli;
using System.ComponentModel;
namespace FFmpegCliMp3Cutter;
internal sealed class TuiCommand : Command<TuiCommand.Settings>
{
    public override int Execute(CommandContext context, Settings settings)
    {
        //Console.WriteLine("We Are executing in TUI mode");
        //debug prints
        //Console.WriteLine($"Path = [{settings.FilePath}]");
        //launch terminal gui app here 
        return 0;
        throw new NotImplementedException();
    }

    public sealed class Settings : CommandSettings
    {
        [Description("Path to target file. Defaults to current directory.")]
        [CommandArgument(0, "[filePath]")]
        public string? FilePath { get; init; }
    }
}
