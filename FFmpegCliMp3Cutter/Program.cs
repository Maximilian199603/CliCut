﻿using Spectre.Console;

namespace FFmpegCliMp3Cutter;

internal class Program
{
    static int Main(string[] args)
    {
        var app = new App(args);
        return app.Run();
    }
}
