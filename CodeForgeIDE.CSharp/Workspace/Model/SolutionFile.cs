using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis;

namespace CodeForgeIDE.CSharp.Workspace.Model
{
    public class SolutionFile : IWorkspaceItem
    {
        public string FilePath { get; private set; }
        public List<SolutionProject> ProjectsInOrder { get; private set; } = new List<SolutionProject>();

        private SolutionFile(string filePath)
        {
            FilePath = filePath;
        }

        public static SolutionFile Parse(string solutionPath)
        {
            if (!File.Exists(solutionPath))
                throw new FileNotFoundException("Solution file not found.", solutionPath);

            var solutionFile = new SolutionFile(solutionPath);

            // Use MSBuild to parse the solution
            var projectCollection = new ProjectCollection();
            foreach (var project in projectCollection.LoadedProjects)
            {
                if (project.FullPath.EndsWith(".csproj"))
                {
                    var projectType = SolutionProjectType.KnownToBeMSBuildFormat;
                    solutionFile.ProjectsInOrder.Add(new SolutionProject(project.GetPropertyValue("ProjectName"), project.FullPath, projectType));
                }
            }

            return solutionFile;
        }

        public string Name => System.IO.Path.GetFileName(FilePath);
        public string Path => FilePath;

        public IEnumerable<ProjectTreeNode> GetProjectTree()
        {
            var nodes = new List<ProjectTreeNode>();
            foreach (var project in ProjectsInOrder)
            {
                nodes.Add(new ProjectTreeNode(project.ProjectName, project.AbsolutePath));
            }
            return nodes;
        }

        public object GetRoslynItem()
        {
            var workspace = MSBuildWorkspace.Create();
            return workspace.OpenSolutionAsync(FilePath).Result;
        }
    }

    public class SolutionProject
    {
        public string ProjectName { get; private set; }
        public string AbsolutePath { get; private set; }
        public SolutionProjectType ProjectType { get; private set; }

        private SolutionProject(string projectName, string absolutePath, SolutionProjectType projectType)
        {
            ProjectName = projectName;
            AbsolutePath = absolutePath;
            ProjectType = projectType;
        }

        public static SolutionProject? Parse(string projectLine, string solutionDirectory)
        {
            var parts = projectLine.Split(',');
            if (parts.Length < 2)
                return null;

            var projectName = parts[0].Split('=')[1].Trim(' ', '"');
            var relativePath = parts[1].Trim(' ', '"');
            var absolutePath = Path.Combine(solutionDirectory, relativePath);

            var projectType = relativePath.EndsWith(".csproj") ? SolutionProjectType.KnownToBeMSBuildFormat : SolutionProjectType.Unknown;

            return new SolutionProject(projectName, absolutePath, projectType);
        }
    }

    public enum SolutionProjectType
    {
        KnownToBeMSBuildFormat,
        Unknown
    }

    public class ProjectTreeNode
    {
        public string Name { get; }
        public string Path { get; }

        public ProjectTreeNode(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }

    public interface IWorkspaceItem
    {
        string Name { get; }
        string Path { get; }
        IEnumerable<ProjectTreeNode> GetProjectTree();
        object GetRoslynItem();
    }
}
