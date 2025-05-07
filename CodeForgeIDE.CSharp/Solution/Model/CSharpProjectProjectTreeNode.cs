using CodeForgeIDE.Core.Solution.Model;

namespace CodeForgeIDE.CSharp.Solution
{
    public class CSharpProjectProjectTreeNode : ProjectTreeNode
    {
        public CSharpProjectProjectTreeNode(string name, string fullPath) : base(Icons.Project, name, fullPath)
        {
        }
    }
}
