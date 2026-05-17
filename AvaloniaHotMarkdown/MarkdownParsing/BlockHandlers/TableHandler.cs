using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Markdig.Extensions.Tables;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Diagnostics;

namespace AvaloniaHotMarkdown.MarkdownParsing.BlockHandlers;

/*
| a | b | c |
|---|---|---|
| 1 | 2 | 3 |
| 4 | 5 | 6 |
 */

internal class TableHandler(StandardMarkdownParser parser) : BlockHandler(parser)
{
    public override Control Handle(Block block, string markdownText, LineInformation[] lineInformations)
    {
        var showFull = lineInformations.Any(x => x.ShowFullText);

        Table table = (Table)block;

        int rowsCount = table.Count;
        int columnsCount = (table[0] as TableRow).Count;

        Control result;

        if(showFull)
            result = ParseAsText(table, markdownText, rowsCount, columnsCount, lineInformations);
        else
            result = ParseAsTable(table, markdownText, rowsCount, columnsCount, lineInformations);

        return result;
    }

    Control ParseAsText(Table table, string markdownText, int rowsCount, int columnsCount, LineInformation[] lineInformation)
    {
        StackPanel container = new();
        container.Spacing = 0;

        string tableText = markdownText.Substring(table.Span.Start, table.Span.End - table.Span.Start + 1);
        string[] lines = tableText.Split(["\n", "\r\n"], StringSplitOptions.None);

        for(int i =0; i < lines.Length; i++)
        {
            var presenter = StylizationHelper.CreateNewPresenter();

            presenter.Text = lines[i];
            presenter.Tag = new CaretPositionOffset(0, lineInformation[i].LineYIndex);

            //dockpanel for builtin updatetexteffects to work
            DockPanel lineContainer = new();
            lineContainer.Children.Add(presenter);

            container.Children.Add(lineContainer);
        }

        container.ApplyTemplate();

        return container;
    }

    Control ParseAsTable(Table table, string markdownText, int rowsCount, int columnsCount, LineInformation[] lineInformations)
    {
        var tableControl = new Grid();

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

                if (cell.Count == 0)
                    continue;

                var control = ParseBlock(cell[0], markdownText, [lineInformations[rowIndex]]);

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
}
