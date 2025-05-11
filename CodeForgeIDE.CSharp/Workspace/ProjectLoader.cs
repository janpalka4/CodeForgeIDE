using System;
using System.IO;
using System.Threading.Tasks;
using CodeForgeIDE.Core.Workspace;
using Microsoft.Build.Construction;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.MSBuild;
using CodeForgeIDE.CSharp.Workspace.Model;

namespace CodeForgeIDE.CSharp.Workspace
{
    public class ProjectLoader
    {
        private readonly CSharpWorkspace _workspace;

        public ProjectLoader(CSharpWorkspace workspace)
        {
            _workspace = workspace;
        }

        // Load an entire solution
        public async Task LoadSolutionAsync(string solutionPath)
        {
            if (!File.Exists(solutionPath))
                throw new FileNotFoundException("Solution file not found.", solutionPath);

            Console.WriteLine($"Loading solution: {solutionPath}");
            var solutionFile = new SolutionFile(solutionPath);
            var roslynSolution = (Solution)solutionFile.GetRoslynItem();

            foreach (var project in roslynSolution.Projects)
            {
                Console.WriteLine($"Loaded project: {project.Name}");
            }

            _workspace.SetSolution(solutionPath, solutionFile.GetProjectTree());
        }

        // Load a single project
        public async Task LoadProjectAsync(string projectPath)
        {
            if (!File.Exists(projectPath))
                throw new FileNotFoundException("Project file not found.", projectPath);

            Console.WriteLine($"Loading project: {projectPath}");
            var projectFile = new ProjectFile(Path.GetFileName(projectPath), projectPath);
            var roslynProject = (Project)projectFile.GetRoslynItem();

            Console.WriteLine($"Loaded project: {roslynProject.Name}");

            _workspace.SetProject(projectPath, projectFile.GetProjectTree());
        }

        // Load a folder
        public async Task LoadFolderAsync(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException("Folder not found.");

            // Logic to load folder contents
            Console.WriteLine($"Loading folder: {folderPath}");
            // TODO: Implement folder parsing and gradual node loading

            _workspace.SetFolder(folderPath); // Store in workspace
        }
    }
}
