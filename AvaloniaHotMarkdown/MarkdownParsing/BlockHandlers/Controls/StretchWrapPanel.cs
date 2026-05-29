using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

public class StretchWrapPanel : WrapPanel
{
    protected override Size ArrangeOverride(Size finalSize)
    {
        bool isHorizontal = Orientation == Orientation.Horizontal;
        double itemWidth = ItemWidth;
        double itemHeight = ItemHeight;
        bool hasItemWidth = !double.IsNaN(itemWidth);
        bool hasItemHeight = !double.IsNaN(itemHeight);

        double lineLength = 0;
        double maxLineBreadth = 0;
        double panelBreadth = 0;
        int lineStart = 0;

        for (int i = 0; i < Children.Count; i++)
        {
            Control child = Children[i];

            double childWidth = hasItemWidth ? itemWidth : child.DesiredSize.Width;
            double childHeight = hasItemHeight ? itemHeight : child.DesiredSize.Height;

            double childLength = isHorizontal ? childWidth : childHeight;
            double childBreadth = isHorizontal ? childHeight : childWidth;
            double totalLength = isHorizontal ? finalSize.Width : finalSize.Height;

            if (lineLength + childLength > totalLength && i > lineStart)
            {
                ArrangeLine(lineStart, i, panelBreadth, maxLineBreadth, finalSize, false);
                panelBreadth += maxLineBreadth;
                lineLength = 0;
                maxLineBreadth = 0;
                lineStart = i;
            }

            lineLength += childLength;

            if (childBreadth > maxLineBreadth)
                maxLineBreadth = childBreadth;
        }

        if (lineStart < Children.Count)
            ArrangeLine(lineStart, Children.Count, panelBreadth, maxLineBreadth, finalSize, true);

        return finalSize;
    }

    private void ArrangeLine(int start, int end, double panelBreadth, double maxLineBreadth, Size finalSize, bool isLastLine)
    {
        bool isHorizontal = Orientation == Orientation.Horizontal;
        double itemWidth = ItemWidth;
        double itemHeight = ItemHeight;
        bool hasItemWidth = !double.IsNaN(itemWidth);
        bool hasItemHeight = !double.IsNaN(itemHeight);

        double currentLength = 0;

        for (int i = start; i < end; i++)
        {
            Control child = Children[i];

            double childWidth = hasItemWidth ? itemWidth : child.DesiredSize.Width;
            double childHeight = hasItemHeight ? itemHeight : child.DesiredSize.Height;

            double childLength = isHorizontal ? childWidth : childHeight;

            if (isLastLine && i == end - 1)
            {
                double totalLength = isHorizontal ? finalSize.Width : finalSize.Height;
                childLength = Math.Max(childLength, totalLength - currentLength);
            }

            Rect rect = isHorizontal
                ? new Rect(currentLength, panelBreadth, childLength, maxLineBreadth)
                : new Rect(panelBreadth, currentLength, maxLineBreadth, childLength);

            child.Arrange(rect);
            currentLength += childLength;
        }
    }
}