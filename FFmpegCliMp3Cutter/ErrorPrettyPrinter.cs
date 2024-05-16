using Spectre.Console;
using System.Reflection;

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
    private Dictionary<int, Func<int>> errorPrinters;
    private readonly string[] palette = ["#9381FF", "#8370FF", "#B8B8FF", "#A67BFF"];

    public ErrorPrettyPrinter(int error)
    {
        this.error = error;
        errorPrinters = InitializeErrorPrinters();
    }

    public ErrorPrettyPrinter() : this(0)
    {
    }

    private Dictionary<int, Func<int>> InitializeErrorPrinters()
    {
        var result = new Dictionary<int, Func<int>>();

        MethodInfo[] methods = typeof(ErrorPrettyPrinter).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (MethodInfo method in methods)
        {
            if (method.Name.StartsWith("PrettyPrintError"))
            {
                int errorCode;
                if (int.TryParse(method.Name.Replace("PrettyPrintError", ""), out errorCode))
                {
                    result.Add(errorCode, (Func<int>)Delegate.CreateDelegate(typeof(Func<int>), this, method));
                }
            }
        }
        return result;
    }

    private string Style(string input, string style)
    {
        return $"[{style}]{input}[/]";
    }

    private string CreatePrettyText(int errorVal, string[] styles)
    {
        if (styles.Length < 3)
        {
            throw new ArgumentException("Styles array must contain at least three values.");
        }

        string errorCodeStyle = styles[0];
        string parenthesesStyle = styles[1];
        string errorValStyle = styles[2];

        return $"During Execution {Style("Error Code:", errorCodeStyle)} {Style("(", parenthesesStyle)}" +
                $"{Style(errorVal.ToString(), errorValStyle)}{Style(")", parenthesesStyle)} was encountered.";
    }

    private void Print(string[] markup)
    {
        foreach (string line in markup)
        {
            AnsiConsole.MarkupLine(line);
        }
    }

    private string[] Merge(string input, string[] lines)
    {
        List<string> result = [input, .. lines];
        return result.ToArray();
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

    public int PrettyPrint(int errorCode)
    {
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
        string message = Style("Operation Successful", "#00C800");
        AnsiConsole.Markup(message);
        return 0;
    }

    // Method to pretty print error 1
    private int PrettyPrintError1()
    {
        string[] description =
            [
                "This Error occurs when an empty or null string is encountered where a path to a file is expected. ",
                "Please ensure that you specify a valid input path and try again."
            ];
        string pretty = CreatePrettyText(1, palette);
        var lines = Merge(pretty, description);
        // Spectre pretty print
        Print(lines);
        return 1;
    }

    // Method to pretty print error 2
    private int PrettyPrintError2()
    {
        // Spectre pretty print
        string[] description =
            [
                "This error occurs when the path specified by the file path string does not exist.",
                "Please ensure that you specify a valid input path and try again."
            ];
        string pretty = CreatePrettyText(2, palette);
        var lines = Merge(pretty, description);
        Print(lines);
        return 2;
    }

    // Method to pretty print error 3
    private int PrettyPrintError3()
    {
        // Spectre pretty print
        string[] description =
            [
                "This error occurs when the file path string points to a file with an unsupported extension.",
                "Please ensure that you specify a valid input path and try again."
            ];
        string pretty = CreatePrettyText(3, palette);
        var lines = Merge(pretty, description);
        Print(lines);
        return 3;
    }

    // Method to pretty print error 4
    private int PrettyPrintError4()
    {
        // Spectre pretty print
        string[] description =
            [
                "This error occurs when the frontcut string does not represent a valid time format.",
                "Supported formats include:",
                $"   [{palette[3]}]-->[/] hh:mm:ss.mmmmmm",
                $"   [{palette[3]}]-->[/]    mm:ss.mmmmmm",
                $"   [{palette[3]}]-->[/]       ss.mmmmmm",
                $"   [{palette[3]}]-->[/] hh:mm:ss",
                $"   [{palette[3]}]-->[/]    mm:ss",
                $"   [{palette[3]}]-->[/]       ss",
                "If a cut from the start of the file is desired, no value is needed.",
                "Please ensure that you specify a valid time format and try again."
            ];
        string pretty = CreatePrettyText(4, palette);
        var lines = Merge(pretty, description);
        Print(lines);
        return 4;
    }

    // Method to pretty print error 5
    private int PrettyPrintError5()
    {
        // Spectre pretty print
        string[] description =
            [
                "This error occurs when the backcut string does not represent a valid time format.",
                "Supported formats include:",
                $"   [{palette[3]}]-->[/] hh:mm:ss.mmmmmm",
                $"   [{palette[3]}]-->[/]    mm:ss.mmmmmm",
                $"   [{palette[3]}]-->[/]       ss.mmmmmm",
                $"   [{palette[3]}]-->[/] hh:mm:ss",
                $"   [{palette[3]}]-->[/]    mm:ss",
                $"   [{palette[3]}]-->[/]       ss",
                "If a cut to the end of the file is desired, no value is needed.",
                "Please ensure that you specify a valid time format and try again."
            ];
        string pretty = CreatePrettyText(5, palette);
        var lines = Merge(pretty, description);
        Print(lines);
        return 5;
    }

    // Method to pretty print error 6
    private int PrettyPrintError6()
    {
        // Spectre pretty print
        string[] description =
           [
               "This error occurs when no interval is specified for cutting. ",
               "If a cut from the start of the file is desired, the frontcut need not be specified. ",
               "If a cut to the end of the file is desired, the backcut need not be specified. ",
               $"   [{palette[3]}]-->[/] SOF - backcut          : Cut from the beginning of the file to the specified time (backcut)",
               $"   [{palette[3]}]-->[/] frontcut - EOL         : Cut from the specified time (frontcut) to the end of the file",
               $"   [{palette[3]}]-->[/] frontcut - backcut     : Cut from the specified start time (frontcut) to the specified end time (backcut)",
               "Without specifying the interval, the cutting operation cannot proceed. ",
               "Please ensure that you provide a valid interval for cutting and try again."
            ];
        string pretty = CreatePrettyText(6, palette);
        var lines = Merge(pretty, description);
        Print(lines);
        return 6;
    }

    // Method to pretty print unknown error code
    private int PrettyPrintUnknownError(int errorCode)
    {
        // Unknown error code
        string first = "During execution,";
        string unkown = $"An [{palette[1]}]Unknown[/] [{palette[0]}]Error Code:[/] [{palette[1]}]([/][bold {palette[2]}]{errorCode}[/][{palette[1]}])[/]";
        string second = $"has been encountered. Please [{palette[3]}]report[/] this issue to the";
        string maintainers = $"[{palette[3]}]project maintainers[/]";

        string error = $"{first} {unkown} {second} {maintainers}.";
        string link = $"[{palette[1]}]Repository:[/] [{palette[2]}][link]https://github.com/EdgeLordKirito/FFmpegCliMp3Cutter/issues [/][/]";
        AnsiConsole.Markup(error);
        Console.WriteLine();
        AnsiConsole.Markup(link);

        return errorCode; // Or any other default value
    }

}
