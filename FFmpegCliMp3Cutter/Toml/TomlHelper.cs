using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tomlet;
using Tomlet.Attributes;
using Tomlet.Models;

namespace FFmpegCliMp3Cutter.Toml;
public static class TomlHelper
{
    public static TomlDocument Empty => GenerateEmpty();

    public static bool AreAllKeysPresent(TomlDocument tomlDoc, Type classType)
    {
        // Get all public properties of the class
        var properties = classType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            // Get the TomlProperty attribute
            var attribute = property.GetCustomAttribute<TomlPropertyAttribute>();
            if (attribute != null)
            {
                // Get the key specified in the attribute using GetMappedString()
                var key = attribute.GetMappedString();
                if (!tomlDoc.ContainsKey(key))
                {
                    return false; // Return false if any key is missing
                }
            }
        }

        return true; // Return true if all keys are present
    }

    private static TomlDocument GenerateEmpty()
    {
        TomlParser parser = new TomlParser();
        return parser.Parse("");
    }
}
