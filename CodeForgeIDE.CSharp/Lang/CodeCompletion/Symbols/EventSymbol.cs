using CodeForgeIDE.Core;

namespace CodeForgeIDE.CSharp.Lang.CodeCompletion.Symbols
{
    /// <summary>
    /// Reprezentuje symbol události pro code completion
    /// </summary>
    public class EventSymbol : CompletionSymbol
    {
        /// <summary>
        /// Typ delegáta události
        /// </summary>
        public string DelegateType { get; set; }

        /// <summary>
        /// Je událost statická?
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// Je událost virtuální?
        /// </summary>
        public bool IsVirtual { get; set; }

        /// <summary>
        /// Je událost abstract?
        /// </summary>
        public bool IsAbstract { get; set; }

        /// <summary>
        /// Je událost override?
        /// </summary>
        public bool IsOverride { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="name">Název události</param>
        public EventSymbol(string name) : base(name, Icons.SymbolEvent.IconPath)
        {
        }

        /// <summary>
        /// Typ symbolu
        /// </summary>
        public override string SymbolType => "Event";

        /// <summary>
        /// Vrací ikonu pro událost
        /// </summary>
        public override IconData GetIcon() => Icons.SymbolEvent;
    }
}
