namespace CurveEditor.Curves;

/// <summary>
/// A curve key plus what the editor needs: how each tangent is set and what is selected.
/// Tangents are per segment, as in XNA: the change across the whole segment to the
/// neighbouring key, not per unit of position.
/// </summary>
public sealed class EditCurveKey
{
    public float Position { get; set; }
    public float Value { get; set; }
    public float TangentIn { get; set; }
    public float TangentOut { get; set; }
    public CurveContinuity Continuity { get; set; }

    public TangentMode TangentInMode { get; set; } = TangentMode.Smooth;
    public TangentMode TangentOutMode { get; set; } = TangentMode.Smooth;

    public KeySelection Selection { get; set; }

    public EditCurveKey Clone() => (EditCurveKey)MemberwiseClone();

    public bool SameAs(EditCurveKey other, bool includeSelection) =>
        Position == other.Position && Value == other.Value && TangentIn == other.TangentIn && TangentOut == other.TangentOut
        && Continuity == other.Continuity && TangentInMode == other.TangentInMode && TangentOutMode == other.TangentOutMode
        && (!includeSelection || Selection == other.Selection);
}
