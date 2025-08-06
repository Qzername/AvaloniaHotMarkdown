using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.HotMarkdown.MarkdownParsing
{
    public struct TextInfo
    {
        public string Text;
        public bool IsBold;
        public bool IsItalic;

        public TextInfo(string text, bool isBold = false, bool isItalic = false)
        {
            Text = text;
            IsBold = isBold;
            IsItalic = isItalic;
        }

        public TextInfo()
        {
            Text = string.Empty;
            IsBold = false;
            IsItalic = false;
        }
    }
}
