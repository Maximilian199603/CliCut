using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace FFmpegCliMp3Cutter;
//Untested
class BatchCommand : Command<BatchCommand.Settings>
{
    private static string[] CommentChars = [@"#", @"//"];

    public override int Execute(CommandContext context, Settings settings)
    {
        FileInfo path = new FileInfo(settings.FilePath);
        //validate file existence
        bool result = ValidateFile(path, out int error);
        if (!result && error != 0)
        {
            return error;
        }

        //read file
        string[] lines = [];
        try
        {
            lines = File.ReadAllLines(path.FullName);
        }
        catch (Exception e)
        {
            AnsiConsole.WriteException(e);
        }
        
        //loop over the lines
        //pass line as arg array to command app
        StringBuilder sb = new StringBuilder();
        DelegateApp second = new DelegateApp();
        foreach (string line in lines)
        {
            string temp = line.Trim();
            bool comment = CommentChars.Any(temp.StartsWith);
            if (!comment)
            {
                int err = second.Execute(line.Split(' '));
                sb.AppendLine(err.ToString()); 
            }
            else
            {
                sb.AppendLine(line);
            }
        }
        DateTime now = DateTime.Now;
        FileInfo info = new FileInfo(Path.Join(path.DirectoryName, $"{path.Name}Output[{now:ddHHmmss}]{path.Extension}"));
        if (!info.Exists)
        {
            File.WriteAllText(info.FullName, sb.ToString());
        }
        else
        {
            return 3;
        }
        return 0;
    }

    public sealed class Settings : CommandSettings
    {
        [Description("Path to target file.")]
        [CommandArgument(0, "[filePath]")]
        public required string FilePath { get; init; }
    }


    private bool ValidateFile(FileInfo path, out int error)
    {
        error = 0;

        if (!path.Exists)
        {
            error = 1;
            return false;
        }

        if(path.Length == 0)
        {
            error = 2;
            return false;
        }
        return true;
    }
}
