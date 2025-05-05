using CodeForgeIDE.Core.Solution;

namespace CodeForgeIDE.Core.Plugins
{
    public interface IProjectTreeProvider
    {
        public Task<ProjectTreeNode> GetProjectTree(string projectPath);
        public Task<ProjectTreeNode> GetProjectNode(string path, bool shallow = false);
    }
}
