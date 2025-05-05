using CodeForgeIDE.Core.Plugins;

namespace CodeForgeIDE.Core.Solution
{
    public class DefaultProjectTreeProvider : IProjectTreeProvider
    {
        public DefaultProjectTreeProvider()
        {
        }

        public async Task<ProjectTreeNode> GetProjectTree(string projectPath)
        {
            return await GetProjectNode(projectPath);
        }

        public async Task<ProjectTreeNode> GetProjectNode(string path, bool shallow = false)
        {
            if(path.EndsWith('/') || path.EndsWith('\\'))
            {
                path = path.TrimEnd('/', '\\');
            }

            ProjectTreeNode projectNode = new ProjectTreeNode() 
            {
                FullPath = path,
                Name = Path.GetFileName(path),
                IconPath = Icons.FileCode
            };

            if (Directory.Exists(path) && !shallow)
            {
                projectNode.IconPath = Icons.Folder;

                foreach (string _path in Directory.GetDirectories(path))
                {
                    ProjectTreeNode dirNode = await GetProjectNode(_path);
                    projectNode.AddChild(dirNode);
                }
                foreach (string _path in Directory.GetFiles(path))
                {
                    ProjectTreeNode fileNode = await GetProjectNode(_path);
                    projectNode.AddChild(fileNode);
                }
            }

            return projectNode;
        }
    }
}
