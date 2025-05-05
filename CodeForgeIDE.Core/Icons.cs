namespace CodeForgeIDE.Core
{
    public static class Icons
    {
        public static string FileCode { get; private set; } = "avares://CodeForgeIDE/Assets/Icons/file-code.svg";
        public static string Folder { get; private set; } = "avares://CodeForgeIDE/Assets/Icons/file-folder.svg";
        public static string ArrowMenuRight { get; private set; } = "avares://CodeForgeIDE/Assets/Icons/menu-right.svg";
        public static string ArrowMenuBottomLeft { get; private set; } = "avares://CodeForgeIDE/Assets/Icons/arrow-bottom-left-2-fill.svg";
        public static string ArrowEject { get; private set; } = "avares://CodeForgeIDE/Assets/Icons/arrow-eject-20-filled.svg";


        public static Dictionary<string,bool> DisableCss = new Dictionary<string, bool>
        {
            { FileCode, false },
            { Folder, true },
            { ArrowMenuRight, false },
            { ArrowMenuBottomLeft, false },
            { ArrowEject, false }
        };
    }
}
