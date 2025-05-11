namespace CodeForgeIDE.Core.Workspace.Model
{
    public class WorkspaceItem
    {
        public IconData Icon { get; set; } = Icons.FileCode;
        public string Name { get; set; } = "Untitled";
        public string FullPath { get; set; } = "";
        public bool IsFile { get => !string.IsNullOrEmpty(FullPath) && File.Exists(FullPath); }
        public bool IsDirectory { get => !string.IsNullOrEmpty(FullPath) && Directory.Exists(FullPath); }
        public virtual WorkspaceItem? Parent { get; private set; } = null;
        public virtual IReadOnlyList<WorkspaceItem> Children { get => _children.AsReadOnly(); }

        protected List<WorkspaceItem> _children { get; set; } = new List<WorkspaceItem>();

        public WorkspaceItem()
        {

        }

        public WorkspaceItem(IconData icon, string name, string fullPath)
        {
            Icon = icon;
            Name = name;
            FullPath = fullPath;
        }

        public void AddChild(WorkspaceItem child)
        {
            if (child == null) return;

            child.Parent?.RemoveChild(child);

            child.Parent = this;
            _children.Add(child);
        }

        public void RemoveChild(WorkspaceItem child)
        {
            if (child == null) return;

            child.Parent?.RemoveChild(child);

            child.Parent = null;
            _children.Remove(child);
        }
    }
}
