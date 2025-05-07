using Microsoft.Extensions.DependencyInjection;

namespace CodeForgeIDE.Core.Plugins
{
    public interface IIDEPlugin
    {
        /// <summary>
        /// Loads the plugin 
        /// </summary>
        public Task LoadAsync(IServiceCollection services);

        /// <summary>
        /// Enables the plugin.
        /// </summary>
        /// <returns></returns>
        public Task EnableAsync();

        /// <summary>
        /// Disables the plugin.
        /// </summary>
        public Task DisableAsync();
    }
}
