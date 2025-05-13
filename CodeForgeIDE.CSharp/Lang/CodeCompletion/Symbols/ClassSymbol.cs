using CodeForgeIDE.Core;
using System.Collections.Generic;

namespace CodeForgeIDE.CSharp.Lang.CodeCompletion.Symbols
{
    /// <summary>
    /// Reprezentuje symbol třídy pro code completion
    /// </summary>
    public class ClassSymbol : CompletionSymbol
    {
        /// <summary>
        /// Seznam metod v třídě
        /// </summary>
        public List<MethodSymbol> Methods { get; set; } = new List<MethodSymbol>();

        /// <summary>
        /// Seznam vlastností v třídě
        /// </summary>
        public List<PropertySymbol> Properties { get; set; } = new List<PropertySymbol>();

        /// <summary>
        /// Seznam polí v třídě
        /// </summary>
        public List<FieldSymbol> Fields { get; set; } = new List<FieldSymbol>();

        /// <summary>
        /// Seznam událostí v třídě
        /// </summary>
        public List<EventSymbol> Events { get; set; } = new List<EventSymbol>();

        /// <summary>
        /// Rodičovská třída, ze které tato třída dědí
        /// </summary>
        public string BaseClass { get; set; }

        /// <summary>
        /// Seznam rozhraní, které třída implementuje
        /// </summary>
        public List<string> Interfaces { get; set; } = new List<string>();

        /// <summary>
        /// Je třída abstraktní?
        /// </summary>
        public bool IsAbstract { get; set; }

        /// <summary>
        /// Je třída statická?
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="name">Název třídy</param>
        public ClassSymbol(string name) : base(name, Icons.SymbolClass.IconPath)
        {
        }

        /// <summary>
        /// Typ symbolu
        /// </summary>
        public override string SymbolType => "Class";

        /// <summary>
        /// Vrací ikonu pro třídu
        /// </summary>
        public override IconData GetIcon() => Icons.SymbolClass;
    }
}
