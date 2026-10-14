namespace CurveEditor.Curves;

/// <summary>What a curve returns outside its keys (XNA's CurveLoopType; the engine's has the same order).</summary>
public enum CurveLoopType
{
    Constant,
    Cycle,
    CycleOffset,
    Oscillate,
    Linear,
}

/// <summary>How a curve leaves a key: interpolating to the next key, or holding the value until it.</summary>
public enum CurveContinuity
{
    Smooth,
    Step,
}

/// <summary>
/// How the editor sets a key's tangent. Flat, Linear and Smooth are recomputed from the
/// neighbouring keys whenever they move; Fixed keeps the value it was dragged to. Stepped
/// is a key with Step continuity (and flat tangents).
/// </summary>
public enum TangentMode
{
    Flat,
    Linear,
    Smooth,
    Stepped,
    Fixed,
}

/// <summary>What is selected on a key. The values are the bits the viewport protocol uses.</summary>
[Flags]
public enum KeySelection
{
    None = 0,
    TangentIn = 1,
    TangentOut = 2,
    Key = 4,
    Tangents = TangentIn | TangentOut,
}
