using Avalonia.Controls;
using Avalonia.Media;
using CodeForgeIDE.Core.Lang;
using CodeForgeIDE.Core.Util;

namespace CodeForgeIDE.CSharp.Lang.CodeCompletion
{
    public class CSharpCompletionData : CompletionData
    {
        public CSharpCompletionData(string text, IImage image) : base(text, image)
        {
        }

        public CSharpCompletionData(CompletionSymbol symbol) : base(symbol.Name, symbol.GetIcon().ToImage())
        {

        }
    }
}
