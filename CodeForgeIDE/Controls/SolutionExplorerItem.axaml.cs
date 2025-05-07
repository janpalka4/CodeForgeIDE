using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CodeForgeIDE.Core;
using CodeForgeIDE.Core.Solution.Model;
using CodeForgeIDE.Core.Util;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CodeForgeIDE.Controls;

public partial class SolutionExplorerItem : UserControl
{
    public ProjectTreeNode TreeNode { get; set; } = new ProjectTreeNode();
    public bool IsExpanded { get; set; } = false;
    public bool IsSelected { get; set; } = false;
    public bool IsExpandable { get => TreeNode?.Children.Count > 0; }
    public string ExpandIconPath { get => IsExpanded ? Icons.CaretBottomRight.IconPath : Icons.CaretRight.IconPath; }

    public SolutionExplorerItem()
    {
        InitializeComponent();
        this.DataContext = this;
    }

    public SolutionExplorerItem(ProjectTreeNode projectTreeNode)
    {
        InitializeComponent();

        TreeNode = projectTreeNode;
        
        this.DataContext = this;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        fileIcon.IconPath = TreeNode.Icon.IconPath;
        fileIcon.DisableCss = TreeNode.Icon == Icons.Folder;
        expandIcon.IsVisible = IsExpandable;

        if (!IsExpandable)
        {
            fileIcon.Margin = new Thickness(24,0,8,0);
        }

        IDE.Editor.OnSelectDocument += IDE_Editor_OnSelectDocument;
    }

    private void IDE_Editor_OnSelectDocument(string path)
    {
        IsSelected = path == TreeNode.FullPath;      
        this.Redraw();
    }

    public void Expand()
    {
        if (IsExpandable && !IsExpanded)
        {
            foreach (var child in TreeNode!.Children)
            {
                ChildStack.Children.Add(new SolutionExplorerItem(child));
            }

            IsExpanded = true;
            expandIcon.IconPath = ExpandIconPath;
        }
    }

    public void Collapse()
    {
        if (IsExpandable && IsExpanded)
        {
            ChildStack.Children.Clear();

            IsExpanded = false;
            expandIcon.IconPath = ExpandIconPath;
        }
    }

    private void GuiIcon_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (IsExpandable)
        {
            if (IsExpanded)
            {
                Collapse();
            }
            else
            {
                Expand();
            }
        }
    }

    private void Grid_PointerPressed_1(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if(e.ClickCount > 1)
        {
            if (TreeNode.IsFile) { 
                IDE.Editor.SelectDocument(TreeNode.FullPath);
                IDE.Editor.OpenDocument(TreeNode.FullPath);
            }
        }
        else
        {
            IDE.Editor.SelectDocument(TreeNode.FullPath);
        }
    }
}