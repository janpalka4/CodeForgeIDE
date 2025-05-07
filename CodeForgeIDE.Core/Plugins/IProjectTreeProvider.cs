using CodeForgeIDE.Core.Solution.Model;

namespace CodeForgeIDE.Core.Plugins
{
    public interface IProjectTreeProvider
    {
        public Task<ProjectTreeNode> GetProjectTree(string projectPath);
        public Task<ProjectTreeNode> GetProjectNode(string path, bool shallow = false);
        public bool ShouldBeUsed(string path);
    }
}
