using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using CodeForgeIDE.Core;
using CodeForgeIDE.Core.Lang;
using CodeForgeIDE.Core.Plugins;
using CodeForgeIDE.CSharp.Workspace;
using Microsoft.CodeAnalysis.CSharp;
using System.Text.RegularExpressions;
using TextMateSharp.Grammars;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.CodeAnalysis;
using CodeForgeIDE.Core.Util;
using Avalonia.Styling;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeForgeIDE.CSharp.Lang
{
    internal class CSharpSyntaxHighlighter : ISyntaxHighlighter
    {

        private List<CustomHighlightRule> _rules = new List<CustomHighlightRule>();
        private CancellationTokenSource _cancellationTokenSource;
        private TextEditor _textEditor;
        private Document document;

        public void Initialize(TextEditor textEditor, string path)
        {
            _textEditor = textEditor;
            //var textMateInstalation = textEditor.InstallTextMate(new RegistryOptions(ThemeName.Dark));
            //textMateInstalation.SetGrammar("source.cs");

            document = ((CSharpWorkspace)IDE.Editor.Workspace).GetDocument(path);

            textEditor.TextArea.TextView.LineTransformers.Add(new CustomHighlightTransformer(_rules));
            Update(textEditor.Text);
        }

        public bool ShouldBeUsed(string path)
        {
            bool ret = path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase);

            return ret;
        }

        public async void Update(string text)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            var token = _cancellationTokenSource.Token;

            /*Dictionary<SyntaxKind,CSharpThemeStyle> themeStyles = new Dictionary<SyntaxKind, CSharpThemeStyle>();
            foreach (var item in Enum.GetValues<SyntaxKind>())
            {
                if(item.ToString().Contains("Keyword"))
                    themeStyles.Add(item, new CSharpThemeStyle { Color = "#0073ff" });
                else if (item.ToString().Contains("Token"))
                    themeStyles.Add(item, new CSharpThemeStyle { Color = "#c9c9c9" });
                else 
                    themeStyles.Add(item, new CSharpThemeStyle { Color = "#FFFFFF" });
            }

            string json = JsonConvert.SerializeObject(themeStyles, Formatting.Indented);*/

            /* Task.Run(() =>
             {*/
            if (token.IsCancellationRequested)
                return;

            List<CustomHighlightRule> newRules = new List<CustomHighlightRule>();
            Regex methodNameRegex = new Regex(@"(?:\w+\.)?(\w+)\("/*@"(?<!""(?:\w+\.)?(\w+)\((?![^""]*""(?:[^""]*""[^""]*"")*[^""]*$))"*/);

            //var syntaxTree = CSharpSyntaxTree.ParseText(text);
            var syntaxTree = await document.GetSyntaxTreeAsync();
            var root = syntaxTree.GetRoot();
            //var compilation = CSharpCompilation.Create("SemanticHighlighting", new[] { syntaxTree });
            var compilation = await document.Project.GetCompilationAsync();
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var tokens = root.DescendantTokens().ToList();
            var comments = tokens.SelectMany(x => x.LeadingTrivia.Concat(x.TrailingTrivia)).ToList();

            /*Dictionary<SymbolKind,CSharpThemeStyle> themeStyles = new Dictionary<SymbolKind, CSharpThemeStyle>();
            foreach (var item in Enum.GetValues<SymbolKind>())
            {
                  themeStyles.Add(item, new CSharpThemeStyle { Color = "#FFFFFF" });
            }
            string json = JsonConvert.SerializeObject(themeStyles, Formatting.Indented);*/


            foreach (var _token in tokens)
            {
                ISymbol? symbol = _token.Parent is not null ? (semanticModel.GetSymbolInfo(_token.Parent).Symbol ?? semanticModel.GetDeclaredSymbol(_token.Parent)) : null;

                SymbolKind? symbolKind = symbol?.Kind;
                if (symbol is IMethodSymbol methodSymbol)
                {
                    if (methodSymbol.MethodKind == MethodKind.Constructor)
                    {
                        symbolKind = SymbolKind.NamedType;
                    }
                }

                if (symbol is not null && CSharpThemeManager.SemanticStyles.TryGetValue(symbolKind.Value, out var semanticStyle) && _token.IsKind(SyntaxKind.IdentifierToken))
                {
                    var rule = new CustomHighlightRule
                    {
                        Start = _token.Span.Start,
                        Length = _token.Span.Length,
                        Brush = new SolidColorBrush(Util.GetColorFromHex(semanticStyle.Color))
                    };
                    newRules.Add(rule);
                }
                else 
            if (CSharpThemeManager.SyntaxStyles.TryGetValue(_token.Kind(), out var style))
                {
                    var rule = new CustomHighlightRule
                    {
                        Start = _token.Span.Start,
                        Length = _token.Span.Length,
                        Brush = new SolidColorBrush(Util.GetColorFromHex(style.Color))
                    };
                    newRules.Add(rule);
                }
            }

            foreach (var _token in comments)
            {
                if (CSharpThemeManager.SyntaxStyles.TryGetValue(_token.Kind(), out var style))
                {
                    var rule = new CustomHighlightRule
                    {
                        Start = _token.Span.Start,
                        Length = _token.Span.Length,
                        Brush = new SolidColorBrush(Util.GetColorFromHex(style.Color))
                    };
                    newRules.Add(rule);
                }
            }

            if (!token.IsCancellationRequested)
            {
                _rules.Clear();
                _rules.AddRange(newRules);

                /* Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                 {*/
                _textEditor.TextArea.TextView.Redraw();
                //});
                //_textEditor.TextArea.TextView.Redraw();
            }
            /* }, token);*/
        }
    }

    public class CSharpThemeStyle
    {
        public string Color { get; set; }
    }
}
