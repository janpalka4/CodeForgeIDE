using Avalonia.Media;
using CodeForgeIDE.Core.Plugins;
using CodeForgeIDE.Core.Workspace;
using Microsoft.Extensions.DependencyInjection;

namespace CodeForgeIDE.Core.Util
{
    public static class Util
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Typeof provider</typeparam>
        /// <typeparam name="D">Default provider</typeparam>
        /// <param name="ServiceProvider"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T GetProviderForFile<T,D>(this IServiceProvider ServiceProvider, string path) where T : IFileScopedProvider where D : IFileScopedProvider
        {
            var providers = ServiceProvider.GetServices<IProjectTreeProvider>();
            foreach (var provider in providers)
            {
                if (provider.ShouldBeUsed(path) && provider is not D)
                {
                    return (T)provider;
                }
            }

            return (T)providers.Single(x => x is D);
        }

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
