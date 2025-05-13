using Avalonia.Media;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;

namespace CodeForgeIDE.Core.Lang
{
    public class CompletionData : ICompletionData
    {
        public string Text { get; }
        public object Content => Text;
        public object Description { get; set; }
        public double Priority => 0;

        public IImage Image { get; private set; }

        public CompletionData(string text, IImage image)
        {
            Text = text;
            Image = image;
        }


        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Text);
        }
    }
}
