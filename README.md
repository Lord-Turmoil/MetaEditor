# Meta Editor

> Copyright &copy; Tony's Studio 2018 - 2023

---

This is a simple editor for music meta data in C#.

> This tool depends on [Audio Tools Library (ATL) for .NET](https://github.com/Zeugma440/atldotnet#audio-tools-library-atl-for-net---). Much thanks! 🥰
>

[![Build](https://github.com/Lord-Turmoil/MetaEditor/actions/workflows/dotnet-desktop.yml/badge.svg?branch=main)](https://github.com/Lord-Turmoil/MetaEditor/actions/workflows/dotnet-desktop.yml)

## Before you start

This is not a out-of-box tool, which means you have to modify the source code for your editing actions. The current code is build on .NET 6.

## How to use it?

### Configuration File

First, you have to tell the program which music files to edit, and some basic parameters. They are in `config.json`.

```json
{
    "root": "G:\\Music\\Classic",  // Root working directory
    "includes": [                  // Specific directories under root.
        "Best of Bach",
        "Best of Beethoven",
        "Best of Mozart"
    ],
    "excludes": [                  // Directories you do not want to edit.
        "Best of Beethoven"
    ],
    "extensions": [                // Specify which extensions you want to scan.
        ".wav",
        ".flac",
        ".ape",
        ".m4a",
        ".mp3"
    ],
    "auto": false,  // whether auto discover directories
    "thread": 8,    // working thread number, default is 4
    "delay": 10     // thread sleep time (ms) after each file, default is 50
}
```

If `auto` is set to true, then you won't need to specify `includes` (it will be ignored, actually), and it will search for all **direct** directories under root.

`delay` is set to avoid too much pressure for disk IO. When music file is large, it may cause heavy IO rate.

### Edit Action

Though not out-of-box, it is easy to use. The only thing to modify is in main function of `Program.cs`.

```csharp
// Create a editor manager with a configuration file.
var manager = new EditorManager("config.json");

// Add as many custom actions as you wish.
manager.AddAction((track, folder, filename) => {
    // ...
}).AddAction((track, folder, filename) => {
   // ... 
});

// Finally, run all the actions on each file.
manager.Run();
```

For user defined action, three parameters are provided.

`track` is `Track` class provided by [ATL](https://github.com/Zeugma440/atldotnet), you can find its usage at its own project page. Then, `folder` and `filename` is the parent folder of the music file and its filename without extension. With these three parameters, you can do some basic editing.

For example, with the configuration file above, we want to add artist and title info for these files, then I can add these -actions.

```csharp
manager.AddAction((track, folder, filename) => {
    // Since the folder name has common prefix, it is rather simple.
    track.Artist = folder[12..];
}).AddAction((track, folder, filename) => {
    // Assume that the filename is exactly what we want for title.
    track.Title = filename;
}
```

> `track.Save()` will be called automatically when all actions on a file are completed.

---

## Limitations

Unfortunately, it cannot recursively edit files yet. 🥹
