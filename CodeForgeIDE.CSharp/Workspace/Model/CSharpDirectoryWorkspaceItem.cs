using CodeForgeIDE.Core;
using CodeForgeIDE.Core.Workspace.Model;
using Microsoft.CodeAnalysis;

namespace CodeForgeIDE.CSharp.Workspace.Model
{
    public class CSharpDirectoryWorkspaceItem : WorkspaceItem
    {
        public override IReadOnlyList<WorkspaceItem> Children => GetChildren();
        protected string? DirectoryPath { get; set; }

        public CSharpDirectoryWorkspaceItem(string path) : base(Core.Icons.Folder, Path.GetFileName(path), path)
        {
            DirectoryPath = File.Exists(path) ? Path.GetDirectoryName(path) : path;
        }

        public CSharpDirectoryWorkspaceItem(IconData icon,string name, string path) : base(icon, name, path)
        {
            DirectoryPath = File.Exists(path) ? Path.GetDirectoryName(path) : path;
        }


        private IReadOnlyList<WorkspaceItem> GetChildren()
        {
            List<WorkspaceItem> children = new List<WorkspaceItem>();

            string? dir = DirectoryPath;
            if (string.IsNullOrEmpty(dir))
                return children;

            foreach (var folder in Directory.GetDirectories(dir))
            {
                if (!IncludeItem(dir))
                    continue;

                children.Add(new CSharpDirectoryWorkspaceItem(folder));
            }

            foreach (var file in Directory.GetFiles(dir))
            {
                if(!IncludeItem(file))
                    continue;

                if (file.EndsWith(".cs"))
                {
                    children.Add(new WorkspaceItem(Icons.CSharp, Path.GetFileName(file), file));
                }
                else
                {
                    children.Add(new WorkspaceItem(Core.Icons.FileCode, Path.GetFileName(file), file));
                }
            }

            return children;
        }

        protected virtual bool IncludeItem(string path) => true;
    }
}
