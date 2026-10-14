using System.Globalization;
using System.Numerics;
using System.Text;
using CurveEditor.Curves;

namespace CurveEditor.Editing;

/// <summary>The mouse tool: select keys (S), add keys (A), move keys (D), pan (W) or zoom (E).</summary>
public enum EditMode { Select, Add, Move, Pan, Zoom }

/// <summary>Which keys or tangents are drawn (the values are the viewport protocol's).</summary>
public enum ShowMode { Always, Never, ActiveOnly }

/// <summary>Pixels between curve samples.</summary>
public enum CurveSmoothness { Coarse = 64, Rough = 32, Medium = 8, Fine = 4 }

/// <summary>
/// All of the editing, the way XNA's Curve Editor did it: the curves, the view, the mouse
/// gestures of each mode, selection, tangents, loops and undo. Positions are viewport
/// pixels. The form forwards input here, and after <see cref="Changed"/> refreshes its
/// controls and sends <see cref="BuildViewportMessages"/> to the viewport.
/// </summary>
public sealed class CurveEditorController
{
    private const float MinSelectSize = 16f;        // a smaller selection box is a click
    private const float TangentArrowLength = 50f;

    private static readonly Color[] CurveColors = [Color.Red, Color.Green, Color.Blue];

    private int _nextColor;
    private bool _isQuickPan;                       // Space held: pan whatever the mode
    private bool _isMiddlePan;                      // dragging with the middle button: pan whatever the mode
    private bool _isPressing;
    private Vector2 _pressPosition;
    private Vector2 _previousPosition;
    private RectangleF? _selectionBox;

    public CurveEditorController(Size viewSize) => View = new GridView(viewSize);

    /// <summary>Raised after anything shown changes.</summary>
    public event EventHandler? Changed;

    public List<EditCurve> Curves { get; private set; } = [];
    public GridView View { get; }
    public UndoHistory History { get; } = new();

    public EditMode Mode { get; private set; } = EditMode.Select;
    public EditMode EffectiveMode => _isQuickPan || _isMiddlePan ? EditMode.Pan : Mode;

    public CurveSmoothness Smoothness { get; private set; } = CurveSmoothness.Fine;
    public ShowMode ShowKeys { get; private set; } = ShowMode.Always;
    public ShowMode ShowTangents { get; private set; } = ShowMode.ActiveOnly;
    public bool ShowInfinity { get; private set; } = true;
    public bool AutoFrame { get; private set; }

    private IEnumerable<EditCurve> ActiveCurves => Curves.Where(curve => curve.IsEditable && curve.IsVisible);

    // ---- Mouse and keys ----

    /// <summary>A left-button press runs the current tool; a middle-button press pans, like Space + left.</summary>
    public void MouseDown(Vector2 position, MouseButtons button)
    {
        if (button is not (MouseButtons.Left or MouseButtons.Middle) || _isPressing) return;

        _isPressing = true;
        _isMiddlePan = button == MouseButtons.Middle;
        _pressPosition = _previousPosition = position;

        if (EffectiveMode == EditMode.Select) _selectionBox = new RectangleF(position.X, position.Y, 0f, 0f);
        else if (EffectiveMode == EditMode.Move) History.Begin(Snapshot());
        OnChanged();
    }

    public void MouseMove(Vector2 position)
    {
        if (!_isPressing) return;

        switch (EffectiveMode)
        {
            case EditMode.Pan:
                View.Pan(position - _previousPosition);
                break;
            case EditMode.Zoom:
                View.Zoom(position - _previousPosition);
                break;
            case EditMode.Select:
                var min = Vector2.Min(_pressPosition, position);
                var max = Vector2.Max(_pressPosition, position);
                _selectionBox = new RectangleF(min.X, min.Y, max.X - min.X, max.Y - min.Y);
                break;
            case EditMode.Move:
                MoveSelection(position, _previousPosition);
                if (AutoFrame) FrameAll();
                break;
        }

        _previousPosition = position;
        OnChanged();
    }

    public void MouseUp(Vector2 position, MouseButtons button)
    {
        if (!_isPressing || button != (_isMiddlePan ? MouseButtons.Middle : MouseButtons.Left)) return;

        _isPressing = false;
        switch (EffectiveMode)
        {
            case EditMode.Add:
                var unit = View.ToUnit(position);
                Edit(() => { foreach (var curve in ActiveCurves) curve.AddKey(unit.X, unit.Y); });
                break;
            case EditMode.Move:
                Commit();
                break;
            case EditMode.Select:
                Edit(() => SelectInBox(_selectionBox ?? RectangleF.Empty, isToggle: (Control.ModifierKeys & Keys.Shift) != 0));
                _selectionBox = null;
                break;
        }
        _isMiddlePan = false;
        OnChanged();
    }

    public void MouseWheel(int notches)
    {
        if (_isPressing) return;

        View.ZoomWheel(notches);
        OnChanged();
    }

    /// <summary>Handles the editor's single-key shortcuts; returns true if key was one.</summary>
    public bool KeyDown(Keys key)
    {
        if (_isQuickPan || _isPressing) return key is Keys.Space or Keys.A or Keys.S or Keys.D or Keys.W or Keys.E or Keys.F or Keys.Delete;

        switch (key)
        {
            case Keys.Space: _isQuickPan = true; OnChanged(); return true;
            case Keys.S: SetMode(EditMode.Select); return true;
            case Keys.A: SetMode(EditMode.Add); return true;
            case Keys.D: SetMode(EditMode.Move); return true;
            case Keys.W: SetMode(EditMode.Pan); return true;
            case Keys.E: SetMode(EditMode.Zoom); return true;
            case Keys.F: FrameAll(); return true;
            case Keys.Delete: DeleteKeys(); return true;
            default: return false;
        }
    }

    public bool KeyUp(Keys key)
    {
        if (key != Keys.Space || !_isQuickPan) return false;

        _isQuickPan = false;
        _isPressing = false;
        OnChanged();
        return true;
    }

    public void SetMode(EditMode mode)
    {
        Mode = mode;
        OnChanged();
    }

    // ---- Commands ----

    public void DeleteKeys() => Edit(() => { foreach (var curve in ActiveCurves) curve.RemoveSelectedKeys(); });

    public void SetLoop(bool isPre, CurveLoopType loop) => Edit(() =>
    {
        foreach (var curve in Curves.Where(curve => curve.IsEditable))
        {
            if (isPre) curve.PreLoop = loop;
            else curve.PostLoop = loop;
        }
    });

    /// <summary>Sets the tangent mode of the selected keys; target is TangentIn, TangentOut or both.</summary>
    public void SetTangents(KeySelection target, TangentMode mode) =>
        Edit(() => { foreach (var curve in Curves.Where(curve => curve.IsEditable)) curve.SetTangentMode(target, mode); });

    public void Undo() => Restore(History.Undo(Snapshot()));

    public void Redo() => Restore(History.Redo(Snapshot()));

    public void SetSmoothness(CurveSmoothness smoothness) { Smoothness = smoothness; OnChanged(); }
    public void SetShowKeys(ShowMode mode) { ShowKeys = mode; OnChanged(); }
    public void SetShowTangents(ShowMode mode) { ShowTangents = mode; OnChanged(); }
    public void SetShowInfinity(bool show) { ShowInfinity = show; OnChanged(); }
    public void SetAutoFrame(bool autoFrame) { AutoFrame = autoFrame; OnChanged(); }

    public void ResizeView(Size size)
    {
        View.Resize(size);
        OnChanged();
    }

    /// <summary>Fits every key of every curve (shown or not) into the view.</summary>
    public void FrameAll() => Frame(allKeys: true);

    /// <summary>Fits the selected keys of the shown curves into the view.</summary>
    public void FrameSelection() => Frame(allKeys: false);

    /// <summary>The one selected key, if exactly one key (or tangent) is selected on the active curves.</summary>
    public (EditCurve Curve, EditCurveKey Key)? GetSingleSelection()
    {
        var selected = ActiveCurves.SelectMany(curve => curve.SelectedKeys.Select(key => (curve, key))).Take(2).ToList();
        return selected.Count == 1 ? selected[0] : null;
    }

    /// <summary>Moves the single selected key (see <see cref="GetSingleSelection"/>).</summary>
    public void SetSingleKey(float position, float value)
    {
        if (GetSingleSelection() is not { } selection) return;

        var (curve, key) = selection;
        Edit(() => curve.SetKeyPosition(key, position, value));
        if (AutoFrame) FrameAll();
    }

    /// <summary>The loop all selected curves share (for menu check marks), or null if they differ or none is selected.</summary>
    public CurveLoopType? GetCommonLoop(bool isPre) =>
        Common(ActiveCurves.Where(curve => curve.HasSelection).Select(curve => isPre ? curve.PreLoop : curve.PostLoop));

    /// <summary>The tangent mode all selected keys share (Stepped for step keys), or null.</summary>
    public TangentMode? GetCommonTangent(bool isOut) =>
        Common(ActiveCurves.SelectMany(curve => curve.SelectedKeys).Select(key =>
            key.Continuity == CurveContinuity.Step ? TangentMode.Stepped : isOut ? key.TangentOutMode : key.TangentInMode));

    // ---- Curves ----

    /// <summary>A name not used by any curve: candidate, or candidate#1, candidate#2...</summary>
    public string MakeUniqueName(string candidate, EditCurve? except = null)
    {
        var name = string.IsNullOrWhiteSpace(candidate) ? "Empty" : candidate.Trim();
        var unique = name;
        for (var number = 1; Curves.Any(curve => curve != except && string.Equals(curve.Name, unique, StringComparison.OrdinalIgnoreCase)); number++)
            unique = $"{name}#{number}";
        return unique;
    }

    public Color NextColor()
    {
        var color = CurveColors[_nextColor];
        _nextColor = (_nextColor + 1) % CurveColors.Length;
        return color;
    }

    /// <summary>Adds a curve and makes it the only one selected for editing.</summary>
    public void AddCurve(EditCurve curve) => Edit(() =>
    {
        Curves.Add(curve);
        foreach (var other in Curves) other.IsEditable = other == curve;
    });

    /// <summary>Removes curves; the first remaining curve becomes the one being edited.</summary>
    public void RemoveCurves(IReadOnlyCollection<EditCurve> curves) => Edit(() =>
    {
        Curves = Curves.Except(curves).ToList();
        foreach (var curve in Curves) curve.IsEditable = curve == Curves.FirstOrDefault();
    });

    public void RenameCurve(EditCurve curve, string name) => Edit(() => curve.Name = MakeUniqueName(name, curve));

    /// <summary>Selects the curves being edited (the curve list's selection; not undoable).</summary>
    public void SetEditableCurves(IReadOnlyCollection<EditCurve> curves)
    {
        foreach (var curve in Curves) curve.IsEditable = curves.Contains(curve);
        OnChanged();
    }

    public void SetVisible(EditCurve curve, bool isVisible)
    {
        curve.IsVisible = isVisible;
        OnChanged();
    }

    /// <summary>Removes every curve and the undo history.</summary>
    public void Reset()
    {
        Curves = [];
        History.Clear();
        _nextColor = 0;
        OnChanged();
    }

    // ---- Viewport ----

    public string CursorName => EffectiveMode switch
    {
        EditMode.Add => "precision",
        EditMode.Move => "move",
        EditMode.Pan => "hand",
        EditMode.Zoom => "resize",
        _ => "arrow",
    };

    /// <summary>The messages that make the viewport show the current state (see CurveEditorGame.h).</summary>
    public IEnumerable<string> BuildViewportMessages()
    {
        yield return $"curves {Curves.Count}";
        for (var index = 0; index < Curves.Count; index++) yield return BuildCurveMessage(index, Curves[index]);

        yield return string.Create(CultureInfo.InvariantCulture, $"view {View.OffsetX:R} {View.OffsetY:R} {View.ScaleX:R} {View.ScaleY:R}");
        yield return $"display {(int)Smoothness} {(int)ShowKeys} {(int)ShowTangents} {(ShowInfinity ? 1 : 0)}";
        yield return _selectionBox is { } box
            ? string.Create(CultureInfo.InvariantCulture, $"box {box.X} {box.Y} {box.Width} {box.Height}")
            : "box none";
        yield return $"cursor {CursorName}";
    }

    private static string BuildCurveMessage(int index, EditCurve curve)
    {
        var message = new StringBuilder();
        message.Append(CultureInfo.InvariantCulture, $"curve {index} {curve.Color.R},{curve.Color.G},{curve.Color.B} ")
            .Append(curve.IsVisible ? "1 " : "0 ").Append(curve.IsEditable ? "1 " : "0 ")
            .Append((int)curve.PreLoop).Append(' ').Append((int)curve.PostLoop).Append(' ');

        if (curve.Keys.Count == 0) return message.Append('-').ToString();

        foreach (var key in curve.Keys)
        {
            message.Append(CurveFile.Format(key.Position)).Append(',').Append(CurveFile.Format(key.Value)).Append(',')
                .Append(CurveFile.Format(key.TangentIn)).Append(',').Append(CurveFile.Format(key.TangentOut)).Append(',')
                .Append(key.Continuity == CurveContinuity.Step ? '1' : '0').Append(',').Append((int)key.Selection).Append(';');
        }
        return message.ToString();
    }

    // ---- Editing ----

    private EditorSnapshot Snapshot() => EditorSnapshot.Capture(Curves);

    // Runs action as one undoable step.
    private void Edit(Action action)
    {
        History.Begin(Snapshot());
        action();
        Commit();
        OnChanged();
    }

    private void Commit()
    {
        if (History.Commit(Snapshot()) is { } before) MarkChangedCurvesDirty(before);
    }

    private void Restore(EditorSnapshot? snapshot)
    {
        if (snapshot == null) return;

        var current = Snapshot();
        Curves = snapshot.Curves.Select(entry => entry.Curve).ToList();
        foreach (var (curve, state) in snapshot.Curves) curve.RestoreState(state);
        MarkChangedCurvesDirty(current);
        OnChanged();
    }

    // Curves whose content differs from before (ignoring selection) need saving.
    private void MarkChangedCurvesDirty(EditorSnapshot before)
    {
        foreach (var curve in Curves)
        {
            var state = before.Find(curve);
            if (state == null || !state.SameContent(curve.CaptureState(), includeSelection: false)) curve.IsDirty = true;
        }
    }

    private void MoveSelection(Vector2 position, Vector2 previous)
    {
        var delta = View.ToUnit(position) - View.ToUnit(previous);

        foreach (var curve in ActiveCurves)
        {
            foreach (var key in curve.SelectedKeys.ToList())
            {
                // Tangents turn with the mouse around their key, as seen on screen (y up).
                var keyPixel = View.ToPixel(key.Position, key.Value);
                if (key.Selection.HasFlag(KeySelection.TangentIn))
                {
                    var angleChange = Math.Atan2(keyPixel.Y - previous.Y, keyPixel.X - previous.X) - Math.Atan2(keyPixel.Y - position.Y, keyPixel.X - position.X);
                    curve.RotateTangent(key, isOut: false, WrapAngle(angleChange), View.ScreenSlope);
                }
                if (key.Selection.HasFlag(KeySelection.TangentOut))
                {
                    var angleChange = Math.Atan2(keyPixel.Y - position.Y, position.X - keyPixel.X) - Math.Atan2(keyPixel.Y - previous.Y, previous.X - keyPixel.X);
                    curve.RotateTangent(key, isOut: true, WrapAngle(angleChange), View.ScreenSlope);
                }

                var moveBy = key.Selection.HasFlag(KeySelection.Key) ? delta : Vector2.Zero;
                curve.SetKeyPosition(key, key.Position + moveBy.X, key.Value + moveBy.Y);
            }
        }
    }

    private static double WrapAngle(double angle) => Math.IEEERemainder(angle, 2.0 * Math.PI);

    // Selects what the box covers on the active curves: keys if it covers any, otherwise
    // tangent arrows. A box smaller than MinSelectSize is a click, which picks one thing on
    // the topmost curve. Shift toggles.
    private void SelectInBox(RectangleF box, bool isToggle)
    {
        var isClick = box.Width < MinSelectSize || box.Height < MinSelectSize;
        if (isClick)
        {
            box = new RectangleF(box.X + (box.Width - MinSelectSize) * 0.5f, box.Y + (box.Height - MinSelectSize) * 0.5f, MinSelectSize, MinSelectSize);
        }

        var hasPicked = false;
        for (var index = Curves.Count - 1; index >= 0; index--)
        {
            var curve = Curves[index];
            if (!curve.IsEditable || !curve.IsVisible) continue;

            if (isClick && hasPicked)
            {
                if (!isToggle) curve.ClearSelection();
                continue;
            }

            SelectInBox(curve, box, isToggle, isClick);
            hasPicked = isClick && curve.HasSelection;
        }
    }

    private void SelectInBox(EditCurve curve, RectangleF box, bool isToggle, bool isClick)
    {
        var picked = new Dictionary<EditCurveKey, KeySelection>();

        if (ShowKeys != ShowMode.Never)
        {
            foreach (var key in ShowKeys == ShowMode.Always ? curve.Keys : curve.SelectedKeys.ToList())
            {
                var pixel = View.ToPixel(key.Position, key.Value);
                if (pixel.X < box.Left || pixel.X > box.Right || pixel.Y < box.Top || pixel.Y > box.Bottom) continue;

                picked[key] = KeySelection.Key;
                if (isClick) break;
            }
        }

        if (picked.Count == 0 && ShowTangents != ShowMode.Never) PickTangents(curve, box, isClick, picked);

        foreach (var key in curve.Keys)
        {
            var selection = picked.GetValueOrDefault(key);
            key.Selection = isToggle ? key.Selection ^ selection : selection;
        }
    }

    private void PickTangents(EditCurve curve, RectangleF box, bool isClick, Dictionary<EditCurveKey, KeySelection> picked)
    {
        var candidates = ShowTangents == ShowMode.Always ? curve.Keys : curve.SelectedKeys.ToList();
        foreach (var key in candidates)
        {
            if (key.Continuity == CurveContinuity.Step) continue;

            var index = curve.Keys.IndexOf(key);
            foreach (var isOut in new[] { false, true })
            {
                var start = View.ToPixel(key.Position, key.Value);
                if (!SegmentTouchesBox(start, GetTangentArrowEnd(curve, index, isOut), box)) continue;

                picked[key] = picked.GetValueOrDefault(key) | (isOut ? KeySelection.TangentOut : KeySelection.TangentIn);
                if (isClick) return;
            }
        }
    }

    // Where a tangent's arrow ends on screen (drawn the same way by the viewport).
    private Vector2 GetTangentArrowEnd(EditCurve curve, int index, bool isOut)
    {
        var key = curve.Keys[index];
        var slope = (isOut ? key.TangentOut : key.TangentIn) / curve.GetKeySpacing(index, isOut) * View.ScreenSlope;
        var angle = isOut ? Math.Atan(slope) : Math.PI + Math.Atan(slope);
        return View.ToPixel(key.Position, key.Value) + new Vector2((float)Math.Cos(angle), -(float)Math.Sin(angle)) * TangentArrowLength;
    }

    // Liang-Barsky: does the segment from start to end cross the box?
    private static bool SegmentTouchesBox(Vector2 start, Vector2 end, RectangleF box)
    {
        var delta = end - start;
        double enter = 0.0, exit = 1.0;
        (double p, double q)[] edges =
        [
            (-delta.X, start.X - box.Left), (delta.X, box.Right - start.X),
            (-delta.Y, start.Y - box.Top), (delta.Y, box.Bottom - start.Y),
        ];

        foreach (var (p, q) in edges)
        {
            if (p == 0.0)
            {
                if (q < 0.0) return false;
                continue;
            }

            var t = q / p;
            if (p < 0.0) enter = Math.Max(enter, t);
            else exit = Math.Min(exit, t);
            if (enter > exit) return false;
        }
        return true;
    }

    private void Frame(bool allKeys)
    {
        var min = new Vector2(float.MaxValue);
        var max = new Vector2(float.MinValue);
        var hasKeys = false;

        foreach (var curve in Curves)
        {
            if (!allKeys && !curve.IsVisible) continue;

            for (var index = 0; index < curve.Keys.Count; index++)
            {
                var key = curve.Keys[index];
                if (!allKeys && key.Selection == KeySelection.None) continue;

                hasKeys = true;
                var point = new Vector2(key.Position, key.Value);
                min = Vector2.Min(min, point);
                max = Vector2.Max(max, point);
                if (index == curve.Keys.Count - 1) continue;

                // Sample the segment to the next key too, for curves that bulge past their keys.
                const int Samples = 4;
                var nextPosition = curve.Keys[index + 1].Position;
                for (var sample = 1; sample <= Samples; sample++)
                {
                    var position = key.Position + (nextPosition - key.Position) * sample / (Samples + 2f);
                    point = new Vector2(position, curve.Evaluate(position));
                    min = Vector2.Min(min, point);
                    max = Vector2.Max(max, point);
                }
            }
        }

        if (!hasKeys)
        {
            min = -Vector2.One;
            max = Vector2.One;
        }

        View.Frame(min, max);
        OnChanged();
    }

    private static T? Common<T>(IEnumerable<T> values) where T : struct
    {
        T? common = null;
        foreach (var value in values)
        {
            if (common == null) common = value;
            else if (!EqualityComparer<T>.Default.Equals(common.Value, value)) return null;
        }
        return common;
    }

    private void OnChanged() => Changed?.Invoke(this, EventArgs.Empty);
}
