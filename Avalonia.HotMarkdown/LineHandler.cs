using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.HotMarkdown.MarkdownParsing;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using System.Diagnostics;

namespace Avalonia.HotMarkdown
{
    public class LineHandler
    {
        public event Action<object, PointerReleasedEventArgs> OnPointerReleased;
        List<RichTextPresenter> presenters;

        public StackPanel LineContainer { get; private set; }
        public IBrush? CaretBrush { get; set; } = Brushes.White;
        public int CaretIndex { get; set; } = 0;

        readonly Block _currentBlock;

        RichTextPresenter currentChild;

        public LineHandler(Block block)
        {
            presenters = new List<RichTextPresenter>();

            _currentBlock = block;

            LineContainer = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            RenderLine(showLongText: false);
        }

        void TextPresenter_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            OnPointerReleased?.Invoke(sender, e);
        }

        public void ShowCaret()
        {
            RenderLine(showLongText: true);

            int indexText = 0;

            foreach (var presenter in presenters)
            {
                var currentLength = presenter.Text.Length;

                if (CaretIndex <= indexText + currentLength)
                {
                    presenter.CaretBrush = Brushes.Red;

                    presenter.CaretIndex = CaretIndex - indexText;
                    presenter.ShowCaret();
                    break;
                }

                indexText += currentLength;
            }
        }

        public void HideCaret()
        {
            RenderLine(showLongText: false);

            foreach (var presenter in presenters)
                presenter.HideCaret();
        }

        public void MoveCaretToPoint(PointerReleasedEventArgs e)
        {
            int allTextLength = 0;

            for (int i = 0; i < presenters.Count; i++)
            {
                var presenter = presenters[i];

                presenter.MoveCaretToPoint(e.GetPosition(presenter));

                if(presenter.CaretIndex < presenter.Text.Length)
                {
                    CaretIndex = allTextLength + presenter.CaretIndex;
                    presenter.ShowCaret();
                    return;
                }

                allTextLength += presenter.Text.Length;
            }

            presenters[^1].ShowCaret();
            CaretIndex = allTextLength;
        }

        public void RenderLine(bool showLongText)
        {
            LineContainer.Children.Clear();
            presenters.Clear();

            currentChild = CreateEmpty();

            int startingIndex = showLongText ? 0 : 1;

            if(_currentBlock.Content.Length == 0)
                startingIndex = 0;

            foreach (var textInfo in _currentBlock.Content[startingIndex..])
            {
                string endings = HandleCurrentChildEmphasis(textInfo);

                string finalText = textInfo.Text;
                
                if(showLongText)
                    finalText = $"{endings}{textInfo.Text}{endings}";

                ConfirmChild(finalText);
            }
        }

        void ConfirmChild(string text)
        {
            currentChild.Text = text;

            LineContainer.Children.Add(currentChild);
            presenters.Add(currentChild);

            currentChild = CreateEmpty();
        }

        string HandleCurrentChildEmphasis(TextInfo textInfo)
        {
            string ending = string.Empty;

            if(textInfo.IsBold)
            {
                ending += "**";
                currentChild.FontWeight = FontWeight.Bold;
            }

            if(textInfo.IsItalic)
            {
                ending += "*";
                currentChild.FontStyle = FontStyle.Italic;
            }

            if(textInfo.IsStrikethrough)
            {
                ending += "~~";
                currentChild.ShowStrikethrough = true;
            }

            if(textInfo.IsUnderline)
            {
                ending += "++";
                currentChild.ShowUnderline = true;
            }

            return ending;
        }

        RichTextPresenter CreateEmpty()
        {
            var textPresenter = new RichTextPresenter()
            {
                Text = string.Empty,
                FontSize = _currentBlock.FontSize,
            };

            textPresenter.PointerReleased += TextPresenter_PointerReleased; ;

            return textPresenter;
        }
    }
}