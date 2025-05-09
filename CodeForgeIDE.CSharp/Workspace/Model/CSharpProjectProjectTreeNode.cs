using CodeForgeIDE.Core.Workspace.Model;

namespace CodeForgeIDE.CSharp.Workspace
{
    public class CSharpProjectProjectTreeNode : ProjectTreeNode
    {
        public CSharpProjectProjectTreeNode(string name, string fullPath) : base(Icons.Project, name, fullPath)
        {
        }
    }
}
