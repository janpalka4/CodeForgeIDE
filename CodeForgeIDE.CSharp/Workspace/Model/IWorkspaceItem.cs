using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace CodeForgeIDE.CSharp.Workspace.Model
{
    public interface IWorkspaceItem
    {
        string Name { get; }
        string Path { get; }
        IEnumerable<ProjectTreeNode> GetProjectTree();
        object GetRoslynItem(); // Returns either Microsoft.CodeAnalysis.Solution or Microsoft.CodeAnalysis.Project
    }
}
