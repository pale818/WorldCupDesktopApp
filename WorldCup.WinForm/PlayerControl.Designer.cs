namespace WorldCup.WinForm
{
    partial class PlayerControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblPlayerInfo = new Label();
            SuspendLayout();
            // 
            // lblPlayerInfo
            // 
            lblPlayerInfo.AutoSize = true;
            lblPlayerInfo.Location = new Point(89, 17);
            lblPlayerInfo.Name = "lblPlayerInfo";
            lblPlayerInfo.Size = new Size(50, 20);
            lblPlayerInfo.TabIndex = 0;
            lblPlayerInfo.Text = "label1";
            lblPlayerInfo.Click += lblPlayerInfo_Click;
            // 
            // PlayerControl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Info;
            Controls.Add(lblPlayerInfo);
            Name = "PlayerControl";
            Size = new Size(365, 49);
            Load += PlayerControl_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblPlayerInfo;
    }
}
