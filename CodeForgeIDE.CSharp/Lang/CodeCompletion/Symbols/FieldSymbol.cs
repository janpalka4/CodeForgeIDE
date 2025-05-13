using CodeForgeIDE.Core;

namespace CodeForgeIDE.CSharp.Lang.CodeCompletion.Symbols
{
    /// <summary>
    /// Reprezentuje symbol pole pro code completion
    /// </summary>
    public class FieldSymbol : CompletionSymbol
    {
        /// <summary>
        /// Typ pole
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Je pole konstanta?
        /// </summary>
        public bool IsConstant { get; set; }

        /// <summary>
        /// Je pole readonly?
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Je pole statické?
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="name">Název pole</param>
        public FieldSymbol(string name) : base(name, Icons.SymbolField.IconPath)
        {
        }

        /// <summary>
        /// Typ symbolu
        /// </summary>
        public override string SymbolType => "Field";

        /// <summary>
        /// Vrací ikonu pro pole
        /// </summary>
        /// <returns></returns>
        public override IconData GetIcon() => IsConstant ? Icons.SymbolConstant : Icons.SymbolField;
    }
}
