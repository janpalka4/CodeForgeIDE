using CodeForgeIDE.CSharp.Lang.CodeCompletion.Symbols;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeForgeIDE.CSharp.Lang.CodeCompletion
{
    /// <summary>
    /// Třída pro analýzu C# kódu pomocí Roslynu.
    /// Poskytuje funkcionalitu pro extrakci symbolů z C# řešení, projektů a souborů.
    /// </summary>
    public class CodeAnalyzer
    {
        private readonly SymbolDatabase _symbolDatabase;
        private MSBuildWorkspace _workspace;
        private Solution _solution;
        private readonly object _lockObject = new object();
        private Dictionary<string, SyntaxTree> _syntaxTrees = new Dictionary<string, SyntaxTree>();
        private Dictionary<string, SemanticModel> _semanticModels = new Dictionary<string, SemanticModel>();

        /// <summary>
        /// Událost vyvolaná po dokončení analýzy
        /// </summary>
        public event EventHandler<AnalysisCompletedEventArgs> AnalysisCompleted;

        /// <summary>
        /// Událost vyvolaná, když došlo ke změně v databázi symbolů
        /// </summary>
        public event EventHandler<SymbolDatabaseChangedEventArgs> SymbolDatabaseChanged;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="symbolDatabase">Databáze symbolů pro ukládání analyzovaných dat</param>
        public CodeAnalyzer(SymbolDatabase symbolDatabase)
        {
            _symbolDatabase = symbolDatabase ?? throw new ArgumentNullException(nameof(symbolDatabase));
        }

        /// <summary>
        /// Inicializuje analyzér
        /// </summary>
        public void Initialize()
        {
            lock (_lockObject)
            {
                _workspace = MSBuildWorkspace.Create();
                _workspace.WorkspaceFailed += (sender, args) =>
                {
                    // Logování chyb workspace
                    Console.WriteLine($"Workspace error: {args.Diagnostic.Message}");
                };
            }
        }

        /// <summary>
        /// Asynchronně analyzuje celé C# řešení
        /// </summary>
        /// <param name="solutionPath">Cesta k souboru řešení (.sln)</param>
        /// <param name="cancellationToken">Token pro zrušení operace</param>
        /// <returns>Úkol představující asynchronní operaci</returns>
        public async Task AnalyzeSolutionAsync(string solutionPath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(solutionPath))
                throw new ArgumentException("Solution path cannot be null or empty", nameof(solutionPath));

            if (!File.Exists(solutionPath))
                throw new FileNotFoundException("Solution file not found", solutionPath);

            try
            {
                // Otevření řešení
                _solution = await _workspace.OpenSolutionAsync(solutionPath, cancellationToken: cancellationToken);
                
                await AnalyzeSolutionAsync(_solution, cancellationToken);
            }
            catch (Exception ex)
            {
                AnalysisCompleted?.Invoke(this, new AnalysisCompletedEventArgs
                {
                    Success = false,
                    Message = $"Error analyzing solution: {ex.Message}",
                    Exception = ex
                });
            }
        }

        /// <summary>
        /// Asynchronně analyzuje celé C# řešení
        /// </summary>
        /// <param name="solution">Solution</param>
        /// <param name="cancellationToken">Token pro zrušení operace</param>
        /// <returns>Úkol představující asynchronní operaci</returns>
        public async Task AnalyzeSolutionAsync(Solution solution, CancellationToken cancellationToken = default)
        {
            try
            {
                // Analyzování všech projektů v řešení
                foreach (var project in solution.Projects)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    await AnalyzeProjectAsync(project, cancellationToken);
                }

                // Oznámení o dokončení analýzy
                AnalysisCompleted?.Invoke(this, new AnalysisCompletedEventArgs
                {
                    Success = true,
                    Message = $"Analysis of solution {Path.GetFileName(solution.FilePath)} completed successfully."
                });
            }
            catch (Exception ex)
            {
                AnalysisCompleted?.Invoke(this, new AnalysisCompletedEventArgs
                {
                    Success = false,
                    Message = $"Error analyzing solution: {ex.Message}",
                    Exception = ex
                });
            }
        }

        /// <summary>
        /// Asynchronně analyzuje C# projekt
        /// </summary>
        /// <param name="projectPath">Cesta k souboru projektu (.csproj)</param>
        /// <param name="cancellationToken">Token pro zrušení operace</param>
        /// <returns>Úkol představující asynchronní operaci</returns>
        public async Task AnalyzeProjectAsync(string projectPath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(projectPath))
                throw new ArgumentException("Project path cannot be null or empty", nameof(projectPath));

            if (!File.Exists(projectPath))
                throw new FileNotFoundException("Project file not found", projectPath);

            try
            {
                // Otevření projektu
                var project = await _workspace.OpenProjectAsync(projectPath, cancellationToken: cancellationToken);
                await AnalyzeProjectAsync(project, cancellationToken);

                // Oznámení o dokončení analýzy
                AnalysisCompleted?.Invoke(this, new AnalysisCompletedEventArgs
                {
                    Success = true,
                    Message = $"Analysis of project {Path.GetFileName(projectPath)} completed successfully."
                });
            }
            catch (Exception ex)
            {
                AnalysisCompleted?.Invoke(this, new AnalysisCompletedEventArgs
                {
                    Success = false,
                    Message = $"Error analyzing project: {ex.Message}",
                    Exception = ex
                });
            }
        }

        /// <summary>
        /// Asynchronně analyzuje instance projektu Roslyn
        /// </summary>
        /// <param name="project">Projekt k analýze</param>
        /// <param name="cancellationToken">Token pro zrušení operace</param>
        /// <returns>Úkol představující asynchronní operaci</returns>
        private async Task AnalyzeProjectAsync(Project project, CancellationToken cancellationToken = default)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            try
            {
                // Získání kompilace pro projekt
                var compilation = await project.GetCompilationAsync(cancellationToken);
                if (compilation == null)
                    return;

                // Analyzování všech syntaktických stromů v projektu
                foreach (var syntaxTree in compilation.SyntaxTrees)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    var filePath = syntaxTree.FilePath;
                    if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                    {
                        // Pro jednotlivé soubory vytvoříme samostatné sémantické modely
                        var semanticModel = compilation.GetSemanticModel(syntaxTree);
                        
                        lock (_lockObject)
                        {
                            _syntaxTrees[filePath] = syntaxTree;
                            _semanticModels[filePath] = semanticModel;
                        }

                        // Analýza souboru
                        await AnalyzeFileAsync(filePath, syntaxTree, semanticModel, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error analyzing project {project.Name}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Asynchronně analyzuje soubor C# 
        /// </summary>
        /// <param name="filePath">Cesta k souboru</param>
        /// <param name="cancellationToken">Token pro zrušení operace</param>
        /// <returns>Úkol představující asynchronní operaci</returns>
        public async Task AnalyzeFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found", filePath);

            try
            {
                // Čtení a parsování souboru
                var sourceText = await File.ReadAllTextAsync(filePath, cancellationToken);
                var syntaxTree = CSharpSyntaxTree.ParseText(sourceText, cancellationToken: cancellationToken);

                // Pro samostatný soubor potřebujeme vytvořit kompilaci a sémantický model
                var compilation = CSharpCompilation.Create(
                    assemblyName: Path.GetFileNameWithoutExtension(filePath),
                    syntaxTrees: new[] { syntaxTree });

                var semanticModel = compilation.GetSemanticModel(syntaxTree);

                lock (_lockObject)
                {
                    _syntaxTrees[filePath] = syntaxTree;
                    _semanticModels[filePath] = semanticModel;
                }

                // Analýza souboru
                await AnalyzeFileAsync(filePath, syntaxTree, semanticModel, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error analyzing file {filePath}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Asynchronně analyzuje C# soubor s použitím existujícího syntaktického stromu a sémantického modelu
        /// </summary>
        /// <param name="filePath">Cesta k souboru</param>
        /// <param name="syntaxTree">Syntaktický strom</param>
        /// <param name="semanticModel">Sémantický model</param>
        /// <param name="cancellationToken">Token pro zrušení operace</param>
        /// <returns>Úkol představující asynchronní operaci</returns>
        private async Task AnalyzeFileAsync(string filePath, SyntaxTree syntaxTree, SemanticModel semanticModel, CancellationToken cancellationToken = default)
        {
            if (syntaxTree == null)
                throw new ArgumentNullException(nameof(syntaxTree));

            if (semanticModel == null)
                throw new ArgumentNullException(nameof(semanticModel));

            await Task.Run(() =>
            {
                try
                {
                    // Získání kořenového uzlu
                    var root = syntaxTree.GetRoot(cancellationToken) as CompilationUnitSyntax;
                    if (root == null)
                        return;

                    // Extrakce using direktiv
                    var usings = root.Usings.Select(u => u.Name.ToString()).ToList();
                    _symbolDatabase.UpdateUsingDirectivesAsync(usings);

                    // Extrakce jmenných prostorů
                    foreach (var namespaceDeclaration in root.DescendantNodes().OfType<NamespaceDeclarationSyntax>())
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        AnalyzeNamespace(namespaceDeclaration, semanticModel);
                    }

                    // Extrakce typů na nejvyšší úrovni (mimo jmenné prostory)
                    foreach (var typeDeclaration in root.DescendantNodes()
                        .Where(n => n.Parent is CompilationUnitSyntax)
                        .OfType<BaseTypeDeclarationSyntax>())
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        AnalyzeTypeDeclaration(typeDeclaration, semanticModel);
                    }

                    // Oznámení o změně databáze symbolů
                    SymbolDatabaseChanged?.Invoke(this, new SymbolDatabaseChangedEventArgs 
                    { 
                        FilePath = filePath 
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error analyzing file content {filePath}: {ex.Message}");
                }
            }, cancellationToken);
        }

        /// <summary>
        /// Analyzuje jmenný prostor a jeho obsah
        /// </summary>
        /// <param name="namespaceDeclaration">Syntaktický uzel jmenného prostoru</param>
        /// <param name="semanticModel">Sémantický model</param>
        private void AnalyzeNamespace(NamespaceDeclarationSyntax namespaceDeclaration, SemanticModel semanticModel)
        {
            if (namespaceDeclaration == null)
                return;

            // Získání jmenného prostoru
            var namespaceName = namespaceDeclaration.Name.ToString();
            var namespaceSymbol = new NamespaceSymbol(namespaceName);
            _symbolDatabase.AddSymbol(namespaceSymbol);

            // Analýza všech typů v jmenném prostoru
            foreach (var typeDeclaration in namespaceDeclaration.DescendantNodes().OfType<BaseTypeDeclarationSyntax>())
            {
                AnalyzeTypeDeclaration(typeDeclaration, semanticModel, namespaceName);
            }
        }

        /// <summary>
        /// Analyzuje deklaraci typu (třída, rozhraní, enum, structure)
        /// </summary>
        /// <param name="typeDeclaration">Syntaktický uzel typu</param>
        /// <param name="semanticModel">Sémantický model</param>
        /// <param name="namespaceName">Název jmenného prostoru (volitelný)</param>
        private void AnalyzeTypeDeclaration(BaseTypeDeclarationSyntax typeDeclaration, SemanticModel semanticModel, string namespaceName = null)
        {
            if (typeDeclaration == null)
                return;

            var typeSymbolInfo = semanticModel.GetDeclaredSymbol(typeDeclaration);
            if (typeSymbolInfo == null)
                return;

            // Získání základních informací o typu
            var typeName = typeDeclaration.Identifier.Text;
            var typeNameFull = !string.IsNullOrEmpty(namespaceName) ? $"{namespaceName}.{typeName}" : typeName;

            // Zpracování podle typu deklarace
            switch (typeDeclaration)
            {
                case ClassDeclarationSyntax classDeclaration:
                    AnalyzeClass(classDeclaration, semanticModel, typeSymbolInfo, namespaceName);
                    break;
                case InterfaceDeclarationSyntax interfaceDeclaration:
                    AnalyzeInterface(interfaceDeclaration, semanticModel, typeSymbolInfo, namespaceName);
                    break;
                case EnumDeclarationSyntax enumDeclaration:
                    AnalyzeEnum(enumDeclaration, semanticModel, typeSymbolInfo, namespaceName);
                    break;
                case StructDeclarationSyntax structDeclaration:
                    AnalyzeStruct(structDeclaration, semanticModel, typeSymbolInfo, namespaceName);
                    break;
            }
        }

        /// <summary>
        /// Analyzuje deklaraci třídy
        /// </summary>
        /// <param name="classDeclaration">Syntaktický uzel třídy</param>
        /// <param name="semanticModel">Sémantický model</param>
        /// <param name="typeSymbol">Symbol třídy</param>
        /// <param name="namespaceName">Název jmenného prostoru</param>
        private void AnalyzeClass(ClassDeclarationSyntax classDeclaration, SemanticModel semanticModel, 
            ISymbol typeSymbol, string namespaceName)
        {
            var className = classDeclaration.Identifier.Text;
            var classSymbol = new ClassSymbol(className)
            {
                Namespace = namespaceName,
                FullyQualifiedName = !string.IsNullOrEmpty(namespaceName) ? $"{namespaceName}.{className}" : className,
                IsPublic = classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)),
                IsAbstract = classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.AbstractKeyword)),
                IsStatic = classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)),
                FilePath = classDeclaration.SyntaxTree.FilePath,
                Line = classDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line,
                Column = classDeclaration.GetLocation().GetLineSpan().StartLinePosition.Character
            };

            // Analýza dědičnosti
            if (classDeclaration.BaseList != null)
            {
                foreach (var baseType in classDeclaration.BaseList.Types)
                {
                    var baseTypeInfo = semanticModel.GetTypeInfo(baseType.Type).Type;
                    if (baseTypeInfo != null)
                    {
                        if (baseTypeInfo.TypeKind == TypeKind.Class)
                        {
                            classSymbol.BaseClass = baseTypeInfo.ToDisplayString();
                        }
                        else if (baseTypeInfo.TypeKind == TypeKind.Interface)
                        {
                            classSymbol.Interfaces.Add(baseTypeInfo.ToDisplayString());
                        }
                    }
                }
            }

            // Analýza členů třídy
            foreach (var member in classDeclaration.Members)
            {
                AnalyzeClassMember(member, semanticModel, classSymbol);
            }

            _symbolDatabase.AddSymbol(classSymbol);
        }

        /// <summary>
        /// Analyzuje člena třídy (metoda, vlastnost, pole, událost)
        /// </summary>
        /// <param name="memberDeclaration">Syntaktický uzel člena</param>
        /// <param name="semanticModel">Sémantický model</param>
        /// <param name="classSymbol">Symbol třídy</param>
        private void AnalyzeClassMember(MemberDeclarationSyntax memberDeclaration, SemanticModel semanticModel, 
            ClassSymbol classSymbol)
        {
            switch (memberDeclaration)
            {
                case MethodDeclarationSyntax methodDeclaration:
                    AnalyzeMethod(methodDeclaration, semanticModel, classSymbol);
                    break;
                case PropertyDeclarationSyntax propertyDeclaration:
                    AnalyzeProperty(propertyDeclaration, semanticModel, classSymbol);
                    break;
                case FieldDeclarationSyntax fieldDeclaration:
                    AnalyzeField(fieldDeclaration, semanticModel, classSymbol);
                    break;
                case EventFieldDeclarationSyntax eventFieldDeclaration:
                    AnalyzeEventField(eventFieldDeclaration, semanticModel, classSymbol);
                    break;
                case EventDeclarationSyntax eventDeclaration:
                    AnalyzeEventDeclaration(eventDeclaration, semanticModel, classSymbol);
                    break;
            }
        }

        /// <summary>
        /// Analyzuje metodu třídy
        /// </summary>
        /// <param name="methodDeclaration">Syntaktický uzel metody</param>
        /// <param name="semanticModel">Sémantický model</param>
        /// <param name="classSymbol">Symbol třídy</param>
        private void AnalyzeMethod(MethodDeclarationSyntax methodDeclaration, SemanticModel semanticModel, 
            ClassSymbol classSymbol)
        {
            var methodName = methodDeclaration.Identifier.Text;
            var methodSymbol = new MethodSymbol(methodName)
            {
                ReturnType = methodDeclaration.ReturnType.ToString(),
                Namespace = classSymbol.Namespace,
                FullyQualifiedName = $"{classSymbol.FullyQualifiedName}.{methodName}",
                IsPublic = methodDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)),
                IsStatic = methodDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)),
                IsAbstract = methodDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.AbstractKeyword)),
                IsVirtual = methodDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.VirtualKeyword)),
                IsOverride = methodDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.OverrideKeyword)),
                IsAsync = methodDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.AsyncKeyword)),
                FilePath = methodDeclaration.SyntaxTree.FilePath,
                Line = methodDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line,
                Column = methodDeclaration.GetLocation().GetLineSpan().StartLinePosition.Character
            };

            // Analýza parametrů metody
            foreach (var parameter in methodDeclaration.ParameterList.Parameters)
            {
                var paramName = parameter.Identifier.Text;
                var paramSymbol = new ParameterSymbol(paramName)
                {
                    Type = parameter.Type.ToString(),
                    IsOptional = parameter.Default != null,
                    DefaultValue = parameter.Default?.Value.ToString(),
                    IsRef = parameter.Modifiers.Any(m => m.IsKind(SyntaxKind.RefKeyword)),
                    IsOut = parameter.Modifiers.Any(m => m.IsKind(SyntaxKind.OutKeyword)),
                    IsIn = parameter.Modifiers.Any(m => m.IsKind(SyntaxKind.InKeyword)),
                    IsParams = parameter.Modifiers.Any(m => m.IsKind(SyntaxKind.ParamsKeyword))
                };

                methodSymbol.Parameters.Add(paramSymbol);
            }

            // Analýza lokálních proměnných v metodě
            var variableDeclarations = methodDeclaration.DescendantNodes().OfType<VariableDeclarationSyntax>();
            foreach (var variableDeclaration in variableDeclarations)
            {
                var type = variableDeclaration.Type.ToString();
                foreach (var variable in variableDeclaration.Variables)
                {
                    var variableName = variable.Identifier.Text;
                    var variableSymbol = new VariableSymbol(variableName)
                    {
                        Type = type,
                        IsReadOnly = variableDeclaration.Parent is LocalDeclarationStatementSyntax localDeclaration &&
                                      localDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.ConstKeyword)),
                        FilePath = methodDeclaration.SyntaxTree.FilePath,
                        Line = variable.GetLocation().GetLineSpan().StartLinePosition.Line,
                        Column = variable.GetLocation().GetLineSpan().StartLinePosition.Character
                    };

                    // Pro jednoduchost přidáme proměnnou do databáze s plně kvalifikovaným názvem
                    variableSymbol.FullyQualifiedName = $"{methodSymbol.FullyQualifiedName}.{variableName}";
                    _symbolDatabase.AddSymbol(variableSymbol);
                }
            }

            classSymbol.Methods.Add(methodSymbol);
            _symbolDatabase.AddSymbol(methodSymbol);
        }

        /// <summary>
        /// Analyzuje vlastnost třídy
        /// </summary>
        /// <param name="propertyDeclaration">Syntaktický uzel vlastnosti</param>
        /// <param name="semanticModel">Sémantický model</param>
        /// <param name="classSymbol">Symbol třídy</param>
        private void AnalyzeProperty(PropertyDeclarationSyntax propertyDeclaration, SemanticModel semanticModel, 
            ClassSymbol classSymbol)
        {
            var propertyName = propertyDeclaration.Identifier.Text;
            var propertySymbol = new PropertySymbol(propertyName)
            {
                Type = propertyDeclaration.Type.ToString(),
                Namespace = classSymbol.Namespace,
                FullyQualifiedName = $"{classSymbol.FullyQualifiedName}.{propertyName}",
                IsPublic = propertyDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)),
                IsStatic = propertyDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)),
                IsAbstract = propertyDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.AbstractKeyword)),
                IsVirtual = propertyDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.VirtualKeyword)),
                IsOverride = propertyDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.OverrideKeyword)),
                HasGetter = propertyDeclaration.AccessorList?.Accessors.Any(a => a.IsKind(SyntaxKind.GetAccessorDeclaration)) ?? false,
                HasSetter = propertyDeclaration.AccessorList?.Accessors.Any(a => a.IsKind(SyntaxKind.SetAccessorDeclaration)) ?? false,
                FilePath = propertyDeclaration.SyntaxTree.FilePath,
                Line = propertyDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line,
                Column = propertyDeclaration.GetLocation().GetLineSpan().StartLinePosition.Character
            };

            classSymbol.Properties.Add(propertySymbol);
            _symbolDatabase.AddSymbol(propertySymbol);
        }

        /// <summary>
        /// Analyzuje pole třídy
        /// </summary>
        /// <param name="fieldDeclaration">Syntaktický uzel pole</param>
        /// <param name="semanticModel">Sémantický model</param>
        /// <param name="classSymbol">Symbol třídy</param>
        private void AnalyzeField(FieldDeclarationSyntax fieldDeclaration, SemanticModel semanticModel, 
            ClassSymbol classSymbol)
        {
            var type = fieldDeclaration.Declaration.Type.ToString();
            var isStatic = fieldDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword));
            var isReadOnly = fieldDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.ReadOnlyKeyword));
            var isConstant = fieldDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.ConstKeyword));
            var isPublic = fieldDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword));

            foreach (var variable in fieldDeclaration.Declaration.Variables)
            {
                var fieldName = variable.Identifier.Text;
                var fieldSymbol = new FieldSymbol(fieldName)
                {
                    Type = type,
                    Namespace = classSymbol.Namespace,
                    FullyQualifiedName = $"{classSymbol.FullyQualifiedName}.{fieldName}",
                    IsPublic = isPublic,
                    IsStatic = isStatic,
                    IsReadOnly = isReadOnly,
                    IsConstant = isConstant,
                    FilePath = fieldDeclaration.SyntaxTree.FilePath,
                    Line = variable.GetLocation().GetLineSpan().StartLinePosition.Line,
                    Column = variable.GetLocation().GetLineSpan().StartLinePosition.Character
                };

                classSymbol.Fields.Add(fieldSymbol);
                _symbolDatabase.AddSymbol(fieldSymbol);
            }
        }

        /// <summary>
        /// Analyzuje pole události
        /// </summary>
        /// <param name="eventFieldDeclaration">Syntaktický uzel pole události</param>
        /// <param name="semanticModel">Sémantický model</param>
        /// <param name="classSymbol">Symbol třídy</param>
        private void AnalyzeEventField(EventFieldDeclarationSyntax eventFieldDeclaration, SemanticModel semanticModel, 
            ClassSymbol classSymbol)
        {
            var delegateType = eventFieldDeclaration.Declaration.Type.ToString();
            var isStatic = eventFieldDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword));
            var isVirtual = eventFieldDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.VirtualKeyword));
            var isAbstract = eventFieldDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.AbstractKeyword));
            var isOverride = eventFieldDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.OverrideKeyword));
            var isPublic = eventFieldDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword));

            foreach (var variable in eventFieldDeclaration.Declaration.Variables)
            {
                var eventName = variable.Identifier.Text;
                var eventSymbol = new EventSymbol(eventName)
                {
                    DelegateType = delegateType,
                    Namespace = classSymbol.Namespace,
                    FullyQualifiedName = $"{classSymbol.FullyQualifiedName}.{eventName}",
                    IsPublic = isPublic,
                    IsStatic = isStatic,
                    IsVirtual = isVirtual,
                    IsAbstract = isAbstract,
                    IsOverride = isOverride,
                    FilePath = eventFieldDeclaration.SyntaxTree.FilePath,
                    Line = variable.GetLocation().GetLineSpan().StartLinePosition.Line,
                    Column = variable.GetLocation().GetLineSpan().StartLinePosition.Character
                };

                classSymbol.Events.Add(eventSymbol);
                _symbolDatabase.AddSymbol(eventSymbol);
            }
        }

        /// <summary>
        /// Analyzuje deklaraci události
        /// </summary>
        /// <param name="eventDeclaration">Syntaktický uzel deklarace události</param>
        /// <param name="semanticModel">Sémantický model</param>
        /// <param name="classSymbol">Symbol třídy</param>
        private void AnalyzeEventDeclaration(EventDeclarationSyntax eventDeclaration, SemanticModel semanticModel, 
            ClassSymbol classSymbol)
        {
            var eventName = eventDeclaration.Identifier.Text;
            var eventSymbol = new EventSymbol(eventName)
            {
                DelegateType = eventDeclaration.Type.ToString(),
                Namespace = classSymbol.Namespace,
                FullyQualifiedName = $"{classSymbol.FullyQualifiedName}.{eventName}",
                IsPublic = eventDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)),
                IsStatic = eventDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)),
                IsVirtual = eventDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.VirtualKeyword)),
                IsAbstract = eventDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.AbstractKeyword)),
                IsOverride = eventDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.OverrideKeyword)),
                FilePath = eventDeclaration.SyntaxTree.FilePath,
                Line = eventDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line,
                Column = eventDeclaration.GetLocation().GetLineSpan().StartLinePosition.Character
            };

            classSymbol.Events.Add(eventSymbol);
            _symbolDatabase.AddSymbol(eventSymbol);
        }

        /// <summary>
        /// Analyzuje deklaraci rozhraní
        /// </summary>
        /// <param name="interfaceDeclaration">Syntaktický uzel rozhraní</param>
        /// <param name="semanticModel">Sémantický model</param>
        /// <param name="typeSymbol">Symbol rozhraní</param>
        /// <param name="namespaceName">Název jmenného prostoru</param>
        private void AnalyzeInterface(InterfaceDeclarationSyntax interfaceDeclaration, SemanticModel semanticModel, 
            ISymbol typeSymbol, string namespaceName)
        {
            var interfaceName = interfaceDeclaration.Identifier.Text;
            var interfaceSymbol = new InterfaceSymbol(interfaceName)
            {
                Namespace = namespaceName,
                FullyQualifiedName = !string.IsNullOrEmpty(namespaceName) ? $"{namespaceName}.{interfaceName}" : interfaceName,
                IsPublic = interfaceDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)),
                FilePath = interfaceDeclaration.SyntaxTree.FilePath,
                Line = interfaceDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line,
                Column = interfaceDeclaration.GetLocation().GetLineSpan().StartLinePosition.Character
            };

            // Analýza dědičnosti rozhraní
            if (interfaceDeclaration.BaseList != null)
            {
                foreach (var baseType in interfaceDeclaration.BaseList.Types)
                {
                    var baseTypeInfo = semanticModel.GetTypeInfo(baseType.Type).Type;
                    if (baseTypeInfo != null && baseTypeInfo.TypeKind == TypeKind.Interface)
                    {
                        interfaceSymbol.BaseInterfaces.Add(baseTypeInfo.ToDisplayString());
                    }
                }
            }

            // Analýza členů rozhraní
            foreach (var member in interfaceDeclaration.Members)
            {
                switch (member)
                {
                    case MethodDeclarationSyntax methodDeclaration:
                        var methodName = methodDeclaration.Identifier.Text;
                        var methodSymbol = new MethodSymbol(methodName)
                        {
                            ReturnType = methodDeclaration.ReturnType.ToString(),
                            Namespace = namespaceName,
                            FullyQualifiedName = $"{interfaceSymbol.FullyQualifiedName}.{methodName}",
                            IsPublic = true, // Všechny členy rozhraní jsou implicitně veřejné
                            FilePath = methodDeclaration.SyntaxTree.FilePath,
                            Line = methodDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line,
                            Column = methodDeclaration.GetLocation().GetLineSpan().StartLinePosition.Character
                        };

                        // Analýza parametrů metody
                        foreach (var parameter in methodDeclaration.ParameterList.Parameters)
                        {
                            var paramName = parameter.Identifier.Text;
                            var paramSymbol = new ParameterSymbol(paramName)
                            {
                                Type = parameter.Type.ToString(),
                                IsOptional = parameter.Default != null,
                                DefaultValue = parameter.Default?.Value.ToString(),
                                IsRef = parameter.Modifiers.Any(m => m.IsKind(SyntaxKind.RefKeyword)),
                                IsOut = parameter.Modifiers.Any(m => m.IsKind(SyntaxKind.OutKeyword)),
                                IsIn = parameter.Modifiers.Any(m => m.IsKind(SyntaxKind.InKeyword)),
                                IsParams = parameter.Modifiers.Any(m => m.IsKind(SyntaxKind.ParamsKeyword))
                            };

                            methodSymbol.Parameters.Add(paramSymbol);
                        }

                        interfaceSymbol.Methods.Add(methodSymbol);
                        _symbolDatabase.AddSymbol(methodSymbol);
                        break;

                    case PropertyDeclarationSyntax propertyDeclaration:
                        var propertyName = propertyDeclaration.Identifier.Text;
                        var propertySymbol = new PropertySymbol(propertyName)
                        {
                            Type = propertyDeclaration.Type.ToString(),
                            Namespace = namespaceName,
                            FullyQualifiedName = $"{interfaceSymbol.FullyQualifiedName}.{propertyName}",
                            IsPublic = true, // Všechny členy rozhraní jsou implicitně veřejné
                            HasGetter = propertyDeclaration.AccessorList.Accessors.Any(a => a.IsKind(SyntaxKind.GetAccessorDeclaration)),
                            HasSetter = propertyDeclaration.AccessorList.Accessors.Any(a => a.IsKind(SyntaxKind.SetAccessorDeclaration)),
                            FilePath = propertyDeclaration.SyntaxTree.FilePath,
                            Line = propertyDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line,
                            Column = propertyDeclaration.GetLocation().GetLineSpan().StartLinePosition.Character
                        };

                        interfaceSymbol.Properties.Add(propertySymbol);
                        _symbolDatabase.AddSymbol(propertySymbol);
                        break;

                    case EventDeclarationSyntax eventDeclaration:
                        var eventName = eventDeclaration.Identifier.Text;
                        var eventSymbol = new EventSymbol(eventName)
                        {
                            DelegateType = eventDeclaration.Type.ToString(),
                            Namespace = namespaceName,
                            FullyQualifiedName = $"{interfaceSymbol.FullyQualifiedName}.{eventName}",
                            IsPublic = true, // Všechny členy rozhraní jsou implicitně veřejné
                            FilePath = eventDeclaration.SyntaxTree.FilePath,
                            Line = eventDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line,
                            Column = eventDeclaration.GetLocation().GetLineSpan().StartLinePosition.Character
                        };

                        interfaceSymbol.Events.Add(eventSymbol);
                        _symbolDatabase.AddSymbol(eventSymbol);
                        break;
                }
            }

            _symbolDatabase.AddSymbol(interfaceSymbol);
        }

        /// <summary>
        /// Analyzuje deklaraci enum typu
        /// </summary>
        /// <param name="enumDeclaration">Syntaktický uzel enum</param>
        /// <param name="semanticModel">Sémantický model</param>
        /// <param name="typeSymbol">Symbol enum</param>
        /// <param name="namespaceName">Název jmenného prostoru</param>
        private void AnalyzeEnum(EnumDeclarationSyntax enumDeclaration, SemanticModel semanticModel, 
            ISymbol typeSymbol, string namespaceName)
        {
            var enumName = enumDeclaration.Identifier.Text;
            var enumSymbol = new EnumSymbol(enumName)
            {
                Namespace = namespaceName,
                FullyQualifiedName = !string.IsNullOrEmpty(namespaceName) ? $"{namespaceName}.{enumName}" : enumName,
                IsPublic = enumDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)),
                FilePath = enumDeclaration.SyntaxTree.FilePath,
                Line = enumDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line,
                Column = enumDeclaration.GetLocation().GetLineSpan().StartLinePosition.Character
            };

            // Získání podkladového typu enumu
            if (enumDeclaration.BaseList != null)
            {
                enumSymbol.UnderlyingType = enumDeclaration.BaseList.Types.FirstOrDefault()?.Type.ToString() ?? "int";
            }

            // Analýza členů enum
            foreach (var member in enumDeclaration.Members)
            {
                var memberName = member.Identifier.Text;
                var memberSymbol = new EnumMemberSymbol(memberName)
                {
                    Namespace = namespaceName,
                    FullyQualifiedName = $"{enumSymbol.FullyQualifiedName}.{memberName}",
                    IsPublic = true, // Členy enum jsou vždy veřejné
                    Value = member.EqualsValue?.Value.ToString(),
                    FilePath = enumDeclaration.SyntaxTree.FilePath,
                    Line = member.GetLocation().GetLineSpan().StartLinePosition.Line,
                    Column = member.GetLocation().GetLineSpan().StartLinePosition.Character
                };

                enumSymbol.Members.Add(memberSymbol);
                _symbolDatabase.AddSymbol(memberSymbol);
            }

            _symbolDatabase.AddSymbol(enumSymbol);
        }

        /// <summary>
        /// Analyzuje deklaraci struktury
        /// </summary>
        /// <param name="structDeclaration">Syntaktický uzel struktury</param>
        /// <param name="semanticModel">Sémantický model</param>
        /// <param name="typeSymbol">Symbol struktury</param>
        /// <param name="namespaceName">Název jmenného prostoru</param>
        private void AnalyzeStruct(StructDeclarationSyntax structDeclaration, SemanticModel semanticModel, 
            ISymbol typeSymbol, string namespaceName)
        {
            var structName = structDeclaration.Identifier.Text;
            var structSymbol = new StructSymbol(structName)
            {
                Namespace = namespaceName,
                FullyQualifiedName = !string.IsNullOrEmpty(namespaceName) ? $"{namespaceName}.{structName}" : structName,
                IsPublic = structDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)),
                IsReadOnly = structDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.ReadOnlyKeyword)),
                FilePath = structDeclaration.SyntaxTree.FilePath,
                Line = structDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line,
                Column = structDeclaration.GetLocation().GetLineSpan().StartLinePosition.Character
            };

            // Analýza implementovaných rozhraní
            if (structDeclaration.BaseList != null)
            {
                foreach (var baseType in structDeclaration.BaseList.Types)
                {
                    var baseTypeInfo = semanticModel.GetTypeInfo(baseType.Type).Type;
                    if (baseTypeInfo != null && baseTypeInfo.TypeKind == TypeKind.Interface)
                    {
                        structSymbol.Interfaces.Add(baseTypeInfo.ToDisplayString());
                    }
                }
            }

            // Analýza členů struktury (podobná analýze jako u třídy)
            foreach (var member in structDeclaration.Members)
            {
                switch (member)
                {
                    case MethodDeclarationSyntax methodDeclaration:
                        AnalyzeMethod(methodDeclaration, semanticModel, new ClassSymbol(structName)
                        {
                            Namespace = namespaceName,
                            FullyQualifiedName = structSymbol.FullyQualifiedName
                        });
                        break;

                    case PropertyDeclarationSyntax propertyDeclaration:
                        var propertyName = propertyDeclaration.Identifier.Text;
                        var propertySymbol = new PropertySymbol(propertyName)
                        {
                            Type = propertyDeclaration.Type.ToString(),
                            Namespace = namespaceName,
                            FullyQualifiedName = $"{structSymbol.FullyQualifiedName}.{propertyName}",
                            IsPublic = propertyDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)),
                            IsStatic = propertyDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)),
                            HasGetter = propertyDeclaration.AccessorList?.Accessors.Any(a => a.IsKind(SyntaxKind.GetAccessorDeclaration)) ?? false,
                            HasSetter = propertyDeclaration.AccessorList?.Accessors.Any(a => a.IsKind(SyntaxKind.SetAccessorDeclaration)) ?? false,
                            FilePath = propertyDeclaration.SyntaxTree.FilePath,
                            Line = propertyDeclaration.GetLocation().GetLineSpan().StartLinePosition.Line,
                            Column = propertyDeclaration.GetLocation().GetLineSpan().StartLinePosition.Character
                        };

                        structSymbol.Properties.Add(propertySymbol);
                        _symbolDatabase.AddSymbol(propertySymbol);
                        break;

                    case FieldDeclarationSyntax fieldDeclaration:
                        var type = fieldDeclaration.Declaration.Type.ToString();
                        var isStatic = fieldDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword));
                        var isReadOnly = fieldDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.ReadOnlyKeyword));
                        var isConstant = fieldDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.ConstKeyword));
                        var isPublic = fieldDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword));

                        foreach (var variable in fieldDeclaration.Declaration.Variables)
                        {
                            var fieldName = variable.Identifier.Text;
                            var fieldSymbol = new FieldSymbol(fieldName)
                            {
                                Type = type,
                                Namespace = namespaceName,
                                FullyQualifiedName = $"{structSymbol.FullyQualifiedName}.{fieldName}",
                                IsPublic = isPublic,
                                IsStatic = isStatic,
                                IsReadOnly = isReadOnly,
                                IsConstant = isConstant,
                                FilePath = fieldDeclaration.SyntaxTree.FilePath,
                                Line = variable.GetLocation().GetLineSpan().StartLinePosition.Line,
                                Column = variable.GetLocation().GetLineSpan().StartLinePosition.Character
                            };

                            structSymbol.Fields.Add(fieldSymbol);
                            _symbolDatabase.AddSymbol(fieldSymbol);
                        }
                        break;
                }
            }

            _symbolDatabase.AddSymbol(structSymbol);
        }

        /// <summary>
        /// Analyzuje lokální proměnné a parametry pro code completion na určité pozici
        /// </summary>
        /// <param name="filePath">Cesta k souboru</param>
        /// <returns>Seznam nalezených lokálních symbolů</returns>
        public async Task<IEnumerable<CompletionSymbol>> AnalyzeLocalScope(string filePath, int caretPosition)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return Enumerable.Empty<CompletionSymbol>();

            return await Task.Run(() =>
            {
                try
                {
                    SyntaxTree syntaxTree;
                    SemanticModel semanticModel;

                    lock (_lockObject)
                    {
                        // Pokud již máme syntaktický strom a sémantický model pro tento soubor, použijeme ho
                        if (!_syntaxTrees.TryGetValue(filePath, out syntaxTree) || 
                            !_semanticModels.TryGetValue(filePath, out semanticModel))
                        {
                            // Jinak vytvoříme nové
                            var sourceText = File.ReadAllText(filePath);
                            syntaxTree = CSharpSyntaxTree.ParseText(sourceText);
                            var compilation = CSharpCompilation.Create(
                                assemblyName: Path.GetFileNameWithoutExtension(filePath),
                                syntaxTrees: new[] { syntaxTree });
                            semanticModel = compilation.GetSemanticModel(syntaxTree);

                            _syntaxTrees[filePath] = syntaxTree;
                            _semanticModels[filePath] = semanticModel;
                        }
                    }

                    // Vytvoření pozice
                    var position = syntaxTree.GetRoot().FindToken(caretPosition).SpanStart;

                    // Seznam všech lokálních proměnných a parametrů
                    var localSymbols = new List<CompletionSymbol>();

                    // Najdeme nejbližší rodičovský uzel, který může obsahovat lokální proměnné
                    var root = syntaxTree.GetRoot();
                    var currentNode = root.FindNode(new Microsoft.CodeAnalysis.Text.TextSpan(position, 0));

                    // Iterujeme nahoru přes rodičovské uzly
                    while (currentNode != null)
                    {
                        // Pokud jsme našli metodu, vlastnost nebo konstruktor
                        if (currentNode is MethodDeclarationSyntax methodDecl)
                        {
                            // Přidáme parametry metody
                            foreach (var param in methodDecl.ParameterList.Parameters)
                            {
                                var paramName = param.Identifier.Text;
                                var paramSymbol = new ParameterSymbol(paramName)
                                {
                                    Type = param.Type.ToString(),
                                    IsOptional = param.Default != null,
                                    DefaultValue = param.Default?.Value.ToString(),
                                    IsRef = param.Modifiers.Any(m => m.IsKind(SyntaxKind.RefKeyword)),
                                    IsOut = param.Modifiers.Any(m => m.IsKind(SyntaxKind.OutKeyword)),
                                    IsIn = param.Modifiers.Any(m => m.IsKind(SyntaxKind.InKeyword)),
                                    IsParams = param.Modifiers.Any(m => m.IsKind(SyntaxKind.ParamsKeyword))
                                };
                                localSymbols.Add(paramSymbol);
                            }

                            // Najdeme a přidáme lokální proměnné
                            var localVariables = methodDecl.DescendantNodes()
                                .OfType<VariableDeclarationSyntax>()
                                .Where(v => v.SpanStart < position); // Jen proměnné deklarované před aktuální pozicí

                            foreach (var variable in localVariables.SelectMany(v => v.Variables))
                            {
                                var variableName = variable.Identifier.Text;
                                var variableDeclaration = variable.Parent as VariableDeclarationSyntax;
                                if (variableDeclaration != null)
                                {
                                    var variableSymbol = new VariableSymbol(variableName)
                                    {
                                        Type = variableDeclaration.Type.ToString(),
                                        IsReadOnly = variableDeclaration.Parent is LocalDeclarationStatementSyntax localDecl &&
                                                    localDecl.Modifiers.Any(m => m.IsKind(SyntaxKind.ConstKeyword))
                                    };
                                    localSymbols.Add(variableSymbol);
                                }
                            }

                            // Získáme třídu, ve které je metoda definována
                            var classNode = methodDecl.Parent as ClassDeclarationSyntax;
                            if (classNode != null)
                            {
                                var className = classNode.Identifier.Text;
                                
                                // Přidáme private a protected členy třídy
                                var privateMembers = classNode.Members
                                    .Where(m => m.Modifiers.Any(mod => mod.IsKind(SyntaxKind.PrivateKeyword) || 
                                                                      mod.IsKind(SyntaxKind.ProtectedKeyword)));

                                foreach (var member in privateMembers)
                                {
                                    if (member is FieldDeclarationSyntax fieldDecl)
                                    {
                                        var type = fieldDecl.Declaration.Type.ToString();
                                        var isStatic = fieldDecl.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword));
                                        var isReadOnly = fieldDecl.Modifiers.Any(m => m.IsKind(SyntaxKind.ReadOnlyKeyword));
                                        var isConstant = fieldDecl.Modifiers.Any(m => m.IsKind(SyntaxKind.ConstKeyword));

                                        foreach (var variable in fieldDecl.Declaration.Variables)
                                        {
                                            var fieldName = variable.Identifier.Text;
                                            var fieldSymbol = new FieldSymbol(fieldName)
                                            {
                                                Type = type,
                                                IsStatic = isStatic,
                                                IsReadOnly = isReadOnly,
                                                IsConstant = isConstant,
                                                IsPublic = false
                                            };
                                            localSymbols.Add(fieldSymbol);
                                        }
                                    }
                                    else if (member is PropertyDeclarationSyntax propertyDecl)
                                    {
                                        var propertyName = propertyDecl.Identifier.Text;
                                        var propertySymbol = new PropertySymbol(propertyName)
                                        {
                                            Type = propertyDecl.Type.ToString(),
                                            IsStatic = propertyDecl.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword)),
                                            IsPublic = false,
                                            HasGetter = propertyDecl.AccessorList?.Accessors.Any(a => a.IsKind(SyntaxKind.GetAccessorDeclaration)) ?? false,
                                            HasSetter = propertyDecl.AccessorList?.Accessors.Any(a => a.IsKind(SyntaxKind.SetAccessorDeclaration)) ?? false
                                        };
                                        localSymbols.Add(propertySymbol);
                                    }
                                }
                                
                                // Přidáme i this jako speciální symbol
                                var thisSymbol = new VariableSymbol("this")
                                {
                                    Type = className,
                                    IsReadOnly = true
                                };
                                localSymbols.Add(thisSymbol);
                            }

                            break;
                        }
                        else if (currentNode is PropertyDeclarationSyntax || 
                                 currentNode is ConstructorDeclarationSyntax)
                        {
                            // Podobná logika jako pro metody by mohla být implementována zde
                            break;
                        }

                        currentNode = currentNode.Parent;
                    }

                    return localSymbols;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error analyzing local scope in {filePath}: {ex.Message}");
                    return Enumerable.Empty<CompletionSymbol>();
                }
            });
        }

        /// <summary>
        /// Asynchronně aktualizuje databázi symbolů po změně souboru
        /// </summary>
        /// <param name="filePath">Cesta k souboru, který byl změněn</param>
        /// <param name="cancellationToken">Token pro zrušení operace</param>
        /// <returns>Úkol představující asynchronní operaci</returns>
        public async Task UpdateFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return;

            try
            {
                // Odstraníme staré symboly ze souboru
                RemoveFileSymbols(filePath);

                // Znovu analyzujeme soubor
                await AnalyzeFileAsync(filePath, cancellationToken);

                // Oznámení o změně databáze symbolů
                SymbolDatabaseChanged?.Invoke(this, new SymbolDatabaseChangedEventArgs 
                { 
                    FilePath = filePath 
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating file {filePath}: {ex.Message}");
            }
        }

        /// <summary>
        /// Odstraní všechny symboly související s daným souborem
        /// </summary>
        /// <param name="filePath">Cesta k souboru</param>
        private void RemoveFileSymbols(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            lock (_lockObject)
            {
                // Najdeme všechny symboly spojené s daným souborem
                var symbolsToRemove = _symbolDatabase.AllSymbols
                    .Where(s => s.FilePath == filePath)
                    .ToList();

                // Odstraníme je z databáze
                foreach (var symbol in symbolsToRemove)
                {
                    _symbolDatabase.RemoveSymbol(symbol);
                }

                // Odstraníme syntaktický strom a sémantický model
                _syntaxTrees.Remove(filePath);
                _semanticModels.Remove(filePath);
            }
        }

        /// <summary>
        /// Uvolní zdroje použité analyzérem
        /// </summary>
        public void Dispose()
        {
            _workspace?.Dispose();
        }
    }

    /// <summary>
    /// Argumenty události dokončení analýzy
    /// </summary>
    public class AnalysisCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Určuje, zda byla analýza úspěšná
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Zpráva o výsledku analýzy
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Výjimka, pokud došlo k chybě
        /// </summary>
        public Exception Exception { get; set; }
    }

    /// <summary>
    /// Argumenty události změny databáze symbolů
    /// </summary>
    public class SymbolDatabaseChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Cesta k souboru, který způsobil změnu
        /// </summary>
        public string FilePath { get; set; }
    }
}
