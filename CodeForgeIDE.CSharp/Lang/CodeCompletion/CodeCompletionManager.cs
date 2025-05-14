using CodeForgeIDE.CSharp.Lang.CodeCompletion.Symbols;
using Microsoft.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeForgeIDE.CSharp.Lang.CodeCompletion
{
    /// <summary>
    /// Hlavní třída pro správu code completion funkcí v C# modulu.
    /// Tato třída integruje všechny komponenty code completion (databáze, analyzátor) a 
    /// poskytuje veřejné API pro inicializaci a použití funkce code completion.
    /// </summary>
    public class CodeCompletionManager : IDisposable
    {
        private readonly SymbolDatabase _symbolDatabase;
        private readonly CodeAnalyzer _codeAnalyzer;
        private CancellationTokenSource _analysisTokenSource;
        private bool _isInitialized = false;
        private readonly object _lockObject = new object();

        /// <summary>
        /// Událost vyvolaná, když se změní databáze symbolů
        /// </summary>
        public event EventHandler<SymbolDatabaseChangedEventArgs> SymbolDatabaseChanged;

        /// <summary>
        /// Událost vyvolaná, když je dokončena analýza
        /// </summary>
        public event EventHandler<AnalysisCompletedEventArgs> AnalysisCompleted;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public CodeCompletionManager()
        {
            _symbolDatabase = new SymbolDatabase();
            _codeAnalyzer = new CodeAnalyzer(_symbolDatabase);
            _codeAnalyzer.SymbolDatabaseChanged += (sender, args) => SymbolDatabaseChanged?.Invoke(this, args);
            _codeAnalyzer.AnalysisCompleted += (sender, args) => AnalysisCompleted?.Invoke(this, args);
        }

        /// <summary>
        /// Inicializuje manažera code completion
        /// </summary>
        public void Initialize()
        {
            lock (_lockObject)
            {
                if (_isInitialized)
                    return;

                _codeAnalyzer.Initialize();
                _isInitialized = true;
            }
        }

        /// <summary>
        /// Asynchronně analyzuje C# řešení
        /// </summary>
        /// <param name="solutionPath">Cesta k souboru řešení (.sln)</param>
        /// <returns>Úkol představující asynchronní operaci</returns>
        public async Task AnalyzeSolutionAsync(string solutionPath)
        {
            if (!_isInitialized)
                Initialize();

            EnsureTokenSource();
            await _codeAnalyzer.AnalyzeSolutionAsync(solutionPath, _analysisTokenSource.Token);
        }

        /// <summary>
        /// Asynchronně analyzuje C# řešení
        /// </summary>
        /// <param name="solution">Solution</param>
        /// <returns>Úkol představující asynchronní operaci</returns>
        public async Task AnalyzeSolutionAsync(Solution solution)
        {
            if (!_isInitialized)
                Initialize();

            EnsureTokenSource();
            await _codeAnalyzer.AnalyzeSolutionAsync(solution, _analysisTokenSource.Token);
        }

        /// <summary>
        /// Asynchronně analyzuje C# projekt
        /// </summary>
        /// <param name="projectPath">Cesta k souboru projektu (.csproj)</param>
        /// <returns>Úkol představující asynchronní operaci</returns>
        public async Task AnalyzeProjectAsync(string projectPath)
        {
            if (!_isInitialized)
                Initialize();

            EnsureTokenSource();
            await _codeAnalyzer.AnalyzeProjectAsync(projectPath, _analysisTokenSource.Token);
        }

        /// <summary>
        /// Asynchronně analyzuje C# soubor
        /// </summary>
        /// <param name="filePath">Cesta k souboru (.cs)</param>
        /// <returns>Úkol představující asynchronní operaci</returns>
        public async Task AnalyzeFileAsync(string filePath)
        {
            if (!_isInitialized)
                Initialize();

            EnsureTokenSource();
            await _codeAnalyzer.AnalyzeFileAsync(filePath, _analysisTokenSource.Token);
        }

        /// <summary>
        /// Aktualizuje databázi po změně souboru
        /// </summary>
        /// <param name="filePath">Cesta k souboru</param>
        /// <returns>Úkol představující asynchronní operaci</returns>
        public async Task OnFileChangedAsync(string filePath)
        {
            if (!_isInitialized)
                return;

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return;

            if (!Path.GetExtension(filePath).Equals(".cs", StringComparison.OrdinalIgnoreCase))
                return;

            EnsureTokenSource();
            await _codeAnalyzer.UpdateFileAsync(filePath, _analysisTokenSource.Token);
        }

        /// <summary>
        /// Asynchronně získá nabídky code completion pro aktuální pozici
        /// </summary>
        /// <param name="filePath">Cesta k souboru</param>
        /// <param name="line">Řádek (0-based)</param>
        /// <param name="column">Sloupec (0-based)</param>
        /// <param name="query">Částečný text pro filtrování výsledků</param>
        /// <returns>Seznam symbolů pro code completion</returns>
        public async Task<IEnumerable<CompletionSymbol>> GetCompletionItemsAsync(string filePath, int caretPosition, string query = null)
        {
            if (!_isInitialized)
                Initialize();

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return Enumerable.Empty<CompletionSymbol>();

            try
            {
                // Získat lokální symboly pro aktuální scope
                var localSymbols = await _codeAnalyzer.AnalyzeLocalScope(filePath, caretPosition);

                // Získat všechny globální symboly
                var globalSymbols = _symbolDatabase.AllSymbols
                    .Where(s => s.IsPublic)  // Jen veřejné symboly
                    .ToList();

                // Kombinace lokálních a globálních symbolů
                var allSymbols = localSymbols.Union(globalSymbols).ToList();

                // Filtrování podle dotazu, pokud je poskytnut
                if (!string.IsNullOrWhiteSpace(query))
                {
                    // Nejprve zkusíme najít přesné shody
                    IEnumerable<CompletionSymbol> exactMatches;

                    if (query.Contains("."))
                    {
                        string[] parts = query.Split('.');
                        string prefix = parts[0];
                        string suffix = parts.Length > 1 ? parts[1] : string.Empty;

                        exactMatches = GetSymbolMembers(prefix)?
                            .Where(s => s.Name.Contains(suffix, StringComparison.OrdinalIgnoreCase))
                            .ToList() ?? new List<CompletionSymbol>();
                    }
                    else
                    {
                        exactMatches = allSymbols.Where(s => s.Name.StartsWith(query, StringComparison.OrdinalIgnoreCase)).ToList();
                    }

                    // Pokud nemáme žádné přesné shody, zkusíme fuzzy vyhledávání
                    if (!exactMatches.Any())
                    {
                        return _symbolDatabase.FindSymbolsByFuzzySearch(query);
                    }

                    return exactMatches;
                }

                // Vrátíme všechny symboly, pokud není specifikován dotaz
                return allSymbols;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting completion items: {ex.Message}");
                return Enumerable.Empty<CompletionSymbol>();
            }
        }

        private IEnumerable<CompletionSymbol>? GetSymbolMembers(string name)
        {
            IEnumerable<CompletionSymbol> symbols = _symbolDatabase.AllSymbols
                .Where(c => c.Name.Equals(name, StringComparison.Ordinal));

            List<CompletionSymbol> members = new List<CompletionSymbol>();

            foreach (CompletionSymbol symbol in symbols)
            {
                if (symbol is ClassSymbol classSymbol)
                {
                    members.AddRange(classSymbol.Methods.Where(x => x.IsStatic));
                    members.AddRange(classSymbol.Properties.Where(x => x.IsStatic));
                    members.AddRange(classSymbol.Fields.Where(x => x.IsStatic));
                    members.AddRange(classSymbol.Events.Where(x => x.IsStatic));
                }
                else if (symbol is PropertySymbol propertySymbol)
                {
                    if (_symbolDatabase.AllSymbols.FirstOrDefault(x => x.Name == propertySymbol.Type) is ClassSymbol classSymbol1)
                    {
                        members.AddRange(classSymbol1.Methods);
                        members.AddRange(classSymbol1.Properties);
                        members.AddRange(classSymbol1.Fields);
                        members.AddRange(classSymbol1.Events);
                    }
                    else if (_symbolDatabase.AllSymbols.FirstOrDefault(x => x.Name == propertySymbol.Type) is EnumSymbol enumSymbol1)
                    {
                        members.AddRange(enumSymbol1.Members);
                    }
                    else if (_symbolDatabase.AllSymbols.FirstOrDefault(x => x.Name == propertySymbol.Type) is StructSymbol structSymbol1)
                    {
                        members.AddRange(structSymbol1.Methods);
                        members.AddRange(structSymbol1.Properties);
                        members.AddRange(structSymbol1.Fields);
                        members.AddRange(structSymbol1.Events);
                    }
                    else if (_symbolDatabase.AllSymbols.FirstOrDefault(x => x.Name == propertySymbol.Type) is InterfaceSymbol interfaceSymbol1)
                    {
                        members.AddRange(interfaceSymbol1.Methods);
                        members.AddRange(interfaceSymbol1.Properties);
                        members.AddRange(interfaceSymbol1.Events);
                    }
                }
                else if (symbol is InterfaceSymbol interfaceSymbol)
                {
                    members.AddRange(interfaceSymbol.Methods.Where(x => x.IsStatic));
                    members.AddRange(interfaceSymbol.Properties.Where(x => x.IsStatic));
                    members.AddRange(interfaceSymbol.Events.Where(x => x.IsStatic));
                }
                else if (symbol is EnumSymbol enumSymbol)
                {
                    members.AddRange(enumSymbol.Members);
                }
                else if (symbol is StructSymbol structSymbol)
                {
                    members.AddRange(structSymbol.Methods.Where(x => x.IsStatic));
                    members.AddRange(structSymbol.Properties.Where(x => x.IsStatic));
                    members.AddRange(structSymbol.Fields.Where(x => x.IsStatic));
                    members.AddRange(structSymbol.Events.Where(x => x.IsStatic));
                }
                else if (symbol is NamespaceSymbol namespaceSymbol)
                {
                    members.AddRange(namespaceSymbol.Classes);
                    members.AddRange(namespaceSymbol.Interfaces);
                    members.AddRange(namespaceSymbol.Structs);
                    members.AddRange(namespaceSymbol.Enums);
                    members.AddRange(namespaceSymbol.ChildNamespaces);
                }
                else if (symbol is FieldSymbol fieldSymbol)
                {
                    if (_symbolDatabase.AllSymbols.FirstOrDefault(x => x.Name == fieldSymbol.Type) is ClassSymbol classSymbol2)
                    {
                        members.AddRange(classSymbol2.Methods);
                        members.AddRange(classSymbol2.Properties);
                        members.AddRange(classSymbol2.Fields);
                        members.AddRange(classSymbol2.Events);
                    }
                    else if (_symbolDatabase.AllSymbols.FirstOrDefault(x => x.Name == fieldSymbol.Type) is EnumSymbol enumSymbol1)
                    {
                        members.AddRange(enumSymbol1.Members);
                    }
                    else if (_symbolDatabase.AllSymbols.FirstOrDefault(x => x.Name == fieldSymbol.Type) is StructSymbol structSymbol1)
                    {
                        members.AddRange(structSymbol1.Methods);
                        members.AddRange(structSymbol1.Properties);
                        members.AddRange(structSymbol1.Fields);
                        members.AddRange(structSymbol1.Events);
                    }
                    else if (_symbolDatabase.AllSymbols.FirstOrDefault(x => x.Name == fieldSymbol.Type) is InterfaceSymbol interfaceSymbol1)
                    {
                        members.AddRange(interfaceSymbol1.Methods);
                        members.AddRange(interfaceSymbol1.Properties);
                        members.AddRange(interfaceSymbol1.Events);
                    }
                }
                else if (symbol is VariableSymbol variableSymbol)
                {
                    if (_symbolDatabase.AllSymbols.FirstOrDefault(x => x.Name == variableSymbol.Type) is ClassSymbol classSymbol3)
                    {
                        members.AddRange(classSymbol3.Methods);
                        members.AddRange(classSymbol3.Properties);
                        members.AddRange(classSymbol3.Fields);
                        members.AddRange(classSymbol3.Events);
                    }
                    else if (_symbolDatabase.AllSymbols.FirstOrDefault(x => x.Name == variableSymbol.Type) is EnumSymbol enumSymbol1)
                    {
                        members.AddRange(enumSymbol1.Members);
                    }
                    else if (_symbolDatabase.AllSymbols.FirstOrDefault(x => x.Name == variableSymbol.Type) is StructSymbol structSymbol1)
                    {
                        members.AddRange(structSymbol1.Methods);
                        members.AddRange(structSymbol1.Properties);
                        members.AddRange(structSymbol1.Fields);
                        members.AddRange(structSymbol1.Events);
                    }
                    else if (_symbolDatabase.AllSymbols.FirstOrDefault(x => x.Name == variableSymbol.Type) is InterfaceSymbol interfaceSymbol1)
                    {
                        members.AddRange(interfaceSymbol1.Methods);
                        members.AddRange(interfaceSymbol1.Properties);
                        members.AddRange(interfaceSymbol1.Events);
                    }
                }
            }
            return members;
        }

        /// <summary>
        /// Zruší všechny běžící analýzy
        /// </summary>
        public void CancelAnalysis()
        {
            lock (_lockObject)
            {
                if (_analysisTokenSource != null && !_analysisTokenSource.IsCancellationRequested)
                {
                    _analysisTokenSource.Cancel();
                    _analysisTokenSource.Dispose();
                    _analysisTokenSource = null;
                }
            }
        }

        /// <summary>
        /// Zajistí, že je vytvořen token source pro zrušení analýz
        /// </summary>
        private void EnsureTokenSource()
        {
            lock (_lockObject)
            {
                if (_analysisTokenSource == null || _analysisTokenSource.IsCancellationRequested)
                {
                    _analysisTokenSource = new CancellationTokenSource();
                }
            }
        }

        /// <summary>
        /// Uvolní zdroje použité manažerem
        /// </summary>
        public void Dispose()
        {
            lock (_lockObject)
            {
                CancelAnalysis();
                _codeAnalyzer.Dispose();
            }
        }
    }
}
