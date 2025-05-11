using CodeForgeIDE.Core.Workspace;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace CodeForgeIDE.CSharp.Workspace
{
    public class CSharpWorkspace : EditorWorkspace
    {
        public MSBuildWorkspace? MsBuildWorkspace { get; private set; }
        public Solution? Solution { get; private set; }


        public CSharpWorkspace(string path) : base(path)
        {
            
        }

        public override async Task Initialize()
        {
            MsBuildWorkspace = await LoadWorkspace(FullPath);
            Solution = MsBuildWorkspace.CurrentSolution;
        }

        public Document? GetDocument(string path)
        {
            var project = MsBuildWorkspace!.CurrentSolution?.Projects.FirstOrDefault(p => p.Documents.Any(d => d.FilePath == path));
            if (project != null)
            {
                var document = project.Documents.FirstOrDefault(d => d.FilePath == path);
                return document;
            }
            return null;
        }

        public async Task<MSBuildWorkspace> LoadWorkspace(string path)
        {
            string? extension = Path.GetExtension(path);
            MSBuildWorkspace workspace = MSBuildWorkspace.Create();

            if (extension == ".sln")
            {
                await workspace.OpenSolutionAsync(path);
            }
            else if (extension == ".csproj")
            {
                await workspace.OpenProjectAsync(path);
            }
            else
            {
                throw new NotSupportedException($"Unsupported file type: {extension}");
            }

            return workspace;
        }
    }
}
