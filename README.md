# Curve Editor

A desktop editor for 1D keyframed curves (the kind games use for fades, easing, spawn rates over time and so on), built on **KatanaEngine**. It works as close as possible to the **Curve Editor** sample from XNA Game Studio 4.0, and reads and writes the same XNA curve XML. A game loads the files with KatanaEngine's `Curve` (`ResourceManager::Load<Curve>`), and XNA and MonoGame can load them too.

## Architecture

```
CurveEditor.exe (C#, WinForms, .NET 10)
├── curve list, menus, toolbar ──► CurveEditorController (C# owns the curves and does all the editing)
└── KatanaViewport (KatanaEngine.WinForms) ── hosts ──► CurveEditor.Native.dll (C++, KatanaEngine)
        │  "curves", "curve", "view", "display", "box", "cursor"
        │      ──►  CurveEditorGame → CurveViewScreen (draws with the engine's Curve)
        ◄──  "ready", "mouse", "wheel", "error"
```

| Project | Language | Role |
|---|---|---|
| `CurveEditor/` | C# WinForms | The editor: curve list, menus, mode toolbar, key Position/Value boxes, status bar. Owns the curves, the view and the undo history (`Editing/CurveEditorController.cs`), and the files (`Curves/CurveFile.cs`). |
| `CurveEditor.Native/` | C++ DLL | The KatanaEngine game that draws the grid, curves, keys, tangents and selection box, and reports the mouse. It evaluates the curves with the engine's own `Curve`, so the editor shows exactly what a game will get. |
| `KatanaEngine/` | submodule | The engine ([rdappel/KatanaEngine](https://github.com/rdappel/KatanaEngine)), including `KatanaEngine.WinForms`, the `KatanaViewport` control. |

The C# side keeps its own copy of the curve maths (`Curves/CurveMath.cs`, the same formulas as the engine's `Curve.cpp`) for framing and computing tangents. It gives bit-identical values to the engine.

### Message protocol

All messages are UTF-8 strings. Numbers use the invariant culture.

| Direction | Message | Meaning |
|---|---|---|
| UI → game | `curves <count>` | How many curves there are. |
| UI → game | `curve <index> <r>,<g>,<b> <visible 0\|1> <editable 0\|1> <preLoop> <postLoop> <keys>` | One curve. Loops are `0`–`4` (Constant, Cycle, CycleOffset, Oscillate, Linear). `keys` is `-` for none, or `position,value,tangentIn,tangentOut,step,selection;` for each key in position order. `step` is `0`/`1`. `selection` has bits `1` (in tangent), `2` (out tangent) and `4` (key). |
| UI → game | `view <offsetX> <offsetY> <scaleX> <scaleY>` | The curve point at the view's bottom left, and curve units per pixel. |
| UI → game | `display <smoothness> <keys> <tangents> <infinity 0\|1>` | Pixels between curve samples. Which keys and tangents are drawn: `0` always, `1` never, `2` active only. Whether to draw the curve past its end keys. |
| UI → game | `box <x> <y> <width> <height>` / `box none` | The selection box, in pixels. |
| UI → game | `cursor default\|arrow\|precision\|move\|hand\|resize` | The mouse cursor over the view. |
| game → UI | `ready` | The view is loaded. The UI responds by sending its whole state. |
| game → UI | `mouse down\|up <x> <y> left\|right`, `mouse move <x> <y>` | Mouse input on the view, in pixels from its top left. Moves are sent only while a button is held. |
| game → UI | `wheel <notches> <x> <y>` | The mouse wheel turned over the view. |
| game → UI | `error <text>` | A message could not be applied. |

After every change the UI resends the whole state. A curve message is small, and this keeps the native side a pure view.

## Using it

The window has the curve list on the left and the curve view on the right, with the tool bar above the view. All commands are in the one menu bar: File, Edit, View, Curves, Tangents, Help.

### Curves

- **File > New** (Ctrl+N) adds a curve with one key at 0, 0. **File > Open** (Ctrl+O) opens one or more curve files. Curves are coloured red, green and blue in turn.
- Select curves in the list to edit them. Ctrl+click or Shift+click selects several, and edits (adding keys, tangents, loops) apply to each selected curve.
- The check box shows or hides a curve.
- Rename a curve by clicking it again or pressing F2. Names are kept unique (`Curve`, `Curve#1`, ...).
- Right-click the list for New Curve, Open, Rename, Visible, Save, Save As and Close.
- **File > Close** (Ctrl+W) closes the selected curves, asking to save any with unsaved changes. Delete (with the list focused) removes them without asking. Both can be undone.
- **File > Save** (Ctrl+S) / **Save As** save the selected curves, one file each. Closing asks about each curve with unsaved changes.

### Modes and shortcuts

| Tool | Key | Mouse |
|---|---|---|
| Select Keys | S | Click a key, or drag a box around keys. Shift adds or removes. If no key is hit, the box picks the tangent arrows of the selected keys instead. |
| Add Key | A | Click to add a key to every selected curve. |
| Move Keys | D | Drag the selected keys. A selected tangent arrow turns around its key instead, and becomes Fixed. |
| Pan Camera | W, or hold Space | Drag the view. |
| Zoom | E | Drag right or up to zoom in. The mouse wheel zooms in any mode. |

Other keys:
- **F:** Frame All.
- **Delete:** removes the selected keys.
- **Ctrl+Z / Ctrl+Y:** Undo / Redo.

Every mouse gesture or command is one undo step, and selection changes are undoable too, as in XNA's editor. The **Position** and **Value** boxes edit the single selected key; press Enter to apply.

### Menus

- **View:**
  - Frame All, Frame Selection, and Auto Frame (reframe while moving keys).
  - **Curve Smoothness:** Coarse, Rough, Medium or Fine (a sample every 64, 32, 8 or 4 pixels).
  - Show **Keys** and **Tangents** Always, Never or for Active (selected) keys only.
  - **Infinity:** draw the curve before its first key and after its last, dimmed.
- **Curves:** Pre Infinity and Post Infinity, how the curve continues past its ends: Cycle, Cycle with Offset, Oscillate, Linear, Constant.
- **Tangents:** how the selected keys' tangents are set, for both sides or only the **In Tangent** / **Out Tangent**. Check marks show what the selection has in common.
  - **Smooth:** follows the neighbouring keys.
  - **Linear:** points at the neighbouring key.
  - **Flat:** horizontal.
  - **Fixed:** keeps the value it was dragged to.
  - **Stepped:** holds the value until the next key.

Whenever a key moves, it and its neighbours get their tangents recomputed by their modes, so smooth curves stay smooth.

### Differences from XNA's Curve Editor

- **Tangent modes of loaded files.** Files only store tangent values. XNA's editor treated every loaded tangent as Smooth, so moving a neighbouring key silently reshaped hand-tuned tangents. This editor works out each tangent's mode when it opens a file: Smooth, Linear or Flat if the value matches what that mode computes, otherwise Fixed.
- **Tangent arrows** are picked and turned in screen space, matching how they are drawn. The original picked them in curve units, which missed when the keys weren't 1 apart.
- **Frame All** always reframes. The original skipped framing when the zoom was already close, even if the keys were off screen.
- **The Position/Value boxes** apply on Enter (or when they lose focus) rather than on every keystroke, so each edit is one undo step.
- **Keyboard:** the hosted view can't take keyboard input, so the shortcuts are handled by the window: they work whenever you aren't typing in a box or renaming a curve. Clicking the view takes focus from the curve list, so Delete then removes keys, not curves.
- **One menu bar.** XNA's editor had two: File and Help for the window, and Edit, View, Curves and Tangents on the curve view. They are merged here.
- **Start-up:** the editor starts with one new curve.

## Curve files

XNA content XML, one curve per file, as written by XNA's `IntermediateSerializer`, MonoGame and KatanaEngine's `Curve::SaveToFile`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<XnaContent xmlns:Framework="Microsoft.Xna.Framework">
  <Asset Type="Framework:Curve">
    <PreLoop>Constant</PreLoop>
    <PostLoop>Cycle</PostLoop>
    <Keys>
      <CurveKey>
        <Position>0</Position>
        <Value>1</Value>
        <TangentIn>0</TangentIn>
        <TangentOut>0</TangentOut>
        <Continuity>Smooth</Continuity>
      </CurveKey>
    </Keys>
  </Asset>
</XnaContent>
```

The editor also reads XNA's compact key form (`<Keys>position value tangentIn tangentOut continuity ...</Keys>`), and writes the expanded form. Tangents are per segment, as in XNA: the change across the whole segment to the neighbouring key, not a slope per unit of position.

In a game:

```cpp
std::shared_ptr<Curve> fade = resourceManager.Load<Curve>("Curves/Fade.xml");
float alpha = fade->Evaluate(secondsSinceStart);
```

## Building

Requirements: Visual Studio 2026 (C++ desktop and .NET desktop workloads, v145 toolset) and the .NET 10 SDK. x64 only.

The engine is a git submodule, so clone with `--recurse-submodules`:

```powershell
git clone --recurse-submodules https://github.com/rdappel/CurveEditor.git
```

**Already cloned without it?** A plain `git clone` leaves the `KatanaEngine/` folder empty, and the build stops with *"The KatanaEngine submodule is missing"*. Fix it from the `CurveEditor` folder, then reload the solution:

```powershell
git submodule update --init
```

Open `CurveEditor.slnx` and build `x64` (Debug or Release). The first build restores Allegro through NuGet (pinned in `KatanaEngine/packages.config`). The C# project builds the native DLL first and copies it, along with the Allegro DLLs, next to `CurveEditor.exe`. Run the `CurveEditor` project.

## Working on the engine

`KatanaEngine/` is a submodule shared with other projects (ParticleRecorder, Altars of Eternity):

1. `cd KatanaEngine && git checkout master` (submodules start detached).
2. Make the change, then commit and push in the engine repo.
3. Commit the updated `KatanaEngine` pointer here.
4. Other projects pick it up with `git submodule update --remote KatanaEngine`.

## Credits

The behaviour and look follow the Curve Editor sample from XNA Game Studio 4.0 (Microsoft, Ms-PL). No code from it is included; this is a reimplementation on WinForms and KatanaEngine.
