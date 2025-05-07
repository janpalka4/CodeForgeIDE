using Avalonia.Controls;

namespace CodeForgeIDE.Core.Util
{
    public static class ControlUtil
    {
        public static void Redraw(this UserControl control)
        {
            var previousDataContext = control.DataContext;
            control.DataContext = null;
            control.DataContext = previousDataContext;
        }
    }
}
