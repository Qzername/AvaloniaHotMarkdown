using Avalonia.Controls;

namespace Avalonia.HotMarkdown.Demo
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var control = new HotMarkdown();
            control.Width = 500;
            control.Height = 500;   

            Content = control;
        }
    }
}