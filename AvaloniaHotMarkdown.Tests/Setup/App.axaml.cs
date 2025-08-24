using Avalonia;
using Avalonia.Markup.Xaml;

namespace AvaloniaHotMarkdown.Tests.Setup;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
}