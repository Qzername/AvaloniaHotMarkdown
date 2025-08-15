using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;
using AvaloniaHotMarkdown.MarkdownParsing;
using Avalonia;

namespace AvaloniaHotMarkdown
{
    public class HotMarkdownEditor : ContentControl
    {
        public static readonly StyledProperty<string> TextProperty =
           AvaloniaProperty.Register<HotMarkdownEditor, string>(nameof(Text), defaultValue: string.Empty);

        public string Text
        {
            get => GetValue(TextProperty);
            set
            {
                SetValue(TextProperty, value);
                RenderText();
            }
        }

        TextCursor textCursor;
        List<AvaloniaBlock> presenters = null!;

        StackPanel mainPanel;

        IMarkdownParser markdownParser;

        public HotMarkdownEditor()
        {
            //this line allows the control to receive focus
            FocusableProperty.OverrideDefaultValue<HotMarkdownEditor>(true);
            
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

            if (e.Key == Key.Enter && Text[textCursor.Index - 1] != '\n')
            {
                Text = Text.Insert(textCursor.Index, "\n");
                textCursor.Index++;
            }

            if (e.Key == Key.Back)
            {
                Text = Text.Remove(textCursor.Index-1, 1);
                textCursor.Index--;
            }

            if (e.Key == Key.Left && textCursor.Index > 0)
                textCursor.Index--;
            else if (e.Key == Key.Right && textCursor.Index < Text.Length)
                textCursor.Index++;

            if (e.Key == Key.Up)
                MoveCursorLineUp();
            else if (e.Key == Key.Down)
                MoveCursorLineDown();

            RenderText();
        }

        void MoveCursorLineUp()
        {
            var lines = Text.Split('\n');

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
            var lines = Text.Split('\n');

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

        private void OnTextInput(object? sender, TextInputEventArgs e)
        {
            Text = Text.Insert(textCursor.Index, e.Text!);

            textCursor.Index += e.Text.Length;

            RenderText();
        }

        void RenderText()
        {
            presenters = new List<AvaloniaBlock>();
            mainPanel.Children.Clear();

            var blocks = markdownParser.Parse(Text);

            foreach (var block in blocks)
            {
                if (block.StartIndex == 0 && block.EndIndex == 0)
                    continue;

                string shortText = Text.Substring(block.ActualStartIndex, block.EndIndex - block.ActualStartIndex);
                string longText = Text.Substring(block.StartIndex, block.EndIndex - block.StartIndex);

                var lineHandler = new LineHandler(block);

                var avaloniaBlock = new AvaloniaBlock()
                {
                    ShortText = shortText,
                    LongText = longText,
                    LineHandler = lineHandler,
                    BaseBlock = block,
                };

                lineHandler.OnPointerReleased += (sender, e) => HandleClickedBlock(avaloniaBlock, e);

                presenters.Add(avaloniaBlock);

                mainPanel.Children.Add(lineHandler.LineContainer);
            }

            HandleCursor();
        }

        void HandleClickedBlock(AvaloniaBlock block, PointerReleasedEventArgs args)
        {
            var lineHandler = block.LineHandler;
            lineHandler.MoveCaretToPoint(args);

            textCursor.Index = block.BaseBlock.StartIndex + lineHandler.CaretIndex;

            HandleCursor();
        }

        void HandleCursor()
        {
            foreach (var avaloniaBlock in presenters)
                avaloniaBlock.LineHandler.HideCaret();

            for (int i = 0 ; i < presenters.Count; i++)
            {
                var avaloniaBlock = presenters[i];
                var lineHandler = avaloniaBlock.LineHandler;
                var block = avaloniaBlock.BaseBlock;

                //we are on \n block
                //go to 0 index of the next line
                if (textCursor.Index < block.StartIndex)
                {
                    ConfigurePresenter(lineHandler, avaloniaBlock, 0);
                    break;
                }

                //we are on actual text on actual block
                if(textCursor.Index <= block.EndIndex+1)
                {
                    ConfigurePresenter(lineHandler, avaloniaBlock, textCursor.Index - block.StartIndex);
                    break;
                }
            }
        }

        void ConfigurePresenter(LineHandler lineHandler, AvaloniaBlock avaloniaBlock, int caretIndex)
        {
            lineHandler.CaretBrush = Brushes.Red;
            lineHandler.CaretIndex = caretIndex;
            lineHandler.ShowCaret();
        }

        public override void Render(DrawingContext context)
        {
            //avalonia will not register keys pressed without this line
            context.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, Bounds.Width, Bounds.Height));

            base.Render(context);
        }

        struct TextCursor(int index, bool showCursor)
        {
            public int Index { get; set; } = index;
            public bool ShowCursor { get; set; } = showCursor;
        }
    }
}