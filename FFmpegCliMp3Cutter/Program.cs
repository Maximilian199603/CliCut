using FFmpegCliMp3Cutter.FFmpeg;
using FFmpegCliMp3Cutter.Spectre;
using Spectre.Console;
using static FFmpegCliMp3Cutter.GlobalValues;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FFmpegCliMp3Cutter;

internal class Program
{
    static void Main(string[] args)
    {
        //check for palette.toml file check if it conforms to the standard
        //load it in the place where defaultpalette is
        //test values
        //common = BB22EE
        //highlight = CC00DD
        //error = CE2029
        //success = 00C800
        //accent = 7755FF
        //bg = 435460
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
