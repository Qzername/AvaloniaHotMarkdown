using Avalonia.Controls;
using AvaloniaHotMarkdown.Demo.ViewModels;

namespace AvaloniaHotMarkdown.Demo.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}