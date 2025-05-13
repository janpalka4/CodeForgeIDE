using CodeForgeIDE.Core;
using System.Collections.Generic;

namespace CodeForgeIDE.CSharp.Lang.CodeCompletion.Symbols
{
    /// <summary>
    /// Reprezentuje symbol enum pro code completion
    /// </summary>
    public class EnumSymbol : CompletionSymbol
    {
        /// <summary>
        /// Seznam členů enumu
        /// </summary>
        public List<EnumMemberSymbol> Members { get; set; } = new List<EnumMemberSymbol>();

        /// <summary>
        /// Podkladový typ enumu
        /// </summary>
        public string UnderlyingType { get; set; } = "int";

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="name">Název enum typu</param>
        public EnumSymbol(string name) : base(name, Icons.SymbolEnum.IconPath)
        {
        }

        /// <summary>
        /// Typ symbolu
        /// </summary>
        public override string SymbolType => "Enum";

        /// <summary>
        /// Vrací ikonu pro enum
        /// </summary>
        public override IconData GetIcon() => Icons.SymbolEnum;
    }

    /// <summary>
    /// Reprezentuje symbol člena enum typu pro code completion
    /// </summary>
    public class EnumMemberSymbol : CompletionSymbol
    {
        /// <summary>
        /// Hodnota člena enum typu
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="name">Název člena enum typu</param>
        public EnumMemberSymbol(string name) : base(name, Icons.SymbolEnumMember.IconPath)
        {
        }

        /// <summary>
        /// Typ symbolu
        /// </summary>
        public override string SymbolType => "EnumMember";

        /// <summary>
        /// Vrací ikonu pro člena enum typu
        /// </summary>
        public override IconData GetIcon() => Icons.SymbolEnumMember;
    }
}
