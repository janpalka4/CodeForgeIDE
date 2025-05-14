using Avalonia;
using Avalonia.Controls;
using System;
using Avalonia.Input;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;

namespace CodeForgeIDE;

public partial class EditorWindow : DraggableWindow
{
    public EditorWindow()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        this.EnsureInitialized();
        var p = this.FindControl<Panel>("dragPanel");
        this.SetupDragControl(p);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void GuiIcon_PointerPressed_Minimize(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void GuiIcon_PointerPressed_Maximize(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void GuiIcon_PointerPressed_Close(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}