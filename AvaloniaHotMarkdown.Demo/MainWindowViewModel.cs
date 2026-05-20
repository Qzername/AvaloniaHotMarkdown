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

            `Code Inline`

            10^10^ H~2~0

            ---

            1. Ordered Item 1
            2. Ordered Item 2
            3. Ordered Item 3

            - [ ] Checkboxes

            | basic | pipe | table |
            | --- | --- | --- |
            | row1 | row1 | row1 |
            | row2 | row2 | row2 |

            [Link to the repo](https://github.com/Qzername/AvaloniaHotMarkdown)

            ![image](avares://AvaloniaHotMarkdown.Demo/picture.png)
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
