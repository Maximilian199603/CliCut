using Spectre.Console;
using static FFmpegCliMp3Cutter.GlobalValues;

namespace FFmpegCliMp3Cutter;

internal class Program
{
    static void Main(string[] args)
    {
        ErrorPrettyPrinter printer = new ErrorPrettyPrinter(1);
        printer.PrettyPrint();
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
