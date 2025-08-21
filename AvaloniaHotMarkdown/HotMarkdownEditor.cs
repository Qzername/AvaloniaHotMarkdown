using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;
using AvaloniaHotMarkdown.MarkdownParsing;
using Avalonia;
using AvaloniaHotMarkdown.InteractionHandling;
using System.Diagnostics;

namespace AvaloniaHotMarkdown
{
    public class HotMarkdownEditor : ContentControl
    {
        public static readonly DirectProperty<HotMarkdownEditor, string> TextProperty =
           AvaloniaProperty.RegisterDirect<HotMarkdownEditor, string>(nameof(Text), (hme) => hme.Text, (hme, s) => hme.Text = s, defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

        List<string> _actualText = [string.Empty];
        Dictionary<Key, IKeyInteractionHandler> interactions;

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
            }
        }
        string selectedText = string.Empty;

        public TextCursor CaretPositionData;
        public TextCursor SelectionPositionData;

        List<AvaloniaBlock> presenters = null!;
        StackPanel mainPanel;
        IMarkdownParser markdownParser;
        
        static HotMarkdownEditor()
        {
            //this line allows the control to receive focus
            FocusableProperty.OverrideDefaultValue<HotMarkdownEditor>(true);
        }

        public HotMarkdownEditor()
        {
            CaretPositionData = new TextCursor(0, true);
            SelectionPositionData = new TextCursor(0, true);

            mainPanel = new StackPanel();  
            Content = mainPanel;

            markdownParser = new StandardMarkdownParser();

            GenerateInteractions();

            TextInput += OnTextInput;
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
            ];

            foreach (var interaction in interactionList)
                interactions.Add(interaction.MainKey, interaction);
        }

        private void OnTextInput(object? sender, TextInputEventArgs e)
        {
            var oldText = Text;

            _actualText[CaretPositionData.Y] = _actualText[CaretPositionData.Y].Insert(CaretPositionData.X, e.Text!);
            CaretPositionData.X+=e.Text!.Length;

            RaisePropertyChanged(TextProperty, oldText, Text);

            GenerateText();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if(e.KeyModifiers.HasFlag(KeyModifiers.Shift))
            {
                if(SelectionPositionData.IsVisible == false)
                {
                    SelectionPositionData.X = CaretPositionData.X;
                    SelectionPositionData.Y = CaretPositionData.Y;
                }

                SelectionPositionData.IsVisible = true;
            }
            else
                SelectionPositionData.IsVisible = false;    

            var oldText = Text;

            if (interactions.ContainsKey(e.Key))
                interactions[e.Key].HandleCombination(e.KeyModifiers, ref _actualText, ref CaretPositionData, ref SelectionPositionData);

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

                var lineHandler = new LineHandler(block);

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

            HandleCursor();
            HandleSelection();
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
            Debug.WriteLine(presenters.IndexOf(block) + " " +args.GetPosition(this));

            if (inSelection)
            {
                MoveCaretToPoint(block, args);
                HandleSelection();
            }
        }

        void HandleReleasedBlock(AvaloniaBlock block, PointerEventArgs args)
        {
            inSelection = false;

            MoveCaretToPoint(block, args);

            HandleCursor();
            HandleSelection();
        }

        void MoveCaretToPoint(AvaloniaBlock block, PointerEventArgs args)
        {
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
                avaloniaBlock.LineHandler.HideCaret();

            var lineHandler = presenters[CaretPositionData.Y].LineHandler;
            lineHandler.CaretBrush = Brushes.White;
            lineHandler.CaretIndex = CaretPositionData.X;
            lineHandler.ShowCaret();
        }

        void HandleSelection()
        {
            if (presenters.Count == 0)
                return;

            foreach (var presenter in presenters)
            {
                presenter.LineHandler.HideSelection();
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