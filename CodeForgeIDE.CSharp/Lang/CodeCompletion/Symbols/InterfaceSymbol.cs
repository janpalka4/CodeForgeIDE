using CodeForgeIDE.Core;
using System.Collections.Generic;

namespace CodeForgeIDE.CSharp.Lang.CodeCompletion.Symbols
{
    /// <summary>
    /// Reprezentuje symbol rozhraní pro code completion
    /// </summary>
    public class InterfaceSymbol : CompletionSymbol
    {
        /// <summary>
        /// Seznam metod v rozhraní
        /// </summary>
        public List<MethodSymbol> Methods { get; set; } = new List<MethodSymbol>();

        /// <summary>
        /// Seznam vlastností v rozhraní
        /// </summary>
        public List<PropertySymbol> Properties { get; set; } = new List<PropertySymbol>();

        /// <summary>
        /// Seznam událostí v rozhraní
        /// </summary>
        public List<EventSymbol> Events { get; set; } = new List<EventSymbol>();

        /// <summary>
        /// Seznam rozhraní, které toto rozhraní rozšiřuje
        /// </summary>
        public List<string> BaseInterfaces { get; set; } = new List<string>();

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="name">Název rozhraní</param>
        public InterfaceSymbol(string name) : base(name, Icons.SymbolInterface.IconPath)
        {
        }

        /// <summary>
        /// Typ symbolu
        /// </summary>
        public override string SymbolType => "Interface";

        /// <summary>
        /// Vrací ikonu pro rozhraní
        /// </summary>
        public override IconData GetIcon() => Icons.SymbolInterface;
    }
}
