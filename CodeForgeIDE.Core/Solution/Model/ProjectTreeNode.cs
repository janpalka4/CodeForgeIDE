namespace CodeForgeIDE.Core.Solution.Model
{
    public class ProjectTreeNode
    {
        public IconData Icon { get; set; } = Icons.FileCode;
        public string Name { get; set; } = "Untitled";
        public string FullPath { get; set; } = "";
        public bool IsFile { get => !string.IsNullOrEmpty(FullPath) && File.Exists(FullPath); }
        public bool IsDirectory { get => !string.IsNullOrEmpty(FullPath) && Directory.Exists(FullPath); }
        public ProjectTreeNode? Parent { get; private set; } = null;
        public IReadOnlyList<ProjectTreeNode> Children { get => _children.AsReadOnly(); }

        private List<ProjectTreeNode> _children { get; set; } = new List<ProjectTreeNode>();

        public ProjectTreeNode()
        {

        }

        public ProjectTreeNode(IconData icon, string name, string fullPath)
        {
            Icon = icon;
            Name = name;
            FullPath = fullPath;
        }

        public void AddChild(ProjectTreeNode child)
        {
            if (child == null) return;

            child.Parent?.RemoveChild(child);

            child.Parent = this;
            _children.Add(child);
        }

        public void RemoveChild(ProjectTreeNode child)
        {
            if (child == null) return;

            child.Parent?.RemoveChild(child);

            child.Parent = null;
            _children.Remove(child);
        }
    }
}
