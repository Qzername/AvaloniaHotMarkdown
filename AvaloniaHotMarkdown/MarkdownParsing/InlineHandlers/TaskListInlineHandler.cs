using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Markdig.Extensions.TaskLists;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers;

internal class TaskListInlineHandler : IInlineHandler
{
    public void Handle(MarkdownObject inlineObject, InlineParsingContext context, TextUpdateRequestHandler textUpdateRequestHandler)
    {
        var taskList = (TaskList)inlineObject;

        int index = Array.IndexOf(context.CurrentLine.Children.ToArray(), context.CurrentPresenter);
        string checkboxText = taskList.Checked ? "- [x]" : "- [ ]";

        if (context.ParseAsFullText)
        {
            var checkboxTextControl = StylizationHelper.CreateNewPresenter();
            checkboxTextControl.Text = checkboxText;

            context.CurrentLine.Children.Insert(index, checkboxTextControl);
        }
        else
        {
            var checkbox = new CheckBox
            {
                IsChecked = taskList.Checked
            };

            checkbox.IsCheckedChanged += (s, e) =>
            {
                textUpdateRequestHandler(checkbox, checkboxText.Length + 1, checkbox.IsChecked.Value ? "- [x] " : "- [ ] ");
            };

            context.XOffset += checkboxText.Length;

            checkbox.Tag = new CaretPositionOffset(context.XOffset, 0);

            context.CurrentLine.Children.Insert(index, checkbox);
        }
    }
}
