using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaEdit.TextMate;
using AvaloniaEdit.Utils;
using CodeForgeIDE.Core;
using CodeForgeIDE.Core.Plugins;
using CodeForgeIDE.Core.Util;
using CodeForgeIDE.CSharp.Lang.DocumentTransformers;
using CodeForgeIDE.CSharp.Workspace;
using System;
using System.Collections.Generic;
using System.IO;
using TextMateSharp.Grammars;
using TextMateSharp.Registry;

namespace CodeForgeIDE.Controls;

public partial class DocumentEditor : UserControl
{
    public string Filename { get; set; } = "";
    public string FullPath { get; set; } = "";

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

        List<SyntaxHighlightTransformer> transformers = IDE.Editor.GetDocumentColorizingTransformers(FullPath);
        textEditor.TextArea.TextView.LineTransformers.AddRange(transformers);

        // Add TextMate grammar
        if (transformers.Count == 0)
        {
            IRegistryOptions registryOptions = new RegistryOptions(ThemeName.DarkPlus);
            TextMate.Installation installation = textEditor.InstallTextMate(registryOptions);
            installation.SetGrammar("source" + Path.GetExtension(FullPath));
        }
    }
}