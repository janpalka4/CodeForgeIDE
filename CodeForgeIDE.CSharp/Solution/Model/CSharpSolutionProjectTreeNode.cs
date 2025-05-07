using CodeForgeIDE.Core.Solution.Model;

namespace CodeForgeIDE.CSharp.Solution.Model
{
    public class CSharpSolutionProjectTreeNode : ProjectTreeNode
    {
        public CSharpSolutionProjectTreeNode(string name, string fullPath) : base(Icons.Solution, name, fullPath)
        {
        }
    }
}
