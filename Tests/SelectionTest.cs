using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using AvaloniaHotMarkdown;

namespace Tests;

internal class SelectionTest
{
    [AvaloniaTest]
    public void Selection_OneLine()
    {
        (Window window, HotMarkdownEditor editor) = BasicWindowBuilder.CreateBasicWindow();
        window.Show();
        editor.Focus();

        string inputText = "Hello World";
        window.KeyTextInput(inputText);

        Assert.That(editor.SelectionPositionData.IsVisible, Is.EqualTo(false));
        
        window.KeyPressQwerty(PhysicalKey.ArrowLeft, RawInputModifiers.Shift);
        window.KeyReleaseQwerty(PhysicalKey.ArrowLeft, RawInputModifiers.Shift);

        Assert.That(editor.SelectionPositionData.IsVisible, Is.EqualTo(true));
        Assert.That(editor.SelectedText, Is.EqualTo("d"));
    }

    [AvaloniaTest]
    public void Selection_MultiLine()
    {
        (Window window, HotMarkdownEditor editor) = BasicWindowBuilder.CreateBasicWindow();

        window.Show();
        editor.Focus();

        window.KeyTextInput("Hello");
        window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);
        window.KeyReleaseQwerty(PhysicalKey.Enter, RawInputModifiers.None);
        window.KeyTextInput("World");
        window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);
        window.KeyReleaseQwerty(PhysicalKey.Enter, RawInputModifiers.None);
        window.KeyTextInput("Word");

        //Hello
        //World
        //Word|

        Assert.That(editor.SelectionPositionData.IsVisible, Is.EqualTo(false));

        //Hello
        //World
        //Wor|d
        window.KeyPressQwerty(PhysicalKey.ArrowLeft, RawInputModifiers.None);
        window.KeyReleaseQwerty(PhysicalKey.ArrowLeft, RawInputModifiers.None);

        //Hello
        //Wor|[ld
        //Wor]d
        window.KeyPressQwerty(PhysicalKey.ArrowUp, RawInputModifiers.Shift);
        window.KeyReleaseQwerty(PhysicalKey.ArrowUp, RawInputModifiers.Shift);
        Assert.That(editor.SelectionPositionData.IsVisible, Is.EqualTo(true));
        Assert.That(editor.SelectedText, Is.EqualTo("ld\nWor"));

        //Hel|[lo
        //World
        //Wor]d
        window.KeyPressQwerty(PhysicalKey.ArrowUp, RawInputModifiers.Shift);
        window.KeyReleaseQwerty(PhysicalKey.ArrowUp, RawInputModifiers.Shift);

        Assert.That(editor.SelectionPositionData.IsVisible, Is.EqualTo(true));
        Assert.That(editor.SelectedText, Is.EqualTo("lo\nWorld\nWor"));
    }
}
