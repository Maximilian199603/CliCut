using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;

namespace FFmpegCliMp3Cutter.Spectre;

public static class ColorExtensions
{
    public static Color FromHex(this Color col, string hex)
    {
        _ = col;
        if (hex.StartsWith("#"))
        {
            hex = hex.Substring(1);
        }

        if (hex.Length != 6)
        {
            throw new ArgumentException("Hex color must be in the format RRGGBB.", nameof(hex));
        }

        byte red = Convert.ToByte(hex.Substring(0, 2), 16);
        byte green = Convert.ToByte(hex.Substring(2, 2), 16);
        byte blue = Convert.ToByte(hex.Substring(4, 2), 16);

        return new Color(red, green, blue);
    }
}
