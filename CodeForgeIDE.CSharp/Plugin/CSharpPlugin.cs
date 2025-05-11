using CodeForgeIDE.Core;
using CodeForgeIDE.Core.Plugins;
using CodeForgeIDE.CSharp.Lang.DocumentTransformers;
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
            IDE.Editor.RegisterWorkspaceValidator<CSharpWorkspace>((path) =>
            {
                return path.EndsWith(".csproj") || path.EndsWith(".sln");
            });
            IDE.Editor.RegisterDocumentColorizingTransformer<CSharpDocumentTransformer>(".cs");
        }

        public async Task LoadAsync(IServiceCollection services)
        {
        }
    }
}
