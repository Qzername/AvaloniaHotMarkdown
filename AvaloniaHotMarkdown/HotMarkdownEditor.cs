using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;
using AvaloniaHotMarkdown.MarkdownParsing;
using Avalonia;
using AvaloniaHotMarkdown.InteractionHandling;

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
        public TextCursor CaretPositionData;

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

            mainPanel = new StackPanel();  
            Content = mainPanel;

            markdownParser = new StandardMarkdownParser();

            GenerateInteractions();

            TextInput += OnTextInput;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            var oldText = Text;

            if(interactions.ContainsKey(e.Key))
                interactions[e.Key].HandleCombination(e.KeyModifiers, ref _actualText, ref CaretPositionData);
            
            RaisePropertyChanged(TextProperty, oldText, Text);
            GenerateText();
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

            CaretPositionData.X = lineHandler.CaretIndex;
            CaretPositionData.Y = presenters.IndexOf(block);

            HandleCursor();
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

        public override void Render(DrawingContext context)
        {
            //avalonia will not register keys pressed without this line
            context.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, Bounds.Width, Bounds.Height));

            base.Render(context);
        }
    }
}