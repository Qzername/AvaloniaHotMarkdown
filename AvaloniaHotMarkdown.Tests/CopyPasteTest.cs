using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Input;

namespace AvaloniaHotMarkdown.Tests;

internal class CopyPasteTest : BaseTest
{
    [AvaloniaTest]
    public void Paste_OneLine()
    {
        (Window window, HotMarkdownEditor editor) = BasicWindowBuilder.CreateBasicWindow();
        ActivateTarget(window, editor);

        string inputText = "Hello World";
        InsertIntoClipboard(inputText);

        Paste();

        Assert.That(editor.Text, Is.EqualTo(inputText));
        Assert.That(editor.CaretPositionData.X, Is.EqualTo(inputText.Length));
    }

    [AvaloniaTest]
    public void Paste_OnSelection_OneLine()
    {
        (Window window, HotMarkdownEditor editor) = BasicWindowBuilder.CreateBasicWindow();
        ActivateTarget(window, editor);

        string inputText = "Hello World";
        InsertIntoClipboard(inputText);

        HandleTextInput(inputText);

        //Hello |[World]
        for (int i = 0; i < 5; i++)
            HandleKeySelection(PhysicalKey.ArrowLeft);

        //Hello Hello World|
        Paste();

        Assert.That(editor.Text, Is.EqualTo("Hello " + inputText));
        Assert.That(editor.CaretPositionData.X, Is.EqualTo(("Hello " + inputText).Length));
    }
}