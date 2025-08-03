using Avalonia.Controls.Presenters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.HotMarkdown
{
    public struct AvaloniaBlock
    {
        public string Content { get; set; }
        public int FontSize { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }   
        public TextPresenter TextPresenter { get; set; }
    }
}
