namespace CodeForgeIDE.Core
{
    public static class IDE
    {
        public static Editor Editor { get; private set; }

        static IDE()
        {
            Editor = new Editor();
        }
    }
}
