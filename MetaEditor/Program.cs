﻿// Copyright (C) 2018 - 2023 Tony's Studio. All rights reserved.

using System;
using ATL;

namespace MetaEditor;

class Program
{
    static void Main(string[] args)
    {
        var manager = new EditorManager("config.json");

        manager.AddAction((track, folder, filename) => {
            track.Album = "Plastic Ono Band (The Ultimate Collection) " + folder;
        });

        manager.Run();
    }
}