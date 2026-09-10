using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal class ThematicBreakBlockHandler(StandardMarkdownParser parser) : BlockHandler(parser)
{
    public override Control Handle(Block block, string markdown, LineInformation[] lineInformations)
    {
        ThematicBreakBlock thematicBreakBlock = (ThematicBreakBlock)block;

        if (lineInformations[0].ShowFullText)
        {
            //required for default caret/selection method
            StackPanel mainContainer = new();
            StretchWrapPanel lineContainer = new();

            mainContainer.Children.Add(lineContainer);

            RichTextPresenter richTextPresenter = StylizationHelper.CreateNewPresenter();
            richTextPresenter.Text = new string(thematicBreakBlock.ThematicChar, thematicBreakBlock.ThematicCharCount);

            lineContainer.Children.Add(richTextPresenter);

            return mainContainer;
        }

        return new Border
        {
            Height = 2,
            Background = Brushes.Gray,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center
        };
    }
}
