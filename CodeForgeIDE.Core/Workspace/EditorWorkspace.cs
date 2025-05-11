using CodeForgeIDE.Core.Plugins;
using CodeForgeIDE.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeForgeIDE.Core.Workspace
{
    public class EditorWorkspace
    {
        public string FullPath { get; set; }

        public EditorWorkspace(string path) 
        {
            FullPath = path;
        }

        public virtual Task Initialize()
        {
            return Task.CompletedTask;
        }
    }
}
