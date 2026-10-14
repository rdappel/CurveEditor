using System.Numerics;

namespace CurveEditor.Editing;

/// <summary>
/// The camera over the curves, with XNA's Curve Editor maths: an offset (the curve point at
/// the view's bottom left) and a scale (curve units per pixel) on each axis. Pixels run down
/// from the top left, as the viewport reports them; curve values run up.
/// </summary>
public sealed class GridView
{
    private const double MinScale = 1e-5;
    private const double MaxScale = 1e3;
    private const double MinOffset = -1e8;
    private const double MaxOffset = 1e8;
    private const float FrameMargin = 50f;      // pixels around framed keys, split between the sides

    public GridView(Size viewSize)
    {
        ViewSize = new Size(Math.Max(viewSize.Width, 10), Math.Max(viewSize.Height, 10));

        // Show -1..1 on both axes.
        SetScale(2.0 / ViewSize.Width, 2.0 / ViewSize.Height);
        SetOffset(-1.0, -1.0);
    }

    public Size ViewSize { get; private set; }
    public double OffsetX { get; private set; }
    public double OffsetY { get; private set; }
    public double ScaleX { get; private set; }
    public double ScaleY { get; private set; }

    /// <summary>Converts a slope in curve units to one on screen.</summary>
    public double ScreenSlope => ScaleX / ScaleY;

    public Vector2 ToPixel(double position, double value) =>
        new((float)((position - OffsetX) / ScaleX), (float)(ViewSize.Height - (value - OffsetY) / ScaleY));

    public Vector2 ToUnit(Vector2 pixel) =>
        new((float)(OffsetX + pixel.X * ScaleX), (float)(OffsetY + (ViewSize.Height - pixel.Y) * ScaleY));

    /// <summary>Moves the view with a drag of delta pixels (the curves follow the mouse).</summary>
    public void Pan(Vector2 delta) => SetOffset(OffsetX - delta.X * ScaleX, OffsetY + delta.Y * ScaleY);

    /// <summary>Zooms by a drag of delta pixels: dragging right or up zooms in, along the longer axis of the drag.</summary>
    public void Zoom(Vector2 delta)
    {
        var dx = delta.X * ScaleX;
        var dy = -delta.Y * ScaleY;
        var amount = -0.001 * (Math.Abs(dx) > Math.Abs(dy) ? dx : dy);
        ZoomBy(amount, amount);
    }

    /// <summary>Zooms by mouse wheel notches, 10% each: rolling away zooms out.</summary>
    public void ZoomWheel(int notches) => ZoomBy(notches * 0.1 * ScaleX, notches * 0.1 * ScaleY);

    /// <summary>Fits min..max into the view, with a margin.</summary>
    public void Frame(Vector2 min, Vector2 max)
    {
        if (min.X == max.X) max.X = min.X + 1f;
        if (min.Y == max.Y) max.Y = min.Y + 1f;

        var range = Vector2.Max(max - min, new Vector2((float)MinScale));
        SetScale(range.X / Math.Max(ViewSize.Width - FrameMargin, 10f), range.Y / Math.Max(ViewSize.Height - FrameMargin, 10f));
        SetOffset(min.X - ScaleX * FrameMargin * 0.5, min.Y - ScaleY * FrameMargin * 0.5);
    }

    /// <summary>Follows the viewport's size: the same curve range stretches to fill it.</summary>
    public void Resize(Size viewSize)
    {
        var newSize = new Size(Math.Max(viewSize.Width, 10), Math.Max(viewSize.Height, 10));
        SetScale(ScaleX * ViewSize.Width / newSize.Width, ScaleY * ViewSize.Height / newSize.Height);
        ViewSize = newSize;
    }

    // Changes the scale by the given amounts, keeping the view's centre in place.
    private void ZoomBy(double dx, double dy)
    {
        var oldScaleX = ScaleX;
        var oldScaleY = ScaleY;
        SetScale(ScaleX + dx, ScaleY + dy);
        SetOffset(OffsetX - (ScaleX - oldScaleX) * ViewSize.Width * 0.5, OffsetY - (ScaleY - oldScaleY) * ViewSize.Height * 0.5);
    }

    private void SetOffset(double x, double y)
    {
        OffsetX = Math.Clamp(x, MinOffset, MaxOffset);
        OffsetY = Math.Clamp(y, MinOffset, MaxOffset);
    }

    private void SetScale(double x, double y)
    {
        ScaleX = Math.Clamp(x, MinScale, MaxScale);
        ScaleY = Math.Clamp(y, MinScale, MaxScale);
    }
}
