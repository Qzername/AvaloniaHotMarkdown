using Avalonia.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaHotMarkdown.InteractionHandling
{
    internal class HomeKeyHandler : IKeyInteractionHandler
    {
        public Key MainKey => Key.Home;

        public void HandleCombination(KeyModifiers keyModifiers, ref List<string> actualText, ref TextCursor caretPositionData)
        {
            caretPositionData.X = 0;
        }
    }
}
