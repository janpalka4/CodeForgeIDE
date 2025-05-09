using CodeForgeIDE.Core.Workspace;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

        public override async void Initialize()
        {
            base.Initialize();

            MsBuildWorkspace = MSBuildWorkspace.Create();
            if (Path.EndsWith(".sln"))
            {
                Solution = await MsBuildWorkspace.OpenSolutionAsync(Path);

            }
            else if (Path.EndsWith(".csproj"))
            {
                MsBuildWorkspace.OpenProjectAsync(Path).Wait();
            }
            else
            {
                throw new InvalidOperationException("Invalid project file type. Only .sln and .csproj files are supported.");
            }
            var comp = CSharpCompilation.Create;
        }

        public Document? GetDocument(string path)
        {
            var project = Solution?.Projects.FirstOrDefault(p => p.Documents.Any(d => d.FilePath == path));
            if (project != null)
            {
                var document = project.Documents.FirstOrDefault(d => d.FilePath == path);
                return document;
            }
            return null;
        }
    }
}
