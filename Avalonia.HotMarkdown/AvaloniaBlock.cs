using Avalonia.Controls.Presenters;
using Avalonia.HotMarkdown.MarkdownParsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.HotMarkdown
{
    public struct AvaloniaBlock
    {
        public string ShortText { get; set; }
        public string LongText { get; set; }
        public Block BaseBlock { get; set; }
        public TextPresenter TextPresenter { get; set; }
    }
}
