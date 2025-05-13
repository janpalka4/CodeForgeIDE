using CodeForgeIDE.Core;
using System.Collections.Generic;

namespace CodeForgeIDE.CSharp.Lang.CodeCompletion.Symbols
{
    /// <summary>
    /// Reprezentuje symbol struktury pro code completion
    /// </summary>
    public class StructSymbol : CompletionSymbol
    {
        /// <summary>
        /// Seznam metod ve struktuře
        /// </summary>
        public List<MethodSymbol> Methods { get; set; } = new List<MethodSymbol>();

        /// <summary>
        /// Seznam vlastností ve struktuře
        /// </summary>
        public List<PropertySymbol> Properties { get; set; } = new List<PropertySymbol>();

        /// <summary>
        /// Seznam polí ve struktuře
        /// </summary>
        public List<FieldSymbol> Fields { get; set; } = new List<FieldSymbol>();

        /// <summary>
        /// Seznam událostí ve struktuře
        /// </summary>
        public List<EventSymbol> Events { get; set; } = new List<EventSymbol>();

        /// <summary>
        /// Seznam rozhraní, které struktura implementuje
        /// </summary>
        public List<string> Interfaces { get; set; } = new List<string>();

        /// <summary>
        /// Je struktura readonly?
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="name">Název struktury</param>
        public StructSymbol(string name) : base(name, Icons.SymbolStructure.IconPath)
        {
        }

        /// <summary>
        /// Typ symbolu
        /// </summary>
        public override string SymbolType => "Struct";

        /// <summary>
        /// Vrací ikonu pro strukturu
        /// </summary>
        public override IconData GetIcon() => Icons.SymbolStructure;
    }
}
