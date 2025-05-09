using AvaloniaEdit;

namespace CodeForgeIDE.Core.Plugins
{
    public interface ISyntaxHighlighter : IFileScopedProvider
    {
        public void Initialize(TextEditor textEditor, string path);
        public void Update(string text);
    }
}
