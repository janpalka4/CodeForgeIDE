using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Svg.Skia;

namespace CodeForgeIDE.Core.Controls;

public partial class GuiIcon : UserControl
{
    public static readonly StyledProperty<string> IconPathProperty =
       AvaloniaProperty.Register<GuiIcon, string>(
           name: nameof(IconPath),
           defaultValue: Icons.Folder.IconPath,
           inherits: false);

    public static readonly StyledProperty<bool> DisableCssProperty =
       AvaloniaProperty.Register<GuiIcon, bool>(
           name: nameof(DisableCss),
           defaultValue: Icons.Folder.DisableCss,
           inherits: false);
    //public static readonly DirectProperty<GuiIcon, string> IconPathProperty =
    //    AvaloniaProperty.RegisterDirect<GuiIcon, string>(
    //        nameof(IconPath),
    //        o => o.IconPath,
    //        (o, v) => o.IconPath = v);


    public string IconPath { get { return GetValue(IconPathProperty); } set { SetValue(IconPathProperty,value); ValidateCss(); } }
    public bool DisableCss { get { return GetValue(DisableCssProperty); } set { SetValue(DisableCssProperty,value);  ValidateCss(); } }
    public bool EnableCss { get => !DisableCss; }


    //private string _iconPath = Icons.Folder;

    public GuiIcon()
    {
        InitializeComponent();

       ValidateCss();

        this.DataContext = this;
    }

    private void ValidateCss()
    {
        this.DataContext = null;
        this.DataContext = this;
    }
}