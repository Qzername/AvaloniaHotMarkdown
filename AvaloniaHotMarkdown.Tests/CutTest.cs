using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Input;

namespace AvaloniaHotMarkdown.Tests;

internal class CutTest : BaseTest
{
    [AvaloniaTest]
    public void Cut_DoubleCutBugCheck()
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
