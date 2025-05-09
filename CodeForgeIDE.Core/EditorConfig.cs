using System;
using System.Collections.Generic;

namespace CodeForgeIDE.Core
{
    public class EditorConfig
    {
        public List<RecentOpenedFile> RecentOpenedFiles { get; set; } = new List<RecentOpenedFile>();
    }

    public class RecentOpenedFile
    {
        public string Path { get; set; } = "";
        public DateTime LastOpened { get; set; }
    }
}
