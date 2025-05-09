using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using CodeForgeIDE.Controls;
using CodeForgeIDE.Core;
using System.Collections.Generic;
using System.Linq;

namespace CodeForgeIDE;

public partial class GatewayWindow : Window
{
    public GatewayWindow()
    {
        InitializeComponent();

        IDE.Editor.OnOpenProjectOrSolution += Editor_OnOpenProjectOrSolution;
    }

    private void Editor_OnOpenProjectOrSolution(string obj)
    {
        EditorWindow window = new EditorWindow();
        window.Show();
        this.Close();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        recentFiles.Children.Clear();
        foreach (var recentFile in IDE.Editor.Config.RecentOpenedFiles)
        {
            RecentFileItem item = new RecentFileItem(recentFile);
            recentFiles.Children.Add(item);
        }
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }

    private void Button_Click_Create(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

    }

    private async void Button_Click_Open(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        TopLevel? topLevel = GetTopLevel(this);

        if (topLevel is null)
            return;

        //Pick file
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new Avalonia.Platform.Storage.FilePickerOpenOptions() 
        { 
            AllowMultiple = false, 
            Title = "Open existing project", 
            FileTypeFilter = new List<FilePickerFileType>() 
                {
                    new FilePickerFileType("C# project or solution file")
                    {
                        Patterns = new List<string>() { "*.csproj", "*.sln" }
                    }
                } 
        });

        if (files is null || files.Count == 0)
            return;

        //Load project or solution
        IDE.Editor.OpenProjectOrSolution(files[0].Path.AbsolutePath);

        //Open editor
        EditorWindow window = new EditorWindow();
        window.Show();

        Close();
    }

    private void Button_Click_Preferences(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

    }

    private async void Button_Click_OpenFolder(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        TopLevel? topLevel = GetTopLevel(this);

        if (topLevel is null)
            return;

        //Pick file
        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new Avalonia.Platform.Storage.FolderPickerOpenOptions()
        {
            AllowMultiple = false,
            Title = "Open folder"
        });

        if (folders is null || folders.Count == 0)
            return;

        //Load project or solution
        IDE.Editor.OpenProjectOrSolution(folders[0].Path.AbsolutePath);

        //Open editor
        EditorWindow window = new EditorWindow();
        window.Show();

        Close();
    }

}