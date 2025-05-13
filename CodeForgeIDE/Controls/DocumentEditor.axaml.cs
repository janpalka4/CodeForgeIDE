using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.TextMate;
using AvaloniaEdit.Utils;
using CodeForgeIDE.Core;
using CodeForgeIDE.Core.Lang;
using CodeForgeIDE.Core.Plugins;
using CodeForgeIDE.Core.Util;
using CodeForgeIDE.CSharp.Lang;
using CodeForgeIDE.CSharp.Lang.CodeCompletion;
using CodeForgeIDE.CSharp.Lang.DocumentTransformers;
using CodeForgeIDE.CSharp.Workspace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TextMateSharp.Grammars;
using TextMateSharp.Registry;

namespace CodeForgeIDE.Controls;

public partial class DocumentEditor : UserControl
{
    public string Filename { get; set; } = "";
    public string FullPath { get; set; } = "";

    private CompletionWindow? _completionWindow;

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

        textEditor.TextArea.KeyDown += TextArea_KeyDown;
    }

    private void TextArea_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        if (e.Key == Avalonia.Input.Key.Space && e.KeyModifiers.HasFlag(Avalonia.Input.KeyModifiers.Control))
        {
            e.Handled = true;
            ShowCompletion();
        }
    }

    private async void ShowCompletion()
    {
        if (_completionWindow != null)
            return;

        // Create a new window for auto-completion  
        _completionWindow = new CompletionWindow(textEditor.TextArea);
        var data = _completionWindow.CompletionList.CompletionData;

        var caretOffset = textEditor.CaretOffset;
        var line = textEditor.Document.GetLineByOffset(caretOffset);
        var column = caretOffset - line.Offset;

        // Add suggestions  
        var completionItems = await CSharpCodeCompletion.Instance.GetCompletionItemsAsync(FullPath, line.LineNumber, column);
        data.AddRange(completionItems.Select(x => new CSharpCompletionData(x)));

        // Show the window  
        _completionWindow.Show();

        // Event for closing the window  
        _completionWindow.Closed += (sender, e) => _completionWindow = null;
    }
}