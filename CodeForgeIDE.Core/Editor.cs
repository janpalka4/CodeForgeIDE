using CodeForgeIDE.Core.Plugins;
using CodeForgeIDE.Core.Services;
using CodeForgeIDE.Core.Solution;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace CodeForgeIDE.Core
{
    public class Editor
    {
        public event Action<string> OnOpenProjectOrSolution;
        public event Action<string> OnOpenDocument;
        public event Action<string> OnSelectDocument;

        public IServiceProvider ServiceProvider { get; private set; }
        public IProjectTreeProvider ProjectTreeProvider { get; private set; }
        public string ProjectRootPath { get; private set; } = string.Empty;

        public List<IIDEPlugin> Plugins { get; private set; } = new List<IIDEPlugin>();

        public Editor()
        {
        }

        public IServiceCollection AddCoreServices()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<ILoggerService, LoggerService>();
            services.AddSingleton<IFileService, FileService>();

            services.AddSingleton<IProjectTreeProvider, DefaultProjectTreeProvider>();

            return services;
        }

        public void Initialize(IServiceCollection services, List<IIDEPlugin>? plugins = null)
        {
            if(plugins != null)
            {
                Plugins = plugins;
            }

            // Initialize the service provider
            ServiceProvider = services.BuildServiceProvider();

            // Resolve services
            var loggerService = ServiceProvider.GetRequiredService<ILoggerService>();

            // Use the services
            loggerService.LogInfo("Editor initialized.", "Editor");
        }

        public void OpenProjectOrSolution(string path)
        {
            // Replace encoded characters in the path  
            path = Uri.UnescapeDataString(path);

            // Select correct provider  
            var providers = ServiceProvider.GetServices<IProjectTreeProvider>();
            foreach (var provider in providers)
            {
                if (provider.ShouldBeUsed(path) && provider is not DefaultProjectTreeProvider)
                {
                    ProjectTreeProvider = provider;
                    break;
                }
            }

            ProjectTreeProvider = ProjectTreeProvider ?? providers.Single(x => x is DefaultProjectTreeProvider);
            ProjectRootPath = path;

            // Notify that the project or solution is opened
            OnOpenProjectOrSolution?.Invoke(path);
        }

        public void OpenDocument(string path)
        {
            OnOpenDocument?.Invoke(path);
        }

        public void SelectDocument(string path)
        {
            OnSelectDocument?.Invoke(path);
        }
    }
}
