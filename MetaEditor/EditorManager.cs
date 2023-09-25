// Copyright (C) 2018 - 2023 Tony's Studio. All rights reserved.

using ATL;
using Newtonsoft.Json;

namespace MetaEditor;

internal class EditorManager
{
    private readonly List<Action<Track, string, string>> _actions = new();
    private readonly Configuration _config;

    private readonly Queue<EditAction> _editActions = new();
    private int _totalTasks;
    private int _finishedTasks;
    private object _lock = new();


    public EditorManager(string configPath)
    {
        using StreamReader reader = new(configPath);
        var json = reader.ReadToEnd();
        var config = JsonConvert.DeserializeObject<Configuration>(json);
        _config = config ?? throw new Exception("Configuration is null");
    }


    private IEnumerable<string> GetPathList()
    {
        return _config.Auto ? Directory.EnumerateDirectories(_config.Root).ToList() : _config.Includes;
    }


    private IEnumerable<string> GetFileList(string path)
    {
        IEnumerable<string> fileList = Directory.EnumerateFiles(path);
        return fileList.Where(file => _config.Extensions.Contains(Path.GetExtension(file))).ToList();
    }


    public EditorManager AddAction(Action<Track, string, string> action)
    {
        _actions.Add(action);
        return this;
    }


    public void Run()
    {
        if (!_actions.Any())
        {
            Console.WriteLine("No action added.");
            return;
        }

        // Change working directory.
        var pwd = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(_config.Root);

        PrepareActions();

        Console.Write("Ready? (Y/N) ");
        string response;
        do
        {
            response = Console.ReadLine();
            if (response == null)
            {
                Console.WriteLine("Aborted.");
                return;
            }
        } while (response.Length == 0);
        if (response[0] != 'Y' && response[0] != 'y')
        {
            Console.WriteLine("Aborted.");
            return;
        }

        RunActions();

        // Restore directory.
        Directory.SetCurrentDirectory(pwd);
    }


    private void PrepareActions()
    {
        // Get path list.
        IEnumerable<string> pathList = GetPathList();
        foreach (var path in pathList)
        {
            if (_config.Excludes.Contains(Path.GetFileName(path)))
            {
                Console.WriteLine($"\n========== Skipping {path}");
                continue;
            }

            Console.WriteLine($"\n========== Processing {path}");
            var folder = Path.GetFileName(path);

            IEnumerable<string> fileList = GetFileList(path);
            Console.WriteLine($"Found {fileList.Count()} files: ");
            foreach (var file in fileList)
            {
                Console.WriteLine($"\tAdding {Path.GetFileName(file)}...");
                var filename = Path.GetFileNameWithoutExtension(file);
                _editActions.Enqueue(new EditAction {
                    Path = file,
                    Folder = folder,
                    Filename = filename
                });
            }

            _totalTasks = _editActions.Count;
            _finishedTasks = 0;
        }
    }


    private void RunActions()
    {
        var threadList = new List<Thread>();
        Thread thread;
        for (var i = 0; i < 8; i++)
        {
            thread = new Thread(EditTask);
            threadList.Add(thread);
            thread.Start();
        }

        thread = new Thread(PrintProgressTask);
        threadList.Add(thread);
        thread.Start();

        foreach (Thread t in threadList)
        {
            t.Join();
        }

        Console.WriteLine();
    }


    private void EditTask()
    {
        while (true)
        {
            EditAction editAction;
            lock (_editActions)
            {
                var count = _editActions.Count;
                if (count == 0)
                {
                    return;
                }

                editAction = _editActions.Dequeue();
            }

            var track = new Track(editAction.Path);
            foreach (Action<Track, string, string> action in _actions)
            {
                action(track, editAction.Folder, editAction.Filename);
            }

            track.Save();

            lock (_lock)
            {
                _finishedTasks++;
            }

            Thread.Sleep(_config.Delay);
        }
    }

    private void PrintProgressTask()
    {
        while (true)
        {
            int finishedTasks;
            lock (_lock)
            {
                finishedTasks = _finishedTasks;
            }

            PrintProgress((double)finishedTasks / _totalTasks);
            if (finishedTasks == _totalTasks)
            {
                break;
            }

            Thread.Sleep(500);
        }

        PrintProgress(1.0);
    }


    private void PrintProgress(double progress)
    {
        var width = Console.WindowWidth;
        var curWidth = (int)(width * progress);

        Console.Write("[");
        for (var i = 1; i < width - 10; i++)
        {
            Console.Write(i < curWidth ? "=" : " ");
        }
        Console.Write($"{progress:P2}]\r");
    }


    private struct EditAction
    {
        public string Path { get; set; }
        public string Folder { get; set; }
        public string Filename { get; set; }
    }
}