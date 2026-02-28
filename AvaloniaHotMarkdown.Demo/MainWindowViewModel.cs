using ReactiveUI;

namespace AvaloniaHotMarkdown.Demo
{
    internal class MainWindowViewModel : ReactiveObject
    {
        string _textSource =
            """
            # Header 1
            ## Header 2
            ### Header 3

            This is a **bold** text and this is an *italic* text. ~~Strikethrough~~ text is also supported. __Underline__ text is here. ==Hightlighted== text too.

            - Unordered Item 1
            - Unordered Item 2
            - Unordered Item 3

            1. Ordered Item 1
            2. Ordered Item 2
            3. Ordered Item 3
            """;

        public string TextSource
        {
            get => _textSource;
            set
            {
                _textSource = value;
                this.RaiseAndSetIfChanged(ref _textSource, value);
            }
        }
    }
}
