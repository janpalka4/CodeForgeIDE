Implementace code completion do modulu CODEFORGEIDE.CSharp

[] Asynchroní task pro analýzu kódu (v průběhu všechen kód i dokumentuj, prezentace uživateli nyní není předmětem úkolů) (Ideálně řešení optimalizovat i pro velké projekty, není třeba řešit prezistenci po zavření a spuštění editoru)
    [] Pro anylýzu používej Roslyn
    [] CompletionData.cs a ICompletionDataProvider.cs jsou pouze pro prezenci dat uživateli, ignoruj tyto třídy
    [] Vytvoř novou třídu, která bude analyzovat C# řešení.
        [] Databáze pojmů (klidně lze implementovat jako novou třídu z důvodů přehlednosti)
            [] Třída bude mít public property, které bude sloužiz jako databáze pojmů
            [] Ideální aby byla datové struktury, ve které se dá rychle hledat podle názvu
            [] Každý pojem má svojí třídu a ta dědí z abstraktní třídy společné pro všechny pojmy
            [] V datové struktuře jde rychle vyhledávat dle názvu pojmu, byť i neuplného (tedy neymslím si že je zde tak vhodné využít Dictionary) (nejde zde jen o prefixové vyhledávání, vyhledávat se bude i podle contains, očekávám vlastně takové fuzzy vyhledávání, lze využít i FuzzySharp)
        [] Prvotní analýza (může být pomalejší)
            [] Analýza obecných pojmů
                [] Třídu bude asynchroně analyzovat celé řešení a získávat názvy všech objektů např. tříd, interfaců, enumů, atd.
                [] Pro každý z těchto obecných názvů/pojmů bude existovat zvlášt třída, která bude dědit z jednotné abstrakní třídy společné pro všechny názvy
                [] V dané abstrakntí třídě bude vlastnost typu string s cestou pro ikonku, která se bude brát ze třídy Icons (již existuje)
                [] Každý název může mít své vlastnosti, např. třída má metody, vlastnosti a polem, tyto vlastnosti se načítají do objektu
                [] Dále se analyzují dostupné jmenné prostory a ukládají podobně jako ostatní pojmy
            [] Analýza lokálních pojmů
                [] Zde dostane analyzér aktulní scope (cestu k souboru a pozici kurzoru) a jeho úkolem je vrátit názvy lokálních proměnných, privátních proměnných, a dalších členů, které nejsou public, tzn. brát i protected property z předků aktuální třídy
        [] Realtime analýza (rychlá, volá se po změně textu v dokumentu či po přidání a odebrání dokumentu do řešení)
            [] Zde se realtime analýza volá pomocí eventu on changed (jen vytvoř metodu, kterou já si pak zavolám), který nám předá i lokální scope (tedy cestu k souboru)
            [] Poté se podíváme na změny (jaké pojmy obecné/lokální ubyli či přibili) a podle toho upravíme naši databázi pojmů 
