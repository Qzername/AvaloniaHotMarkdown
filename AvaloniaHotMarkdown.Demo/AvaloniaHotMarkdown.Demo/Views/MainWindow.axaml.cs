using Avalonia.Controls;
using AvaloniaHotMarkdown.Demo.ViewModels;

namespace AvaloniaHotMarkdown.Demo.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}