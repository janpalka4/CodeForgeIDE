namespace CodeForgeIDE.Core
{
    public static class Icons
    {
        public static IconData FileCode { get; private set; } = new IconData() { IconPath = "avares://CodeForgeIDE/Assets/Icons/file-code.svg", DisableCss = false };
        public static IconData Folder { get; private set; } = new IconData() { IconPath = "avares://CodeForgeIDE/Assets/Icons/file-folder.svg", DisableCss = true };
        public static IconData ArrowMenuRight { get; private set; } = new IconData() { IconPath = "avares://CodeForgeIDE/Assets/Icons/menu-right.svg", DisableCss = false };
        public static IconData ArrowMenuBottomLeft { get; private set; } = new IconData() { IconPath = "avares://CodeForgeIDE/Assets/Icons/arrow-bottom-left-2-fill.svg", DisableCss = false };
        public static IconData ArrowEject { get; private set; } = new IconData() { IconPath = "avares://CodeForgeIDE/Assets/Icons/arrow-eject-20-filled.svg", DisableCss = false };
        public static IconData CaretRight { get; private set; } = new IconData() { IconPath = "avares://CodeForgeIDE/Assets/Icons/caret-right-8.svg", DisableCss = false };
        public static IconData CaretBottomRight { get; private set; } = new IconData() { IconPath = "avares://CodeForgeIDE/Assets/Icons/caret-bottom-right-solid-8.svg", DisableCss = false };
    }
}
