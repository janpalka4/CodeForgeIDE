using CodeForgeIDE.Core.Plugins;
using CodeForgeIDE.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CodeForgeIDE.Core
{
    public class Editor
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public List<IIDEPlugin> Plugins { get; private set; } = new List<IIDEPlugin>();

        public Editor()
        {
        }

        public IServiceCollection AddCoreServices()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<ILoggerService, LoggerService>();
            services.AddSingleton<IFileService, FileService>();

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
            
        }
    }
}
