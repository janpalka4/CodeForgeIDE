using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace CodeForgeIDE.Core.Util
{
    public static class IconDataExtension
    {
        private static Dictionary<string, IImage> _iconCache = new Dictionary<string, IImage>();

        public static IImage ToImage(this IconData iconData)
        {
            if (_iconCache.ContainsKey(iconData.IconPath))
            {
                return _iconCache[iconData.IconPath];
            }
            var icon = new Bitmap(AssetLoader.Open(new Uri(iconData.IconPath.Replace(".svg",".png"))));
            if (icon != null)
            {
                _iconCache[iconData.IconPath] = icon;
                return icon;
            }
            throw new Exception($"Icon '{iconData.IconPath}' not found.");
        }
    }
}
