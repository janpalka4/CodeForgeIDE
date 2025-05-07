using CodeForgeIDE.Core.Solution.Model;

namespace CodeForgeIDE.CSharp.Solution
{
    public class CSharpFileProjectTreeNode : ProjectTreeNode
    {
        public CSharpFileProjectTreeNode(string name, string fullPath) : base(Icons.CSharp, name, fullPath)
        {
        }
    }
}
