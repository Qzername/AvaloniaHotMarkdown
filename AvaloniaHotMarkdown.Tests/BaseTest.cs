using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;

namespace AvaloniaHotMarkdown.Tests;

public abstract class BaseTest
{
    Window _currentTarget;

    protected void ActivateTarget(Window window, HotMarkdownEditor editor)
    {
        window.Show();
        editor.Focus();

        _currentTarget = window;
    }

    /// <summary>
    /// press the key and release it
    /// </summary>
    protected void HandleKey(PhysicalKey key, RawInputModifiers modifiers = RawInputModifiers.None)
    {
        _currentTarget.KeyPressQwerty(key, modifiers);
        _currentTarget.KeyReleaseQwerty(key, modifiers);
    }

    /// <summary>
    /// press the key with shift and release it
    /// </summary>
    protected void HandleKeySelection(PhysicalKey key)
    {
        _currentTarget.KeyPressQwerty(key, RawInputModifiers.Shift);
        _currentTarget.KeyReleaseQwerty(key, RawInputModifiers.Shift);
    }

    /// <summary>
    /// press the enter key and release it
    /// </summary>
    protected void Enter()
    {
        _currentTarget.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);
        _currentTarget.KeyReleaseQwerty(PhysicalKey.Enter, RawInputModifiers.None);
    }

    /// <summary>
    /// Handle text input by sending a string to the current target.
    /// </summary>
    protected void HandleTextInput(string text) => _currentTarget.KeyTextInput(text);

    /// <summary>
    /// Insert text into the clipboard of the current target.
    /// </summary>
    protected void InsertIntoClipboard(string text)
    {
        _currentTarget.Clipboard!.SetTextAsync(text);
    }

    protected void Copy()
    {
        _currentTarget.KeyPressQwerty(PhysicalKey.C, RawInputModifiers.Control);
        _currentTarget.KeyReleaseQwerty(PhysicalKey.C, RawInputModifiers.Control);
    }

    protected void Cut()
    {
        _currentTarget.KeyPressQwerty(PhysicalKey.X, RawInputModifiers.Control);
        _currentTarget.KeyReleaseQwerty(PhysicalKey.X, RawInputModifiers.Control);
    }

    protected void Paste()
    {
        _currentTarget.KeyPressQwerty(PhysicalKey.V, RawInputModifiers.Control);
        _currentTarget.KeyReleaseQwerty(PhysicalKey.V, RawInputModifiers.Control);
    }
}
