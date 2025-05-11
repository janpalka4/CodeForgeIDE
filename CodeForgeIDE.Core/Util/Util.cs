using Avalonia.Media;
using CodeForgeIDE.Core.Plugins;
using CodeForgeIDE.Core.Workspace;
using Microsoft.Extensions.DependencyInjection;

namespace CodeForgeIDE.Core.Util
{
    public static class Util
    {
        /// <summary>
        /// Converts a hex string to a Color object.
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Color GetColorFromHex(string hex)
        {
            if (hex.StartsWith("#"))
                hex = hex[1..];
            if (hex.Length == 6)
                hex += "FF";
            return Color.FromArgb(
                byte.Parse(hex[6..8], System.Globalization.NumberStyles.HexNumber),
                byte.Parse(hex[0..2], System.Globalization.NumberStyles.HexNumber),
                byte.Parse(hex[2..4], System.Globalization.NumberStyles.HexNumber),
                byte.Parse(hex[4..6], System.Globalization.NumberStyles.HexNumber));
        }
    }
}
