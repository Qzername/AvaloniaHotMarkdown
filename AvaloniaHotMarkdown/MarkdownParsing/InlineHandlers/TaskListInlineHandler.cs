using Avalonia.Controls;
using Markdig.Extensions.TaskLists;
using Markdig.Syntax;

namespace AvaloniaHotMarkdown.MarkdownParsing.InlineHandlers;

internal class TaskListInlineHandler(StandardMarkdownParser parser) : InlineHandler(parser)
{
    const string CheckedButton = "[x]";
    const string UncheckedButton = "[ ]";

    const int XLineOffset = 5; // length of "- [ ]"

    public override void Handle(MarkdownObject inlineObject, InlineParsingContext context, TextUpdateRequestHandler textUpdateRequestHandler)
    {
        var taskList = (TaskList)inlineObject;

        int index = Array.IndexOf(context.CurrentLine.Children.ToArray(), context.CurrentPresenter);
        string checkboxText = taskList.Checked ? CheckedButton : UncheckedButton;

        if (context.ParseAsFullText)
        {
            var checkboxTextControl = StylizationHelper.CreateNewPresenter();
            checkboxTextControl.Text = checkboxText;

            checkboxTextControl.Tag = new CaretPositionOffset(XLineOffset, 0);

            context.CurrentLine.Children.Insert(index, checkboxTextControl);
        }
        else
        {
            var checkbox = new CheckBox
            {
                IsChecked = taskList.Checked
            };

            //for some reason this is required for the mobile version to make button work
            checkbox.Tapped += (s, e) =>
            {
                CheckBox checkbox = (CheckBox)s;
                checkbox.IsChecked = !checkbox.IsChecked;
            };

            checkbox.IsCheckedChanged += (s, e) =>
            {
                textUpdateRequestHandler(checkbox, checkboxText.Length, checkbox.IsChecked.Value ? CheckedButton : UncheckedButton);
            };

            context.XOffset += checkboxText.Length;
            checkbox.Tag = new CaretPositionOffset(XLineOffset, 0);

            context.CurrentLine.Children.Insert(index, checkbox);
        }
    }
}
