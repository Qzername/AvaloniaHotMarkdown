using Avalonia.Controls;
using AvaloniaHotMarkdown.MarkdownParsing;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AvaloniaHotMarkdown
{
    public class LineHandler
    {
        public event Action<object, PointerEventArgs> OnPointerMoved;
        public event Action<object, PointerPressedEventArgs> OnPointerPressed;
        public event Action<object, PointerReleasedEventArgs> OnPointerReleased;
        List<RichTextPresenter> presenters;

        public DockPanel LineContainer { get; private set; }
        public IBrush? CaretBrush { get; set; } = Brushes.White;
        public int CaretIndex { get; set; } = 0;

        readonly Block _currentBlock;

        RichTextPresenter currentChild;

        public LineHandler(Block block)
        {
            presenters = new List<RichTextPresenter>();

            _currentBlock = block;

            LineContainer = new DockPanel
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };

            RenderLine(showLongText: false);
        }

        void TextPresenter_PointerMoved(object? sender, PointerEventArgs e) => OnPointerMoved?.Invoke(sender, e);
        void TextPresenter_PointerPressed(object? sender, PointerPressedEventArgs e) => OnPointerPressed?.Invoke(sender, e);
        void TextPresenter_PointerReleased(object? sender, PointerReleasedEventArgs e) => OnPointerReleased?.Invoke(sender, e);
       
        public void ShowCaret()
        {
            HideCaret();

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
            foreach (var presenter in presenters)
                presenter.HideCaret();
        }

        /// <returns>selected text</returns>
        public string ShowSelection(int startSelection, int endSelection)
        {
            HideSelection();

            int currentOffset = 0;

            string selectedText = string.Empty;

            foreach (var presenter in presenters)
            {
                if (string.IsNullOrWhiteSpace(presenter.Text))
                    continue;

                currentOffset += presenter.Text.Length;

                if (currentOffset >= startSelection)
                {
                    int selectionStart = startSelection - (currentOffset - presenter.Text.Length);
                    presenter.SelectionStart = selectionStart;
                    presenter.InvalidateVisual();

                    if (currentOffset >= endSelection)
                    {
                        int selectionEnd = endSelection - (currentOffset - presenter.Text.Length);
                        presenter.SelectionEnd = selectionEnd;

                        selectedText += presenter.Text.Substring(selectionStart, selectionEnd - selectionStart);
                        break;
                    }
                    else
                        selectedText += presenter.Text.Substring(selectionStart);
                }
                else if (currentOffset > startSelection && currentOffset < endSelection)
                    selectedText += presenter.Text;
            }

            return selectedText;
        }

        public void HideSelection()
        {
            foreach (var presenter in presenters)
            {
                presenter.SelectionStart = 0;
                presenter.SelectionEnd = 0;
            }
        }

        public void MoveCaretToPoint(PointerEventArgs e)
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
                    presenter.InvalidateVisual();
                    return;
                }

                allTextLength += presenter.Text.Length;
            }

            presenters[^1].ShowCaret();
            presenters[^1].InvalidateVisual();
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

            TextInfo[] finalArray = _currentBlock.Content[startingIndex..];

            if (!showLongText && _currentBlock.ReplacementPrefix is not null)
                finalArray = [_currentBlock.ReplacementPrefix.Value, .. finalArray];

            foreach (var textInfo in finalArray)
            {
                string endings = HandleCurrentChildEmphasis(textInfo);

                string finalText = textInfo.Text;
                
                if(showLongText)
                    finalText = $"{endings}{textInfo.Text}{endings}";

                ConfirmChild(finalText);
            }
        }

        public void InvalidateVisuals()
        {
            foreach (var presenter in presenters)
                presenter.InvalidateVisual();
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
                SelectionBrush = Brushes.LightBlue,
            };

            textPresenter.PointerMoved += TextPresenter_PointerMoved;
            textPresenter.PointerPressed += TextPresenter_PointerPressed;
            textPresenter.PointerReleased += TextPresenter_PointerReleased;

            return textPresenter;
        }
    }
}