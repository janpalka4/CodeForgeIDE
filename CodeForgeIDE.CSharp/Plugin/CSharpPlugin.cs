using CodeForgeIDE.Core.Plugins;
using CodeForgeIDE.CSharp.Solution;
using Microsoft.Extensions.DependencyInjection;

namespace CodeForgeIDE.CSharp.Plugin
{
    public class CSharpPlugin : IIDEPlugin
    {
        public async Task DisableAsync()
        {
           
        }

        public async Task EnableAsync()
        {
            
        }

        public async Task LoadAsync(IServiceCollection services)
        {
            services.AddSingleton<IProjectTreeProvider,CSharpSolutionProjectTreeProvider>();
        }
    }
}
