using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.UpDock.Controls;
using CodeForgeIDE.Core;
using System;
using System.IO;

namespace CodeForgeIDE.Controls;

public partial class Editor : UserControl
{
    public Editor()
    {
        InitializeComponent();

        IDE.Editor.OnOpenDocument += OnOpenDocument;
    }

    private void OnOpenDocument(string path)
    {
        //Add child to contentTab
        contentTab.Items.Add(new ClosableTabItem()
        {
            Header = Path.GetFileName(path),
            Content = new DocumentEditor(path)
        });
    }
}