namespace CurveEditor.Curves;

/// <summary>
/// XNA's curve maths, the same as KatanaEngine's Curve (Source/Core/Math/Curve.cpp), on
/// keys sorted by position. The viewport draws with the engine's own Curve; this is for
/// the editor's framing, tangent computation and anything else on this side.
/// </summary>
public static class CurveMath
{
    /// <summary>The curve's value at position: 0 with no keys, the key's value with one.</summary>
    public static float Evaluate(IReadOnlyList<EditCurveKey> keys, CurveLoopType preLoop, CurveLoopType postLoop, float position)
    {
        if (keys.Count == 0) return 0f;

        var first = keys[0];
        var last = keys[^1];
        if (keys.Count == 1) return first.Value;

        var isBefore = position < first.Position;
        var isAfter = position > last.Position;
        if (!isBefore && !isAfter) return EvaluateInRange(keys, position);

        var range = last.Position - first.Position;
        var loop = isBefore ? preLoop : postLoop;
        if (loop == CurveLoopType.Constant || range <= 0f) return isBefore ? first.Value : last.Value;

        if (loop == CurveLoopType.Linear)
        {
            return isBefore
                ? first.Value - first.TangentIn * (first.Position - position)
                : last.Value + last.TangentOut * (position - last.Position);
        }

        var cycle = GetCycle(keys, position);
        var offset = cycle * range;
        if (loop == CurveLoopType.Cycle) return EvaluateInRange(keys, position - offset);
        if (loop == CurveLoopType.CycleOffset) return EvaluateInRange(keys, position - offset) + cycle * (last.Value - first.Value);

        // Oscillate: even cycles run forwards, odd cycles backwards.
        var isForwards = cycle % 2 == 0;
        return EvaluateInRange(keys, isForwards ? position - offset : last.Position - position + first.Position + offset);
    }

    /// <summary>
    /// Sets a key's tangents from its neighbours (a key at either end stands in for its missing
    /// neighbour). Only Flat, Linear and Smooth compute anything; other modes give Flat.
    /// </summary>
    public static void ComputeTangent(IReadOnlyList<EditCurveKey> keys, int index, TangentMode tangentIn, TangentMode tangentOut)
    {
        if (index < 0 || index >= keys.Count) return;

        var key = keys[index];
        var previous = index > 0 ? keys[index - 1] : key;
        var next = index + 1 < keys.Count ? keys[index + 1] : key;

        var span = next.Position - previous.Position;
        var change = next.Value - previous.Value;
        var hasSpan = MathF.Abs(span) >= 1.0e-38f;

        key.TangentIn = tangentIn switch
        {
            TangentMode.Linear => key.Value - previous.Value,
            TangentMode.Smooth => hasSpan ? change * ((key.Position - previous.Position) / span) : 0f,
            _ => 0f,
        };

        key.TangentOut = tangentOut switch
        {
            TangentMode.Linear => next.Value - key.Value,
            TangentMode.Smooth => hasSpan ? change * ((next.Position - key.Position) / span) : 0f,
            _ => 0f,
        };
    }

    private static int GetCycle(IReadOnlyList<EditCurveKey> keys, float position)
    {
        var cycle = (position - keys[0].Position) / (keys[^1].Position - keys[0].Position);
        if (cycle < 0f) cycle -= 1f;
        return (int)cycle;
    }

    private static float EvaluateInRange(IReadOnlyList<EditCurveKey> keys, float position)
    {
        // The segment [previous, next] holding position: next is the first key past it.
        var nextIndex = 0;
        while (nextIndex < keys.Count && keys[nextIndex].Position <= position) nextIndex++;
        nextIndex = Math.Clamp(nextIndex, 1, keys.Count - 1);

        var previous = keys[nextIndex - 1];
        var next = keys[nextIndex];
        var span = next.Position - previous.Position;
        var t = span > 0f ? (position - previous.Position) / span : 1f;

        if (previous.Continuity == CurveContinuity.Step) return t >= 1f ? next.Value : previous.Value;
        return Hermite(previous.Value, previous.TangentOut, next.Value, next.TangentIn, t);
    }

    private static float Hermite(float value0, float tangent0, float value1, float tangent1, float t)
    {
        var t2 = t * t;
        var t3 = t2 * t;
        return (2f * t3 - 3f * t2 + 1f) * value0
            + (t3 - 2f * t2 + t) * tangent0
            + (3f * t2 - 2f * t3) * value1
            + (t3 - t2) * tangent1;
    }
}
