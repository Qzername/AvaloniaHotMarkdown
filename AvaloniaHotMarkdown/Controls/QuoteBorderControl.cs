using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AvaloniaHotMarkdown.Controls;

internal class QuoteBorderControl : Border
{
    public QuoteBorderControl()
    {
        //default style
        //TODO: make it customizable
        BorderThickness = new Thickness(4, 0, 0, 0);
        BorderBrush = Brushes.Gray;
        Padding = new Thickness(15, 10);
        Margin = new Thickness(0, 10);
    }
}