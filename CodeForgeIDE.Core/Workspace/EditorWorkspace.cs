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
        public IProjectTreeProvider ProjectTreeProvider { get; private set; }
        public string Path { get; set; }

        public EditorWorkspace(string path) 
        {
            Path = path;

            Initialize();
        }

        public virtual void Initialize()
        {
            ProjectTreeProvider = IDE.Editor.ServiceProvider.GetProviderForFile<IProjectTreeProvider, DefaultProjectTreeProvider>(Path);
        }
    }
}
