using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CodeForgeIDE.Core;
using System.IO;
using System.Reflection.Metadata.Ecma335;

namespace CodeForgeIDE.Controls;

public partial class RecentFileItem : UserControl
{
    private RecentOpenedFile? _recentFile;
    public RecentFileItem()
    {
        InitializeComponent();
        this.DataContext = this;
    }

    public RecentFileItem(RecentOpenedFile recentFile)
    {
        InitializeComponent();

        _recentFile = recentFile;
        this.DataContext = this;
    }

    protected override void OnInitialized()
    {
        if (_recentFile != null)
        {
            IconData iconData;
            if (Directory.Exists(_recentFile.Path))
            {
                iconData = Icons.Folder;
            }
            else
            {
                string extension = Path.GetExtension(_recentFile.Path);
                if (extension == ".sln")
                {
                    iconData = CodeForgeIDE.CSharp.Icons.Solution;
                }
                else if (extension == ".csproj")
                {
                    iconData = CodeForgeIDE.CSharp.Icons.Project;
                }
                else
                {
                    iconData = Icons.FileCode;
                }
            }

            icon.IconPath = iconData.IconPath;
            icon.DisableCss = iconData.DisableCss;

            textFileName.Text = Path.GetFileName(_recentFile.Path);
            textFilePath.Text = _recentFile.Path;
            textFileTime.Text = _recentFile.LastOpened.ToString("yyyy-MM-dd HH:mm:ss");
        }
        base.OnInitialized();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_recentFile == null)
            return;

        IDE.Editor.OpenProjectOrSolution(_recentFile.Path);
    }
}