namespace CampusFlow.Util.Logging;

public class FileLogSink(IFileWriter fileWriter) : ILogSink
{
    public void Write(string line) => fileWriter.AppendLine(line);
}

public class ConsoleLogSink : ILogSink
{
    public void Write(string line) => Console.WriteLine(line);
}

public class DebugLogSink : ILogSink
{
    public void Write(string line) => System.Diagnostics.Debug.WriteLine(line);
}