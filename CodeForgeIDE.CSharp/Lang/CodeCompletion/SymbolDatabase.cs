using CodeForgeIDE.CSharp.Lang.CodeCompletion.Symbols;
using FuzzySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeForgeIDE.CSharp.Lang.CodeCompletion
{
    /// <summary>
    /// Databáze symbolů pro code completion.
    /// Tato třída slouží jako centrální úložiště pro všechny symboly, které byly analyzovány.
    /// </summary>
    public class SymbolDatabase
    {
        private readonly Dictionary<string, CompletionSymbol> _symbolsByFullName = new Dictionary<string, CompletionSymbol>();
        private readonly Dictionary<string, NamespaceSymbol> _namespaces = new Dictionary<string, NamespaceSymbol>();
        private readonly Dictionary<string, List<CompletionSymbol>> _symbolsByNameOnly = new Dictionary<string, List<CompletionSymbol>>();
        private readonly object _lockObject = new object();

        /// <summary>
        /// Získá všechny symboly v databázi
        /// </summary>
        public IReadOnlyList<CompletionSymbol> AllSymbols
        {
            get
            {
                lock (_lockObject)
                {
                    return _symbolsByFullName.Values.ToList().AsReadOnly();
                }
            }
        }

        /// <summary>
        /// Získá všechny jmenné prostory v databázi
        /// </summary>
        public IReadOnlyList<NamespaceSymbol> Namespaces
        {
            get
            {
                lock (_lockObject)
                {
                    return _namespaces.Values.ToList().AsReadOnly();
                }
            }
        }

        /// <summary>
        /// Přidá symbol do databáze
        /// </summary>
        /// <param name="symbol">Symbol, který má být přidán</param>
        public void AddSymbol(CompletionSymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            lock (_lockObject)
            {
                // Přidání do hlavního slovníku podle plně kvalifikovaného názvu
                if (!string.IsNullOrEmpty(symbol.FullyQualifiedName))
                {
                    _symbolsByFullName[symbol.FullyQualifiedName] = symbol;
                }

                // Přidání do slovníku podle názvu (bez jmenného prostoru)
                if (!_symbolsByNameOnly.ContainsKey(symbol.Name))
                {
                    _symbolsByNameOnly[symbol.Name] = new List<CompletionSymbol>();
                }
                _symbolsByNameOnly[symbol.Name].Add(symbol);

                // Správa jmenných prostorů
                if (symbol is NamespaceSymbol namespaceSymbol)
                {
                    _namespaces[namespaceSymbol.Name] = namespaceSymbol;
                }
                else if (!string.IsNullOrEmpty(symbol.Namespace))
                {
                    // Zajistíme existenci jmenného prostoru
                    if (!_namespaces.TryGetValue(symbol.Namespace, out var ns))
                    {
                        ns = new NamespaceSymbol(symbol.Namespace);
                        _namespaces[symbol.Namespace] = ns;
                    }

                    // Přidáme symbol do správné kolekce v jmenném prostoru
                    switch (symbol)
                    {
                        case ClassSymbol classSymbol:
                            ns.Classes.Add(classSymbol);
                            break;
                        case InterfaceSymbol interfaceSymbol:
                            ns.Interfaces.Add(interfaceSymbol);
                            break;
                        case EnumSymbol enumSymbol:
                            ns.Enums.Add(enumSymbol);
                            break;
                        case StructSymbol structSymbol:
                            ns.Structs.Add(structSymbol);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Odebere symbol z databáze
        /// </summary>
        /// <param name="symbol">Symbol, který má být odebrán</param>
        public void RemoveSymbol(CompletionSymbol symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(nameof(symbol));

            lock (_lockObject)
            {
                // Odebrání z hlavního slovníku
                if (!string.IsNullOrEmpty(symbol.FullyQualifiedName))
                {
                    _symbolsByFullName.Remove(symbol.FullyQualifiedName);
                }

                // Odebrání ze slovníku podle názvu
                if (_symbolsByNameOnly.TryGetValue(symbol.Name, out var symbolList))
                {
                    symbolList.Remove(symbol);
                    if (symbolList.Count == 0)
                    {
                        _symbolsByNameOnly.Remove(symbol.Name);
                    }
                }

                // Správa jmenných prostorů
                if (symbol is NamespaceSymbol namespaceSymbol)
                {
                    _namespaces.Remove(namespaceSymbol.Name);
                }
                else if (!string.IsNullOrEmpty(symbol.Namespace) && _namespaces.TryGetValue(symbol.Namespace, out var ns))
                {
                    // Odebereme symbol ze správné kolekce v jmenném prostoru
                    switch (symbol)
                    {
                        case ClassSymbol classSymbol:
                            ns.Classes.Remove(classSymbol);
                            break;
                        case InterfaceSymbol interfaceSymbol:
                            ns.Interfaces.Remove(interfaceSymbol);
                            break;
                        case EnumSymbol enumSymbol:
                            ns.Enums.Remove(enumSymbol);
                            break;
                        case StructSymbol structSymbol:
                            ns.Structs.Remove(structSymbol);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Najde symboly podle přesného názvu
        /// </summary>
        /// <param name="name">Název pro vyhledávání</param>
        /// <returns>Seznam odpovídajících symbolů</returns>
        public IEnumerable<CompletionSymbol> FindSymbolsByExactName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return Enumerable.Empty<CompletionSymbol>();

            lock (_lockObject)
            {
                if (_symbolsByNameOnly.TryGetValue(name, out var symbols))
                {
                    return symbols.ToList();
                }
                return Enumerable.Empty<CompletionSymbol>();
            }
        }

        /// <summary>
        /// Najde symboly podle přesného plně kvalifikovaného názvu
        /// </summary>
        /// <param name="fullyQualifiedName">Plně kvalifikovaný název</param>
        /// <returns>Symbol, pokud existuje, jinak null</returns>
        public CompletionSymbol FindSymbolByFullyQualifiedName(string fullyQualifiedName)
        {
            if (string.IsNullOrEmpty(fullyQualifiedName))
                return null;

            lock (_lockObject)
            {
                if (_symbolsByFullName.TryGetValue(fullyQualifiedName, out var symbol))
                {
                    return symbol;
                }
                return null;
            }
        }

        /// <summary>
        /// Najde symboly obsahující zadaný řetězec v názvu
        /// </summary>
        /// <param name="partialName">Částečný název pro vyhledávání</param>
        /// <returns>Seznam odpovídajících symbolů</returns>
        public IEnumerable<CompletionSymbol> FindSymbolsByPartialName(string partialName)
        {
            if (string.IsNullOrEmpty(partialName))
                return Enumerable.Empty<CompletionSymbol>();

            lock (_lockObject)
            {
                return _symbolsByNameOnly.Keys
                    .Where(name => name.Contains(partialName, StringComparison.OrdinalIgnoreCase))
                    .SelectMany(name => _symbolsByNameOnly[name])
                    .ToList();
            }
        }

        /// <summary>
        /// Najde symboly pomocí fuzzy vyhledávání
        /// </summary>
        /// <param name="query">Dotaz pro vyhledávání</param>
        /// <param name="limit">Maximální počet výsledků</param>
        /// <param name="minScore">Minimální skóre shody (0-100)</param>
        /// <returns>Seznam odpovídajících symbolů seřazených podle relevance</returns>
        public IEnumerable<CompletionSymbol> FindSymbolsByFuzzySearch(string query, int limit = 10, int minScore = 60)
        {
            if (string.IsNullOrEmpty(query))
                return Enumerable.Empty<CompletionSymbol>();

            lock (_lockObject)
            {
                var symbolNames = _symbolsByNameOnly.Keys.ToList();
                
                // Použití FuzzySharp pro fuzzy matching
                var results = Process.ExtractTop(query, symbolNames, limit: limit)
                    .Where(match => match.Score >= minScore)
                    .SelectMany(match => _symbolsByNameOnly[match.Value])
                    .ToList();
                
                return results;
            }
        }

        /// <summary>
        /// Vyhledá všechny symboly patřící do určitého jmenného prostoru
        /// </summary>
        /// <param name="namespaceName">Název jmenného prostoru</param>
        /// <returns>Seznam symbolů v daném jmenném prostoru</returns>
        public IEnumerable<CompletionSymbol> FindSymbolsInNamespace(string namespaceName)
        {
            if (string.IsNullOrEmpty(namespaceName))
                return Enumerable.Empty<CompletionSymbol>();

            lock (_lockObject)
            {
                if (_namespaces.TryGetValue(namespaceName, out var ns))
                {
                    var result = new List<CompletionSymbol>();
                    result.AddRange(ns.Classes);
                    result.AddRange(ns.Interfaces);
                    result.AddRange(ns.Enums);
                    result.AddRange(ns.Structs);
                    return result;
                }
                return Enumerable.Empty<CompletionSymbol>();
            }
        }

        /// <summary>
        /// Vyčistí celou databázi symbolů
        /// </summary>
        public void Clear()
        {
            lock (_lockObject)
            {
                _symbolsByFullName.Clear();
                _symbolsByNameOnly.Clear();
                _namespaces.Clear();
            }
        }

        /// <summary>
        /// Aktualizuje databázi jmenných prostorů s použitím using direktiv
        /// </summary>
        /// <param name="usings">Seznam using direktiv</param>
        public async Task UpdateUsingDirectivesAsync(IEnumerable<string> usings)
        {
            if (usings == null)
                return;

            await Task.Run(() =>
            {
                foreach (var usingNamespace in usings)
                {
                    if (!string.IsNullOrEmpty(usingNamespace) && !_namespaces.ContainsKey(usingNamespace))
                    {
                        var ns = new NamespaceSymbol(usingNamespace);
                        AddSymbol(ns);
                    }
                }
            });
        }
    }
}
