using System.Drawing.Drawing2D;
using CurveEditor.Editing;

namespace CurveEditor;

/// <summary>Draws the 16 x 16 toolbar icons for the edit modes, so the repo needs no image files.</summary>
internal static class ToolIcons
{
    public static Bitmap Create(EditMode mode)
    {
        var bitmap = new Bitmap(16, 16);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(Color.FromArgb(40, 40, 40), 1.4f);
        using var fill = new SolidBrush(Color.FromArgb(40, 40, 40));

        switch (mode)
        {
            case EditMode.Select:
                // An arrow pointer.
                PointF[] arrow = [new(4, 1.5f), new(4, 13), new(7, 10.2f), new(9.2f, 14.5f), new(11, 13.6f), new(8.8f, 9.4f), new(12.5f, 9.4f)];
                graphics.FillPolygon(Brushes.White, arrow);
                graphics.DrawPolygon(pen, arrow);
                break;

            case EditMode.Add:
                // A key with a plus.
                graphics.FillRectangle(fill, 2, 9, 5, 5);
                graphics.DrawLine(pen, 11, 2, 11, 10);
                graphics.DrawLine(pen, 7, 6, 15, 6);
                break;

            case EditMode.Move:
                // Arrows in four directions.
                graphics.DrawLine(pen, 8, 1.5f, 8, 14.5f);
                graphics.DrawLine(pen, 1.5f, 8, 14.5f, 8);
                foreach (var (tip, left, right) in new (PointF, PointF, PointF)[]
                {
                    (new(8, 1), new(5.5f, 4), new(10.5f, 4)), (new(8, 15), new(5.5f, 12), new(10.5f, 12)),
                    (new(1, 8), new(4, 5.5f), new(4, 10.5f)), (new(15, 8), new(12, 5.5f), new(12, 10.5f)),
                })
                {
                    graphics.FillPolygon(fill, new[] { tip, left, right });
                }
                break;

            case EditMode.Pan:
                // An open hand.
                using (var palm = new GraphicsPath())
                {
                    palm.AddArc(4, 7, 9, 8, 0, 180);
                    palm.AddLine(4, 11, 4, 7);
                    palm.AddLine(13, 7, 13, 11);
                    graphics.FillPath(Brushes.White, palm);
                    graphics.DrawArc(pen, 4, 7, 9, 8, 0, 180);
                }
                foreach (var x in new[] { 5f, 7.5f, 10f, 12.5f })
                {
                    graphics.DrawLine(pen, x, 11, x, x is 7.5f or 10f ? 2 : 4);
                }
                graphics.DrawLine(pen, 4, 11, 1.5f, 8);
                break;

            case EditMode.Zoom:
                // A magnifying glass.
                graphics.FillEllipse(Brushes.White, 2, 2, 8, 8);
                graphics.DrawEllipse(pen, 2, 2, 8, 8);
                using (var handle = new Pen(pen.Color, 2.5f))
                {
                    graphics.DrawLine(handle, 9, 9, 14, 14);
                }
                break;
        }

        return bitmap;
    }
}
