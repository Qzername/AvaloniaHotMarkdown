using Avalonia.Headless;
using Avalonia;
using AvaloniaHotMarkdown.Tests.Setup;

[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]

namespace AvaloniaHotMarkdown.Tests.Setup;

public class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
        .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}