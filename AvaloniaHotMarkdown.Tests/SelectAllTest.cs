using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Input;

namespace AvaloniaHotMarkdown.Tests;

internal class SelectAllTest : BaseTest
{
    [AvaloniaTest]
    public void SelectAll()
    {
        (Window window, HotMarkdownEditor editor) = BasicWindowBuilder.CreateBasicWindow();
        ActivateTarget(window, editor);

        for (int i = 0; i < 3; i++)
        {
            HandleTextInput("Hello");
            Enter();
        }

        Assert.That(editor.SelectionPositionData.IsVisible, Is.EqualTo(false));

        HandleKeySelection(PhysicalKey.ArrowUp);
        HandleKeySelection(PhysicalKey.ArrowRight);
        HandleKeySelection(PhysicalKey.ArrowRight);
        HandleKeySelection(PhysicalKey.ArrowRight);

        HandleKey(PhysicalKey.A, RawInputModifiers.Control);

        Assert.That(editor.SelectionPositionData.IsVisible, Is.EqualTo(true));
        Assert.That(editor.SelectedText, Is.EqualTo("Hello\nHello\nHello\n"));
    }

    [AvaloniaTest]
    public void SelectAll_WhileSomethingIsSelected()
    {
        (Window window, HotMarkdownEditor editor) = BasicWindowBuilder.CreateBasicWindow();
        ActivateTarget(window, editor);
        for (int i = 0; i < 3; i++)
        {
            HandleTextInput("Hello");
            Enter();
        }
        Assert.That(editor.SelectionPositionData.IsVisible, Is.EqualTo(false));
        HandleKey(PhysicalKey.ArrowLeft);
        HandleKey(PhysicalKey.ArrowUp);
        HandleKey(PhysicalKey.ArrowLeft);

        //Hello
        //H[ell]o
        //Hello
        for(int i = 0; i < 3; i++)
            HandleKeySelection(PhysicalKey.ArrowLeft);

        Assert.That(editor.SelectedText, Is.EqualTo("ell"));

        HandleKey(PhysicalKey.A, RawInputModifiers.Control);
        
        Assert.That(editor.SelectedText, Is.EqualTo("Hello\nHello\nHello\n"));
    }
}