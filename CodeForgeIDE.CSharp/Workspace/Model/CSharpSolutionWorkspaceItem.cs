using CodeForgeIDE.Core;
using CodeForgeIDE.Core.Workspace.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace CodeForgeIDE.CSharp.Workspace.Model
{
    public class CSharpSolutionWorkspaceItem : WorkspaceItem
    {
        public override IReadOnlyList<WorkspaceItem> Children => GetChildren();

        public CSharpSolutionWorkspaceItem() : base(Icons.Solution, Path.GetFileNameWithoutExtension(IDE.Editor.GetWorkspaceAs<CSharpWorkspace>().Solution.FilePath!), IDE.Editor.GetWorkspaceAs<CSharpWorkspace>().Solution.FilePath!)
        {
        }

        private IReadOnlyList<WorkspaceItem> GetChildren()
        {
            var workspace = IDE.Editor.GetWorkspaceAs<CSharpWorkspace>();
            List<WorkspaceItem> children = new List<WorkspaceItem>();

            foreach (Project project in workspace.Solution!.Projects)
            {
                children.Add(new CSharpProjectWorkspaceItem(project));
            }

            return children;
        }
    }
}
