using CodeForgeIDE.Core;
using System.Collections.Generic;

namespace CodeForgeIDE.CSharp.Lang.CodeCompletion.Symbols
{
    /// <summary>
    /// Reprezentuje symbol jmenného prostoru pro code completion
    /// </summary>
    public class NamespaceSymbol : CompletionSymbol
    {
        /// <summary>
        /// Seznam tříd v tomto jmenném prostoru
        /// </summary>
        public List<ClassSymbol> Classes { get; set; } = new List<ClassSymbol>();

        /// <summary>
        /// Seznam enum typů v tomto jmenném prostoru
        /// </summary>
        public List<EnumSymbol> Enums { get; set; } = new List<EnumSymbol>();

        /// <summary>
        /// Seznam rozhraní v tomto jmenném prostoru
        /// </summary>
        public List<InterfaceSymbol> Interfaces { get; set; } = new List<InterfaceSymbol>();

        /// <summary>
        /// Seznam struktur v tomto jmenném prostoru
        /// </summary>
        public List<StructSymbol> Structs { get; set; } = new List<StructSymbol>();

        /// <summary>
        /// Seznam podjmenných prostorů
        /// </summary>
        public List<NamespaceSymbol> ChildNamespaces { get; set; } = new List<NamespaceSymbol>();

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="name">Název jmenného prostoru</param>
        public NamespaceSymbol(string name) : base(name, Icons.SymbolNamespace.IconPath)
        {
        }

        /// <summary>
        /// Typ symbolu
        /// </summary>
        public override string SymbolType => "Namespace";

        /// <summary>
        /// Vrací ikonu pro jmenný prostor
        /// </summary>
        public override IconData GetIcon() => Icons.SymbolNamespace;
    }
}
