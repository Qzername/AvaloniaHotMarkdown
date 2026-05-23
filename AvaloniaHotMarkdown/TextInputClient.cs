using Avalonia;
using Avalonia.Controls;
using Avalonia.Input.TextInput;

namespace AvaloniaHotMarkdown;

public class MyTextInputClient : TextInputMethodClient
{
    private readonly Control _parent;

    public MyTextInputClient(Control parent)
    {
        _parent = parent;
    }

    public override Visual TextViewVisual => _parent;
    public override Rect CursorRectangle => new Rect(0, 0, 10, 20);
    public override string SurroundingText => string.Empty;
    public override TextSelection Selection { get; set; } = new TextSelection();
    public override bool SupportsSurroundingText => false;
    public override bool SupportsPreedit => false;

    public void OnTapped()
    {
        RaiseInputPaneActivationRequested();
    }
}