using System.Globalization;
using System.Numerics;
using CurveEditor.Curves;
using CurveEditor.Editing;

namespace CurveEditor;

public sealed partial class MainForm : Form
{
    private const string AppName = "Curve Editor";

    private readonly CurveEditorController _editor;
    private readonly ToolStripButton[] _modeButtons;
    private bool _isSyncingControls;                // refreshing controls from the editor; ignore their events
    private bool _isRenaming;                       // a curve's name is being edited in the list

    public MainForm()
    {
        InitializeComponent();

        _editor = new CurveEditorController(viewport.ClientSize);
        _editor.Changed += (_, _) => RefreshAll();

        // Runtime data the designer can't hold: the toolbar icons.
        _modeButtons = [selectButton, addKeyButton, moveKeyButton, panButton, zoomButton];
        foreach (var button in _modeButtons) button.Image = ToolIcons.Create(Enum.Parse<EditMode>((string)button.Tag!));

        CreateCurve();
    }

    // ---- Menu: File ----

    private void newMenuItem_Click(object? sender, EventArgs e) => CreateCurve();

    private void openMenuItem_Click(object? sender, EventArgs e)
    {
        if (openCurveDialog.ShowDialog(this) != DialogResult.OK) return;

        foreach (var path in openCurveDialog.FileNames) OpenCurve(path);
    }

    private void saveMenuItem_Click(object? sender, EventArgs e) => SaveSelectedCurves(saveAs: false);

    private void saveAsMenuItem_Click(object? sender, EventArgs e) => SaveSelectedCurves(saveAs: true);

    private void exitMenuItem_Click(object? sender, EventArgs e) => Close();

    private void aboutMenuItem_Click(object? sender, EventArgs e)
    {
        using var about = new AboutDialog();
        about.ShowDialog(this);
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        foreach (var curve in _editor.Curves.Where(curve => curve.IsDirty).ToList())
        {
            var answer = MessageBox.Show(this, $"Save changes to {curve.Name}?", AppName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
            if (answer == DialogResult.Yes && !SaveCurve(curve, saveAs: false)) answer = DialogResult.Cancel;
            if (answer != DialogResult.Cancel) continue;

            e.Cancel = true;
            return;
        }
    }

    // ---- Menus: curve editing ----

    private void undoMenuItem_Click(object? sender, EventArgs e) => _editor.Undo();

    private void redoMenuItem_Click(object? sender, EventArgs e) => _editor.Redo();

    private void frameAllMenuItem_Click(object? sender, EventArgs e) => _editor.FrameAll();

    private void frameSelectionMenuItem_Click(object? sender, EventArgs e) => _editor.FrameSelection();

    private void autoFrameMenuItem_Click(object? sender, EventArgs e) => _editor.SetAutoFrame(!_editor.AutoFrame);

    private void infinityMenuItem_Click(object? sender, EventArgs e) => _editor.SetShowInfinity(!_editor.ShowInfinity);

    private void smoothnessMenuItem_Click(object? sender, EventArgs e) => _editor.SetSmoothness(Enum.Parse<CurveSmoothness>(GetTag(sender)));

    private void keysViewMenuItem_Click(object? sender, EventArgs e) => _editor.SetShowKeys(Enum.Parse<ShowMode>(GetTag(sender)));

    private void tangentsViewMenuItem_Click(object? sender, EventArgs e) => _editor.SetShowTangents(Enum.Parse<ShowMode>(GetTag(sender)));

    // Tag: "Pre,<loop>" or "Post,<loop>".
    private void loopMenuItem_Click(object? sender, EventArgs e)
    {
        var parts = GetTag(sender).Split(',');
        _editor.SetLoop(parts[0] == "Pre", Enum.Parse<CurveLoopType>(parts[1]));
    }

    // Tag: "Tangents,<mode>", "TangentIn,<mode>" or "TangentOut,<mode>".
    private void tangentMenuItem_Click(object? sender, EventArgs e)
    {
        var parts = GetTag(sender).Split(',');
        _editor.SetTangents(Enum.Parse<KeySelection>(parts[0]), Enum.Parse<TangentMode>(parts[1]));
    }

    private void modeButton_Click(object? sender, EventArgs e) => _editor.SetMode(Enum.Parse<EditMode>(GetTag(sender)));

    private static string GetTag(object? sender) => (string)((ToolStripItem)sender!).Tag!;

    // ---- Keyboard ----

    // The viewport never gets keys, so the editor's shortcuts are handled for the whole form,
    // except while typing in a box or renaming a curve.
    private void MainForm_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Modifiers != Keys.None || IsTyping()) return;
        if (e.KeyCode == Keys.Delete && curveListView.Focused) return;  // the list deletes curves
        if (!_editor.KeyDown(e.KeyCode)) return;

        e.Handled = true;
        e.SuppressKeyPress = true;
    }

    private void MainForm_KeyUp(object? sender, KeyEventArgs e)
    {
        if (_editor.KeyUp(e.KeyCode)) e.Handled = true;
    }

    private bool IsTyping() => positionTextBox.Focused || valueTextBox.Focused || _isRenaming;

    // ---- Curve list ----

    private void curveListView_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_isSyncingControls) return;

        _editor.SetEditableCurves(curveListView.SelectedItems.Cast<ListViewItem>().Select(GetCurve).ToList());
    }

    private void curveListView_ItemChecked(object? sender, ItemCheckedEventArgs e)
    {
        if (_isSyncingControls) return;

        _editor.SetVisible(GetCurve(e.Item), e.Item.Checked);
    }

    private void curveListView_BeforeLabelEdit(object? sender, LabelEditEventArgs e) => _isRenaming = true;

    private void curveListView_AfterLabelEdit(object? sender, LabelEditEventArgs e)
    {
        _isRenaming = false;
        e.CancelEdit = true;    // the list shows the name the editor settles on (made unique)
        if (string.IsNullOrWhiteSpace(e.Label)) return;

        var curve = GetCurve(curveListView.Items[e.Item]);
        var name = e.Label;
        BeginInvoke(() => _editor.RenameCurve(curve, name));
    }

    private void curveListView_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete && curveListView.SelectedItems.Count > 0)
        {
            _editor.RemoveCurves(curveListView.SelectedItems.Cast<ListViewItem>().Select(GetCurve).ToList());
            e.Handled = true;
        }
        else if (e.KeyCode == Keys.F2 && curveListView.FocusedItem != null)
        {
            curveListView.FocusedItem.BeginEdit();
            e.Handled = true;
        }
    }

    private static EditCurve GetCurve(ListViewItem item) => (EditCurve)item.Tag!;

    // ---- Viewport ----

    private void viewport_Resize(object? sender, EventArgs e)
    {
        if (viewport.ClientSize.Width > 0 && viewport.ClientSize.Height > 0) _editor?.ResizeView(viewport.ClientSize);
    }

    private void viewport_MessageReceived(object? sender, string message)
    {
        var parts = message.Split(' ');
        switch (parts[0])
        {
            case "ready":
                statusLabel.Text = "Ready";
                SyncViewport();
                break;
            case "mouse" when parts.Length >= 4:
                HandleMouse(parts);
                break;
            case "wheel" when parts.Length >= 2:
                _editor.MouseWheel(int.Parse(parts[1], CultureInfo.InvariantCulture));
                break;
            case "error":
                statusLabel.Text = $"Viewport: {message[6..]}";
                break;
        }
    }

    // mouse down|up <x> <y> left|right, or mouse move <x> <y>
    private void HandleMouse(string[] parts)
    {
        var position = new Vector2(float.Parse(parts[2], CultureInfo.InvariantCulture), float.Parse(parts[3], CultureInfo.InvariantCulture));
        var button = parts.Length > 4 && parts[4] == "right" ? MouseButtons.Right : MouseButtons.Left;

        switch (parts[1])
        {
            case "down":
                // Take focus from the list and boxes (finishing any rename or typing), so keys go to
                // the editor: Delete removes keys, not curves.
                viewport.Focus();
                _editor.MouseDown(position, button);
                break;
            case "move":
                _editor.MouseMove(position);
                break;
            case "up":
                _editor.MouseUp(position, button);
                break;
        }
    }

    private void SyncViewport()
    {
        foreach (var message in _editor.BuildViewportMessages()) viewport.Post(message);
    }

    // ---- Key position and value boxes ----

    private void keyTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            ApplyKeyTextBoxes();
            e.SuppressKeyPress = true;
        }
        else if (e.KeyCode == Keys.Escape)
        {
            RefreshKeyTextBoxes();
            e.SuppressKeyPress = true;
        }
    }

    private void keyTextBox_Leave(object? sender, EventArgs e) => ApplyKeyTextBoxes();

    private void ApplyKeyTextBoxes()
    {
        if (_isSyncingControls || _editor.GetSingleSelection() is not { } selection) return;

        var key = selection.Key;
        var position = ParseOr(positionTextBox.Text, key.Position);
        var value = ParseOr(valueTextBox.Text, key.Value);
        if (position != key.Position || value != key.Value) _editor.SetSingleKey(position, value);
        else RefreshKeyTextBoxes();
    }

    private static float ParseOr(string text, float fallback) =>
        float.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out var value) && float.IsFinite(value)
            ? Math.Clamp(value, -EditCurve.MaxKeyEntry, EditCurve.MaxKeyEntry) : fallback;

    // ---- Curves ----

    private void CreateCurve()
    {
        var curve = new EditCurve(_editor.MakeUniqueName("Curve"), _editor.NextColor());
        curve.Keys.Add(new EditCurveKey());
        _editor.AddCurve(curve);
        statusLabel.Text = $"New curve: {curve.Name}";
    }

    private void OpenCurve(string path)
    {
        try
        {
            var contents = CurveFile.Load(path);
            var curve = new EditCurve(_editor.MakeUniqueName(Path.GetFileNameWithoutExtension(path)), _editor.NextColor())
            {
                PreLoop = contents.PreLoop,
                PostLoop = contents.PostLoop,
                FilePath = path,
            };
            curve.Keys.AddRange(contents.Keys);
            curve.DetectTangentModes();

            _editor.AddCurve(curve);
            curve.IsDirty = false;
            statusLabel.Text = $"Opened {Path.GetFileName(path)}";
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException)
        {
            MessageBox.Show(this, $"Could not open {Path.GetFileName(path)}:\n{ex.Message}", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Saves the curves selected in the list, as XNA's editor did.
    private void SaveSelectedCurves(bool saveAs)
    {
        foreach (var curve in _editor.Curves.Where(curve => curve.IsEditable).ToList())
            if (!SaveCurve(curve, saveAs)) break;

        RefreshAll();
    }

    // Returns false if the save was cancelled or failed.
    private bool SaveCurve(EditCurve curve, bool saveAs)
    {
        var path = curve.FilePath;
        if (path == null || saveAs)
        {
            saveCurveDialog.Title = $"Save {curve.Name}";
            saveCurveDialog.FileName = curve.Name + ".xml";
            if (curve.FilePath != null) saveCurveDialog.InitialDirectory = Path.GetDirectoryName(curve.FilePath);
            if (saveCurveDialog.ShowDialog(this) != DialogResult.OK) return false;

            path = saveCurveDialog.FileName;
        }

        try
        {
            CurveFile.Save(path, curve);
            curve.FilePath = path;
            curve.IsDirty = false;
            statusLabel.Text = $"Saved {Path.GetFileName(path)}";
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            MessageBox.Show(this, $"Could not save {Path.GetFileName(path)}:\n{ex.Message}", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    // ---- Showing the editor's state ----

    private void RefreshAll()
    {
        SyncViewport();

        _isSyncingControls = true;
        try
        {
            RefreshCurveList();
            RefreshMenus();
            RefreshKeyTextBoxes();
        }
        finally
        {
            _isSyncingControls = false;
        }

        var dirty = _editor.Curves.Any(curve => curve.IsDirty) ? "*" : string.Empty;
        Text = $"{AppName}{dirty}";
    }

    private void RefreshCurveList()
    {
        var curves = _editor.Curves;
        var items = curveListView.Items;
        if (items.Count != curves.Count || curves.Where((curve, index) => GetCurve(items[index]) != curve).Any())
        {
            curveListView.BeginUpdate();
            items.Clear();
            foreach (var curve in curves) items.Add(new ListViewItem(curve.Name) { Tag = curve });
            curveListView.EndUpdate();
        }

        foreach (ListViewItem item in items)
        {
            var curve = GetCurve(item);
            if (item.Text != curve.Name) item.Text = curve.Name;
            if (item.Checked != curve.IsVisible) item.Checked = curve.IsVisible;
            if (item.Selected != curve.IsEditable) item.Selected = curve.IsEditable;
            item.ForeColor = curve.Color;
            item.ToolTipText = curve.FilePath ?? "Not saved yet";
        }
    }

    private void RefreshMenus()
    {
        undoMenuItem.Enabled = _editor.History.CanUndo;
        redoMenuItem.Enabled = _editor.History.CanRedo;
        autoFrameMenuItem.Checked = _editor.AutoFrame;
        infinityMenuItem.Checked = _editor.ShowInfinity;
        CheckByTag(smoothnessMenu, _editor.Smoothness.ToString());
        CheckByTag(keysMenu, _editor.ShowKeys.ToString());
        CheckByTag(tangentsViewMenu, _editor.ShowTangents.ToString());

        CheckByTag(preInfinityMenu, $"Pre,{_editor.GetCommonLoop(isPre: true)}");
        CheckByTag(postInfinityMenu, $"Post,{_editor.GetCommonLoop(isPre: false)}");

        var tangentIn = _editor.GetCommonTangent(isOut: false);
        var tangentOut = _editor.GetCommonTangent(isOut: true);
        CheckByTag(inTangentMenu, $"TangentIn,{tangentIn}");
        CheckByTag(outTangentMenu, $"TangentOut,{tangentOut}");
        CheckByTag(tangentsMenu, tangentIn == tangentOut ? $"Tangents,{tangentIn}" : string.Empty);

        var hasSelection = _editor.Curves.Any(curve => curve.IsEditable && curve.HasSelection);
        tangentsMenu.Enabled = hasSelection;
        saveMenuItem.Enabled = saveAsMenuItem.Enabled = _editor.Curves.Any(curve => curve.IsEditable);

        foreach (var button in _modeButtons) button.Checked = (string)button.Tag! == _editor.EffectiveMode.ToString();
    }

    // Checks the items of menu whose tag is tag (none if no tag matches).
    private static void CheckByTag(ToolStripMenuItem menu, string tag)
    {
        foreach (var item in menu.DropDownItems.OfType<ToolStripMenuItem>())
            if (item.Tag is string itemTag) item.Checked = itemTag == tag;
    }

    private void RefreshKeyTextBoxes()
    {
        var selection = _editor.GetSingleSelection();
        positionLabel.Enabled = valueLabel.Enabled = positionTextBox.Enabled = valueTextBox.Enabled = selection != null;

        // Don't overwrite what's being typed.
        if (positionTextBox.Focused || valueTextBox.Focused) return;

        positionTextBox.Text = selection?.Key.Position.ToString("F3", CultureInfo.CurrentCulture) ?? string.Empty;
        valueTextBox.Text = selection?.Key.Value.ToString("F3", CultureInfo.CurrentCulture) ?? string.Empty;
    }
}
