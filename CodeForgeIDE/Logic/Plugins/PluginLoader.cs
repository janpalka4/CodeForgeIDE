using Avalonia;
using CodeForgeIDE.Core.Plugins;
using CodeForgeIDE.CSharp.Plugin;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CodeForgeIDE.Logic.Plugins
{
    public class PluginLoader
    {
        public async Task<List<IIDEPlugin>> LoadPlugins(IServiceCollection services)
        {
            // List to store loaded plugins
            List<IIDEPlugin> plugins = new List<IIDEPlugin>() { new CSharpPlugin() };

            // Load each plugin asynchronously
            foreach (var plugin in plugins)
            {
                await plugin.LoadAsync(services);
            }

            return plugins;
        }
    }
}
