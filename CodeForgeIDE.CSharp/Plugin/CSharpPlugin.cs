using CodeForgeIDE.Core;
using CodeForgeIDE.Core.Plugins;
using CodeForgeIDE.CSharp.Lang;
using CodeForgeIDE.CSharp.Workspace;
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
            IDE.Editor.RegisterWorkspaceType<CSharpWorkspace>((path) =>
            {
                return path.EndsWith(".csproj") || path.EndsWith(".sln");
            });
        }

        public async Task LoadAsync(IServiceCollection services)
        {
            services.AddSingleton<IProjectTreeProvider,CSharpSolutionProjectTreeProvider>();
        }
    }
}
