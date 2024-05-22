﻿using FFmpegCliMp3Cutter.Toml;
using System.Reflection;
using System.Runtime.CompilerServices;
using Tomlet;
using Tomlet.Attributes;
using Tomlet.Models;

namespace FFmpegCliMp3Cutter.Styling;
internal class ColourPalette
{
    private readonly TomlDocument _ext;
    private readonly TomlDocument _def;

    private bool _extEmpty = false;

    //Add properties
    [TomlProperty("Reserved.Common")]
    public string Common => RetrieveValue();
    [TomlProperty("Reserved.HighLight")]
    public string HighLight => RetrieveValue();
    [TomlProperty("Reserved.Error")]
    public string Error => RetrieveValue();
    [TomlProperty("Reserved.Success")]
    public string Success => RetrieveValue();
    [TomlProperty("Reserved.Accent")]
    public string Accent => RetrieveValue();
    [TomlProperty("Reserved.Background")]
    public string Background => RetrieveValue();
    [TomlProperty("Primary.Red")]
    public string Red => RetrieveValue(); 
    [TomlProperty("Primary.Green")]
    public string Green => RetrieveValue();
    [TomlProperty("Primary.Blue")]
    public string Blue => RetrieveValue();
    [TomlProperty("Primary.Yellow")]
    public string Yellow => RetrieveValue();
    [TomlProperty("Primary.Orange")]
    public string Orange => RetrieveValue();
    [TomlProperty("Primary.Indigo")]
    public string Indigo => RetrieveValue();
    [TomlProperty("Primary.Purple")]
    public string Purple => RetrieveValue();
    [TomlProperty("Contrast.Black")]
    public string Black => RetrieveValue();
    [TomlProperty("Contrast.White")]
    public string White => RetrieveValue();
    [TomlProperty("Contrast.Gray")]
    public string Gray => RetrieveValue();
    [TomlProperty("Contrast.LightGray")]
    public string LightGray => RetrieveValue();
    [TomlProperty("Contrast.DarkGray")]
    public string DarkGray => RetrieveValue();

    public ColourPalette()
    {
        //populate default tomldoc
        _def = GenerateDefault();
        _ext = FromFile(Path.Combine(GlobalValues.ConfigPath,"palette.toml"));
    }

    private TomlDocument FromFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            _extEmpty = true;
            return TomlHelper.Empty;
        }

        FileInfo file = new FileInfo(path);
        if (!file.Exists)
        {
            _extEmpty = true;
            return TomlHelper.Empty;
        }
        if (!file.Extension.Equals(".toml"))
        {
            _extEmpty = true;
            return TomlHelper.Empty;
        }
        TomlDocument doc = TomlParser.ParseFile(file.FullName);
        //ensure it contains the needed keys
        if (!HasAllKeys(doc))
        {
            return TomlHelper.Empty;
        }

        return doc;
    }

    private bool HasAllKeys(TomlDocument doc)
    {
        return TomlHelper.AreAllKeysPresent(doc, GetType());
    }

    private string GetValue(string key)
    {
        TomlDocument doc;
        if (_extEmpty)
        {
            doc = _def;
        }
        else
        {
            doc = _ext;
        }

        TomlValue value = doc.GetValue(key);
        return value.StringValue;
    }

    private string RetrieveValue([CallerMemberName] string propertyName = "")
    {
        PropertyInfo? property = typeof(ColourPalette).GetProperty(propertyName);
        if (property is null)
        {
            return string.Empty;
        }
        var attribute = property.GetCustomAttribute<TomlPropertyAttribute>();
        if (attribute != null)
        {
            string key = attribute.GetMappedString();
            return GetValue(key);
        }
        return string.Empty;
    }

    private TomlDocument GenerateDefault()
    {
        TomlParser parser = new TomlParser();
        TomlDocument d = parser.Parse("");

        d.Put("Reserved.Common", "#BB22EE");
        d.Put("Reserved.HighLight", "#CC00DD");
        d.Put("Reserved.Error", "#CE2029");
        d.Put("Reserved.Success", "#00C800");
        d.Put("Reserved.Accent", "#7755FF");
        d.Put("Reserved.Background", "#435460");

        d.Put("Primary.Red", "#FF0000"); 
        d.Put("Primary.Orange", "#FFA500");
        d.Put("Primary.Yellow", "#FFFF00");
        d.Put("Primary.Green", "#008000");
        d.Put("Primary.Blue", "#0000FF");
        d.Put("Primary.Indigo", "#4B0082");
        d.Put("Primary.Purple", "#EE82EE");

        d.Put("Contrast.Black", "#000000");
        d.Put("Contrast.White", "#FFFFFF");
        d.Put("Contrast.Gray", "#808080");
        d.Put("Contrast.LightGray", "#A1A1A1");
        d.Put("Contrast.DarkGray", "#606060");
        return d;
    }
}
