using Avalonia.Headless;
using Avalonia;
using Tests.Setup;
using Avalonia.Controls;

[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]

namespace Tests.Setup;

public class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
        .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}