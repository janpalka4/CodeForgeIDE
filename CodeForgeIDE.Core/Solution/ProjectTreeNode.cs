namespace CodeForgeIDE.Core.Solution
{
    public class ProjectTreeNode
    {
        public string IconPath { get; set; } = Icons.FileCode;
        public string Name { get; set; } = "Untitled";
        public string FullPath { get; set; } = "";
        public ProjectTreeNode? Parent { get; private set; } = null;
        public IReadOnlyList<ProjectTreeNode> Children { get => _children.AsReadOnly(); }

        private List<ProjectTreeNode> _children { get; set; } = new List<ProjectTreeNode>();

        public ProjectTreeNode() 
        { 
        
        }

        public ProjectTreeNode(string iconPath, string name, string fullPath)
        {
            IconPath = iconPath;
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
