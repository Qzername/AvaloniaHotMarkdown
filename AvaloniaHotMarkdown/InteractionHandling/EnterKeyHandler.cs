using Avalonia.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaHotMarkdown.InteractionHandling
{
    internal class EnterKeyHandler : IKeyInteractionHandler
    {
        public Key MainKey => Key.Enter;

        public void HandleCombination(KeyModifiers keyModifiers, HotMarkdownEditor editor, ref List<string> actualText, ref MemoryBank memoryBank)
        {
            //previ|[ous line] <- substring
            var substring = actualText[editor.CaretPositionData.Y].Substring(editor.CaretPositionData.X);
            //previ|
            actualText[editor.CaretPositionData.Y] = actualText[editor.CaretPositionData.Y].Remove(editor.CaretPositionData.X);

            //previ
            //|
            actualText.Insert(editor.CaretPositionData.Y + 1, string.Empty);

            editor.CaretPositionData.X = 0;
            editor.CaretPositionData.Y++;

            //previ
            //|ous line
            actualText[editor.CaretPositionData.Y] = substring + actualText[editor.CaretPositionData.Y];
        }
    }
}
