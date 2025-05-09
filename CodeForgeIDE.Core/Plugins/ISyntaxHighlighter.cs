using AvaloniaEdit;

namespace CodeForgeIDE.Core.Plugins
{
    public interface ISyntaxHighlighter : IFileScopedProvider
    {
        public void Initialize(TextEditor textEditor);
        public void Update(string text);
    }
}
