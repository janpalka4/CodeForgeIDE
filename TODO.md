# CODEFORGEIDE
Rozšiřitelné integrované vývojové prostředí zaměřené primárně na .NET

### Vylepšení modulu CodeForgeIDE.CSharp
- [ ] Implementace pokročilého syntax highlightingu pro C#  
  - [ ]  V CSharpSyntaxHighlighter je nyní testovací implementace vlastních highlighting pravidel pomocí DynamicHighlightRule, tento systém chceme rozšířit.
  - [ ]  Syntax highlighting bude zvýrazňovat i názvy tříd, metod, rozhraní, enumů, atd. 
  - [ ]  Zvýrazňovat se budou pouze názvy existujích tříd, metod, rozhraní, enumů, atd.  (optional).
  - [ ]  Barvy pro zvýraznění se budeme pokoušet brát z TextMate theme (Pokud to bude možné a bude se to hodit).
  - [ ]  Jelikož se bude metoda Update v CSharpSyntaxHighlighter volat po každé změně v editoru, je třeba případnou analýzu kódu spusti na pozadí.
  - [ ]  Běh analýzy na pozadí je třeba nějakým způsobem řídit, aby běžela vždy jen jedna analýza.
  - [ ]  V žádném případě nesmí analýza kódu ovblivnit výkon editoru.
  - [ ]  Pokud bude třeba implementovat nové třídy či rozhraní do modulu CodeForgeIDE.Core, implementuj je, aby je mohli pužívat i další moduly.
  - [ ]  Analýza bude prováděna přes Roslyn tedy Microsoft.CodeAnalysis.

### Hotové tasky ✓
- [x] Název dokončeného tasku 