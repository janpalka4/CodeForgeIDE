using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CodeForgeIDE.Core;
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
        if(string.IsNullOrEmpty(IDE.Editor.Workspace?.Path))
            return;

        var projectTree = await IDE.Editor.Workspace!.ProjectTreeProvider!.GetProjectTree(IDE.Editor.Workspace.Path);

        if (projectTree != null)
        {
            stackContent.Children.Clear();
            stackContent.Children.Add(new SolutionExplorerItem(projectTree));
        }
    }
}