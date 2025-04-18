namespace StupidWareGames.CrazyFilter.Utils;

using System;
using System.IO;

public class DirectoryWatcher : IDisposable
{
    private readonly string _watchPath;
    private FileSystemWatcher? _watcher;

    public event Action? FiltersUpdated;

    public DirectoryWatcher(string watchPath)
    {
        _watchPath = watchPath;
    }

    public void Start()
    {
        if (!Directory.Exists(_watchPath))
            return;

        _watcher = new FileSystemWatcher(_watchPath)
        {
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite
        };

        _watcher.Created += OnChange;
        _watcher.Deleted += OnChange;
        _watcher.Renamed += OnChange;
        _watcher.Changed += OnChange;
        _watcher.EnableRaisingEvents = true;
    }

    private void OnChange(object sender, FileSystemEventArgs e)
    {
        FiltersUpdated?.Invoke();
    }

    public void Stop()
    {
        if (_watcher != null)
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();
            _watcher = null;
        }
    }

    public void Dispose()
    {
        Stop();
    }
}
