using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Markdig.Syntax;
using Markdig;
using Markdig.Syntax.Inlines;
using System.Diagnostics;
using Avalonia.Media;
using Avalonia.Input;
using System.Diagnostics.Tracing;

namespace Avalonia.HotMarkdown
{
    public class HotMarkdown : ContentControl
    {
        string text = string.Empty;

        TextCursor textCursor;
        List<AvaloniaBlock> presenters = null!;

        StackPanel mainPanel;

        IMarkdownParser markdownParser;

        public HotMarkdown()
        {
            //this line allows the control to receive focus
            FocusableProperty.OverrideDefaultValue<HotMarkdown>(true);
            
            textCursor = new TextCursor(0, true);

            mainPanel = new StackPanel();  
            Content = mainPanel;

            markdownParser = new StandardMarkdownParser();

            TextInput += OnTextInput;

            RenderText();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key == Key.Enter)
            {
                text = text.Insert(textCursor.Index, "\n");
                textCursor.Index++;
            }

            if (e.Key == Key.Back)
            {
                text = text.Remove(textCursor.Index-1, 1);
                textCursor.Index--;
            }

            if (e.Key == Key.Left && textCursor.Index > 0)
                textCursor.Index--;
            else if (e.Key == Key.Right && textCursor.Index < text.Length)
                textCursor.Index++;

            if (e.Key == Key.Up)
                MoveCursorLineUp();
            else if (e.Key == Key.Down)
                MoveCursorLineDown();

            RenderText();
        }

        void MoveCursorLineUp()
        {
            var lines = text.Split('\n');

            int countLineLength = 0;

            int finalIndex = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                countLineLength += lines[i].Length+1; //plus one for \n

                if (countLineLength < textCursor.Index)
                    continue;

                finalIndex = i;
                break;
            }

            if (finalIndex == 0)
                return;

            int currentLineLength = lines[finalIndex].Length;
            int previousLineLength = lines[finalIndex - 1].Length;

            int currentLineLeftOffset = currentLineLength - countLineLength + textCursor.Index +1;
            int previousLineRightOffset = previousLineLength - currentLineLeftOffset;

            if (previousLineLength > currentLineLeftOffset)
                textCursor.Index -= currentLineLeftOffset + previousLineRightOffset + 1;
            else
                textCursor.Index -= currentLineLeftOffset + 1;

            HandleCursor();
        }
        

        void MoveCursorLineDown()
        {
            var lines = text.Split('\n');

            int countLineLength = 0;

            int finalIndex = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                countLineLength += lines[i].Length+1; //plus one for \n

                if (countLineLength < textCursor.Index)
                    continue;

                finalIndex = i;
                break;
            }

            if(finalIndex == lines.Length - 1)
                return;

            int currentLineLength = lines[finalIndex].Length;
            int nextLineLength = lines[finalIndex + 1].Length;

            int currentLineRightOffset = countLineLength - textCursor.Index;
            int currentLineLeftOffset = currentLineLength - currentLineRightOffset + 1;//plus one for \n

            if (nextLineLength < currentLineLeftOffset)
                textCursor.Index += currentLineRightOffset + nextLineLength;
            else
                textCursor.Index += currentLineRightOffset + currentLineLeftOffset;

            HandleCursor();
        }

        private void OnTextInput(object? sender, Input.TextInputEventArgs e)
        {
            text = text.Insert(textCursor.Index, e.Text!);

            textCursor.Index += e.Text.Length;

            RenderText();
        }

        void RenderText()
        {
            presenters = new List<AvaloniaBlock>();
            mainPanel.Children.Clear();

            var blocks = markdownParser.Parse(text);

            foreach (var block in blocks)
            {
                if (block.StartIndex == 0 && block.EndIndex == 0)
                    continue;

                string shortText = text.Substring(block.ActualStartIndex, block.EndIndex - block.ActualStartIndex + 1);
                string longText = text.Substring(block.StartIndex, block.EndIndex - block.StartIndex + 1);

                var textPresenter = new TextPresenter()
                {
                    Text = shortText,
                    FontSize = block.FontSize,
                };

                presenters.Add(new AvaloniaBlock()
                {
                    ShortText = shortText,
                    LongText = longText,
                    TextPresenter = textPresenter,
                    BaseBlock = block,
                });

                mainPanel.Children.Add(textPresenter);
            }

            HandleCursor();
        }

        void HandleCursor()
        {
            foreach (var avaloniaBlock in presenters)
            {
                var presenter = avaloniaBlock.TextPresenter;

                presenter.Text = avaloniaBlock.ShortText;
                presenter.HideCaret();
            }

            for(int i = 0 ; i < presenters.Count; i++)
            {
                var avaloniaBlock = presenters[i];
                var presenter = avaloniaBlock.TextPresenter;
                var block = avaloniaBlock.BaseBlock;

                //we are on \n block
                //go to 0 index of the next line
                if (textCursor.Index < block.StartIndex)
                {
                    ConfigurePresenter(presenter, avaloniaBlock, 0);
                    break;
                }

                //we are on actual text on actual block
                if(textCursor.Index <= block.EndIndex+1)
                {
                    ConfigurePresenter(presenter, avaloniaBlock, textCursor.Index - block.StartIndex);
                    break;
                }
            }
        }

        void ConfigurePresenter(TextPresenter presenter, AvaloniaBlock avaloniaBlock, int caretIndex)
        {
            presenter.Text = avaloniaBlock.LongText;
            presenter.FontSize = avaloniaBlock.BaseBlock.FontSize;
            presenter.CaretBrush = Brushes.Red;
            presenter.CaretIndex = caretIndex;
            presenter.ShowCaret();
        }

        //exists for debug reasons
        public override void Render(DrawingContext context)
        {
            context.DrawRectangle(Brushes.Gray, null, new Rect(0, 0, Bounds.Width, Bounds.Height));

            base.Render(context);
        }

        struct TextCursor(int index, bool showCursor)
        {
            public int Index { get; set; } = index;
            public bool ShowCursor { get; set; } = showCursor;
        }
    }
}