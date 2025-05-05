using CodeForgeIDE.Core.Plugins;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeForgeIDE.Logic.Plugins
{
    public class PluginLoader
    {
        public async Task<List<IIDEPlugin>> LoadPlugins(IServiceCollection services)
        {
            //TODO: Load plugins from a ./Plugins directory
            List<IIDEPlugin> plugins = new List<IIDEPlugin>();
            await Task.Delay(1000);

            return plugins;
        }
    }
}
