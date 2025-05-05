using Avalonia.Controls;
using CodeForgeIDE.Logic;

namespace CodeForgeIDE
{
    public partial class MainWindow : Window
    {
        private EditorInitializer _editorInitializer;
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override async void OnInitialized()
        {
            base.OnInitialized();

            _editorInitializer = new EditorInitializer();
            _editorInitializer.OnInitializationInfo += (e) =>
            {
                statusLabel.Content = e;
            };

            await _editorInitializer.InitializeEditor();

            //Open editor
            var gateway = new GatewayWindow();
            gateway.Show();
            this.Close();
        }
    }
}