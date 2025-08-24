using Avalonia.Controls;

namespace AvaloniaHotMarkdown.Tests;

public static class BasicWindowBuilder
{
    public static (Window, HotMarkdownEditor) CreateBasicWindow()
    {
        var hotEditor = new HotMarkdownEditor();

        var panel = new Panel
        {
            Children =
            {
                hotEditor
            }
        };

        var window = new Window { Content = panel };

        return (window, hotEditor);
    }
}