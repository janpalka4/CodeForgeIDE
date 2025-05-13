using CodeForgeIDE.Core;

namespace CodeForgeIDE.CSharp.Lang.CodeCompletion.Symbols
{
    /// <summary>
    /// Reprezentuje symbol parametru pro code completion
    /// </summary>
    public class ParameterSymbol : CompletionSymbol
    {
        /// <summary>
        /// Typ parametru
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Je parametr volitelný?
        /// </summary>
        public bool IsOptional { get; set; }

        /// <summary>
        /// Výchozí hodnota parametru (pro volitelné parametry)
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Je parametr ref?
        /// </summary>
        public bool IsRef { get; set; }

        /// <summary>
        /// Je parametr out?
        /// </summary>
        public bool IsOut { get; set; }

        /// <summary>
        /// Je parametr in?
        /// </summary>
        public bool IsIn { get; set; }

        /// <summary>
        /// Je parametr params?
        /// </summary>
        public bool IsParams { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="name">Název parametru</param>
        public ParameterSymbol(string name) : base(name, Icons.SymbolParameter.IconPath)
        {
        }

        /// <summary>
        /// Typ symbolu
        /// </summary>
        public override string SymbolType => "Parameter";

        /// <summary>
        /// Vrací ikonu pro parametr
        /// </summary>
        public override IconData GetIcon() => Icons.SymbolParameter;
    }
}
