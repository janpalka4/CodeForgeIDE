using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using AvaloniaEdit.Rendering;
using CodeForgeIDE.Core.Plugins;
using CodeForgeIDE.Core.Services;
using CodeForgeIDE.Core.Workspace;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace CodeForgeIDE.Core
{
    public class Editor
    {
        public event Action<string>? OnOpenProjectOrSolution;
        public event Action<string>? OnOpenDocument;
        public event Action<string>? OnSelectDocument;
        public event Action OnWorkspaceLoaded;

        public IServiceProvider? ServiceProvider { get; private set; }
        public EditorConfig Config { get; private set; } = new EditorConfig();
        public EditorWorkspace? Workspace { get; private set; } = null;
        public List<IIDEPlugin> Plugins { get; private set; } = new List<IIDEPlugin>();

        private Dictionary<Type, Func<string, bool>> WorkspaceTypeValidators = new Dictionary<Type, Func<string, bool>>();
        private Dictionary<string,List<Type>> DocumentColorizingTransformers = new Dictionary<string, List<Type>>();

        public Editor()
        {
            Config = LoadConfiguration();

            if(Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Exit += Desktop_Exit;
            }
        }

        private void Desktop_Exit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
        {
            SaveConfiguration();
        }

        public IServiceCollection AddCoreServices()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<ILoggerService, LoggerService>();
            services.AddSingleton<IFileService, FileService>();

            return services;
        }

        public async void Initialize(IServiceCollection services, List<IIDEPlugin>? plugins = null)
        {
            if (plugins != null)
            {
                Plugins = plugins;
            }

            // Initialize the service provider
            ServiceProvider = services.BuildServiceProvider();

            // Resolve services
            var loggerService = ServiceProvider.GetRequiredService<ILoggerService>();

            // Use the services
            loggerService.LogInfo("Editor initialized.", "Editor");

            foreach(var plugin in Plugins)
            {
                await plugin.EnableAsync();
            }
        }

        public async void OpenProjectOrSolution(string path)
        {
            // Replace encoded characters in the path  
            path = Uri.UnescapeDataString(path);

            // Init workspace
            Workspace = (EditorWorkspace)(Activator.CreateInstance(WorkspaceTypeValidators.Where(x => x.Key.IsAssignableTo(typeof(EditorWorkspace)))
                .First(x => x.Value(path)).Key,path) ?? new EditorWorkspace(path));

            await Workspace.Initialize();

            // Notify that the project or solution is opened
            OnOpenProjectOrSolution?.Invoke(path);

            if (Config.RecentOpenedFiles.FirstOrDefault(x => x.Path == path) is RecentOpenedFile recentFile)
            {
                recentFile.LastOpened = DateTime.Now;
            }
            else
            {
                Config.RecentOpenedFiles.Add(new RecentOpenedFile() { Path = path, LastOpened = DateTime.Now });
            }

            SaveConfiguration();

            // Notify that the workspace is loaded
            OnWorkspaceLoaded?.Invoke();
        }

        public void OpenDocument(string path)
        {
            OnOpenDocument?.Invoke(path);
        }

        public void SelectDocument(string path)
        {
            OnSelectDocument?.Invoke(path);
        }

        public void RegisterWorkspaceValidator<T>(Func<string, bool> check) where T : EditorWorkspace
        {
            WorkspaceTypeValidators.Add(typeof(T), check);
        }

        public void RegisterDocumentColorizingTransformer<T>(string extension) where T : SyntaxHighlightTransformer
        {
            if (DocumentColorizingTransformers.ContainsKey(extension))
            {
                DocumentColorizingTransformers[extension].Add(typeof(T));
            }
            else
            {
                DocumentColorizingTransformers.Add(extension, new List<Type>() { typeof(T) });
            }
        }

        public List<SyntaxHighlightTransformer> GetDocumentColorizingTransformers(string path)
        {
            string extension = Path.GetExtension(path);

            if (DocumentColorizingTransformers.ContainsKey(extension))
            {
                return DocumentColorizingTransformers[extension].Select(x => (SyntaxHighlightTransformer)Activator.CreateInstance(x,path)!).ToList();
            }
            else
            {
                return new List<SyntaxHighlightTransformer>();
            }
        }

        public T GetWorkspaceAs<T>() where T : EditorWorkspace
        {
            if (Workspace is T workspace)
            {
                return workspace;
            }
            else
            {
                throw new InvalidOperationException($"The current workspace is not of type {typeof(T).Name}.");
            }
        }

        private EditorConfig LoadConfiguration() { 
            if(File.Exists("ide/config.json"))
            {
                string json = File.ReadAllText("ide/config.json");
                EditorConfig config = JsonConvert.DeserializeObject<EditorConfig>(json) ?? new EditorConfig();
                config.RecentOpenedFiles = config.RecentOpenedFiles.OrderByDescending(x => x.LastOpened).Take(25).ToList();

                return config;
            }
            else
            {
                return new EditorConfig();
            }
        }

        private void SaveConfiguration()
        {
            string json = JsonConvert.SerializeObject(Config, Formatting.Indented);
            Directory.CreateDirectory("ide");
            File.WriteAllText("ide/config.json", json);
        }
    }
}
