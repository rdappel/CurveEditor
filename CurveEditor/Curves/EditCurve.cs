namespace CurveEditor.Curves;

/// <summary>
/// A curve being edited: its keys (always sorted by position), loops, and how it shows in
/// the editor. Edits keep the keys' tangents up to date: after a key changes, it and its
/// neighbours get their tangents recomputed by their modes, as in XNA's Curve Editor.
/// </summary>
public sealed class EditCurve
{
    /// <summary>The largest position or value a key may have (XNA's Curve Editor limit).</summary>
    public const float MaxKeyEntry = 0x4000007F;

    private const double MaxTangentAngle = 89.99999 * Math.PI / 180.0;

    public EditCurve(string name, Color color)
    {
        Name = name;
        Color = color;
    }

    public string Name { get; set; }
    public Color Color { get; }
    public CurveLoopType PreLoop { get; set; }
    public CurveLoopType PostLoop { get; set; }
    public List<EditCurveKey> Keys { get; private set; } = [];

    public bool IsVisible { get; set; } = true;

    /// <summary>Selected in the curve list: only editable curves take edits.</summary>
    public bool IsEditable { get; set; }

    public string? FilePath { get; set; }
    public bool IsDirty { get; set; }

    public bool HasSelection => Keys.Any(key => key.Selection != KeySelection.None);

    public IEnumerable<EditCurveKey> SelectedKeys => Keys.Where(key => key.Selection != KeySelection.None);

    public float Evaluate(float position) => CurveMath.Evaluate(Keys, PreLoop, PostLoop, position);

    /// <summary>Inserts key in position order (after keys at the same position) and returns its index.</summary>
    public int InsertKey(EditCurveKey key)
    {
        var index = 0;
        while (index < Keys.Count && Keys[index].Position <= key.Position) index++;
        Keys.Insert(index, key);
        return index;
    }

    /// <summary>Adds a smooth key, selected alone, and fits the tangents around it.</summary>
    public void AddKey(float position, float value)
    {
        ClearSelection();
        var index = InsertKey(new EditCurveKey
        {
            Position = Math.Clamp(position, -MaxKeyEntry, MaxKeyEntry),
            Value = Math.Clamp(value, -MaxKeyEntry, MaxKeyEntry),
            Selection = KeySelection.Key,
        });
        ComputeTangents(index);
    }

    /// <summary>Removes every key with anything selected.</summary>
    public void RemoveSelectedKeys()
    {
        for (var index = Keys.Count - 1; index >= 0; index--)
        {
            if (Keys[index].Selection == KeySelection.None) continue;

            Keys.RemoveAt(index);
            ComputeTangents(index);
        }
    }

    /// <summary>Moves a key to a new position and value, keeping the keys in order.</summary>
    public void SetKeyPosition(EditCurveKey key, float position, float value)
    {
        var oldIndex = Keys.IndexOf(key);
        Keys.RemoveAt(oldIndex);
        key.Position = Math.Clamp(position, -MaxKeyEntry, MaxKeyEntry);
        key.Value = Math.Clamp(value, -MaxKeyEntry, MaxKeyEntry);

        var newIndex = InsertKey(key);
        ComputeTangents(newIndex);
        if (newIndex != oldIndex) ComputeTangents(oldIndex);
    }

    /// <summary>
    /// Turns a tangent by angleChange radians as seen on screen (counterclockwise, clamped
    /// short of vertical) and fixes it there. screenSlope converts a slope in curve units to
    /// one in pixels.
    /// </summary>
    public void RotateTangent(EditCurveKey key, bool isOut, double angleChange, double screenSlope)
    {
        var index = Keys.IndexOf(key);
        var spacing = GetKeySpacing(index, isOut);
        if (Math.Abs(spacing) <= 1e-5f) return;

        var tangent = isOut ? key.TangentOut : key.TangentIn;
        var angle = Math.Clamp(Math.Atan(tangent / spacing * screenSlope) + angleChange, -MaxTangentAngle, MaxTangentAngle);
        var newTangent = (float)(Math.Tan(angle) / screenSlope * spacing);

        if (isOut)
        {
            key.TangentOut = newTangent;
            key.TangentOutMode = TangentMode.Fixed;
        }
        else
        {
            key.TangentIn = newTangent;
            key.TangentInMode = TangentMode.Fixed;
        }
    }

    /// <summary>Sets the tangent mode of the selected keys' tangents (target picks in, out or both).</summary>
    public void SetTangentMode(KeySelection target, TangentMode mode)
    {
        var selected = SelectedKeys.ToList();
        foreach (var key in selected)
        {
            if (mode == TangentMode.Stepped)
            {
                if (key.Continuity == CurveContinuity.Step) continue;

                key.Continuity = CurveContinuity.Step;
                key.TangentIn = key.TangentOut = 0f;
                key.TangentInMode = key.TangentOutMode = TangentMode.Flat;
                continue;
            }

            key.Continuity = CurveContinuity.Smooth;
            if (target.HasFlag(KeySelection.TangentIn)) key.TangentInMode = mode;
            if (target.HasFlag(KeySelection.TangentOut)) key.TangentOutMode = mode;
        }

        foreach (var key in selected) ComputeTangents(Keys.IndexOf(key));
    }

    /// <summary>Recomputes the tangents of the key at index and its neighbours; Fixed tangents keep their values.</summary>
    public void ComputeTangents(int index)
    {
        for (var i = index - 1; i <= index + 1; i++)
        {
            if (i < 0 || i >= Keys.Count) continue;

            var key = Keys[i];
            var tangentIn = key.TangentIn;
            var tangentOut = key.TangentOut;
            CurveMath.ComputeTangent(Keys, i, key.TangentInMode, key.TangentOutMode);

            if (key.TangentInMode == TangentMode.Fixed || float.IsNaN(key.TangentIn)) key.TangentIn = key.TangentInMode == TangentMode.Fixed ? tangentIn : 0f;
            if (key.TangentOutMode == TangentMode.Fixed || float.IsNaN(key.TangentOut)) key.TangentOut = key.TangentOutMode == TangentMode.Fixed ? tangentOut : 0f;
        }
    }

    /// <summary>
    /// The distance a tangent's slope is measured over: to the neighbouring key, or at the
    /// ends 1 (or the end segment's length when the curve oscillates there).
    /// </summary>
    public float GetKeySpacing(int index, bool isOut)
    {
        if (!isOut)
        {
            if (index > 0) return Keys[index].Position - Keys[index - 1].Position;
            if (PreLoop == CurveLoopType.Oscillate && Keys.Count > 1) return Keys[1].Position - Keys[0].Position;
            return 1f;
        }

        if (index < Keys.Count - 1) return Keys[index + 1].Position - Keys[index].Position;
        if (PostLoop == CurveLoopType.Oscillate && Keys.Count > 1) return Keys[index].Position - Keys[index - 1].Position;
        return 1f;
    }

    public void ClearSelection()
    {
        foreach (var key in Keys) key.Selection = KeySelection.None;
    }

    /// <summary>
    /// Works out each key's tangent modes from its tangents (for curves from files, which
    /// only store the values): Smooth, Linear or Flat if the tangent matches what that mode
    /// computes, otherwise Fixed.
    /// </summary>
    public void DetectTangentModes()
    {
        for (var index = 0; index < Keys.Count; index++)
        {
            var key = Keys[index];
            var probe = Keys.Select(other => other == key ? key.Clone() : other).ToList();

            key.TangentInMode = DetectMode(probe, index, key.TangentIn, isOut: false);
            key.TangentOutMode = DetectMode(probe, index, key.TangentOut, isOut: true);
        }
    }

    private static TangentMode DetectMode(List<EditCurveKey> probe, int index, float tangent, bool isOut)
    {
        foreach (var mode in new[] { TangentMode.Smooth, TangentMode.Linear, TangentMode.Flat })
        {
            CurveMath.ComputeTangent(probe, index, mode, mode);
            var computed = isOut ? probe[index].TangentOut : probe[index].TangentIn;
            if (Math.Abs(computed - tangent) <= 1e-5f * Math.Max(1f, Math.Abs(tangent))) return mode;
        }
        return TangentMode.Fixed;
    }

    // ---- Undo ----

    public CurveState CaptureState() => new(Name, PreLoop, PostLoop, IsEditable, Keys.Select(key => key.Clone()).ToList());

    public void RestoreState(CurveState state)
    {
        Name = state.Name;
        PreLoop = state.PreLoop;
        PostLoop = state.PostLoop;
        IsEditable = state.IsEditable;
        Keys = state.Keys.Select(key => key.Clone()).ToList();
    }
}

/// <summary>A copy of everything about a curve that undo restores.</summary>
public sealed record CurveState(string Name, CurveLoopType PreLoop, CurveLoopType PostLoop, bool IsEditable, IReadOnlyList<EditCurveKey> Keys)
{
    /// <summary>
    /// True if the curve is the same. Which curves are editable never counts; the key
    /// selection counts for undo, but doesn't make a file need saving.
    /// </summary>
    public bool SameContent(CurveState other, bool includeSelection = true) =>
        Name == other.Name && PreLoop == other.PreLoop && PostLoop == other.PostLoop
        && Keys.Count == other.Keys.Count && Keys.Zip(other.Keys).All(pair => pair.First.SameAs(pair.Second, includeSelection));
}
