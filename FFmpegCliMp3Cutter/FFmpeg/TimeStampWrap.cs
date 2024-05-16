using System.Text.RegularExpressions;

namespace FFmpegCliMp3Cutter.FFmpeg;
internal class TimeStampWrap
{
    private string? _stamp;
    public string Value => _stamp ?? string.Empty;
    public bool Empty => string.IsNullOrEmpty(_stamp);
    public TimeStampWrap(string? stamp)
    {
        bool valid = ValidateStamp(stamp);
        if (!valid)
        {
            throw new ArgumentException("Invalid Timestamp format");
        }
        _stamp = stamp;
    }

    public TimeStampWrap() : this(string.Empty) { }

    private bool ValidateStamp(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return true;
        }

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
