using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CodeForgeIDE.Core;
using CodeForgeIDE.Core.Plugins;
using CodeForgeIDE.Core.Util;
using System;

namespace CodeForgeIDE.Controls;

public partial class DocumentEditor : UserControl
{
    public string Filename { get; set; }
    public string FullPath { get; set; }

    private ISyntaxHighlighter? syntaxHighlighter;

    public DocumentEditor()
    {
        InitializeComponent();

        this.DataContext = this;
    }

    public DocumentEditor(string path) : this()
    {
        Filename = System.IO.Path.GetFileName(path);
        FullPath = path;

        this.Redraw();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (string.IsNullOrEmpty(FullPath))
            return;

        textEditor.Text = System.IO.File.ReadAllText(FullPath);
        textEditor.TextChanged += TextEditor_TextChanged;

        syntaxHighlighter = IDE.Editor.GetSyntaxHighlighter(FullPath);
        syntaxHighlighter?.Initialize(textEditor,FullPath);
    }

    private void TextEditor_TextChanged(object? sender, EventArgs e)
    {
        syntaxHighlighter?.Update(textEditor.Text);
    }
}