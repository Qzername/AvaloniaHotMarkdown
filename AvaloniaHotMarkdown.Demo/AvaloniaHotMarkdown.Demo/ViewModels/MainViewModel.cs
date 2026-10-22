
using ReactiveUI;

namespace AvaloniaHotMarkdown.Demo.ViewModels;

public class MainViewModel : ViewModelBase
{
    string _textSource =
            """
            asdasdasasdas **asdasda** asdsad
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
