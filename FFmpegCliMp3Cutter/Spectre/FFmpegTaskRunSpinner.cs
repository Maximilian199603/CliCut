using FFmpegCliMp3Cutter.FFmpeg;
using Spectre.Console;

namespace FFmpegCliMp3Cutter.Spectre;

internal class FFmpegTaskRunSpinner
{
    public FFmpegTaskRunSpinner() { }
    public void Run(FFmpegCutTask task)
    {
        // Create a status indicator
        AnsiConsole.Status()
            .Spinner(Palette.Instance.Spinner)
            //Add the colour
            .SpinnerStyle(new Style().Foreground(new Color().FromHex(Palette.Instance.HighLight)))
            .Start($"Cutting File", ctx =>
            {
                try
                {
                    // Execute your long-running task asynchronously and wait for it
                    Task executionTask = task.Execute();
                    executionTask.Wait();
                }
                catch (AggregateException ex)
                {
                    ColourPalette c = Palette.Instance;
                    foreach (var innerException in ex.InnerExceptions)
                    {
                        
                        AnsiConsole.WriteException(innerException, new ExceptionSettings
                        {
                            Format = ExceptionFormats.ShortenPaths | ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks,
                            Style = new ExceptionStyle
                            {
                                Exception = new Style().Foreground(new Color().FromHex(c.Accent)),
                                Message = new Style().Foreground(new Color().FromHex(c.HighLight)),
                                NonEmphasized = new Style().Foreground(new Color().FromHex(c.Common)),
                                Parenthesis = new Style().Foreground(new Color().FromHex(c.Common)),
                                Method = new Style().Foreground(new Color().FromHex(c.HighLight)),
                                ParameterName = new Style().Foreground(new Color().FromHex(c.Common)),
                                ParameterType = new Style().Foreground(new Color().FromHex(c.Accent)),
                                Path = new Style().Foreground(new Color().FromHex(c.HighLight)),
                                LineNumber = new Style().Foreground(new Color().FromHex(c.Accent)),
                            }
                        });
                    }
                }
            });
    }
}
