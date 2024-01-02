// Copyright (C) 2018 - 2023 Tony's Studio. All rights reserved.

using System;
using System.Diagnostics;
using System.Net.Mime;
using ATL;

namespace MetaEditor;

class Program
{
    static void Main(string[] args)
    {
        var manager = new EditorManager("config.json");

        manager.AddFileAction((path, folder, filename) => {
            string ext = Path.GetExtension(path);
            if (ext != ext.ToLower())
            {
                string newPath = Path.Combine(folder, filename + ext.ToLower());
                File.Move(path, newPath);
            }
        });

        manager.Run();
    }
}