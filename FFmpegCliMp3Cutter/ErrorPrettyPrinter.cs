using Spectre.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FFmpegCliMp3Cutter.GlobalValues;

namespace FFmpegCliMp3Cutter;
internal class ErrorPrettyPrinter
{
    // 1 = path is null or empty
    // 2 = inputfile does not exist
    // 3 = inputfile is not supported
    // 4 = frontcut does not represent timevalue
    // 5 = backcut does not represent timevalue
    // 6 = front and back not specified impedes execution.

    private int error;
    private Dictionary<int, Func<int>> errorPrinters = new Dictionary<int, Func<int>>();

    public ErrorPrettyPrinter(int error)
    {
        this.error = error;
        PopulateErrorPrinters();
    }

    public ErrorPrettyPrinter()
    {
        error = 0;
        PopulateErrorPrinters();
    }

    private void PopulateErrorPrinters()
    {
        errorPrinters.Add(0, PrettyPrintError0);
        errorPrinters.Add(1, PrettyPrintError1);
        errorPrinters.Add(2, PrettyPrintError2);
        errorPrinters.Add(3, PrettyPrintError3);
        errorPrinters.Add(4, PrettyPrintError4);
        errorPrinters.Add(5, PrettyPrintError5);
        errorPrinters.Add(6, PrettyPrintError6);
    }

    public int PrettyPrint()
    {
        int errorCode = error;
        // Try to retrieve the pretty print function for the given error code
        if (errorPrinters.TryGetValue(errorCode, out Func<int>? prettyPrintFunction))
        {
            // Invoke the pretty print function and return its result
            return prettyPrintFunction?.Invoke() ?? PrettyPrintUnknownError(errorCode);
        }
        else
        {
            // Handle unknown error code
            return PrettyPrintUnknownError(errorCode);
        }
    }

    // Method to pretty print error 0
    private int PrettyPrintError0()
    {
        // Spectre pretty print
        string message = "[#00C800]Operation Successful[/]";
        AnsiConsole.Markup(message);
        return 0;
    }

    // Method to pretty print error 1
    private int PrettyPrintError1()
    {
        // Spectre pretty print
        string lineBreak = Environment.NewLine;
        string part1 = "During Execution, the Error Code (1) has been encountered.";
        string part2 = "This Error occurs when an empty or null string is encountered where a path to a file is expected. ";
        string part3 = "Please ensure that you specify a valid input path and try again.";
        string full = $"{part1}{lineBreak}{part2}{lineBreak}{part3}";
        return 1;
    }

    // Method to pretty print error 2
    private int PrettyPrintError2()
    {
        // Spectre pretty print
        return 2;
    }

    // Method to pretty print error 3
    private int PrettyPrintError3()
    {
        // Spectre pretty print
        return 3;
    }

    // Method to pretty print error 4
    private int PrettyPrintError4()
    {
        // Spectre pretty print
        return 4;
    }

    // Method to pretty print error 5
    private int PrettyPrintError5()
    {
        // Spectre pretty print
        return 5;
    }

    // Method to pretty print error 6
    private int PrettyPrintError6()
    {
        // Spectre pretty print
        return 6;
    }

    // Method to pretty print unknown error code
    public int PrettyPrintUnknownError(int errorCode)
    {
        // Unknown error code
        string first = "During execution,";
        string unkown = $"[bold underline #FFA500]an unknown Error Code:[/] [#7DF9FF]([/][bold #CE2029]{errorCode}[/][#7DF9FF])[/]";
        string second = "has been encountered. Please report this issue to the";
        string maintainers = $"[blue]project maintainers[/]";

        string error = $"{first} {unkown} {second} {maintainers}.";
        string link = "[#FFA500]Repository:[/] [#7DF9FF][link]https://github.com/EdgeLordKirito/FFmpegCliMp3Cutter/issues [/][/]";
        AnsiConsole.Markup(error);
        Console.WriteLine();
        AnsiConsole.Markup(link);

        return errorCode; // Or any other default value
    }

}
