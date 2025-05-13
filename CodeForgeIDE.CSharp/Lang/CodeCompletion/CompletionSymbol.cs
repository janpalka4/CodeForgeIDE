using System;
using System.Collections.Generic;
using CodeForgeIDE.Core;

namespace CodeForgeIDE.CSharp.Lang.CodeCompletion
{
    /// <summary>
    /// Abstraktní třída představující symbol pro code completion.
    /// Slouží jako základní třída pro všechny typy symbolů (třídy, metody, proměnné, apod.).
    /// </summary>
    public abstract class CompletionSymbol
    {
        /// <summary>
        /// Název symbolu
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Cesta k ikoně symbolu
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        /// Plně kvalifikovaný název včetně jmenného prostoru
        /// </summary>
        public string FullyQualifiedName { get; set; }

        /// <summary>
        /// Jmenný prostor symbolu
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Popis/dokumentace symbolu
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Určuje, zda je symbol veřejný (public)
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// Umístění v kódu (cesta k souboru)
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Řádek v souboru
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Sloupec v souboru
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Dodatečná data specifická pro daný typ symbolu
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="name">Název symbolu</param>
        /// <param name="iconPath">Cesta k ikoně</param>
        protected CompletionSymbol(string name, string iconPath)
        {
            Name = name;
            IconPath = iconPath;
            FullyQualifiedName = name;
        }

        /// <summary>
        /// Vrací typ symbolu
        /// </summary>
        public abstract string SymbolType { get; }

        /// <summary>
        /// Vrací ikonu pro daný symbol
        /// </summary>
        public abstract IconData GetIcon();
    }
}
