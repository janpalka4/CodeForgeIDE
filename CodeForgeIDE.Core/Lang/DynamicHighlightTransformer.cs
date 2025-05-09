using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using System.Text.RegularExpressions;

namespace CodeForgeIDE.Core.Lang
{
    public class DynamicHighlightTransformer : DocumentColorizingTransformer
    {
        private readonly List<DynamicHighlightRule> _rules;

        public DynamicHighlightTransformer(List<DynamicHighlightRule> rules)
        {
            _rules = rules;
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            var lineText = CurrentContext.Document.GetText(line);

            foreach (var rule in _rules)
            {
                foreach (Match match in rule.Regex.Matches(lineText))
                {
                    if (match.Groups.Count == 0)
                    {
                        ChangeLinePart(
                            line.Offset + match.Index,
                            line.Offset + match.Index + match.Length,
                            element =>
                            {
                                var textRunProperties = element.TextRunProperties;
                                textRunProperties.SetForegroundBrush(rule.Brush);
                            }
                        );
                    }
                    else
                    {
                        for (int i = 1; i < match.Groups.Count; i++)
                        {
                            var group = match.Groups[i];
                            ChangeLinePart(
                                line.Offset + group.Index,
                                line.Offset + group.Index + group.Length,
                                element =>
                                {
                                    var textRunProperties = element.TextRunProperties;
                                    textRunProperties.SetForegroundBrush(rule.Brush);
                                }
                            );
                        }
                    }
                }
            }
        }
    }

    public class DynamicHighlightRule
    {
        public Regex Regex { get; set; }
        public IBrush Brush { get; set; }
    }
}

