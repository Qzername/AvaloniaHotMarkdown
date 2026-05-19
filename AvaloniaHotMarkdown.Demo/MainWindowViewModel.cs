using ReactiveUI;

namespace AvaloniaHotMarkdown.Demo
{
    internal class MainWindowViewModel : ReactiveObject
    {
        string _textSource =
            """


            ---
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
