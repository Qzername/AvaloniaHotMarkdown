using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Input;

namespace AvaloniaHotMarkdown.Tests;

internal class TextRemovingTest : BaseTest
{
    [AvaloniaTest]
    public void TextRemovingTest_CtrlBackspace()
    {
        (Window window, HotMarkdownEditor editor) = BasicWindowBuilder.CreateBasicWindow();
        ActivateTarget(window, editor);

        HandleTextInput("Hello");

        HandleKey(PhysicalKey.Backspace, RawInputModifiers.Control);

        Assert.That(editor.Text, Is.EqualTo(string.Empty));

        HandleTextInput("Hello World");

        HandleKey(PhysicalKey.Backspace, RawInputModifiers.Control);

        Assert.That(editor.Text, Is.EqualTo("Hello "));
    }

    [AvaloniaTest]
    public void TextRemovingTest_Enter()
    {
        (Window window, HotMarkdownEditor editor) = BasicWindowBuilder.CreateBasicWindow();
        ActivateTarget(window, editor);

        HandleTextInput("Hello");

        for(int i = 0; i < 5; i++) 
            HandleKeySelection(PhysicalKey.ArrowLeft);
    
        Enter();

        Assert.That(editor.Text, Is.EqualTo("\n"));
    }

    [AvaloniaTest]
    public void TextRemovingTest_Cut_DoubleCutBugCheck()
    {
        (Window window, HotMarkdownEditor editor) = BasicWindowBuilder.CreateBasicWindow();
        ActivateTarget(window, editor);

        HandleTextInput("Hello");

        HandleKeySelection(PhysicalKey.ArrowLeft);
        HandleKey(PhysicalKey.X, RawInputModifiers.Control);
        HandleKey(PhysicalKey.X, RawInputModifiers.Control);

        Assert.Pass();
    }
}
