using Spectre.Console;

namespace FFmpegCliMp3Cutter.Spectre;
public class SpinnerStyle
{
    private string message;
    public string StatusMessage => ApplyStyleToMessage();
    public Spinner Type { get; set; }
    public Style StyleSpinner { get; set; }
    public Style StyleText { get; set; }

    public SpinnerStyle(string statusMessage, Spinner spinnerType, Style spinnerStyle, Style textStyle)
    {
        message = statusMessage;
        Type = spinnerType;
        StyleSpinner = spinnerStyle;
        StyleText = textStyle;
    }

    public string ApplyStyleToMessage()
    {
        string mark = StyleText.ToMarkup();
        return $"[{mark}]{message}[/]";
    }
}
