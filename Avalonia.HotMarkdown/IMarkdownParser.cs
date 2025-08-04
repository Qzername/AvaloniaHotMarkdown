using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.HotMarkdown
{
    public interface IMarkdownParser
    {
        public Block[] Parse(string markdownText);
    }
}
