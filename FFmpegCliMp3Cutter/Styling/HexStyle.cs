using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpegCliMp3Cutter.Styling;

class HexStyle
{
    private readonly string _foreground = string.Empty;
    private readonly string _background = string.Empty;
    private readonly Decoration _decoration;

    public HexStyle(string foreground, string background, Decoration decorator)
    {
        if (IsValidForeGroundHexColor(foreground))
        {
            _foreground = foreground;
        }
        else
        {
            throw new ArgumentException(nameof(foreground));
        }

        if (IsValidHexColor(background))
        {
            _background = background;
        }
        else
        {
            throw new ArgumentException(nameof(background));
        }

        _decoration = decorator;
    }

    public HexStyle(string foreground, Decoration decorator) : this(foreground, "", decorator){}

    public HexStyle(string foreground) : this(foreground, "", Decoration.None) { }

    private bool IsValidHexColor(string? hexColor)
    {
        // Check if hexColor is null
        if (hexColor == null)
        {
            return false;
        }

        if (hexColor.Equals(string.Empty))
        {
            return true;
        }

        // Check if hexColor is empty or consists only of whitespace characters
        if (string.IsNullOrWhiteSpace(hexColor))
        {
            return false;
        }

        // Remove leading '#' if present
        if (hexColor.StartsWith('#'))
        {
            hexColor = hexColor.Substring(1);
        }

        // Check if hexColor has a valid length and consists only of hexadecimal characters
        return (hexColor.Length == 6) && hexColor.All(Uri.IsHexDigit);
    }

    private bool IsValidForeGroundHexColor(string? hexColor)
    {
        // Check if hexColor is empty or consists only of whitespace characters
        if (string.IsNullOrWhiteSpace(hexColor))
        {
            return false;
        }

        // Remove leading '#' if present
        if (hexColor.StartsWith('#'))
        {
            hexColor = hexColor.Substring(1);
        }

        // Check if hexColor has a valid length and consists only of hexadecimal characters
        return (hexColor.Length == 6) && hexColor.All(Uri.IsHexDigit);
    }

    public string ToMarkup()
    {
        StringBuilder result = new StringBuilder();
        string foregroundColor = _foreground.StartsWith('#') ? _foreground : $"{'#'}{_foreground}";
        // Append background color if it's not empty
        if (!string.IsNullOrEmpty(_background))
        {
            // Ensure background color starts with '#'
            string backgroundColor = _background.StartsWith('#') ? _background : $"{'#'}{_background}";
            result.Append($"{foregroundColor} on {backgroundColor}");
        }
        else
        {
            result.Append($"{foregroundColor}");
        }

        if (_decoration != Decoration.None)
        {
            result.Append($" {_decoration.ToString().ToLower().Replace(", ", " ")}");
        }

        // Return the result
        return result.ToString();
    }
}
