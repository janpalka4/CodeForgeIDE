using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System.Collections.Generic;

namespace CodeForgeIDE.CSharp.Workspace.Model
{
    public class ProjectFile : IWorkspaceItem
    {
        public string Name { get; private set; }
        public string Path { get; private set; }

        public ProjectFile(string name, string path)
        {
            Name = name;
            Path = path;
        }

        public IEnumerable<ProjectTreeNode> GetProjectTree()
        {
            var nodes = new List<ProjectTreeNode>();
            // Logic to add files and directories as tree nodes
            // TODO: Implement file and directory parsing
            return nodes;
        }

        public object GetRoslynItem()
        {
            var workspace = MSBuildWorkspace.Create();
            return workspace.OpenProjectAsync(Path).Result;
        }
    }
}
