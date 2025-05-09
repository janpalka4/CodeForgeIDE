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

namespace CodeForgeIDE.CSharp.Lang
{
    internal class CSharpSyntaxHighlighter : ISyntaxHighlighter
    {
        private List<DynamicHighlightRule> _rules = new List<DynamicHighlightRule>();

        public void Initialize(TextEditor textEditor)
        {
            var textMateInstalation = textEditor.InstallTextMate(new RegistryOptions(ThemeName.Dark));
            textMateInstalation.SetGrammar("source.cs");

            textEditor.TextArea.TextView.LineTransformers.Add(new DynamicHighlightTransformer(_rules));
            Update(textEditor.Text);
        }

        public bool ShouldBeUsed(string path)
        {
            return path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase);
        }

        public void Update(string text)
        {
            _rules.Clear();

            var syntaxTree = CSharpSyntaxTree.ParseText(text);
            var root = syntaxTree.GetRoot();

            var methodCalls = root.DescendantNodes()
                .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.InvocationExpressionSyntax>()
                .Select(x => x.ToString())
                .ToList();

            //Methods from calls
            Regex methodNameRegex = new Regex(/*@"(?:\w+\.)?(\w+)\("*/@"(?<!"")(?:\w+\.)?(\w+)\((?![^""]*""(?:[^""]*""[^""]*"")*[^""]*$)");
            /*foreach (var methodCall in methodCalls)
            {
                var match = methodNameRegex.Match(methodCall);
                if (match.Success)
                {
                    var methodName = match.Groups[1].Value;
                    var rule = new DynamicHighlightRule
                    {
                        Regex = new Regex($@"({methodName})[(]"),
                        Brush = Brushes.Yellow
                    };
                    _rules.Add(rule);
                }
                //await Task.Delay(1);
            }

            //Methods from definitions
            var methodDefinitions = root.DescendantNodes()
            .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax>();

            foreach (var method in methodDefinitions)
            {
                var methodName = method.Identifier.Text;
                var rule = new DynamicHighlightRule
                {
                    Regex = new Regex($@"({methodName})[(]"),
                    Brush = Brushes.Yellow
                };
                _rules.Add(rule);
            }*/
            _rules.Add(new DynamicHighlightRule
            {
                Regex = methodNameRegex,
                Brush = Brushes.Yellow
            });
        }
    }
}
