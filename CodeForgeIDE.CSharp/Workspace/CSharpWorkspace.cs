using CodeForgeIDE.Core.Workspace;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace CodeForgeIDE.CSharp.Workspace
{
    public class CSharpWorkspace : EditorWorkspace
    {
        public MSBuildWorkspace MsBuildWorkspace { get; private set; }

        public CSharpWorkspace(string path) : base(path)
        {
        }

        public override async void Initialize()
        {
            base.Initialize();

            MsBuildWorkspace = MSBuildWorkspace.Create();
            if (Path.EndsWith(".sln"))
            {
                MsBuildWorkspace.OpenSolutionAsync(Path).Wait();
            }
            else if (Path.EndsWith(".csproj"))
            {
                MsBuildWorkspace.OpenProjectAsync(Path).Wait();
            }
            else
            {
                throw new InvalidOperationException("Invalid project file type. Only .sln and .csproj files are supported.");
            }
        }
    }
}
