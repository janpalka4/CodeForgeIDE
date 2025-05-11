using CodeForgeIDE.Core.Workspace.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeForgeIDE.CSharp.Workspace.Model
{
    public class CSharpProjectWorkspaceItem : CSharpDirectoryWorkspaceItem
    {
        private Project Project { get; set; }

        public CSharpProjectWorkspaceItem(Project project) : base(Icons.Project, project.Name, project.FilePath!)
        {
            Project = project;
        }

        protected override bool IncludeItem(string path)
        {
            return path != Project.FilePath;
        }
    }
}
