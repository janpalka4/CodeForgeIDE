using CodeForgeIDE.Core;
using CodeForgeIDE.Core.Plugins;
using CodeForgeIDE.Logic.Plugins;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeForgeIDE.Logic
{
    public class EditorInitializer
    {
        public event Action<string> OnInitializationInfo;

        public async Task InitializeEditor()
        {
            await Task.Delay(200);
            IServiceCollection services = IDE.Editor.AddCoreServices();

            PrintInitializationInfo("Loading plugins...");

            List<IIDEPlugin> plugins = new List<IIDEPlugin>();

            PluginLoader loader = new PluginLoader();
            plugins = await loader.LoadPlugins(services);

            PrintInitializationInfo("Finishig initialization...");

            IDE.Editor.Initialize(services,plugins);
        }

        private void PrintInitializationInfo(string info)
        {
            OnInitializationInfo?.Invoke(info);
        }
    }
}
