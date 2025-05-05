using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CodeForgeIDE.Core;
using CodeForgeIDE.Core.Solution;
using System.Collections.Generic;

namespace CodeForgeIDE.Controls.ProjectExplorer;

public partial class ProjectExplorerItem : UserControl
{
    public string IconPath { get; set; } = Icons.Folder;
    public string ItemName { get; set; } = "Project";
    public bool IsExpanded { get; set; } = false;
    public bool IsSelected { get; set; } = false;
    public bool IsExpandable { get => ChildrenNodes.Count > 0; }
    public List<ProjectTreeNode> ChildrenNodes { get; set; } = new List<ProjectTreeNode>();

    public ProjectExplorerItem()
    {
        InitializeComponent();
        this.DataContext = this;
    }

    public ProjectExplorerItem(string iconPath, string itemName, List<ProjectTreeNode>? children = null)
    {
        InitializeComponent();
        this.DataContext = this;
        IconPath = iconPath;
        ItemName = itemName;
        ChildrenNodes = children ?? new List<ProjectTreeNode>();
    }

    public void Expand()
    {
        if (IsExpandable)
        {
            foreach (var child in ChildrenNodes)
            {
                
            }

            IsExpanded = true;
        }
    }

    public void Collapse()
    {
        if (IsExpanded)
        {
            
            IsExpanded = false;
        }
    }
}