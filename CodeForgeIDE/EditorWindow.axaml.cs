using Avalonia;
using Avalonia.Controls;
using System;
using Avalonia.Input;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace CodeForgeIDE;

public partial class EditorWindow : Window
{
    public EditorWindow()
    {
        InitializeComponent();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }


    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}