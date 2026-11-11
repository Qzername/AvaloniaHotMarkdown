using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Markdig.Extensions.Tables;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers.Implementations;

/*
| a | b | c |
|---|---|---|
| 1 | 2 | 3 |
| 4 | 5 | 6 |
 */

public class TableHandler : BlockHandler<Table>
{
    public override Control[] Parse(Table table)
    {
        if (table.Count == 0)
            return [];

        var tableControl = new Grid();

        int rowsCount = table.Count;
        int columnsCount = (table[0] as TableRow)?.Count ?? 0;

        if (columnsCount == 0)
            return [];

        // Add row definitions
        for (int i = 0; i < rowsCount; i++)
            tableControl.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        // Add column definitions
        for (int i = 0; i < columnsCount; i++)
            tableControl.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Auto)));

        // Populate table cells
        for (int rowIndex = 0; rowIndex < rowsCount; rowIndex++)
        {
            var row = (TableRow)table[rowIndex];

            for (int colIndex = 0; colIndex < columnsCount; colIndex++)
            {
                if (colIndex >= row.Count)
                    continue;

                var cell = (TableCell)row[colIndex];
                var cellContent = ParseCellContent(cell);

                var cellContainer = new Border
                {
                    BorderBrush = new SolidColorBrush(Colors.Gray),
                    BorderThickness = new Thickness(1),
                    Padding = new Thickness(10, 8),
                    Child = cellContent
                };

                Grid.SetRow(cellContainer, rowIndex);
                Grid.SetColumn(cellContainer, colIndex);
                tableControl.Children.Add(cellContainer);
            }
        }

        return [tableControl];
    }

    static TextBlock ParseCellContent(TableCell cell)
    {
        if (cell.Count == 0)
            return new TextBlock();

        if (cell[0] is Markdig.Syntax.ParagraphBlock paragraphBlock && 
            paragraphBlock.Inline is not null)
            return InlineParser.ParseContainer(paragraphBlock.Inline);

        return new TextBlock();
    }
}
