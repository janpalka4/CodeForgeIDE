using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using CodeForgeIDE.Core;
using CodeForgeIDE.Core.Lang;
using CodeForgeIDE.Core.Plugins;
using CodeForgeIDE.Core.Util;
using CodeForgeIDE.CSharp.Workspace;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace CodeForgeIDE.CSharp.Lang.DocumentTransformers
{
    /// <summary>  
    /// The CSharpDocumentTransformer class is responsible for syntax highlighting in C# documents.  
    /// It uses the Roslyn API to analyze the syntax and semantics of the source code,  
    /// grouping tokens and comments by line and applying appropriate colors based on predefined styles.  
    /// This class integrates with AvaloniaEdit to provide a rich text editing experience.  
    /// </summary>
    public class CSharpDocumentTransformer : SyntaxHighlightTransformer
    {
        // The Roslyn Document representing the source code to be transformed  
        private Document document { get; set; }

        // The SemanticModel provides semantic information about the source code  
        private SemanticModel? semanticModel { get; set; }

        // Dictionary to store syntax tokens grouped by their line numbers  
        private Dictionary<int, List<SyntaxToken>> _tokensByLine = new();

        // Dictionary to store syntax trivia (e.g., comments) grouped by their line numbers  
        private Dictionary<int, List<SyntaxTrivia>> _commentsByLine = new();

        // Constructor to initialize the transformer with a Roslyn Document  
        public CSharpDocumentTransformer(string path) : base(path)
        {
            this.document = IDE.Editor.GetWorkspaceAs<CSharpWorkspace>().GetDocument(path)!;
        }

        // Main method to colorize the document by analyzing its syntax and semantics  
        protected override void Colorize(ITextRunConstructionContext context)
        {
            // Retrieve the text from the document and update the Roslyn Document  
            string text = context.Document.Text;
            document = document.WithText(SourceText.From(text));

            // Get the semantic model and syntax tree for the document  
            semanticModel = document.GetSemanticModelAsync().GetAwaiter().GetResult();
            SyntaxTree? syntaxTree = document.GetSyntaxTreeAsync().GetAwaiter().GetResult();
            SyntaxNode? root = syntaxTree?.GetRoot();

            // Extract all syntax tokens and comments (trivia) from the syntax tree  
            List<SyntaxToken>? tokens = root?.DescendantTokens().ToList();
            List<SyntaxTrivia>? comments = tokens?.SelectMany(x => x.LeadingTrivia.Concat(x.TrailingTrivia)).ToList();

            // Clear previous token and comment mappings  
            _tokensByLine?.Clear();
            _commentsByLine?.Clear();

            // Group tokens by their line numbers  
            if (tokens != null)
            {
                foreach (var token in tokens)
                {
                    int startLine = token.GetLocation().GetLineSpan().StartLinePosition.Line;
                    int endLine = token.GetLocation().GetLineSpan().EndLinePosition.Line;
                    for (int line = startLine; line <= endLine; line++)
                    {
                        if (!_tokensByLine!.ContainsKey(line))
                        {
                            _tokensByLine[line] = new List<SyntaxToken>();
                        }
                        _tokensByLine[line].Add(token);
                    }
                }
            }

            // Group comments by their line numbers  
            if (comments != null)
            {
                foreach (var comment in comments)
                {
                    int startLine = comment.GetLocation().GetLineSpan().StartLinePosition.Line;
                    int endLine = comment.GetLocation().GetLineSpan().EndLinePosition.Line;
                    for (int line = startLine; line <= endLine; line++)
                    {
                        if (!_commentsByLine!.ContainsKey(line))
                        {
                            _commentsByLine[line] = new List<SyntaxTrivia>();
                        }
                        _commentsByLine[line].Add(comment);
                    }
                }
            }

            // Call the base class's Colorize method to apply the changes  
            base.Colorize(context);
        }

        // Method to colorize a specific line in the document  
        protected override void ColorizeLine(DocumentLine line)
        {
            int lineNumber = line.LineNumber - 1; // Convert 1-based line number to 0-based  

            // Apply colorization to tokens on the line  
            if (_tokensByLine.TryGetValue(lineNumber, out var _tokens))
            {
                foreach (var token in _tokens)
                {
                    var color = GetColorForToken(token);
                    var tokenStartLine = token.GetLocation().GetLineSpan().StartLinePosition.Line;
                    var tokenEndLine = token.GetLocation().GetLineSpan().EndLinePosition.Line;

                    if (tokenStartLine <= lineNumber && tokenEndLine >= lineNumber)
                    {
                        var start = Math.Max(token.Span.Start, line.Offset);
                        var end = Math.Min(token.Span.Start + token.Span.Length, line.EndOffset);

                        ChangeLinePart(
                            start,
                            end,
                            element =>
                            {
                                var textRunProperties = element?.TextRunProperties;
                                textRunProperties?.SetForegroundBrush(new SolidColorBrush(color));
                            }
                        );
                    }
                }
            }

            // Apply colorization to comments on the line  
            if (_commentsByLine.TryGetValue(lineNumber, out var _comments))
            {
                foreach (var comment in _comments)
                {
                    var color = GetColorForTrivia(comment);
                    var commentStartLine = comment.GetLocation().GetLineSpan().StartLinePosition.Line;
                    var commentEndLine = comment.GetLocation().GetLineSpan().EndLinePosition.Line;

                    if (commentStartLine <= lineNumber && commentEndLine >= lineNumber)
                    {
                        var start = Math.Max(comment.Span.Start, line.Offset);
                        var end = Math.Min(comment.Span.End, line.EndOffset);

                        ChangeLinePart(
                            start,
                            end,
                            element => element.TextRunProperties.SetForegroundBrush(new SolidColorBrush(color))
                        );
                    }
                }
            }
        }

        // Determines the color for a given syntax token based on its semantic and syntactic information  
        private Color GetColorForToken(SyntaxToken token)
        {
            ISymbol? symbol = token.Parent is not null ? (semanticModel?.GetSymbolInfo(token.Parent).Symbol ?? semanticModel?.GetDeclaredSymbol(token.Parent)) : null;

            SymbolKind? symbolKind = symbol?.Kind;
            if (symbol is IMethodSymbol methodSymbol)
            {
                if (methodSymbol.MethodKind == MethodKind.Constructor)
                {
                    symbolKind = SymbolKind.NamedType;
                }
            }

            if (symbolKind.HasValue && CSharpThemeManager.SemanticStyles.TryGetValue(symbolKind.Value, out var semanticStyle) && token.IsKind(SyntaxKind.IdentifierToken))
            {
                return Util.GetColorFromHex(semanticStyle.Color);
            }
            else if (CSharpThemeManager.SyntaxStyles.TryGetValue(token.Kind(), out var style))
            {
                return Util.GetColorFromHex(style.Color);
            }

            return Colors.White; // Default color if no specific style is found  
        }

        // Determines the color for a given syntax trivia (e.g., comments)  
        private Color GetColorForTrivia(SyntaxTrivia trivia)
        {
            if (CSharpThemeManager.SyntaxStyles.TryGetValue(trivia.Kind(), out var style))
            {
                return Util.GetColorFromHex(style.Color);
            }
            return Colors.White; // Default color if no specific style is found  
        }
    }
}
