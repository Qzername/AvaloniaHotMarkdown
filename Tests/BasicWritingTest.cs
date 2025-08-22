using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using AvaloniaHotMarkdown;

namespace Tests;

public class BasicWritingTest
{
    [AvaloniaTest]
    public void Basic_Input()
    {
        (Window window, HotMarkdownEditor editor) = BasicWindowBuilder.CreateBasicWindow();

        window.Show();

        editor.Focus();
        window.KeyTextInput("Hello World");

        Assert.That("Hello World", Is.EqualTo(editor.Text));
    }

    [AvaloniaTest]
    public void Basic_Arrow_Movement()
    {
        (Window window, HotMarkdownEditor editor) = BasicWindowBuilder.CreateBasicWindow();

        window.Show();
        editor.Focus();

        window.KeyPressQwerty(PhysicalKey.ArrowLeft, RawInputModifiers.None);
        window.KeyPressQwerty(PhysicalKey.ArrowUp, RawInputModifiers.None);
        window.KeyPressQwerty(PhysicalKey.ArrowRight, RawInputModifiers.None);
        window.KeyPressQwerty(PhysicalKey.ArrowDown, RawInputModifiers.None);

        string inputText = "Hello World";
        window.KeyTextInput(inputText);

        for (int i = 0; i < 5; i++)
            window.KeyPressQwerty(PhysicalKey.ArrowLeft, RawInputModifiers.None);

        Assert.That(editor.CaretPositionData.X, Is.EqualTo(inputText.Length - 5));
    }

    [AvaloniaTest]
    public void Multiline_Input()
    {
        (Window window, HotMarkdownEditor editor) = BasicWindowBuilder.CreateBasicWindow();
        window.Show();
        editor.Focus();

        window.KeyTextInput("Hello World");
        window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);
        window.KeyTextInput("This is a test");
        window.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);
        window.KeyTextInput("This is a third line");

        string expectedText = "Hello World\nThis is a test\nThis is a third line";

        Assert.That(editor.Text, Is.EqualTo(expectedText));
    }
}
