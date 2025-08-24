using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;

namespace AvaloniaHotMarkdown.Tests;

internal class SelectionTest : BaseTest
{
    [AvaloniaTest]
    public void Selection_OneLine()
    {
        (Window window, HotMarkdownEditor editor) = BasicWindowBuilder.CreateBasicWindow();
        ActivateTarget(window, editor);

        string inputText = "Hello World";
        HandleTextInput(inputText);

        Assert.That(editor.SelectionPositionData.IsVisible, Is.EqualTo(false));
        
        HandleKeySelection(PhysicalKey.ArrowLeft);

        Assert.That(editor.SelectionPositionData.IsVisible, Is.EqualTo(true));
        Assert.That(editor.SelectedText, Is.EqualTo("d"));
    }

    [AvaloniaTest]
    public void Selection_MultiLine()
    {
        (Window window, HotMarkdownEditor editor) = BasicWindowBuilder.CreateBasicWindow();
        ActivateTarget(window, editor);

        HandleTextInput("Hello");
        Enter();
        HandleTextInput("World");
        Enter();
        HandleTextInput("Word");

        //Hello
        //World
        //Word|
        Assert.That(editor.SelectionPositionData.IsVisible, Is.EqualTo(false));

        //Hello
        //World
        //Wor|d

        HandleKey(PhysicalKey.ArrowLeft);

        //Hello
        //Wor|[ld
        //Wor]d
        HandleKeySelection(PhysicalKey.ArrowUp);
        Assert.That(editor.SelectionPositionData.IsVisible, Is.EqualTo(true));
        Assert.That(editor.SelectedText, Is.EqualTo("ld\nWor"));

        //Hel|[lo
        //World
        //Wor]d
        HandleKeySelection(PhysicalKey.ArrowUp);

        Assert.That(editor.SelectionPositionData.IsVisible, Is.EqualTo(true));
        Assert.That(editor.SelectedText, Is.EqualTo("lo\nWorld\nWor"));
    }

    [AvaloniaTest]
    public void Selection_Empty()
    {
        (Window window, HotMarkdownEditor editor) = BasicWindowBuilder.CreateBasicWindow();
        ActivateTarget(window, editor);

        HandleTextInput("Hello");

        Enter();
        Enter();

        HandleTextInput("World");

        HandleKeySelection(PhysicalKey.ArrowUp);
        HandleKeySelection(PhysicalKey.ArrowUp);

        Assert.That(editor.SelectionPositionData.IsVisible, Is.EqualTo(true));
        Assert.That(editor.SelectedText, Is.EqualTo("Hello\n\nWorld"));
    }

    [AvaloniaTest]
    public void Selection_List()
    {
        (Window window, HotMarkdownEditor editor) = BasicWindowBuilder.CreateBasicWindow();
        ActivateTarget(window, editor);

        HandleTextInput("- Item 1");
        Enter();

        // - Item 1
        // - Item 2|
        HandleTextInput("- Item 2");

        // - Item 1|
        // - Item 2
        HandleKey(PhysicalKey.ArrowUp);

        // - Item 1[\n
        // - Item 2]|
        HandleKeySelection(PhysicalKey.ArrowDown);

        Assert.That(editor.SelectionPositionData.IsVisible, Is.EqualTo(true));
        Assert.That(editor.SelectedText, Is.EqualTo("\n- Item 2"));
    }
}
