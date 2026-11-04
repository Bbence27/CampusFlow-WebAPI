namespace CampusFlow.Util.Logging;

public class AppLogger(ILogSink sink, TimeProvider timeProvider) : IAppLogger
{
    public void Info(string message) => Log("INFO", message);

    public void Warning(string message) => Log("WARN", message);

    public void Error(string message, Exception? exception = null) =>
        Log("ERROR", exception is null ? message : $"{message}{Environment.NewLine}{exception}");

    private void Log(string level, string message) =>
        sink.Write($"{timeProvider.GetUtcNow():yyyy-MM-dd HH:mm:ss.fff} [{level}] {message}");
}
