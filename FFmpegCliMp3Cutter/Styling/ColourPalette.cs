using Tomlet;
using Tomlet.Models;

namespace FFmpegCliMp3Cutter.Styling;
internal class ColourPalette
{
    private readonly TomlDocument? _ext;
    private readonly TomlDocument _def;

    //Add properties

    public ColourPalette()
    {
        //populate default tomldoc
        _ext = FromFile("");
    }

    private TomlDocument? FromFile(string path)
    {
        FileInfo file = new FileInfo(path);
        if (!file.Exists)
        {
            return null;
        }
        if (!file.Extension.Equals(".toml"))
        {
            return null;
        }
        //ensure it contains the needed keys
        TomlDocument doc = TomlParser.ParseFile(file.FullName);
        return doc;
    }
}
