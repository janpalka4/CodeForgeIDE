using System.Collections.Generic;

namespace CodeForgeIDE.CSharp.Workspace.Model
{
    public class ProjectTreeNode
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public List<ProjectTreeNode> Children { get; set; } = new List<ProjectTreeNode>();

        public ProjectTreeNode(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
}
