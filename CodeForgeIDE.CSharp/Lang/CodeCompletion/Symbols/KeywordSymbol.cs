using CodeForgeIDE.Core;

namespace CodeForgeIDE.CSharp.Lang.CodeCompletion.Symbols
{
    /// <summary>
    /// Reprezentuje symbol klíčového slova pro code completion
    /// </summary>
    public class KeywordSymbol : CompletionSymbol
    {
        /// <summary>
        /// Popisek - detailnější vysvětlení klíčového slova
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// Ukázka použití klíčového slova
        /// </summary>
        public string Example { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="name">Název klíčového slova</param>
        public KeywordSymbol(string name) : base(name, Icons.SymbolKeyword.IconPath)
        {
        }

        /// <summary>
        /// Typ symbolu
        /// </summary>
        public override string SymbolType => "Keyword";

        /// <summary>
        /// Vrací ikonu pro klíčové slovo
        /// </summary>
        public override IconData GetIcon() => Icons.SymbolKeyword;
    }
}
