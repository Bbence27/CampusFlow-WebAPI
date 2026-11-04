namespace CampusFlow.Util.Logging;

public class FileWriter : IFileWriter
{
    private readonly string _path;
    private readonly Lock _lock = new();

    public FileWriter(string path)
    {
        _path = path;

        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public void AppendLine(string line)
    {
        lock (_lock)
        {
            File.AppendAllText(_path, line + Environment.NewLine);
        }
    }
}
