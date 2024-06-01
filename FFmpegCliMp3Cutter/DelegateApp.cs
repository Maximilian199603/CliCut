using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpegCliMp3Cutter;
class DelegateApp
{
    public int Execute(string[] args)
    {
        var app = SpinUpCommandApp();
        return app.Run(args);
    }


    private CommandApp SpinUpCommandApp()
    {
        var app = new CommandApp();
        app.Configure(config =>
        {
            config.AddCommand<CliCommand>("");
        });
        return app;
    }
}
