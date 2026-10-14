namespace CurveEditor
{
    partial class AboutDialog
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
            titleLabel = new Label();
            descriptionLabel = new Label();
            okButton = new Button();
            SuspendLayout();
            //
            // titleLabel
            //
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            titleLabel.Location = new Point(16, 16);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(129, 25);
            titleLabel.TabIndex = 0;
            titleLabel.Text = "Curve Editor";
            //
            // descriptionLabel
            //
            descriptionLabel.Location = new Point(18, 52);
            descriptionLabel.Name = "descriptionLabel";
            descriptionLabel.Size = new Size(380, 96);
            descriptionLabel.TabIndex = 1;
            descriptionLabel.Text = "Edits 1D keyframed curves in XNA's curve XML format, for KatanaEngine's Curve class (and XNA and MonoGame).\r\n\r\nA rebuild of the Curve Editor sample from XNA Game Studio 4.0, on WinForms and KatanaEngine.";
            //
            // okButton
            //
            okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            okButton.DialogResult = DialogResult.OK;
            okButton.Location = new Point(323, 156);
            okButton.Name = "okButton";
            okButton.Size = new Size(75, 27);
            okButton.TabIndex = 2;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            //
            // AboutDialog
            //
            AcceptButton = okButton;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = okButton;
            ClientSize = new Size(414, 197);
            Controls.Add(okButton);
            Controls.Add(descriptionLabel);
            Controls.Add(titleLabel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutDialog";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "About Curve Editor";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label titleLabel;
        private Label descriptionLabel;
        private Button okButton;
    }
}
