using CodeForgeIDE.Core.Workspace.Model;

namespace CodeForgeIDE.Core.Plugins
{
    public interface IProjectTreeProvider : IFileScopedProvider
    {
        public Task<ProjectTreeNode> GetProjectTree(string projectPath);
        public Task<ProjectTreeNode> GetProjectNode(string path, bool shallow = false);
    }
}
