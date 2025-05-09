using CodeForgeIDE.Core;
using CodeForgeIDE.Core.Plugins;
using CodeForgeIDE.Core.Workspace.Model;

namespace CodeForgeIDE.CSharp.Workspace
{
    public class CSharpSolutionProjectTreeProvider : IProjectTreeProvider
    {
        public async Task<ProjectTreeNode> GetProjectNode(string path, bool shallow = false)
        {
            if (!File.Exists(path) && !Directory.Exists(path))
                throw new FileNotFoundException($"The path '{path}' does not exist.");

            // Ensure the required namespace is imported and the MSBuildWorkspace is properly referenced  
            using var workspace = ((CSharpWorkspace)IDE.Editor.Workspace).MsBuildWorkspace;

            if (path.EndsWith(".sln"))
            {
                var solution = workspace.CurrentSolution;
                var rootNode = new ProjectTreeNode(Icons.Solution, Path.GetFileName(path), path);

                foreach (var project in solution.Projects)
                {
                    var projectNode = await GetProjectNode(project.FilePath, shallow);
                    rootNode.AddChild(projectNode);
                }

                return rootNode;
            }
            else if (path.EndsWith(".csproj"))
            {
                var project = workspace.CurrentSolution.Projects.FirstOrDefault(x => x.FilePath == path);
                var projectNode = new ProjectTreeNode(Icons.Project, Path.GetFileNameWithoutExtension(path), path);

                var folderPath = Path.GetDirectoryName(path)!;

                if (!shallow)
                {
                    foreach (var document in project!.Documents)
                    {
                        if (document is not null && document.FilePath != null) {
                            ProjectTreeNode fileNode = Path.GetExtension(document.FilePath) == ".cs" ? new CSharpFileProjectTreeNode(Path.GetFileName(document.FilePath), document.FilePath) : new ProjectTreeNode(Core.Icons.FileCode, Path.GetFileName(document.FilePath), document.FilePath);

                            var currentNode = projectNode;
                            var folders = document.FilePath.Substring(folderPath.Length, document.FilePath.Length - folderPath.Length).Split(Path.DirectorySeparatorChar);
                            folders = folders.Take(folders.Length - 1).ToArray();

                            foreach (var folder in folders) { 
                                if(string.IsNullOrEmpty(folder))
                                    continue;

                                var newNode = currentNode.Children.FirstOrDefault(x => x.Name == folder) ?? new ProjectTreeNode(Core.Icons.Folder, folder, Path.Combine(folderPath, folder));

                                if(!currentNode.Children.Contains(newNode))
                                {
                                    currentNode.AddChild(newNode);
                                }

                                currentNode = newNode;
                            }

                            currentNode.AddChild(fileNode);
                        }
                    }
                }

                return projectNode;
            }

            throw new NotSupportedException($"The path '{path}' is not a supported project or solution file.");
        }

        public async Task<ProjectTreeNode> GetProjectTree(string projectPath)
        {
            return await GetProjectNode(projectPath);
        }

        public bool ShouldBeUsed(string path)
        {
            return path.EndsWith(".sln") || path.EndsWith(".csproj");
        }
    }
}
