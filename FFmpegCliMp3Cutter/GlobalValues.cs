namespace FFmpegCliMp3Cutter;
internal class GlobalValues
{
    public const string ApplicationName = "App";
    //TODO: add project link
    public const string RepoLink = @"https://github.com/EdgeLordKirito/FFmpegCliMp3Cutter";
    public static readonly string DomainPath = AppDomain.CurrentDomain.BaseDirectory;
    public static readonly string ConfigPath = Path.Combine(DomainPath, "config");
}
