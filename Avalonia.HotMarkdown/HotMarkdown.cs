using Avalonia.Controls;

namespace Avalonia.HotMarkdown
{
    public class HotMarkdown : ContentControl
    {
        public HotMarkdown()
        { 
            var panel = new StackPanel();
            panel.Children.Add(new TextBlock()
            {
                Text = "Hello, World! Test",
            });

            Content = panel;
        }
    }
}