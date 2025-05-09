using CodeForgeIDE.Core.Workspace.Model;

namespace CodeForgeIDE.CSharp.Workspace
{
    public class CSharpFileProjectTreeNode : ProjectTreeNode
    {
        public CSharpFileProjectTreeNode(string name, string fullPath) : base(Icons.CSharp, name, fullPath)
        {
        }
    }
}
