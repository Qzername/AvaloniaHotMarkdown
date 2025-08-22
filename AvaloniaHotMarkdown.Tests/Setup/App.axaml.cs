using Avalonia;
using Avalonia.Markup.Xaml;

namespace Tests.Setup;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
}