# CLAUDE.md

Guidance for Claude Code in this repository. See `README.md` for what the tool does, its architecture and the message protocol.

## Solution structure (`CurveEditor.slnx`, x64 only)

- **CurveEditor** (`CurveEditor/`): C# WinForms app (.NET 10). The UI, which owns the curves and does all the editing.
- **CurveEditor.Native** (`CurveEditor.Native/`): C++ DLL, a KatanaEngine `Game` hosted in the UI's `KatanaViewport`. It only draws and reports the mouse.
- **KatanaEngine** (`KatanaEngine/`, git submodule `rdappel/KatanaEngine`): the C++ engine (`KatanaEngine.vcxproj`) plus `WinForms/KatanaEngine.WinForms.csproj` (the `KatanaViewport` control).

Build from the command line (MSBuild, not `dotnet build`, because of the C++ projects):

```
MSBuild CurveEditor.slnx -t:restore -p:RestorePackagesConfig=true
MSBuild CurveEditor.slnx -p:Configuration=Debug -p:Platform=x64
```

Output: `CurveEditor/bin/<Configuration>/net10.0-windows/CurveEditor.exe` (the native DLL and Allegro DLLs are copied there).

## Rules

### C# owns the data
- **The editing:** curves, keys, tangent modes, selection, the view (`Editing/GridView.cs`) and undo live in C#. `Editing/CurveEditorController.cs` does every edit.
- **The native side is a pure view:** it holds a copy of the state from the last messages (`CurveViewScreen`) and never changes it on its own. On `ready`, and after every change, the UI resends the whole state (`BuildViewportMessages`).

### Match XNA
- **Behaviour:** the editor follows XNA Game Studio 4.0's Curve Editor sample. Keep its gestures, menus, shortcuts, maths (`GridView`: offsets, scale limits, grid spacing, framing) and look (colours, 50 px tangent arrows, 4 px keys).
- **Deliberate differences:** listed in `README.md`. Add any new one there.

### Curve maths stays identical to the engine
- **Two copies:** `Curves/CurveMath.cs` mirrors KatanaEngine's `Source/Core/Math/Curve.cpp` (`Evaluate`, `ComputeTangent`), in float arithmetic, so values are bit-identical.
- **Changing it:** change the engine first, then mirror it here.
- **The view:** evaluates with the engine's `Curve`, never its own maths.

### Files
- **Format:** `Curves/CurveFile.cs` reads and writes the same XNA XML as the engine's `Curve::LoadFromFile` / `SaveToFile`. It reads expanded `<CurveKey>` elements or the compact key list, and writes the expanded form.
- **Compatibility:** files must keep loading in the engine, XNA and MonoGame.
- **Tangent modes:** only tangent values are stored. Modes are detected on load (`EditCurve.DetectTangentModes`).

### Messages are the only boundary
- **The only channel:** the UI and the game talk only through `KatanaViewport.Post` / `MessageReceived` and `OnHostMessage` / `SendHostMessage`. No other exports, no shared memory.
- **New messages:** add them to the protocol table in `README.md` and the comment in `CurveEditorGame.h`.

### Input in the viewport
- **Mouse:** the hosted game window receives mouse input (including the wheel) but **not keyboard input**. The native side only forwards `mouse` and `wheel` messages.
- **Keyboard:** shortcuts are handled by `MainForm` (`KeyPreview`). A click in the view focuses the viewport control, so keys stop going to the curve list or the boxes.

### Threading
- **Game thread:** `OnHostMessage` and everything in the native game run on it.
- **UI thread:** `MessageReceived` is already marshalled to it.

### Engine vs tool
- **Tool features** belong here. Only truly engine-level pieces go into `KatanaEngine` (e.g. `Curve` itself).
- **Engine rules:** in `KatanaEngine/CLAUDE.md`. They apply to native code here too.
- **Engine changes** are commits in the engine repo:
  1. Check out `master` in the submodule first.
  2. Commit and push there.
  3. Commit the pointer here.
- **Other users of the engine:** ParticleRecorder and AltarsOfEternity share it, so before pushing engine changes, verify they still build.

### Conventions
- **C#:** file-scoped namespaces, `_camelCase` private fields, nullable enabled, warnings are errors. Forms are designer-built (`*.Designer.cs`); toolbar icons are drawn in code (`ToolIcons.cs`), so there are no image files.
- **C++:** match the engine's style: tabs, `m_` members, `PascalCase` methods.
- **Colours in native code:** don't build file-scope constants from engine statics such as `Color::BLACK`. Static initialisation order across the DLL and the engine library isn't defined, so they can come out transparent. Use literal values (`Rgb(...)` in `CurveViewScreen.cpp`).
- **Dependencies:** don't add NuGet packages or other third-party libraries without asking first.
