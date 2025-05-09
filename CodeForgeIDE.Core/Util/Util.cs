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
    }
}
