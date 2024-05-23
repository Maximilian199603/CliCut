namespace FFmpegCliMp3Cutter;
public class Palette
{
    private static readonly Lazy<ColourPalette> _instance = new Lazy<ColourPalette>(() => new ColourPalette());

    private Palette() { }

    public static ColourPalette Instance
    {
        get
        {
            return _instance.Value;
        }
    }
}

