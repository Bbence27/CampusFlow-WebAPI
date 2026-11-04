namespace CampusFlow.Util.Logging;

public class AppLoggingOptions
{
    public const string SectionName = "AppLogging";

    public LoggerType Type { get; set; } = LoggerType.File;
    public string FilePath { get; set; } = "logs/campusflow.log";
}
