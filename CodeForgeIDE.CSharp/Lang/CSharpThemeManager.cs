using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;

namespace CodeForgeIDE.CSharp.Lang
{
    public static class CSharpThemeManager
    {
        public static Dictionary<SyntaxKind, CSharpThemeStyle> SyntaxStyles { get; set; }
        public static Dictionary<SymbolKind, CSharpThemeStyle> SemanticStyles { get; set; }

        static CSharpThemeManager() {
            SyntaxStyles = JsonConvert.DeserializeObject<Dictionary<SyntaxKind, CSharpThemeStyle>>(
                File.ReadAllText("./CSharpThemes/DefaultCSharp.json")
            )!;
            SemanticStyles = JsonConvert.DeserializeObject<Dictionary<SymbolKind, CSharpThemeStyle>>(
                File.ReadAllText("./CSharpThemes/DefaultCSharpSemantic.json")
            )!;
        }
    }
}
