using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Markdig.Syntax;
using Markdig;
using Markdig.Syntax.Inlines;
using System.Diagnostics;
using Avalonia.Media;
using Avalonia.Input;

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

            if (e.Key == Key.Left)
                textCursor.Index--;
            else if (e.Key == Key.Right)
                textCursor.Index++;

            RenderText();
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

            Debug.WriteLine(text.Length);

            foreach (var block in blocks)
            {
                if (block.StartIndex == 0 && block.EndIndex == 0)
                    continue;

                var textPresenter = new TextPresenter()
                {
                    Text = text.Substring(block.StartIndex, block.EndIndex-block.StartIndex+1),
                    FontSize = block.FontSize,
                };

                presenters.Add(new AvaloniaBlock()
                {
                    StartIndex = block.StartIndex,
                    EndIndex = block.EndIndex,
                    TextPresenter = textPresenter
                });
                mainPanel.Children.Add(textPresenter);
            }

            HandleCursor();
        }

        void HandleCursor()
        {
            foreach(var block in presenters)
            {
                var presenter = block.TextPresenter;

                if(textCursor.Index < block.StartIndex)
                {
                    Debug.WriteLine("yep");

                    presenter.CaretBrush = Brushes.Red;
                    presenter.CaretIndex = 0;
                    presenter.ShowCaret();
                    break;
                }

                if(textCursor.Index <= block.EndIndex+1)
                {
                    Debug.WriteLine("ye2p");

                    presenter.CaretBrush = Brushes.Red;
                    presenter.CaretIndex = textCursor.Index - block.StartIndex;
                    presenter.ShowCaret();
                    break;
                }
            }
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