using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaEditor;

internal class Configuration
{
    public string Root { get; set; } = @".\";
    public List<string> Includes { get; set; } = new List<string>();
    public List<string> Excludes { get; set; } = new List<string>();
    public List<string> Extensions { get; set; } = new List<string>();
    public bool Auto { get; set; } = false;
    public int Threads { get; set; } = 4;
    public int Delay { get; set; } = 50;
}