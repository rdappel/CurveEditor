using CurveEditor.Curves;

namespace CurveEditor.Editing;

/// <summary>Every curve in the editor, in list order, with the state undo restores.</summary>
public sealed record EditorSnapshot(IReadOnlyList<(EditCurve Curve, CurveState State)> Curves)
{
    public static EditorSnapshot Capture(IEnumerable<EditCurve> curves) =>
        new(curves.Select(curve => (curve, curve.CaptureState())).ToList());

    /// <summary>True if both have the same curves with the same content.</summary>
    public bool SameContent(EditorSnapshot other) =>
        Curves.Count == other.Curves.Count
        && Curves.Zip(other.Curves).All(pair => pair.First.Curve == pair.Second.Curve && pair.First.State.SameContent(pair.Second.State));

    /// <summary>The state of curve in this snapshot, or null if it isn't in it.</summary>
    public CurveState? Find(EditCurve curve) => Curves.FirstOrDefault(entry => entry.Curve == curve).State;
}

/// <summary>
/// Undo and redo by snapshots: an action takes a snapshot before it starts (Begin) and is
/// recorded when it ends (Commit) if it changed anything. Each mouse gesture or command is
/// one step, as in XNA's Curve Editor.
/// </summary>
public sealed class UndoHistory
{
    private readonly Stack<EditorSnapshot> _undo = new();
    private readonly Stack<EditorSnapshot> _redo = new();
    private EditorSnapshot? _before;

    public bool CanUndo => _undo.Count > 0;
    public bool CanRedo => _redo.Count > 0;

    /// <summary>Starts an action. Nested calls keep the first snapshot.</summary>
    public void Begin(EditorSnapshot current) => _before ??= current;

    /// <summary>Ends an action; returns the snapshot from before it if it changed anything (and records it).</summary>
    public EditorSnapshot? Commit(EditorSnapshot current)
    {
        var before = _before;
        _before = null;
        if (before == null || before.SameContent(current)) return null;

        _undo.Push(before);
        _redo.Clear();
        return before;
    }

    /// <summary>Returns the snapshot to restore, or null if there is nothing to undo.</summary>
    public EditorSnapshot? Undo(EditorSnapshot current) => Step(_undo, _redo, current);

    public EditorSnapshot? Redo(EditorSnapshot current) => Step(_redo, _undo, current);

    public void Clear()
    {
        _undo.Clear();
        _redo.Clear();
        _before = null;
    }

    private EditorSnapshot? Step(Stack<EditorSnapshot> from, Stack<EditorSnapshot> to, EditorSnapshot current)
    {
        if (from.Count == 0) return null;

        _before = null;
        to.Push(current);
        return from.Pop();
    }
}
