using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CodeForgeIDE.Core;
using CodeForgeIDE.CSharp.Workspace;
using System.Threading.Tasks;

namespace CodeForgeIDE.Controls;

public partial class SolutionExplorer : UserControl
{
    private ProjectLoader? _projectLoader;

    public SolutionExplorer()
    {
        InitializeComponent();
    }

    protected override async void OnInitialized()
    {
        base.OnInitialized();

        // Initialize ProjectLoader with the workspace
        if (IDE.Editor.Workspace is CSharpWorkspace csharpWorkspace)
        {
            _projectLoader = new ProjectLoader(csharpWorkspace);
        }

        // Initialize the solution explorer with the project tree
        await LoadProjectTree();
    }

    private async Task LoadProjectTree()
    {
        if (string.IsNullOrEmpty(IDE.Editor.Workspace?.Path) || _projectLoader == null)
            return;

        // Gradually load the project tree using ProjectLoader
        if (IDE.Editor.Workspace.Path.EndsWith(".sln"))
        {
            await _projectLoader.LoadSolutionAsync(IDE.Editor.Workspace.Path);
        }
        else if (IDE.Editor.Workspace.Path.EndsWith(".csproj"))
        {
            await _projectLoader.LoadProjectAsync(IDE.Editor.Workspace.Path);
        }
        else
        {
            await _projectLoader.LoadFolderAsync(IDE.Editor.Workspace.Path);
        }

        // TODO: Update UI to reflect the loaded project tree
    }
}