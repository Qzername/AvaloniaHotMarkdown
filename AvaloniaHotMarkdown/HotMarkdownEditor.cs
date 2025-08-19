using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;
using AvaloniaHotMarkdown.MarkdownParsing;
using Avalonia;
using System.Diagnostics;

namespace AvaloniaHotMarkdown
{
    public class HotMarkdownEditor : ContentControl
    {
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
                _actualText.AddRange(value.Split(["\r\n", "n"], StringSplitOptions.None));
                
                RaisePropertyChanged(TextProperty, string.Join("\n", old), value);
                RenderText();
            }
        }

        TextCursor textCursor;
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
            textCursor = new TextCursor(0, true);

            mainPanel = new StackPanel();  
            Content = mainPanel;

            markdownParser = new StandardMarkdownParser();

            TextInput += OnTextInput;

            RenderText();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Enter)
            {
                //previ|[ous line] <- substring
                var substring = _actualText[textCursor.Y].Substring(textCursor.X);
                //previ|
                _actualText[textCursor.Y] = _actualText[textCursor.Y].Remove(textCursor.X);

                //previ
                //|
                _actualText.Insert(textCursor.Y+1, string.Empty);

                textCursor.X = 0;
                textCursor.Y++;

                //previ
                //|ous line
                _actualText[textCursor.Y] = substring + _actualText[textCursor.Y];
            }

            if (e.Key == Key.Back)
            {
                //previous line
                //|current line
                if(textCursor.X == 0)
                {
                    var currentLine = _actualText[textCursor.Y];

                    //previous linecurrent line
                    //|current line
                    _actualText[textCursor.Y - 1] += currentLine;

                    //previous linecurrent line
                    _actualText.RemoveAt(textCursor.Y);

                    //previous line|current line
                    textCursor.Y--;
                    textCursor.X = _actualText[textCursor.Y].Length - currentLine.Length; 
                }                
                else //in other case remove just last character
                {
                    _actualText[textCursor.Y] = _actualText[textCursor.Y].Remove(textCursor.X - 1, 1);
                    textCursor.X--;
                }
            }

            if (e.Key == Key.Left)
            {
                if (textCursor.X == 0 && textCursor.Y != 0)
                {
                    textCursor.Y--;
                    textCursor.X = _actualText[textCursor.Y].Length; //move to the end of the previous line
                }
                else if(textCursor.X != 0)
                    textCursor.X--;
            }
            else if (e.Key == Key.Right)
            {
                if (textCursor.X == _actualText[textCursor.Y].Length && textCursor.Y != _actualText.Count-1)
                {
                    textCursor.Y++;
                    textCursor.X = 0; //move to the start of the next line
                }
                else if (textCursor.X != _actualText[textCursor.Y].Length)
                    textCursor.X++;
            }

            if (e.Key == Key.Up && textCursor.Y != 0)
                textCursor.Y--;
            else if (e.Key == Key.Down && textCursor.Y != presenters.Count-1)
                textCursor.Y++;
            
            if(textCursor.X > _actualText[textCursor.Y].Length)
                textCursor.X = _actualText[textCursor.Y].Length;

            RenderText();
        }

        private void OnTextInput(object? sender, TextInputEventArgs e)
        {
            _actualText[textCursor.Y] = _actualText[textCursor.Y].Insert(textCursor.X, e.Text!);
            textCursor.X+=e.Text!.Length;

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

            textCursor.X = lineHandler.CaretIndex;
            textCursor.Y = presenters.IndexOf(block);

            HandleCursor();
        }

        void HandleCursor()
        {
            if(presenters.Count == 0)
                return;
           
            foreach (var avaloniaBlock in presenters)
                avaloniaBlock.LineHandler.HideCaret();

            var lineHandler = presenters[textCursor.Y].LineHandler;
            lineHandler.CaretBrush = Brushes.White;
            lineHandler.CaretIndex = textCursor.X;
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
            public int X { get; set; } = 0;
            public int Y { get; set; } = 0;
            public bool ShowCursor { get; set; } = showCursor;
        }
    }
}