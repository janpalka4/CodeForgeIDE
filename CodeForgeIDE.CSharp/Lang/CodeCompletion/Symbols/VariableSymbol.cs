using CodeForgeIDE.Core;

namespace CodeForgeIDE.CSharp.Lang.CodeCompletion.Symbols
{
    /// <summary>
    /// Reprezentuje symbol lokální proměnné pro code completion
    /// </summary>
    public class VariableSymbol : CompletionSymbol
    {
        /// <summary>
        /// Typ proměnné
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Je proměnná read-only (readonly nebo const)?
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="name">Název proměnné</param>
        public VariableSymbol(string name) : base(name, Icons.SymbolVariable.IconPath)
        {
        }

        /// <summary>
        /// Typ symbolu
        /// </summary>
        public override string SymbolType => "Variable";

        /// <summary>
        /// Vrací ikonu pro proměnnou
        /// </summary>
        public override IconData GetIcon() => Icons.SymbolVariable;
    }
}
