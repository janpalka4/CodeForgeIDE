using CodeForgeIDE.Core.Workspace.Model;

namespace CodeForgeIDE.CSharp.Workspace.Model
{
    public class CSharpSolutionProjectTreeNode : ProjectTreeNode
    {
        public CSharpSolutionProjectTreeNode(string name, string fullPath) : base(Icons.Solution, name, fullPath)
        {
        }
    }
}
