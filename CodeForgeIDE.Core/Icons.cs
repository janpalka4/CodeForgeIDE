namespace CodeForgeIDE.Core
{
    public static class Icons
    {
        public static string FileCode { get; private set; } = "avares://CodeForgeIDE/Assets/Icons/file-code.svg";
        public static string Folder { get; private set; } = "avares://CodeForgeIDE/Assets/Icons/file-folder.svg";
        public static string ArrowMenuRight { get; private set; } = "avares://CodeForgeIDE/Assets/Icons/menu-right.svg";
        public static string ArrowMenuBottomLeft { get; private set; } = "avares://CodeForgeIDE/Assets/Icons/arrow-bottom-left-2-fill.svg";
        public static string ArrowEject { get; private set; } = "avares://CodeForgeIDE/Assets/Icons/arrow-eject-20-filled.svg";
        public static string CaretRight { get; private set; } = "avares://CodeForgeIDE/Assets/Icons/caret-right-8.svg";
        public static string CaretBottomRight { get; private set; } = "avares://CodeForgeIDE/Assets/Icons/caret-bottom-right-solid-8.svg";


        public static Dictionary<string,bool> DisableCss = new Dictionary<string, bool>
        {
            { Folder, true }
        };
    }
}
