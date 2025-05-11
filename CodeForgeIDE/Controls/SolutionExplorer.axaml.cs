using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CodeForgeIDE.Core;
using CodeForgeIDE.CSharp.Workspace;
using CodeForgeIDE.CSharp.Workspace.Model;
using System.Threading.Tasks;

namespace CodeForgeIDE.Controls;

public partial class SolutionExplorer : UserControl
{
    public SolutionExplorer()
    {
        InitializeComponent();
    }

    protected override async void OnInitialized()
    {
        base.OnInitialized();

        // Initialize the solution explorer with the project tree
        await LoadProjectTree();
    }

    private async Task LoadProjectTree()
    {
        if (string.IsNullOrEmpty(IDE.Editor.Workspace?.FullPath))
            return;

        
        stackContent.Children.Clear();
        stackContent.Children.Add(new SolutionExplorerItem(new CSharpSolutionWorkspaceItem()));
    }
}