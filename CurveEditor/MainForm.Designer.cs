namespace CurveEditor
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip = new MenuStrip();
            fileMenu = new ToolStripMenuItem();
            newMenuItem = new ToolStripMenuItem();
            openMenuItem = new ToolStripMenuItem();
            fileSeparator1 = new ToolStripSeparator();
            saveMenuItem = new ToolStripMenuItem();
            saveAsMenuItem = new ToolStripMenuItem();
            fileSeparator2 = new ToolStripSeparator();
            exitMenuItem = new ToolStripMenuItem();
            helpMenu = new ToolStripMenuItem();
            aboutMenuItem = new ToolStripMenuItem();
            splitContainer = new SplitContainer();
            curveListView = new ListView();
            viewport = new KatanaEngine.WinForms.KatanaViewport();
            toolStrip = new ToolStrip();
            selectButton = new ToolStripButton();
            addKeyButton = new ToolStripButton();
            moveKeyButton = new ToolStripButton();
            panButton = new ToolStripButton();
            zoomButton = new ToolStripButton();
            toolSeparator = new ToolStripSeparator();
            positionLabel = new ToolStripLabel();
            positionTextBox = new ToolStripTextBox();
            valueLabel = new ToolStripLabel();
            valueTextBox = new ToolStripTextBox();
            curveMenuStrip = new MenuStrip();
            editMenu = new ToolStripMenuItem();
            undoMenuItem = new ToolStripMenuItem();
            redoMenuItem = new ToolStripMenuItem();
            viewMenu = new ToolStripMenuItem();
            frameAllMenuItem = new ToolStripMenuItem();
            frameSelectionMenuItem = new ToolStripMenuItem();
            autoFrameMenuItem = new ToolStripMenuItem();
            viewSeparator = new ToolStripSeparator();
            smoothnessMenu = new ToolStripMenuItem();
            coarseMenuItem = new ToolStripMenuItem();
            roughMenuItem = new ToolStripMenuItem();
            mediumMenuItem = new ToolStripMenuItem();
            fineMenuItem = new ToolStripMenuItem();
            keysMenu = new ToolStripMenuItem();
            keysAlwaysMenuItem = new ToolStripMenuItem();
            keysNeverMenuItem = new ToolStripMenuItem();
            keysActiveOnlyMenuItem = new ToolStripMenuItem();
            tangentsViewMenu = new ToolStripMenuItem();
            tangentsAlwaysMenuItem = new ToolStripMenuItem();
            tangentsNeverMenuItem = new ToolStripMenuItem();
            tangentsActiveOnlyMenuItem = new ToolStripMenuItem();
            infinityMenuItem = new ToolStripMenuItem();
            curvesMenu = new ToolStripMenuItem();
            preInfinityMenu = new ToolStripMenuItem();
            preCycleMenuItem = new ToolStripMenuItem();
            preCycleOffsetMenuItem = new ToolStripMenuItem();
            preOscillateMenuItem = new ToolStripMenuItem();
            preLinearMenuItem = new ToolStripMenuItem();
            preConstantMenuItem = new ToolStripMenuItem();
            postInfinityMenu = new ToolStripMenuItem();
            postCycleMenuItem = new ToolStripMenuItem();
            postCycleOffsetMenuItem = new ToolStripMenuItem();
            postOscillateMenuItem = new ToolStripMenuItem();
            postLinearMenuItem = new ToolStripMenuItem();
            postConstantMenuItem = new ToolStripMenuItem();
            tangentsMenu = new ToolStripMenuItem();
            smoothMenuItem = new ToolStripMenuItem();
            linearMenuItem = new ToolStripMenuItem();
            steppedMenuItem = new ToolStripMenuItem();
            flatMenuItem = new ToolStripMenuItem();
            fixedMenuItem = new ToolStripMenuItem();
            tangentsSeparator = new ToolStripSeparator();
            inTangentMenu = new ToolStripMenuItem();
            inSmoothMenuItem = new ToolStripMenuItem();
            inLinearMenuItem = new ToolStripMenuItem();
            inFlatMenuItem = new ToolStripMenuItem();
            inFixedMenuItem = new ToolStripMenuItem();
            outTangentMenu = new ToolStripMenuItem();
            outSmoothMenuItem = new ToolStripMenuItem();
            outLinearMenuItem = new ToolStripMenuItem();
            outFlatMenuItem = new ToolStripMenuItem();
            outFixedMenuItem = new ToolStripMenuItem();
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            openCurveDialog = new OpenFileDialog();
            saveCurveDialog = new SaveFileDialog();
            menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            toolStrip.SuspendLayout();
            curveMenuStrip.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            //
            // menuStrip
            //
            menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, helpMenu });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(1200, 24);
            menuStrip.TabIndex = 0;
            //
            // fileMenu
            //
            fileMenu.DropDownItems.AddRange(new ToolStripItem[] { newMenuItem, openMenuItem, fileSeparator1, saveMenuItem, saveAsMenuItem, fileSeparator2, exitMenuItem });
            fileMenu.Name = "fileMenu";
            fileMenu.Size = new Size(37, 20);
            fileMenu.Text = "&File";
            //
            // newMenuItem
            //
            newMenuItem.Name = "newMenuItem";
            newMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            newMenuItem.Size = new Size(155, 22);
            newMenuItem.Text = "&New";
            newMenuItem.Click += newMenuItem_Click;
            //
            // openMenuItem
            //
            openMenuItem.Name = "openMenuItem";
            openMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            openMenuItem.Size = new Size(155, 22);
            openMenuItem.Text = "&Open...";
            openMenuItem.Click += openMenuItem_Click;
            //
            // fileSeparator1
            //
            fileSeparator1.Name = "fileSeparator1";
            fileSeparator1.Size = new Size(152, 6);
            //
            // saveMenuItem
            //
            saveMenuItem.Name = "saveMenuItem";
            saveMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            saveMenuItem.Size = new Size(155, 22);
            saveMenuItem.Text = "&Save";
            saveMenuItem.Click += saveMenuItem_Click;
            //
            // saveAsMenuItem
            //
            saveAsMenuItem.Name = "saveAsMenuItem";
            saveAsMenuItem.Size = new Size(155, 22);
            saveAsMenuItem.Text = "Save &As...";
            saveAsMenuItem.Click += saveAsMenuItem_Click;
            //
            // fileSeparator2
            //
            fileSeparator2.Name = "fileSeparator2";
            fileSeparator2.Size = new Size(152, 6);
            //
            // exitMenuItem
            //
            exitMenuItem.Name = "exitMenuItem";
            exitMenuItem.Size = new Size(155, 22);
            exitMenuItem.Text = "E&xit";
            exitMenuItem.Click += exitMenuItem_Click;
            //
            // helpMenu
            //
            helpMenu.DropDownItems.AddRange(new ToolStripItem[] { aboutMenuItem });
            helpMenu.Name = "helpMenu";
            helpMenu.Size = new Size(44, 20);
            helpMenu.Text = "&Help";
            //
            // aboutMenuItem
            //
            aboutMenuItem.Name = "aboutMenuItem";
            aboutMenuItem.Size = new Size(116, 22);
            aboutMenuItem.Text = "&About...";
            aboutMenuItem.Click += aboutMenuItem_Click;
            //
            // splitContainer
            //
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.FixedPanel = FixedPanel.Panel1;
            splitContainer.Location = new Point(0, 24);
            splitContainer.Name = "splitContainer";
            //
            // splitContainer.Panel1
            //
            splitContainer.Panel1.Controls.Add(curveListView);
            //
            // splitContainer.Panel2
            //
            splitContainer.Panel2.Controls.Add(viewport);
            splitContainer.Panel2.Controls.Add(toolStrip);
            splitContainer.Panel2.Controls.Add(curveMenuStrip);
            splitContainer.Size = new Size(1200, 714);
            splitContainer.SplitterDistance = 180;
            splitContainer.TabIndex = 1;
            //
            // curveListView
            //
            curveListView.CheckBoxes = true;
            curveListView.Dock = DockStyle.Fill;
            curveListView.HideSelection = false;
            curveListView.LabelEdit = true;
            curveListView.Location = new Point(0, 0);
            curveListView.Name = "curveListView";
            curveListView.Size = new Size(180, 714);
            curveListView.TabIndex = 0;
            curveListView.UseCompatibleStateImageBehavior = false;
            curveListView.View = View.List;
            curveListView.AfterLabelEdit += curveListView_AfterLabelEdit;
            curveListView.BeforeLabelEdit += curveListView_BeforeLabelEdit;
            curveListView.ItemChecked += curveListView_ItemChecked;
            curveListView.SelectedIndexChanged += curveListView_SelectedIndexChanged;
            curveListView.KeyDown += curveListView_KeyDown;
            //
            // viewport
            //
            viewport.BackColor = Color.FromArgb(228, 233, 240);
            viewport.Dock = DockStyle.Fill;
            viewport.GameLibraryPath = "CurveEditor.Native.dll";
            viewport.Location = new Point(0, 49);
            viewport.Name = "viewport";
            viewport.Size = new Size(1016, 665);
            viewport.TabIndex = 2;
            viewport.MessageReceived += viewport_MessageReceived;
            viewport.Resize += viewport_Resize;
            //
            // toolStrip
            //
            toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            toolStrip.Items.AddRange(new ToolStripItem[] { selectButton, addKeyButton, moveKeyButton, panButton, zoomButton, toolSeparator, positionLabel, positionTextBox, valueLabel, valueTextBox });
            toolStrip.Location = new Point(0, 24);
            toolStrip.Name = "toolStrip";
            toolStrip.Size = new Size(1016, 25);
            toolStrip.TabIndex = 1;
            //
            // selectButton
            //
            selectButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            selectButton.Name = "selectButton";
            selectButton.Size = new Size(23, 22);
            selectButton.Tag = "Select";
            selectButton.Text = "Select Keys (S)";
            selectButton.Click += modeButton_Click;
            //
            // addKeyButton
            //
            addKeyButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            addKeyButton.Name = "addKeyButton";
            addKeyButton.Size = new Size(23, 22);
            addKeyButton.Tag = "Add";
            addKeyButton.Text = "Add Key (A)";
            addKeyButton.Click += modeButton_Click;
            //
            // moveKeyButton
            //
            moveKeyButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            moveKeyButton.Name = "moveKeyButton";
            moveKeyButton.Size = new Size(23, 22);
            moveKeyButton.Tag = "Move";
            moveKeyButton.Text = "Move Keys (D)";
            moveKeyButton.Click += modeButton_Click;
            //
            // panButton
            //
            panButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            panButton.Name = "panButton";
            panButton.Size = new Size(23, 22);
            panButton.Tag = "Pan";
            panButton.Text = "Pan Camera (W, or hold Space)";
            panButton.Click += modeButton_Click;
            //
            // zoomButton
            //
            zoomButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            zoomButton.Name = "zoomButton";
            zoomButton.Size = new Size(23, 22);
            zoomButton.Tag = "Zoom";
            zoomButton.Text = "Zoom (E, or the mouse wheel)";
            zoomButton.Click += modeButton_Click;
            //
            // toolSeparator
            //
            toolSeparator.Name = "toolSeparator";
            toolSeparator.Size = new Size(6, 25);
            //
            // positionLabel
            //
            positionLabel.Name = "positionLabel";
            positionLabel.Size = new Size(50, 22);
            positionLabel.Text = "Position";
            //
            // positionTextBox
            //
            positionTextBox.Name = "positionTextBox";
            positionTextBox.Size = new Size(80, 25);
            positionTextBox.KeyDown += keyTextBox_KeyDown;
            positionTextBox.Leave += keyTextBox_Leave;
            //
            // valueLabel
            //
            valueLabel.Name = "valueLabel";
            valueLabel.Size = new Size(35, 22);
            valueLabel.Text = "Value";
            //
            // valueTextBox
            //
            valueTextBox.Name = "valueTextBox";
            valueTextBox.Size = new Size(80, 25);
            valueTextBox.KeyDown += keyTextBox_KeyDown;
            valueTextBox.Leave += keyTextBox_Leave;
            //
            // curveMenuStrip
            //
            curveMenuStrip.Items.AddRange(new ToolStripItem[] { editMenu, viewMenu, curvesMenu, tangentsMenu });
            curveMenuStrip.Location = new Point(0, 0);
            curveMenuStrip.Name = "curveMenuStrip";
            curveMenuStrip.Size = new Size(1016, 24);
            curveMenuStrip.TabIndex = 0;
            //
            // editMenu
            //
            editMenu.DropDownItems.AddRange(new ToolStripItem[] { undoMenuItem, redoMenuItem });
            editMenu.Name = "editMenu";
            editMenu.Size = new Size(39, 20);
            editMenu.Text = "&Edit";
            //
            // undoMenuItem
            //
            undoMenuItem.Name = "undoMenuItem";
            undoMenuItem.ShortcutKeys = Keys.Control | Keys.Z;
            undoMenuItem.Size = new Size(144, 22);
            undoMenuItem.Text = "&Undo";
            undoMenuItem.Click += undoMenuItem_Click;
            //
            // redoMenuItem
            //
            redoMenuItem.Name = "redoMenuItem";
            redoMenuItem.ShortcutKeys = Keys.Control | Keys.Y;
            redoMenuItem.Size = new Size(144, 22);
            redoMenuItem.Text = "&Redo";
            redoMenuItem.Click += redoMenuItem_Click;
            //
            // viewMenu
            //
            viewMenu.DropDownItems.AddRange(new ToolStripItem[] { frameAllMenuItem, frameSelectionMenuItem, autoFrameMenuItem, viewSeparator, smoothnessMenu, keysMenu, tangentsViewMenu, infinityMenuItem });
            viewMenu.Name = "viewMenu";
            viewMenu.Size = new Size(44, 20);
            viewMenu.Text = "&View";
            //
            // frameAllMenuItem
            //
            frameAllMenuItem.Name = "frameAllMenuItem";
            frameAllMenuItem.ShortcutKeyDisplayString = "F";
            frameAllMenuItem.Size = new Size(180, 22);
            frameAllMenuItem.Text = "Frame &All";
            frameAllMenuItem.Click += frameAllMenuItem_Click;
            //
            // frameSelectionMenuItem
            //
            frameSelectionMenuItem.Name = "frameSelectionMenuItem";
            frameSelectionMenuItem.Size = new Size(180, 22);
            frameSelectionMenuItem.Text = "&Frame Selection";
            frameSelectionMenuItem.Click += frameSelectionMenuItem_Click;
            //
            // autoFrameMenuItem
            //
            autoFrameMenuItem.Name = "autoFrameMenuItem";
            autoFrameMenuItem.Size = new Size(180, 22);
            autoFrameMenuItem.Text = "Aut&o Frame";
            autoFrameMenuItem.Click += autoFrameMenuItem_Click;
            //
            // viewSeparator
            //
            viewSeparator.Name = "viewSeparator";
            viewSeparator.Size = new Size(177, 6);
            //
            // smoothnessMenu
            //
            smoothnessMenu.DropDownItems.AddRange(new ToolStripItem[] { coarseMenuItem, roughMenuItem, mediumMenuItem, fineMenuItem });
            smoothnessMenu.Name = "smoothnessMenu";
            smoothnessMenu.Size = new Size(180, 22);
            smoothnessMenu.Text = "Curve &Smoothness";
            //
            // coarseMenuItem
            //
            coarseMenuItem.Name = "coarseMenuItem";
            coarseMenuItem.Size = new Size(119, 22);
            coarseMenuItem.Tag = "Coarse";
            coarseMenuItem.Text = "&Coarse";
            coarseMenuItem.Click += smoothnessMenuItem_Click;
            //
            // roughMenuItem
            //
            roughMenuItem.Name = "roughMenuItem";
            roughMenuItem.Size = new Size(119, 22);
            roughMenuItem.Tag = "Rough";
            roughMenuItem.Text = "&Rough";
            roughMenuItem.Click += smoothnessMenuItem_Click;
            //
            // mediumMenuItem
            //
            mediumMenuItem.Name = "mediumMenuItem";
            mediumMenuItem.Size = new Size(119, 22);
            mediumMenuItem.Tag = "Medium";
            mediumMenuItem.Text = "&Medium";
            mediumMenuItem.Click += smoothnessMenuItem_Click;
            //
            // fineMenuItem
            //
            fineMenuItem.Name = "fineMenuItem";
            fineMenuItem.Size = new Size(119, 22);
            fineMenuItem.Tag = "Fine";
            fineMenuItem.Text = "&Fine";
            fineMenuItem.Click += smoothnessMenuItem_Click;
            //
            // keysMenu
            //
            keysMenu.DropDownItems.AddRange(new ToolStripItem[] { keysAlwaysMenuItem, keysNeverMenuItem, keysActiveOnlyMenuItem });
            keysMenu.Name = "keysMenu";
            keysMenu.Size = new Size(180, 22);
            keysMenu.Text = "&Keys";
            //
            // keysAlwaysMenuItem
            //
            keysAlwaysMenuItem.Name = "keysAlwaysMenuItem";
            keysAlwaysMenuItem.Size = new Size(135, 22);
            keysAlwaysMenuItem.Tag = "Always";
            keysAlwaysMenuItem.Text = "&Always";
            keysAlwaysMenuItem.Click += keysViewMenuItem_Click;
            //
            // keysNeverMenuItem
            //
            keysNeverMenuItem.Name = "keysNeverMenuItem";
            keysNeverMenuItem.Size = new Size(135, 22);
            keysNeverMenuItem.Tag = "Never";
            keysNeverMenuItem.Text = "&Never";
            keysNeverMenuItem.Click += keysViewMenuItem_Click;
            //
            // keysActiveOnlyMenuItem
            //
            keysActiveOnlyMenuItem.Name = "keysActiveOnlyMenuItem";
            keysActiveOnlyMenuItem.Size = new Size(135, 22);
            keysActiveOnlyMenuItem.Tag = "ActiveOnly";
            keysActiveOnlyMenuItem.Text = "A&ctive Only";
            keysActiveOnlyMenuItem.Click += keysViewMenuItem_Click;
            //
            // tangentsViewMenu
            //
            tangentsViewMenu.DropDownItems.AddRange(new ToolStripItem[] { tangentsAlwaysMenuItem, tangentsNeverMenuItem, tangentsActiveOnlyMenuItem });
            tangentsViewMenu.Name = "tangentsViewMenu";
            tangentsViewMenu.Size = new Size(180, 22);
            tangentsViewMenu.Text = "&Tangents";
            //
            // tangentsAlwaysMenuItem
            //
            tangentsAlwaysMenuItem.Name = "tangentsAlwaysMenuItem";
            tangentsAlwaysMenuItem.Size = new Size(135, 22);
            tangentsAlwaysMenuItem.Tag = "Always";
            tangentsAlwaysMenuItem.Text = "&Always";
            tangentsAlwaysMenuItem.Click += tangentsViewMenuItem_Click;
            //
            // tangentsNeverMenuItem
            //
            tangentsNeverMenuItem.Name = "tangentsNeverMenuItem";
            tangentsNeverMenuItem.Size = new Size(135, 22);
            tangentsNeverMenuItem.Tag = "Never";
            tangentsNeverMenuItem.Text = "&Never";
            tangentsNeverMenuItem.Click += tangentsViewMenuItem_Click;
            //
            // tangentsActiveOnlyMenuItem
            //
            tangentsActiveOnlyMenuItem.Name = "tangentsActiveOnlyMenuItem";
            tangentsActiveOnlyMenuItem.Size = new Size(135, 22);
            tangentsActiveOnlyMenuItem.Tag = "ActiveOnly";
            tangentsActiveOnlyMenuItem.Text = "A&ctive Only";
            tangentsActiveOnlyMenuItem.Click += tangentsViewMenuItem_Click;
            //
            // infinityMenuItem
            //
            infinityMenuItem.Name = "infinityMenuItem";
            infinityMenuItem.Size = new Size(180, 22);
            infinityMenuItem.Text = "&Infinity";
            infinityMenuItem.Click += infinityMenuItem_Click;
            //
            // curvesMenu
            //
            curvesMenu.DropDownItems.AddRange(new ToolStripItem[] { preInfinityMenu, postInfinityMenu });
            curvesMenu.Name = "curvesMenu";
            curvesMenu.Size = new Size(55, 20);
            curvesMenu.Text = "&Curves";
            //
            // preInfinityMenu
            //
            preInfinityMenu.DropDownItems.AddRange(new ToolStripItem[] { preCycleMenuItem, preCycleOffsetMenuItem, preOscillateMenuItem, preLinearMenuItem, preConstantMenuItem });
            preInfinityMenu.Name = "preInfinityMenu";
            preInfinityMenu.Size = new Size(140, 22);
            preInfinityMenu.Text = "&Pre Infinity";
            //
            // preCycleMenuItem
            //
            preCycleMenuItem.Name = "preCycleMenuItem";
            preCycleMenuItem.Size = new Size(170, 22);
            preCycleMenuItem.Tag = "Pre,Cycle";
            preCycleMenuItem.Text = "C&ycle";
            preCycleMenuItem.Click += loopMenuItem_Click;
            //
            // preCycleOffsetMenuItem
            //
            preCycleOffsetMenuItem.Name = "preCycleOffsetMenuItem";
            preCycleOffsetMenuItem.Size = new Size(170, 22);
            preCycleOffsetMenuItem.Tag = "Pre,CycleOffset";
            preCycleOffsetMenuItem.Text = "Cycle with &Offset";
            preCycleOffsetMenuItem.Click += loopMenuItem_Click;
            //
            // preOscillateMenuItem
            //
            preOscillateMenuItem.Name = "preOscillateMenuItem";
            preOscillateMenuItem.Size = new Size(170, 22);
            preOscillateMenuItem.Tag = "Pre,Oscillate";
            preOscillateMenuItem.Text = "O&scillate";
            preOscillateMenuItem.Click += loopMenuItem_Click;
            //
            // preLinearMenuItem
            //
            preLinearMenuItem.Name = "preLinearMenuItem";
            preLinearMenuItem.Size = new Size(170, 22);
            preLinearMenuItem.Tag = "Pre,Linear";
            preLinearMenuItem.Text = "&Linear";
            preLinearMenuItem.Click += loopMenuItem_Click;
            //
            // preConstantMenuItem
            //
            preConstantMenuItem.Name = "preConstantMenuItem";
            preConstantMenuItem.Size = new Size(170, 22);
            preConstantMenuItem.Tag = "Pre,Constant";
            preConstantMenuItem.Text = "&Constant";
            preConstantMenuItem.Click += loopMenuItem_Click;
            //
            // postInfinityMenu
            //
            postInfinityMenu.DropDownItems.AddRange(new ToolStripItem[] { postCycleMenuItem, postCycleOffsetMenuItem, postOscillateMenuItem, postLinearMenuItem, postConstantMenuItem });
            postInfinityMenu.Name = "postInfinityMenu";
            postInfinityMenu.Size = new Size(140, 22);
            postInfinityMenu.Text = "P&ost Infinity";
            //
            // postCycleMenuItem
            //
            postCycleMenuItem.Name = "postCycleMenuItem";
            postCycleMenuItem.Size = new Size(170, 22);
            postCycleMenuItem.Tag = "Post,Cycle";
            postCycleMenuItem.Text = "C&ycle";
            postCycleMenuItem.Click += loopMenuItem_Click;
            //
            // postCycleOffsetMenuItem
            //
            postCycleOffsetMenuItem.Name = "postCycleOffsetMenuItem";
            postCycleOffsetMenuItem.Size = new Size(170, 22);
            postCycleOffsetMenuItem.Tag = "Post,CycleOffset";
            postCycleOffsetMenuItem.Text = "Cycle with &Offset";
            postCycleOffsetMenuItem.Click += loopMenuItem_Click;
            //
            // postOscillateMenuItem
            //
            postOscillateMenuItem.Name = "postOscillateMenuItem";
            postOscillateMenuItem.Size = new Size(170, 22);
            postOscillateMenuItem.Tag = "Post,Oscillate";
            postOscillateMenuItem.Text = "O&scillate";
            postOscillateMenuItem.Click += loopMenuItem_Click;
            //
            // postLinearMenuItem
            //
            postLinearMenuItem.Name = "postLinearMenuItem";
            postLinearMenuItem.Size = new Size(170, 22);
            postLinearMenuItem.Tag = "Post,Linear";
            postLinearMenuItem.Text = "&Linear";
            postLinearMenuItem.Click += loopMenuItem_Click;
            //
            // postConstantMenuItem
            //
            postConstantMenuItem.Name = "postConstantMenuItem";
            postConstantMenuItem.Size = new Size(170, 22);
            postConstantMenuItem.Tag = "Post,Constant";
            postConstantMenuItem.Text = "&Constant";
            postConstantMenuItem.Click += loopMenuItem_Click;
            //
            // tangentsMenu
            //
            tangentsMenu.DropDownItems.AddRange(new ToolStripItem[] { smoothMenuItem, linearMenuItem, steppedMenuItem, flatMenuItem, fixedMenuItem, tangentsSeparator, inTangentMenu, outTangentMenu });
            tangentsMenu.Name = "tangentsMenu";
            tangentsMenu.Size = new Size(67, 20);
            tangentsMenu.Text = "&Tangents";
            //
            // smoothMenuItem
            //
            smoothMenuItem.Name = "smoothMenuItem";
            smoothMenuItem.Size = new Size(140, 22);
            smoothMenuItem.Tag = "Tangents,Smooth";
            smoothMenuItem.Text = "&Smooth";
            smoothMenuItem.Click += tangentMenuItem_Click;
            //
            // linearMenuItem
            //
            linearMenuItem.Name = "linearMenuItem";
            linearMenuItem.Size = new Size(140, 22);
            linearMenuItem.Tag = "Tangents,Linear";
            linearMenuItem.Text = "&Linear";
            linearMenuItem.Click += tangentMenuItem_Click;
            //
            // steppedMenuItem
            //
            steppedMenuItem.Name = "steppedMenuItem";
            steppedMenuItem.Size = new Size(140, 22);
            steppedMenuItem.Tag = "Tangents,Stepped";
            steppedMenuItem.Text = "S&tepped";
            steppedMenuItem.Click += tangentMenuItem_Click;
            //
            // flatMenuItem
            //
            flatMenuItem.Name = "flatMenuItem";
            flatMenuItem.Size = new Size(140, 22);
            flatMenuItem.Tag = "Tangents,Flat";
            flatMenuItem.Text = "&Flat";
            flatMenuItem.Click += tangentMenuItem_Click;
            //
            // fixedMenuItem
            //
            fixedMenuItem.Name = "fixedMenuItem";
            fixedMenuItem.Size = new Size(140, 22);
            fixedMenuItem.Tag = "Tangents,Fixed";
            fixedMenuItem.Text = "Fi&xed";
            fixedMenuItem.Click += tangentMenuItem_Click;
            //
            // tangentsSeparator
            //
            tangentsSeparator.Name = "tangentsSeparator";
            tangentsSeparator.Size = new Size(137, 6);
            //
            // inTangentMenu
            //
            inTangentMenu.DropDownItems.AddRange(new ToolStripItem[] { inSmoothMenuItem, inLinearMenuItem, inFlatMenuItem, inFixedMenuItem });
            inTangentMenu.Name = "inTangentMenu";
            inTangentMenu.Size = new Size(140, 22);
            inTangentMenu.Text = "&In Tangent";
            //
            // inSmoothMenuItem
            //
            inSmoothMenuItem.Name = "inSmoothMenuItem";
            inSmoothMenuItem.Size = new Size(120, 22);
            inSmoothMenuItem.Tag = "TangentIn,Smooth";
            inSmoothMenuItem.Text = "&Smooth";
            inSmoothMenuItem.Click += tangentMenuItem_Click;
            //
            // inLinearMenuItem
            //
            inLinearMenuItem.Name = "inLinearMenuItem";
            inLinearMenuItem.Size = new Size(120, 22);
            inLinearMenuItem.Tag = "TangentIn,Linear";
            inLinearMenuItem.Text = "&Linear";
            inLinearMenuItem.Click += tangentMenuItem_Click;
            //
            // inFlatMenuItem
            //
            inFlatMenuItem.Name = "inFlatMenuItem";
            inFlatMenuItem.Size = new Size(120, 22);
            inFlatMenuItem.Tag = "TangentIn,Flat";
            inFlatMenuItem.Text = "&Flat";
            inFlatMenuItem.Click += tangentMenuItem_Click;
            //
            // inFixedMenuItem
            //
            inFixedMenuItem.Name = "inFixedMenuItem";
            inFixedMenuItem.Size = new Size(120, 22);
            inFixedMenuItem.Tag = "TangentIn,Fixed";
            inFixedMenuItem.Text = "Fi&xed";
            inFixedMenuItem.Click += tangentMenuItem_Click;
            //
            // outTangentMenu
            //
            outTangentMenu.DropDownItems.AddRange(new ToolStripItem[] { outSmoothMenuItem, outLinearMenuItem, outFlatMenuItem, outFixedMenuItem });
            outTangentMenu.Name = "outTangentMenu";
            outTangentMenu.Size = new Size(140, 22);
            outTangentMenu.Text = "&Out Tangent";
            //
            // outSmoothMenuItem
            //
            outSmoothMenuItem.Name = "outSmoothMenuItem";
            outSmoothMenuItem.Size = new Size(120, 22);
            outSmoothMenuItem.Tag = "TangentOut,Smooth";
            outSmoothMenuItem.Text = "&Smooth";
            outSmoothMenuItem.Click += tangentMenuItem_Click;
            //
            // outLinearMenuItem
            //
            outLinearMenuItem.Name = "outLinearMenuItem";
            outLinearMenuItem.Size = new Size(120, 22);
            outLinearMenuItem.Tag = "TangentOut,Linear";
            outLinearMenuItem.Text = "&Linear";
            outLinearMenuItem.Click += tangentMenuItem_Click;
            //
            // outFlatMenuItem
            //
            outFlatMenuItem.Name = "outFlatMenuItem";
            outFlatMenuItem.Size = new Size(120, 22);
            outFlatMenuItem.Tag = "TangentOut,Flat";
            outFlatMenuItem.Text = "&Flat";
            outFlatMenuItem.Click += tangentMenuItem_Click;
            //
            // outFixedMenuItem
            //
            outFixedMenuItem.Name = "outFixedMenuItem";
            outFixedMenuItem.Size = new Size(120, 22);
            outFixedMenuItem.Tag = "TangentOut,Fixed";
            outFixedMenuItem.Text = "Fi&xed";
            outFixedMenuItem.Click += tangentMenuItem_Click;
            //
            // statusStrip
            //
            statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
            statusStrip.Location = new Point(0, 738);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(1200, 22);
            statusStrip.TabIndex = 2;
            //
            // statusLabel
            //
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(1185, 17);
            statusLabel.Spring = true;
            statusLabel.Text = "Starting...";
            statusLabel.TextAlign = ContentAlignment.MiddleLeft;
            //
            // openCurveDialog
            //
            openCurveDialog.Filter = "Curve files (*.xml)|*.xml|All files (*.*)|*.*";
            openCurveDialog.Multiselect = true;
            openCurveDialog.Title = "Open Curves";
            //
            // saveCurveDialog
            //
            saveCurveDialog.DefaultExt = "xml";
            saveCurveDialog.Filter = "Curve files (*.xml)|*.xml|All files (*.*)|*.*";
            //
            // MainForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 760);
            Controls.Add(splitContainer);
            Controls.Add(statusStrip);
            Controls.Add(menuStrip);
            KeyPreview = true;
            MainMenuStrip = menuStrip;
            MinimumSize = new Size(640, 400);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Curve Editor";
            FormClosing += MainForm_FormClosing;
            KeyDown += MainForm_KeyDown;
            KeyUp += MainForm_KeyUp;
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            curveMenuStrip.ResumeLayout(false);
            curveMenuStrip.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip;
        private ToolStripMenuItem fileMenu;
        private ToolStripMenuItem newMenuItem;
        private ToolStripMenuItem openMenuItem;
        private ToolStripSeparator fileSeparator1;
        private ToolStripMenuItem saveMenuItem;
        private ToolStripMenuItem saveAsMenuItem;
        private ToolStripSeparator fileSeparator2;
        private ToolStripMenuItem exitMenuItem;
        private ToolStripMenuItem helpMenu;
        private ToolStripMenuItem aboutMenuItem;
        private SplitContainer splitContainer;
        private ListView curveListView;
        private KatanaEngine.WinForms.KatanaViewport viewport;
        private ToolStrip toolStrip;
        private ToolStripButton selectButton;
        private ToolStripButton addKeyButton;
        private ToolStripButton moveKeyButton;
        private ToolStripButton panButton;
        private ToolStripButton zoomButton;
        private ToolStripSeparator toolSeparator;
        private ToolStripLabel positionLabel;
        private ToolStripTextBox positionTextBox;
        private ToolStripLabel valueLabel;
        private ToolStripTextBox valueTextBox;
        private MenuStrip curveMenuStrip;
        private ToolStripMenuItem editMenu;
        private ToolStripMenuItem undoMenuItem;
        private ToolStripMenuItem redoMenuItem;
        private ToolStripMenuItem viewMenu;
        private ToolStripMenuItem frameAllMenuItem;
        private ToolStripMenuItem frameSelectionMenuItem;
        private ToolStripMenuItem autoFrameMenuItem;
        private ToolStripSeparator viewSeparator;
        private ToolStripMenuItem smoothnessMenu;
        private ToolStripMenuItem coarseMenuItem;
        private ToolStripMenuItem roughMenuItem;
        private ToolStripMenuItem mediumMenuItem;
        private ToolStripMenuItem fineMenuItem;
        private ToolStripMenuItem keysMenu;
        private ToolStripMenuItem keysAlwaysMenuItem;
        private ToolStripMenuItem keysNeverMenuItem;
        private ToolStripMenuItem keysActiveOnlyMenuItem;
        private ToolStripMenuItem tangentsViewMenu;
        private ToolStripMenuItem tangentsAlwaysMenuItem;
        private ToolStripMenuItem tangentsNeverMenuItem;
        private ToolStripMenuItem tangentsActiveOnlyMenuItem;
        private ToolStripMenuItem infinityMenuItem;
        private ToolStripMenuItem curvesMenu;
        private ToolStripMenuItem preInfinityMenu;
        private ToolStripMenuItem preCycleMenuItem;
        private ToolStripMenuItem preCycleOffsetMenuItem;
        private ToolStripMenuItem preOscillateMenuItem;
        private ToolStripMenuItem preLinearMenuItem;
        private ToolStripMenuItem preConstantMenuItem;
        private ToolStripMenuItem postInfinityMenu;
        private ToolStripMenuItem postCycleMenuItem;
        private ToolStripMenuItem postCycleOffsetMenuItem;
        private ToolStripMenuItem postOscillateMenuItem;
        private ToolStripMenuItem postLinearMenuItem;
        private ToolStripMenuItem postConstantMenuItem;
        private ToolStripMenuItem tangentsMenu;
        private ToolStripMenuItem smoothMenuItem;
        private ToolStripMenuItem linearMenuItem;
        private ToolStripMenuItem steppedMenuItem;
        private ToolStripMenuItem flatMenuItem;
        private ToolStripMenuItem fixedMenuItem;
        private ToolStripSeparator tangentsSeparator;
        private ToolStripMenuItem inTangentMenu;
        private ToolStripMenuItem inSmoothMenuItem;
        private ToolStripMenuItem inLinearMenuItem;
        private ToolStripMenuItem inFlatMenuItem;
        private ToolStripMenuItem inFixedMenuItem;
        private ToolStripMenuItem outTangentMenu;
        private ToolStripMenuItem outSmoothMenuItem;
        private ToolStripMenuItem outLinearMenuItem;
        private ToolStripMenuItem outFlatMenuItem;
        private ToolStripMenuItem outFixedMenuItem;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private OpenFileDialog openCurveDialog;
        private SaveFileDialog saveCurveDialog;
    }
}
