using CodeForgeIDE.CSharp.Lang.CodeCompletion;
using CodeForgeIDE.CSharp.Lang.CodeCompletion.Symbols;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeForgeIDE.CSharp.Lang
{
    /// <summary>
    /// Veřejné API pro code completion v C# modulu.
    /// Tato třída slouží jako fasáda pro zbytek aplikace a integruje code completion systém.
    /// </summary>
    public class CSharpCodeCompletion : IDisposable
    {
        private readonly CodeCompletionManager _completionManager;
        private static CSharpCodeCompletion _instance;
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Událost vyvolaná, když je dokončena analýza
        /// </summary>
        public event EventHandler<AnalysisCompletedEventArgs> AnalysisCompleted;

        /// <summary>
        /// Událost vyvolaná, když došlo ke změně v databázi symbolů
        /// </summary>
        public event EventHandler<SymbolDatabaseChangedEventArgs> SymbolDatabaseChanged;

        /// <summary>
        /// Singleton instance CSharpCodeCompletion
        /// </summary>
        public static CSharpCodeCompletion Instance
        {
            get
            {
                lock (_lockObject)
                {
                    if (_instance == null)
                    {
                        _instance = new CSharpCodeCompletion();
                    }
                    return _instance;
                }
            }
        }

        /// <summary>
        /// Privátní konstruktor pro singleton pattern
        /// </summary>
        private CSharpCodeCompletion()
        {
            _completionManager = new CodeCompletionManager();
            _completionManager.AnalysisCompleted += (sender, args) => AnalysisCompleted?.Invoke(this, args);
            _completionManager.SymbolDatabaseChanged += (sender, args) => SymbolDatabaseChanged?.Invoke(this, args);
            _completionManager.Initialize();
        }

        /// <summary>
        /// Inicializuje analýzu C# řešení
        /// </summary>
        /// <param name="solutionPath">Cesta k souboru řešení (.sln)</param>
        /// <returns>Úkol představující asynchronní operaci</returns>
        public async Task InitializeSolutionAnalysisAsync(string solutionPath)
        {
            await _completionManager.AnalyzeSolutionAsync(solutionPath);
        }

        /// <summary>
        /// Inicializuje analýzu C# řešení
        /// </summary>
        /// <param name="solutionPath">Cesta k souboru řešení (.sln)</param>
        /// <returns>Úkol představující asynchronní operaci</returns>
        public async Task InitializeSolutionAnalysisAsync(Solution solution)
        {
            await _completionManager.AnalyzeSolutionAsync(solution);
        }

        /// <summary>
        /// Inicializuje analýzu C# projektu
        /// </summary>
        /// <param name="projectPath">Cesta k souboru projektu (.csproj)</param>
        /// <returns>Úkol představující asynchronní operaci</returns>
        public async Task InitializeProjectAnalysisAsync(string projectPath)
        {
            await _completionManager.AnalyzeProjectAsync(projectPath);
        }

        /// <summary>
        /// Analyzuje jeden C# soubor
        /// </summary>
        /// <param name="filePath">Cesta k souboru</param>
        /// <returns>Úkol představující asynchronní operaci</returns>
        public async Task AnalyzeFileAsync(string filePath)
        {
            await _completionManager.AnalyzeFileAsync(filePath);
        }

        /// <summary>
        /// Zpracuje změny v souboru pro real-time aktualizaci databáze symbolů
        /// </summary>
        /// <param name="filePath">Cesta k souboru, který byl změněn</param>
        /// <returns>Úkol představující asynchronní operaci</returns>
        public async Task OnFileChangedAsync(string filePath)
        {
            await _completionManager.OnFileChangedAsync(filePath);
        }

        /// <summary>
        /// Získá položky pro code completion na dané pozici v souboru
        /// </summary>
        /// <param name="filePath">Cesta k souboru</param>
        /// <param name="line">Řádek (0-based)</param>
        /// <param name="column">Sloupec (0-based)</param>
        /// <param name="query">Částečný text pro filtrování výsledků</param>
        /// <returns>Seznam symbolů pro code completion</returns>
        public async Task<IEnumerable<CompletionSymbol>> GetCompletionItemsAsync(string filePath, int line, int column, string query = null)
        {
            return await _completionManager.GetCompletionItemsAsync(filePath, line, column, query);
        }

        /// <summary>
        /// Zruší všechny běžící analýzy
        /// </summary>
        public void CancelAnalysis()
        {
            _completionManager.CancelAnalysis();
        }

        /// <summary>
        /// Uvolní zdroje použité pro code completion
        /// </summary>
        public void Dispose()
        {
            _completionManager.Dispose();
            lock (_lockObject)
            {
                _instance = null;
            }
        }
    }
}
