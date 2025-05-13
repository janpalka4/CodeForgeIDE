using CodeForgeIDE.Core;
using System.Collections.Generic;

namespace CodeForgeIDE.CSharp.Lang.CodeCompletion.Symbols
{
    /// <summary>
    /// Reprezentuje symbol metody pro code completion
    /// </summary>
    public class MethodSymbol : CompletionSymbol
    {
        /// <summary>
        /// Návratový typ metody
        /// </summary>
        public string ReturnType { get; set; }

        /// <summary>
        /// Seznam parametrů metody
        /// </summary>
        public List<ParameterSymbol> Parameters { get; set; } = new List<ParameterSymbol>();

        /// <summary>
        /// Je metoda statická?
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// Je metoda abstraktní?
        /// </summary>
        public bool IsAbstract { get; set; }

        /// <summary>
        /// Je metoda virtuální?
        /// </summary>
        public bool IsVirtual { get; set; }

        /// <summary>
        /// Je metoda override?
        /// </summary>
        public bool IsOverride { get; set; }

        /// <summary>
        /// Je metoda async?
        /// </summary>
        public bool IsAsync { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="name">Název metody</param>
        public MethodSymbol(string name) : base(name, Icons.SymbolMethod.IconPath)
        {
        }

        /// <summary>
        /// Typ symbolu
        /// </summary>
        public override string SymbolType => "Method";

        /// <summary>
        /// Vrací ikonu pro metodu
        /// </summary>
        public override IconData GetIcon() => Icons.SymbolMethod;

        /// <summary>
        /// Vrací signaturu metody
        /// </summary>
        public string GetSignature()
        {
            var paramList = string.Join(", ", Parameters.ConvertAll(p => $"{p.Type} {p.Name}"));
            return $"{ReturnType} {Name}({paramList})";
        }
    }
}
