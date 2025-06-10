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
            playerImage = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)playerImage).BeginInit();
            SuspendLayout();
            // 
            // lblPlayerInfo
            // 
            lblPlayerInfo.AutoSize = true;
            lblPlayerInfo.Location = new Point(3, 32);
            lblPlayerInfo.Name = "lblPlayerInfo";
            lblPlayerInfo.Size = new Size(50, 20);
            lblPlayerInfo.TabIndex = 0;
            lblPlayerInfo.Text = "label1";
            lblPlayerInfo.Click += lblPlayerInfo_Click;
            // 
            // playerImage
            // 
            playerImage.Location = new Point(273, 3);
            playerImage.Name = "playerImage";
            playerImage.Size = new Size(80, 80);
            playerImage.SizeMode = PictureBoxSizeMode.StretchImage;
            playerImage.TabIndex = 1;
            playerImage.TabStop = false;
            playerImage.Click += playerImage_Click;
            // 
            // PlayerControl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Info;
            Controls.Add(playerImage);
            Controls.Add(lblPlayerInfo);
            Name = "PlayerControl";
            Size = new Size(417, 88);
            Load += PlayerControl_Load;
            ((System.ComponentModel.ISupportInitialize)playerImage).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblPlayerInfo;
        private PictureBox playerImage;
    }
}
