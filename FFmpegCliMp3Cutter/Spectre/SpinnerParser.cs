using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FFmpegCliMp3Cutter.Spectre;
//untested
internal class SpinnerParser
{
    private static Dictionary<string, Spinner> map = PopulateSpinnerMap(typeof(Spinner.Known));
    public static Spinner Parse(string input)
    {
        _ = map.TryGetValue(input, out Spinner? spinner);
        Spinner n = spinner ?? Spinner.Known.Default;
        return n;
    }


    private static Dictionary<string, Spinner> PopulateSpinnerMap( Type type)
    {
        Dictionary<string, Spinner> map = new Dictionary<string, Spinner>(StringComparer.OrdinalIgnoreCase);
        // Get all properties of the specified type
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Static);

        foreach (var property in properties)
        {
            // Get the value of the property
            Spinner spinner = (Spinner)property.GetValue(null, null);

            // Add the spinner to the map using its name as the key
            map[property.Name] = spinner;
        }
        return map;
    }
}
