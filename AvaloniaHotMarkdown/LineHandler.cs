using Avalonia.Controls;
using AvaloniaHotMarkdown.MarkdownParsing;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using System.Diagnostics;

namespace AvaloniaHotMarkdown
{
    public class LineHandler
    {
        public event Action<object, PointerEventArgs> OnPointerMoved;
        public event Action<object, PointerPressedEventArgs> OnPointerPressed;
        public event Action<object, PointerReleasedEventArgs> OnPointerReleased;
        
        List<PresenterInfo> presentersInfos;
        PresenterInfo Prefix => presentersInfos[0];

        public DockPanel LineContainer { get; private set; }
        public IBrush? CaretBrush { get; set; } = Brushes.White;
        public IBrush? Foreground { get; set; } = Brushes.White;
        public IBrush? SelectionBrush { get; set; } = Brushes.LightBlue;
        public int CaretIndex { get; set; } = 0;

        readonly Block _currentBlock;

        public LineHandler(Block block)
        {
            presentersInfos = new List<PresenterInfo>();

            _currentBlock = block;

            LineContainer = new DockPanel
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };

            RenderLine();
        }

        public void RenderLine()
        {
            RichTextPresenter currentChild;

            LineContainer.Children.Clear();
            presentersInfos.Clear();

            currentChild = CreateEmpty();

            foreach (var textInfo in _currentBlock.Content)
            {
                string endings = HandleCurrentChildEmphasis(currentChild, textInfo);

                string shortText = textInfo.Text;

                string longText = textInfo.Text;
                
                if(!string.IsNullOrWhiteSpace(endings))
                    longText = $"{endings}{textInfo.Text}{new string(endings.Reverse().ToArray())}";

                currentChild.Text = shortText;

                LineContainer.Children.Add(currentChild);
                presentersInfos.Add(new PresenterInfo()
                {
                    LongText = longText,
                    ShortText = shortText,
                    Presenter = currentChild
                });

                currentChild = CreateEmpty();
            }

            ShowShortText();
        }

        public void InvalidateVisuals()
        {
            foreach (var presenterInfo in presentersInfos)
            {
                presenterInfo.Presenter.InvalidateVisual();
                presenterInfo.Presenter.InvalidateMeasure();
                presenterInfo.Presenter.InvalidateArrange();
            }
        }

        public void ShowCaret()
        {
            ShowLongText();

            int indexText = 0;

            foreach (var presenterInfo in presentersInfos)
            {
                var presenter = presenterInfo.Presenter;

                var currentLength = presenter.Text.Length;

                if (CaretIndex <= indexText + currentLength)
                {
                    presenter.CaretIndex = CaretIndex - indexText;
                    presenter.ShowCaret();
                    break;
                }

                indexText += currentLength;
            }
        }

        public void HideCaret()
        {
            foreach (var presenterInfo in presentersInfos)
                presenterInfo.Presenter.HideCaret();
        }

        /// <returns>selected text</returns>
        public string ShowSelection(int startSelectionIndex, int endSelectionIndex)
        {
            ShowLongText();

            int currentOffset = 0;

            string selectedText = string.Empty;

            foreach (var presenterInfo in presentersInfos)
            {
                var presenter = presenterInfo.Presenter;
                var presenterText = presenterInfo.LongText;

                if (string.IsNullOrWhiteSpace(presenterText))
                    continue;

                //Fir[stSecondTh]ird
                if(currentOffset + presenterText.Length > startSelectionIndex)
                {
                    presenter.SelectionStart = startSelectionIndex - currentOffset;

                    if (currentOffset + presenterText.Length > endSelectionIndex)
                        presenterInfo.Presenter.SelectionEnd = endSelectionIndex - currentOffset;
                    else
                        presenter.SelectionEnd = presenterText.Length;

                    selectedText += presenterText.Substring(presenter.SelectionStart, presenter.SelectionEnd-presenter.SelectionStart);
                }

                currentOffset += presenterText.Length;

                if (currentOffset > endSelectionIndex)
                    break;
            }

            return selectedText;
        }

        public void HideSelection()
        {
            ShowShortText();

            foreach (var presenterInfos in presentersInfos)
            {
                var presenter = presenterInfos.Presenter;
                presenter.SelectionStart = 0;
                presenter.SelectionEnd = 0;
            }
        }

        public void MoveCaretToPoint(PointerEventArgs e)
        {
            int allTextLength = 0;

            for (int i = 0; i < presentersInfos.Count; i++)
            {
                var presenter = presentersInfos[i].Presenter;

                presenter.MoveCaretToPoint(e.GetPosition(presenter));

                if (presenter.CaretIndex < presenter.Text.Length)
                {
                    CaretIndex = allTextLength + presenter.CaretIndex;
                    presenter.ShowCaret();
                    presenter.InvalidateVisual();
                    return;
                }

                allTextLength += presenter.Text.Length;
            }

            presentersInfos[^1].Presenter.ShowCaret();
            presentersInfos[^1].Presenter.InvalidateVisual();
            CaretIndex = allTextLength;
        }

        string HandleCurrentChildEmphasis(RichTextPresenter currentChild, TextInfo textInfo)
        {
            string ending = textInfo.DelimiterText;

            if (textInfo.IsBold)
                currentChild.FontWeight = FontWeight.Bold;

            if (textInfo.IsItalic)
                currentChild.FontStyle = FontStyle.Italic;

            currentChild.ShowStrikethrough = textInfo.IsStrikethrough;
            currentChild.ShowUnderline = textInfo.IsUnderline;

            return ending;
        }

        void ShowLongText()
        {
            foreach (var presenterInfo in presentersInfos)
                presenterInfo.Presenter.Text = presenterInfo.LongText;

            if (_currentBlock.ReplacementPrefix is not null)
                Prefix.Presenter.Text = _currentBlock.Content[0].Text;
            else
                Prefix.Presenter.IsVisible = true;

            InvalidateVisuals();
        }

        void ShowShortText()
        {
            foreach (var presenterInfo in presentersInfos)
                presenterInfo.Presenter.Text = presenterInfo.ShortText;

            if (_currentBlock.ReplacementPrefix is not null)
                Prefix.Presenter.Text = _currentBlock.ReplacementPrefix.Value.Text;
            else
                Prefix.Presenter.IsVisible = false;

            InvalidateVisuals();
        }

        RichTextPresenter CreateEmpty()
        {
            var textPresenter = new RichTextPresenter()
            {
                Text = string.Empty,
                FontSize = _currentBlock.FontSize,
                SelectionBrush = SelectionBrush,
                CaretBrush = CaretBrush,
                Foreground = Foreground,
            };

            textPresenter.PointerMoved += TextPresenter_PointerMoved;
            textPresenter.PointerPressed += TextPresenter_PointerPressed;
            textPresenter.PointerReleased += TextPresenter_PointerReleased;

            return textPresenter;
        }

        void TextPresenter_PointerMoved(object? sender, PointerEventArgs e) => OnPointerMoved?.Invoke(sender, e);
        void TextPresenter_PointerPressed(object? sender, PointerPressedEventArgs e) => OnPointerPressed?.Invoke(sender, e);
        void TextPresenter_PointerReleased(object? sender, PointerReleasedEventArgs e) => OnPointerReleased?.Invoke(sender, e);

        struct PresenterInfo
        {
            public string LongText { get; set; }
            public string ShortText { get; set; }
            public RichTextPresenter Presenter { get; set; }
        }
    }
}