using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Markdig.Extensions.Tables;
using Markdig.Syntax;
using System;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

internal class TableHandler(StandardMarkdownParser parser) : BlockHandler(parser)
{
    public override Control Handle(Block block, LineInformation[] lineInformations)
    {
        Table table = (Table)block;

        var tableControl = new Grid();

        int rowsCount = table.Count;
        int columnsCount = (table[0] as TableRow).Count;

        for (int i = 0; i < rowsCount; i++)
            tableControl.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        for (int i = 0; i < columnsCount; i++)
            tableControl.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

        for (int rowIndex = 0; rowIndex < rowsCount; rowIndex++)
        {
            var row = (TableRow)table[rowIndex];

            for (int colIndex = 0; colIndex < columnsCount; colIndex++)
            {
                var cell = (TableCell)row[colIndex];
                var control = ParseBlock(cell[0], [lineInformations[rowIndex]]);
                
                var cellContainer = new Border
                {
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1),
                    Padding = new Thickness(10, 5),
                    Child = control
                };

                Grid.SetRow(cellContainer, rowIndex);
                Grid.SetColumn(cellContainer, colIndex);

                tableControl.Children.Add(cellContainer);
            }
        }

        return tableControl;
    }

    public override void UpdateTextEffects(Control control, LineInformation[] lineInformations)
    {
    }
}
