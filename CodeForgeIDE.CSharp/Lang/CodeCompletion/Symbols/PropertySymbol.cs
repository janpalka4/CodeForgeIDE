using CodeForgeIDE.Core;

namespace CodeForgeIDE.CSharp.Lang.CodeCompletion.Symbols
{
    /// <summary>
    /// Reprezentuje symbol vlastnosti pro code completion
    /// </summary>
    public class PropertySymbol : CompletionSymbol
    {
        /// <summary>
        /// Typ vlastnosti
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Má vlastnost getter?
        /// </summary>
        public bool HasGetter { get; set; }

        /// <summary>
        /// Má vlastnost setter?
        /// </summary>
        public bool HasSetter { get; set; }

        /// <summary>
        /// Je vlastnost statická?
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// Je vlastnost abstraktní?
        /// </summary>
        public bool IsAbstract { get; set; }

        /// <summary>
        /// Je vlastnost virtuální?
        /// </summary>
        public bool IsVirtual { get; set; }

        /// <summary>
        /// Je vlastnost override?
        /// </summary>
        public bool IsOverride { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="name">Název vlastnosti</param>
        public PropertySymbol(string name) : base(name, Icons.SymbolProperty.IconPath)
        {
        }

        /// <summary>
        /// Typ symbolu
        /// </summary>
        public override string SymbolType => "Property";

        /// <summary>
        /// Vrací ikonu pro vlastnost
        /// </summary>
        public override IconData GetIcon() => Icons.SymbolProperty;
    }
}
