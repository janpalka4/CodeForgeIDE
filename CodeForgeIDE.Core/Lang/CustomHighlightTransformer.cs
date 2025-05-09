using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;
using System.Text.RegularExpressions;

namespace CodeForgeIDE.Core.Lang
{
    public class CustomHighlightTransformer : DocumentColorizingTransformer
    {
        private readonly List<CustomHighlightRule> _rules;

        public CustomHighlightTransformer(List<CustomHighlightRule> rules)
        {
            _rules = rules;
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            foreach (var rule in _rules)
            {
                if(line.EndOffset < rule.Start + rule.Length)
                    continue;
                if(line.Offset > rule.Start)
                    continue;

                ChangeLinePart(
                    rule.Start,
                    rule.Start + rule.Length,
                    element =>
                    {
                        var textRunProperties = element.TextRunProperties;
                        textRunProperties.SetForegroundBrush(rule.Brush);
                    }
                );
            }
        }
    }

    public class CustomHighlightRule()
    {
        public int Start { get; set; }
        public int Length { get; set; }
        public IBrush Brush { get; set; }
    }
}
