namespace WorldCup.WinForm
{
    partial class Form1
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
            cmbCountry = new ComboBox();
            lblCountry = new Label();
            btnPlayers = new Button();
            btnMatches = new Button();
            btnRemoveTeam = new Button();
            btnAddTeam = new Button();
            lstMatches = new ListBox();
            lstFavTeam = new ListBox();
            lblMatchesList = new Label();
            lblFavTeam = new Label();
            flpPlayers = new FlowLayoutPanel();
            flpFavPlayers = new FlowLayoutPanel();
            lblPlayers = new Label();
            lblFavPlayers = new Label();
            btnSettings = new Button();
            SuspendLayout();
            // 
            // cmbCountry
            // 
            cmbCountry.FormattingEnabled = true;
            cmbCountry.Location = new Point(230, 41);
            cmbCountry.Name = "cmbCountry";
            cmbCountry.Size = new Size(158, 28);
            cmbCountry.TabIndex = 1;
            cmbCountry.SelectedIndexChanged += cmbCountry_SelectedIndexChanged;
            // 
            // lblCountry
            // 
            lblCountry.AutoSize = true;
            lblCountry.Location = new Point(257, 19);
            lblCountry.Name = "lblCountry";
            lblCountry.Size = new Size(60, 20);
            lblCountry.TabIndex = 3;
            lblCountry.Text = "Country";
            lblCountry.Click += lblCountry_Click;
            // 
            // btnPlayers
            // 
            btnPlayers.Location = new Point(22, 139);
            btnPlayers.Name = "btnPlayers";
            btnPlayers.Size = new Size(133, 29);
            btnPlayers.TabIndex = 6;
            btnPlayers.Text = "Load Players";
            btnPlayers.UseVisualStyleBackColor = true;
            btnPlayers.Click += btnPlayers_Click;
            // 
            // btnMatches
            // 
            btnMatches.Location = new Point(638, 39);
            btnMatches.Name = "btnMatches";
            btnMatches.Size = new Size(259, 29);
            btnMatches.TabIndex = 7;
            btnMatches.Text = "Load Matches";
            btnMatches.UseVisualStyleBackColor = true;
            btnMatches.Click += btnMatches_Click;
            // 
            // btnRemoveTeam
            // 
            btnRemoveTeam.Location = new Point(638, 352);
            btnRemoveTeam.Name = "btnRemoveTeam";
            btnRemoveTeam.Size = new Size(259, 29);
            btnRemoveTeam.TabIndex = 8;
            btnRemoveTeam.Text = "Remove Team";
            btnRemoveTeam.UseVisualStyleBackColor = true;
            btnRemoveTeam.Click += btnRemoveTeam_Click;
            // 
            // btnAddTeam
            // 
            btnAddTeam.Location = new Point(230, 84);
            btnAddTeam.Name = "btnAddTeam";
            btnAddTeam.Size = new Size(192, 29);
            btnAddTeam.TabIndex = 9;
            btnAddTeam.Text = "Add Favourite Team";
            btnAddTeam.UseVisualStyleBackColor = true;
            btnAddTeam.Click += btnAddTeam_Click;
            // 
            // lstMatches
            // 
            lstMatches.FormattingEnabled = true;
            lstMatches.Location = new Point(638, 84);
            lstMatches.Name = "lstMatches";
            lstMatches.Size = new Size(259, 144);
            lstMatches.TabIndex = 10;
            lstMatches.SelectedIndexChanged += lstMatches_SelectedIndexChanged;
            // 
            // lstFavTeam
            // 
            lstFavTeam.FormattingEnabled = true;
            lstFavTeam.Location = new Point(638, 415);
            lstFavTeam.Name = "lstFavTeam";
            lstFavTeam.Size = new Size(259, 144);
            lstFavTeam.TabIndex = 11;
            lstFavTeam.SelectedIndexChanged += lstFavTeam_SelectedIndexChanged;
            // 
            // lblMatchesList
            // 
            lblMatchesList.AutoSize = true;
            lblMatchesList.Location = new Point(638, 248);
            lblMatchesList.Name = "lblMatchesList";
            lblMatchesList.Size = new Size(108, 20);
            lblMatchesList.TabIndex = 12;
            lblMatchesList.Text = "List of matches";
            // 
            // lblFavTeam
            // 
            lblFavTeam.AutoSize = true;
            lblFavTeam.Location = new Point(638, 588);
            lblFavTeam.Name = "lblFavTeam";
            lblFavTeam.Size = new Size(109, 20);
            lblFavTeam.TabIndex = 13;
            lblFavTeam.Text = "Favourite Team";
            // 
            // flpPlayers
            // 
            flpPlayers.AllowDrop = true;
            flpPlayers.AutoScroll = true;
            flpPlayers.BorderStyle = BorderStyle.FixedSingle;
            flpPlayers.Location = new Point(22, 193);
            flpPlayers.Name = "flpPlayers";
            flpPlayers.Size = new Size(445, 165);
            flpPlayers.TabIndex = 14;
            flpPlayers.Paint += flpPlayers_Paint;
            // 
            // flpFavPlayers
            // 
            flpFavPlayers.AllowDrop = true;
            flpFavPlayers.AutoScroll = true;
            flpFavPlayers.BorderStyle = BorderStyle.FixedSingle;
            flpFavPlayers.Location = new Point(22, 415);
            flpFavPlayers.Name = "flpFavPlayers";
            flpFavPlayers.Size = new Size(445, 159);
            flpFavPlayers.TabIndex = 15;
            flpFavPlayers.Paint += flpFavPlayers_Paint;
            // 
            // lblPlayers
            // 
            lblPlayers.AutoSize = true;
            lblPlayers.Location = new Point(22, 374);
            lblPlayers.Name = "lblPlayers";
            lblPlayers.Size = new Size(100, 20);
            lblPlayers.TabIndex = 16;
            lblPlayers.Text = "List of players";
            // 
            // lblFavPlayers
            // 
            lblFavPlayers.AutoSize = true;
            lblFavPlayers.Location = new Point(22, 588);
            lblFavPlayers.Name = "lblFavPlayers";
            lblFavPlayers.Size = new Size(120, 20);
            lblFavPlayers.TabIndex = 17;
            lblFavPlayers.Text = "Favourite players";
            // 
            // btnSettings
            // 
            btnSettings.Location = new Point(28, 39);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new Size(94, 29);
            btnSettings.TabIndex = 18;
            btnSettings.Text = "Settings";
            btnSettings.UseVisualStyleBackColor = true;
            btnSettings.Click += btnSettings_Click;
            // 
            // Form1
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(976, 627);
            Controls.Add(btnSettings);
            Controls.Add(lblFavPlayers);
            Controls.Add(lblPlayers);
            Controls.Add(flpFavPlayers);
            Controls.Add(flpPlayers);
            Controls.Add(lblFavTeam);
            Controls.Add(lblMatchesList);
            Controls.Add(lstFavTeam);
            Controls.Add(lstMatches);
            Controls.Add(btnAddTeam);
            Controls.Add(btnRemoveTeam);
            Controls.Add(btnMatches);
            Controls.Add(btnPlayers);
            Controls.Add(lblCountry);
            Controls.Add(cmbCountry);
            Name = "Form1";
            Text = "Form1";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ComboBox cmbCountry;
        private Label lblCountry;
        private Button btnPlayers;
        private Button btnMatches;
        private Button btnRemoveTeam;
        private Button btnAddTeam;
        private ListBox lstMatches;
        private ListBox lstFavTeam;
        private Label lblMatchesList;
        private Label lblFavTeam;
        private FlowLayoutPanel flpPlayers;
        private FlowLayoutPanel flpFavPlayers;
        private Label lblPlayers;
        private Label lblFavPlayers;
        private Button btnSettings;
    }
}
