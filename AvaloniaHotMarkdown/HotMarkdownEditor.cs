using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;
using AvaloniaHotMarkdown.MarkdownParsing;
using Avalonia;
using AvaloniaHotMarkdown.InteractionHandling;
using AvaloniaHotMarkdown.InteractionHandling.KeyCombinations;
using System.Diagnostics;

namespace AvaloniaHotMarkdown
{
    public class HotMarkdownEditor : ContentControl
    {
        public static readonly DirectProperty<HotMarkdownEditor, IBrush> CaretBrushProperty =
            AvaloniaProperty.RegisterDirect<HotMarkdownEditor, IBrush>(nameof(CaretBrush), (hme) => hme.CaretBrush, (hme, b) => hme.CaretBrush = b);
        public IBrush CaretBrush { get; set; } = Brushes.White;
        
        public static readonly DirectProperty<HotMarkdownEditor, IBrush> TextForegroundProperty =
            AvaloniaProperty.RegisterDirect<HotMarkdownEditor, IBrush>(nameof(Foreground), (hme) => hme.Foreground, (hme, b) => hme.Foreground = b);
        public IBrush TextForeground { get; set; } = Brushes.White;
        
        public static readonly DirectProperty<HotMarkdownEditor, IBrush> SelectionBrushProperty =
            AvaloniaProperty.RegisterDirect<HotMarkdownEditor, IBrush>(nameof(SelectionBrush), (hme) => hme.SelectionBrush, (hme, b) => hme.SelectionBrush = b);
        public IBrush SelectionBrush { get; set; } = Brushes.LightBlue;

        public static readonly DirectProperty<HotMarkdownEditor, string> TextProperty =
           AvaloniaProperty.RegisterDirect<HotMarkdownEditor, string>(nameof(Text), (hme) => hme.Text, (hme, s) => hme.Text = s, defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

        List<string> _actualText = [string.Empty];

        public string Text
        {
            get => string.Join("\n", _actualText);
            set
            {
                var old = _actualText;

                _actualText.Clear();
                _actualText.AddRange(value.Split(["\r\n", "\n"], StringSplitOptions.None));
                
                RaisePropertyChanged(TextProperty, string.Join("\n", old), value);
                GenerateText();
                InvalidateVisual();
            }
        }
        
        string selectedText = string.Empty;
        public string SelectedText => selectedText;

        public TextCursor CaretPositionData;
        public TextCursor SelectionPositionData;

        List<AvaloniaBlock> presenters = null!;
        Dictionary<Key, IKeyInteractionHandler> interactions;
        StackPanel mainPanel;
        IMarkdownParser markdownParser;

        MemoryBank memoryBank;
        
        static HotMarkdownEditor()
        {
            //this line allows the control to receive focus
            FocusableProperty.OverrideDefaultValue<HotMarkdownEditor>(true);
        }

        public HotMarkdownEditor()
        {
            CaretPositionData = new TextCursor(0,0, true);
            SelectionPositionData = new TextCursor(0,0, false);

            mainPanel = new StackPanel();  
            Content = mainPanel;

            markdownParser = new StandardMarkdownParser();
            memoryBank = new MemoryBank();

            GenerateInteractions();

            TextInput += OnTextInput;
        }

        public void ReplaceSelectionWith(string text)
        {
            if (string.IsNullOrEmpty(selectedText) || 
                (CaretPositionData.X == SelectionPositionData.X && CaretPositionData.Y == SelectionPositionData.Y))
                return;

            int selectionStartX = 0, selectionEndX = 0;
            int selectionStartY = 0, selectionEndY = 0;

            //selection is to the left
            if (CaretPositionData.Y < SelectionPositionData.Y)
            {
                selectionStartX = CaretPositionData.X;
                selectionStartY = CaretPositionData.Y;
                
                selectionEndX = SelectionPositionData.X;
                selectionEndY = SelectionPositionData.Y;
            }
            //selection is to the right
            else if(CaretPositionData.Y > SelectionPositionData.Y)
            {
                selectionStartX = SelectionPositionData.X;
                selectionStartY = SelectionPositionData.Y;

                selectionEndX = CaretPositionData.X;
                selectionEndY = CaretPositionData.Y;
            }
            //we are on the same line
            else
            {
                selectionStartY = CaretPositionData.Y;
                selectionEndY = CaretPositionData.Y;

                selectionStartX = Math.Min(CaretPositionData.X, SelectionPositionData.X);
                selectionEndX = Math.Max(CaretPositionData.X, SelectionPositionData.X);
            }

            memoryBank.Shorten(new TextCursor(selectionStartX, selectionStartY), SelectedText);

            CaretPositionData = new TextCursor(selectionStartX, selectionStartY);

            if (selectionStartY == selectionEndY)
                _actualText[selectionStartY] = _actualText[selectionStartY].Remove(selectionStartX, selectionEndX - selectionStartX);
            else
            {
                _actualText[selectionStartY] = _actualText[selectionStartY].Remove(selectionStartX);
                _actualText[selectionEndY] = _actualText[selectionEndY].Remove(0, selectionEndX);

                //add remaning contents of last selected line to the first selected line and remove it
                _actualText[selectionStartY] += _actualText[selectionEndY];
                _actualText.RemoveAt(selectionEndY);

                for (int i = selectionEndY - 1; i >= selectionStartY + 1; i--)
                    _actualText.RemoveAt(i);
            }

            SelectionPositionData.IsVisible = false;

            //insert text
            if (string.IsNullOrEmpty(text))
                return;

            Text = Text.Insert(IKeyInteractionHandler.GetGlobalIndexFromLines(CaretPositionData, _actualText), text);

            var lines = text.Split('\n');

            if (lines.Length == 1)
                CaretPositionData.X += lines[0].Length;
            else
            {
                CaretPositionData.Y += lines.Length - 1;
                CaretPositionData.X = lines[^1].Length;
            }
        }

        void GenerateInteractions()
        {
            interactions = new Dictionary<Key, IKeyInteractionHandler>();

            IKeyInteractionHandler[] interactionList = [
                new EnterKeyHandler(),
                new BackKeyHandler(),
                new LeftKeyHandler(),
                new RightKeyHandler(),
                new UpKeyHandler(),
                new DownKeyHandler(),
                new DeleteKeyHandler(),
                new HomeKeyHandler(),
                new EndKeyHandler(),
                new TabKeyHandler(),

                new SelectAllHandler(),
                new CopyHandler(),
                new PasteHandler(),
                new CutHandler(),
                new UndoHandler(),
            ];

            foreach (var interaction in interactionList)
                interactions.Add(interaction.MainKey, interaction);
        }

        private void OnTextInput(object? sender, TextInputEventArgs e)
        {
            var oldText = Text;

            //OnKeyDown will be called first and change SelectionPositionData.IsVisible to false
            if (SelectionPositionData.PreviousIsVisible || SelectionPositionData.IsVisible)
                ReplaceSelectionWith(string.Empty);

            SelectionPositionData.IsVisible = false;

            memoryBank.Append(CaretPositionData, e.Text!);


            if (_actualText[CaretPositionData.Y].Length < CaretPositionData.X)
                CaretPositionData.X = _actualText[CaretPositionData.Y].Length;

            _actualText[CaretPositionData.Y] = _actualText[CaretPositionData.Y].Insert(CaretPositionData.X, e.Text!);
            CaretPositionData.X+=e.Text!.Length;

            RaisePropertyChanged(TextProperty, oldText, Text);

            GenerateText();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            var oldText = Text;

            if (e.Key == Key.LeftShift || e.Key == Key.LeftCtrl)
                return;

            TextCursor oldPosition = CaretPositionData;

            if (interactions.ContainsKey(e.Key))
                interactions[e.Key].HandleCombination(e.KeyModifiers, this, ref _actualText, ref memoryBank);

            if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
            {
                if (SelectionPositionData.IsVisible == false)
                {
                    SelectionPositionData.X = oldPosition.X;
                    SelectionPositionData.Y = oldPosition.Y;
                }

                //check if beginning of the selected text wasnt removed
                if(_actualText.Count > oldPosition.Y && _actualText[oldPosition.Y].Length >= oldPosition.X)
                    SelectionPositionData.IsVisible = true;
            }
            else
                SelectionPositionData.IsVisible = false;

            RaisePropertyChanged(TextProperty, oldText, Text);
            GenerateText();
        }

        /// <summary>
        /// Generates or regenerates text objects that are used to render the markdown text.
        /// </summary>
        void GenerateText()
        {
            presenters = new List<AvaloniaBlock>();
            mainPanel.Children.Clear();

            var blocks = markdownParser.Parse(Text);

            foreach (var block in blocks)
            {
                if (block.StartIndex == 0 && block.EndIndex == 0)
                    continue;

                var lineHandler = new LineHandler(block)
                {
                    CaretBrush = CaretBrush,
                    Foreground = TextForeground,
                    SelectionBrush = SelectionBrush,
                };

                var avaloniaBlock = new AvaloniaBlock()
                {
                    LineHandler = lineHandler,
                    BaseBlock = block,
                };

                lineHandler.OnPointerMoved += (sender, e) => HandleMovedOnBlock(avaloniaBlock, e);
                lineHandler.OnPointerPressed += (sender, e) => HandlePressedBlock(avaloniaBlock, e);
                lineHandler.OnPointerReleased += (sender, e) => HandleReleasedBlock(avaloniaBlock, e);

                presenters.Add(avaloniaBlock);

                mainPanel.Children.Add(lineHandler.LineContainer);
            }

            HandleSelection();
            HandleCursor();
        }

        bool inSelection = false;

        void HandlePressedBlock(AvaloniaBlock block, PointerPressedEventArgs args)
        {
            MoveCaretToPoint(block, args);

            if (!inSelection)
            {
                inSelection = true;
                SelectionPositionData.IsVisible = true;
                SelectionPositionData.X = CaretPositionData.X;
                SelectionPositionData.Y = CaretPositionData.Y;
            }
        }

        void HandleMovedOnBlock(AvaloniaBlock block, PointerEventArgs args)
        {
            if (inSelection)
            {
                /*
                 * avalonia when you press and move the mouse, 
                 * the object who will call this method is the one that was pressed.
                 * not the one that is currently under the pointer.
                 * this is a workaround to fix that.
                 */
                var actualBlock = presenters.FirstOrDefault(x => x.LineHandler.LineContainer.Bounds.Y + x.LineHandler.LineContainer.Bounds.Height > args.GetPosition(this).Y - Padding.Top);

                MoveCaretToPoint(actualBlock, args);
                HandleSelection();
            }
        }

        void HandleReleasedBlock(AvaloniaBlock block, PointerEventArgs args)
        {
            if(!inSelection)
                MoveCaretToPoint(block, args);

            inSelection = false;

            HandleSelection();
            HandleCursor();
        }

        void MoveCaretToPoint(AvaloniaBlock block, PointerEventArgs args)
        {
            if (block.LineHandler is null)
                return;

            var lineHandler = block.LineHandler;
            lineHandler.MoveCaretToPoint(args);

            CaretPositionData.X = lineHandler.CaretIndex;
            CaretPositionData.Y = presenters.IndexOf(block);
        }

        void HandleCursor()
        {
            if(presenters.Count == 0)
                return;

            foreach (var avaloniaBlock in presenters)
            {
                avaloniaBlock.LineHandler.HideCaret();
                avaloniaBlock.LineHandler.InvalidateVisuals();
            }

            var lineHandler = presenters[CaretPositionData.Y].LineHandler;
            lineHandler.CaretBrush = CaretBrush;
            lineHandler.CaretIndex = CaretPositionData.X;
            lineHandler.ShowCaret();
            lineHandler.InvalidateVisuals();
        }

        void HandleSelection()
        {
            if (presenters.Count == 0)
                return;

            foreach (var presenter in presenters)
            {
                presenter.LineHandler.HideSelection();
                presenter.LineHandler.HideCaret();
                presenter.LineHandler.InvalidateVisuals();
            }

            if (!SelectionPositionData.IsVisible)
                return;

            if(SelectionPositionData.Y == CaretPositionData.Y)
            {
                int smaller = SelectionPositionData.X > CaretPositionData.X ? CaretPositionData.X : SelectionPositionData.X;
                int bigger  = SelectionPositionData.X > CaretPositionData.X ? SelectionPositionData.X : CaretPositionData.X;

                selectedText = presenters[SelectionPositionData.Y].LineHandler.ShowSelection(smaller, bigger);
                presenters[SelectionPositionData.Y].LineHandler.InvalidateVisuals();
                return;
            }

            int smallerIndex = SelectionPositionData.Y < CaretPositionData.Y ? SelectionPositionData.Y : CaretPositionData.Y;
            int biggerIndex = SelectionPositionData.Y < CaretPositionData.Y ? CaretPositionData.Y : SelectionPositionData.Y;

            int smallerValue = SelectionPositionData.Y < CaretPositionData.Y ? SelectionPositionData.X : CaretPositionData.X;
            int biggerValue = SelectionPositionData.Y < CaretPositionData.Y ? CaretPositionData.X : SelectionPositionData.X;

            string[] lines = new string[biggerIndex-smallerIndex+1];

            for (int y = smallerIndex; y < biggerIndex; y++)
            {
                lines[y - smallerIndex] = presenters[y].LineHandler.ShowSelection(0, _actualText[y].Length);
                presenters[y].LineHandler.InvalidateVisuals();
            }

            lines[0] = presenters[smallerIndex].LineHandler.ShowSelection(smallerValue, _actualText[smallerIndex].Length);
            presenters[smallerIndex].LineHandler.InvalidateVisuals();

            lines[^1] = presenters[biggerIndex].LineHandler.ShowSelection(0, biggerValue);
            presenters[biggerIndex].LineHandler.InvalidateVisuals();

            selectedText = string.Join("\n", lines);
        }

        public override void Render(DrawingContext context)
        {
            //avalonia will not register keys pressed without this line
            context.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, Bounds.Width, Bounds.Height));

            base.Render(context);
        }
    }
}