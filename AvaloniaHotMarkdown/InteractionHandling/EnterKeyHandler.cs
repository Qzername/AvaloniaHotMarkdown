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

        public void HandleCombination(KeyModifiers keyModifiers, ref List<string> actualText, ref TextCursor caretPositionData, ref TextCursor selectionPositionData)
        {
            //previ|[ous line] <- substring
            var substring = actualText[caretPositionData.Y].Substring(caretPositionData.X);
            //previ|
            actualText[caretPositionData.Y] = actualText[caretPositionData.Y].Remove(caretPositionData.X);

            //previ
            //|
            actualText.Insert(caretPositionData.Y + 1, string.Empty);

            caretPositionData.X = 0;
            caretPositionData.Y++;

            //previ
            //|ous line
            actualText[caretPositionData.Y] = substring + actualText[caretPositionData.Y];
        }
    }
}
