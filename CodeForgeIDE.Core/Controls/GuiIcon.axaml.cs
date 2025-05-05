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
           defaultValue: Icons.Folder,
           inherits: false);
    //public static readonly DirectProperty<GuiIcon, string> IconPathProperty =
    //    AvaloniaProperty.RegisterDirect<GuiIcon, string>(
    //        nameof(IconPath),
    //        o => o.IconPath,
    //        (o, v) => o.IconPath = v);


    public string IconPath { get { return GetValue(IconPathProperty); } set { SetValue(IconPathProperty,value); ValidateCss();  } }
    public bool DisableCss { get; set; }
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
        DisableCss = Icons.DisableCss.GetValueOrDefault(IconPath, false);

        this.DataContext = null;
        this.DataContext = this;
    }
}