using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Input;

namespace AvaloniaHotMarkdown.Tests;

public class BasicWritingTest : BaseTest
{
    [AvaloniaTest]
    public void Basic_Input()
    {
        (Window window, HotMarkdownEditor editor) = BasicWindowBuilder.CreateBasicWindow();
        ActivateTarget(window, editor);

        HandleTextInput("Hello World");

        Assert.That("Hello World", Is.EqualTo(editor.Text));
    }

    [AvaloniaTest]
    public void Multiline_Input()
    {
        (Window window, HotMarkdownEditor editor) = BasicWindowBuilder.CreateBasicWindow();
        ActivateTarget(window, editor);

        HandleTextInput("Hello World");
        Enter();
        HandleTextInput("This is a test");
        Enter();
        HandleTextInput("This is a third line");

        string expectedText = "Hello World\nThis is a test\nThis is a third line";

        Assert.That(editor.Text, Is.EqualTo(expectedText));
    }
}
