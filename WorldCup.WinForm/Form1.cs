using System.Diagnostics.Metrics;
using System.IO;
using System.Text.Json;
using WorldCup.Data.Models;
using WorldCup.Data.Services;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace WorldCup.WinForm
{
    public partial class Form1 : Form
    {

        // creates objects for each service
        private TeamService _teamService;
        private MatchService _matchService;
        private SettingsService _settingsService;
        private LocalizationService _localizationService;
        private ContextMenuStrip _favoritePlayerContextMenu;



        // lists
        private List<Team> _teams = new();
        private List<Match> _matches = new();
        private List<Player> _favoritePlayers = new();
        private List<Player> _allPlayersInMatch = new();

        // config services
        private ConfigService _configService;

        private TeamStatistics statsHome;
        private TeamStatistics statsAway;



        /* 
         Initializes localization and loads the language file.
        Sets up drag-and-drop behavior for players (between favorites and normal list).
        Sets up right-click menu for favorite players.
        Calls ApplyLocalization() to update UI texts.
         */
        public Form1()
        {
            InitializeComponent();

            
            _localizationService = new LocalizationService();
            _configService = new ConfigService();
            _settingsService = new SettingsService();


            //LoadLang creates a dictionary based on lang
            _localizationService.LoadLanguage(_configService.Settings.Language);


            /*
             This creates a right-click context menu for removing a favorite player
            Menu item is translated dynamically
            When clicked, calls the method RemoveFromFavorites_Click
             */
            _favoritePlayerContextMenu = new ContextMenuStrip();
            _favoritePlayerContextMenu.Items.Add(_localizationService["removeFavoritePlayer"], null, RemoveFromFavorites_Click);

            ApplyLocalization();


            // make drag-and-drop between the two FlowLayoutPanels
            flpPlayers.DragEnter += Panel_DragEnter;
            flpFavPlayers.DragEnter += Panel_DragEnter;

            flpPlayers.DragDrop += PanelPlayers_DragDrop;
            flpFavPlayers.DragDrop += PanelFavoritePlayers_DragDrop;


        }


        /*
         Initializes services (TeamService, MatchService)
        Loads list of teams into cmbCountry
        Loads favorite teams from a text file
        Loads favorite players and displays them as controls inside flpFavPlayers
         */
        private async void Form1_Load(object sender, EventArgs e)
        {

            //OBJECTS OF DIFF SERVICES
            //use the values from "Settings" to change gender
            _teamService = new TeamService(_configService);
            _matchService = new MatchService(_configService);



            //loading teams in combo box Country
            await LoadTeams();


            // load favourite teams from txt file 
            var favTeamFile = "./Data/favourite_teams.txt";
            if (File.Exists(favTeamFile))
            {
                var lines = File.ReadAllLines(favTeamFile);
                foreach (var team in lines)
                {
                    lstFavTeam.Items.Add(team);
                }
            }


            //LOAD FAV PLAYERS
            //getting players from txt file
            _favoritePlayers = _settingsService.LoadFavoritePlayers();
            flpFavPlayers.Controls.Clear();



            /*
             For each player:
            Create a PlayerControl (custom user control showing name, shirt number, etc.)
            Add some margin around it
            Assign right-click menu (Remove from Favorites)
            Add it visually to flpFavPlayers
             */
            foreach (var p in _favoritePlayers)
            {
                try
                {
                    var pControl = new PlayerControl(p, true);
                    pControl.Margin = new Padding(5);
                    pControl.ContextMenuStrip = _favoritePlayerContextMenu;
                    flpFavPlayers.Controls.Add(pControl);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR {ex}");
                    // continue to the next player
                }

            }
        }


        //DRAG AND DROP FUNCTIONS: Handles dragging player controls between: flpPlayers - flpFavPlayers


        // gets triggered when an item is clicked on
        private void Panel_DragEnter(object sender, DragEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"DRAG ENTER e {e}");

            //check if the selected item is PlayerControl type
            if (e.Data.GetDataPresent(typeof(PlayerControl)))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        //detection of moving the panel 
        private void PlayerControl_MouseDown(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"MOUSE DOWN e {e}");

            if (e.Button == MouseButtons.Left)
            {
                this.DoDragDrop(this, DragDropEffects.Move);
            }
        }

        //when the panel is "dropped" in the favourite box below
        private void PanelFavoritePlayers_DragDrop(object sender, DragEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"DRAG DROP e {e}");

            if (e.Data != null && e.Data.GetData(typeof(PlayerControl)) is PlayerControl control)
            {
                flpPlayers.Controls.Remove(control);
                flpFavPlayers.Controls.Add(control);
                control.ContextMenuStrip = _favoritePlayerContextMenu;

                // Update favorite list file
                var player = control.PlayerData;
                if (!_favoritePlayers.Any(p => p.Name == player.Name))
                    _favoritePlayers.Add(player);

                _settingsService.SaveFavoritePlayers(_favoritePlayers);
            }
        }


        //drag from favourite and return to original box
        private void PanelPlayers_DragDrop(object sender, DragEventArgs e)
        {

            System.Diagnostics.Debug.WriteLine($"DRAG DROP TO ORIGINAL e {e}");


            if (e.Data != null && e.Data.GetData(typeof(PlayerControl)) is PlayerControl control)
            {
                var player = control.PlayerData;

                // Check if player is in the original match list
                bool existsInMatch = _allPlayersInMatch.Any(p => p.Name == player.Name);
                if (!existsInMatch)
                {
                    MessageBox.Show(_localizationService["playerDoesNotBelong"]);
                    return;
                }

                // Avoid duplicates
                if (!flpPlayers.Controls.Contains(control))
                {
                    flpFavPlayers.Controls.Remove(control);
                    flpPlayers.Controls.Add(control);
                }

                control.SetFavourite(false);
                control.ContextMenuStrip = null;

                // Update favorite list
                _favoritePlayers.RemoveAll(p => p.Name == player.Name);
                _settingsService.SaveFavoritePlayers(_favoritePlayers);
            }
        }

        //LANGUAGE

        //changes the names of lables,buttons etc based on lang
        private void ApplyLocalization()
        {

            if (_localizationService == null) return;

            this.Text = _localizationService["title"];
            lblFavTeam.Text = _localizationService["favoriteTeam"];
            lblFavPlayers.Text = _localizationService["favoritePlayer"];
            btnMatches.Text = _localizationService["loadMatches"];
            btnPlayers.Text = _localizationService["loadPlayers"];
            btnAddTeam.Text = _localizationService["addToFavorites"];
            btnRemoveTeam.Text = _localizationService["remove"];
            lblCountry.Text = _localizationService["country"];
            lblMatchesList.Text = _localizationService["listOfMatches"];
            lblPlayers.Text = _localizationService["listOfPlayers"];
            btnSettings.Text = _localizationService["settings"];


            // Context menu for remove fav player update 
            _favoritePlayerContextMenu.Items.Clear(); //  = new ContextMenuStrip();
            _favoritePlayerContextMenu.Items.Add(_localizationService["removeFavoritePlayer"], null, RemoveFromFavorites_Click);


            this.Invalidate();
            this.Refresh();
        }



        //based on choosen gender "Country" combo box is filled with teams 
        /*
         Reads gender setting from config
        Calls the API or loads local file using TeamService
        Fills cmbCountry with team names ("CRO - Croatia")
         */
        private async Task LoadTeams()
        {
            _configService.LoadConfig();
            var gender = _configService.Settings.Gender;

            try
            {
                _teams = await _teamService.GetTeamsAsync(gender);
            }
            catch (Exception ex)
            {
                MessageBox.Show(_localizationService["dataLoadingError"]);
                return;
            }

            cmbCountry.Items.Clear();
            foreach (var team in _teams)
            {
                cmbCountry.Items.Add($"{team.FifaCode} - {team.Country}");

            }
        }

       
        private void RemoveFromFavorites_Click(object sender, EventArgs e)
        {
            // The 'sender' is the ToolStripMenuItem that was clicked.
            // Its 'Owner' property will be the ContextMenuStrip.
            // The 'SourceControl' property of the ContextMenuStrip will be the PlayerControl that was right-clicked.
            if (sender is ToolStripItem menuItem && menuItem.Owner is ContextMenuStrip contextMenu)
            {
                if (contextMenu.SourceControl is PlayerControl playerControl)
                {
                    var player = playerControl.PlayerData;

                    // Perform the same logic as when dragging a player out of favorites
                    flpFavPlayers.Controls.Remove(playerControl);

                    playerControl.SetFavourite(false);
                    playerControl.ContextMenuStrip = null; // Remove context menu as it's no longer a favorite

                    _favoritePlayers.RemoveAll(p => p.Name == player.Name);
                    _settingsService.SaveFavoritePlayers(_favoritePlayers);

                    MessageBox.Show($"{player.Name} {_localizationService["playerRemovedFromFavouite"]}");
                    loadPlayers(); // Refresh the main players panel
                }
            }
        }


        //************************************************************************************//



        //COUNTRY
        private void cmbCountry_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lblCountry_Click(object sender, EventArgs e)
        {

        }



        /*
        Removes teams from the list box lstFavTeam
        Saves the updated list to favourite_teams.txt
        */

        //ADD TEAM 
        private void btnAddTeam_Click(object sender, EventArgs e)
        {
            var selectedTeam = cmbCountry.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedTeam)) return;

            if (!lstFavTeam.Items.Contains(selectedTeam))
            {
                lstFavTeam.Items.Add(selectedTeam);

                // Ensure Data folder exists
                Directory.CreateDirectory("./Data");
                // Save favorite teams
                File.WriteAllLines("./Data/favourite_teams.txt", lstFavTeam.Items.Cast<string>());
            }
            else
            {
                MessageBox.Show($"{_localizationService["teamAlredyInFavList"]}");
            }
        }


        //REMOVE FAV TEAMS
        private void btnRemoveTeam_Click(object sender, EventArgs e)
        {
            var selectedIndex = lstFavTeam.SelectedIndex;


            //-1 is if the list is empty
            if (selectedIndex == -1)
            {
                MessageBox.Show($"{_localizationService["selectTeamToRemove"]}");
                return;
            }

            // Remove from UI
            lstFavTeam.Items.RemoveAt(selectedIndex);

            // Save updated list to disk
            Directory.CreateDirectory("./Data");
            File.WriteAllLines("./Data/favourite_teams.txt", lstFavTeam.Items.Cast<string>());

        }


        //LOAD MATCHES BUTTON
        /*
        Uses MatchService.GetMatchesForTeamAsync() to load matches based on selected team and gender
        Parses FIFA code from selected item in cmbCountry
        Populates lstMatches with match info (stage + teams)
         */
        private async void btnMatches_Click(object sender, EventArgs e)
        {
            // fetch from cmbGender value, converts into string and in case nothing is found takes "man"
            _configService.LoadConfig();
            var gender = _configService.Settings.Gender;


            // takes from cmbCountry value
            var selectedTeam = cmbCountry.SelectedItem?.ToString();
            // if no team is selected in cmbCountry exits
            if (string.IsNullOrEmpty(selectedTeam)) return;


            // extracts fifaCode from the string, splits by "-" and takes first part (e.g. CRO-Croatia, only CRO)
            var fifaCode = selectedTeam.Split('-')[0].Trim();
            try
            {
                // fetches all matches for specific gender and team and saves it to _matches list
                _matches = await _matchService.GetMatchesForTeamAsync(gender, fifaCode);

                if (_matches.Count == 0)
                {
                    MessageBox.Show(_localizationService["noMatchesFound"]);

                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(_localizationService["matchLoadError"]);
                System.Diagnostics.Debug.WriteLine("LoadMatches error: " + ex.Message);
                return;
            }
            // clears list 
            lstMatches.Items.Clear();
            // fills list of matches 
            foreach (var match in _matches)
            {
                lstMatches.Items.Add($"{match.StageName}: {match.HomeTeamCountry} vs {match.AwayTeamCountry}");
            }
        }


        //LOAD PLAYERS BUTTON
        /*
        Gets the selected match from lstMatches
        Based on selected team (home/away), shows players from that side
        Skips players already in favorites
        Adds PlayerControl components to flpPlayers
         */
        private void btnPlayers_Click(object sender, EventArgs e)
        {
            loadPlayers();
        }

        //to swithh players based on choosen country
        private void loadPlayers()
        {
            //Get the selected match
            var selectedIndex = lstMatches.SelectedIndex;
            if (selectedIndex == -1) return;

            //Read match data and store home/away stats
            var selectedMatch = _matches[selectedIndex];
            statsHome = selectedMatch.HomeTeamStatistics;
            statsAway = selectedMatch.AwayTeamStatistics;

            //Fill cmbShowTeam with both teams
            cmbShowTeam.Items.Clear();
            cmbShowTeam.Items.AddRange(new[] { statsHome.Country, statsAway.Country });
            // set defualt to the first team
            cmbShowTeam.SelectedIndex = 0;

            //  check which team is the first, assume home
            var stats = statsHome;
            if (cmbShowTeam.SelectedItem == statsAway.Country)
            {
                stats = statsAway;
            }

            //If the selected country is actually the away team, swap the home/ away stats
            var fifaCode = cmbCountry.SelectedItem?.ToString().Split('-')[0].Trim();
            System.Diagnostics.Debug.WriteLine($"fifaCode {fifaCode}");
            if (selectedMatch.AwayTeam.Code == fifaCode)
            {
                statsHome = selectedMatch.AwayTeamStatistics;
                statsAway = selectedMatch.HomeTeamStatistics;
            }

            if (statsHome == null)
            {
                MessageBox.Show($"{_localizationService["noStatistic"]}");
                return;
            }

            List<Player> _allPlayers = new();


            //debug
            var homeTeam = JsonSerializer.Serialize(statsHome, new JsonSerializerOptions { WriteIndented = true });
            var awayTeam = JsonSerializer.Serialize(statsAway, new JsonSerializerOptions { WriteIndented = true });
            System.Diagnostics.Debug.WriteLine($"statsHome: {homeTeam}");
            System.Diagnostics.Debug.WriteLine($"statsAway: {awayTeam}");


            //combines all players
            _allPlayersInMatch = stats.StartingEleven.Concat(stats.Substitutes).ToList();
          

            flpPlayers.Controls.Clear();
            // Iterate through all players (starting eleven + substitutes)
            foreach (var player in _allPlayersInMatch)
            {
                // do not add player to the main list since it is in the facourite list
                bool isFavorite = _favoritePlayers.Any(p => p.Name == player.Name);
                if (isFavorite) continue;

                try
                {
                    // add player to the list of players
                    var playerControl = new PlayerControl(player, isFavorite);
                    playerControl.Margin = new Padding(5);
                    flpPlayers.Controls.Add(playerControl);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR {ex}");
                    // continue to the next player
                }
            }
        }


        //Switches between home and away players
        private void cmbShowTeam_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Coubtry {cmbShowTeam.SelectedItem}");

            var stats = statsHome;
            if (cmbShowTeam.SelectedItem == statsAway.Country)
            {
                stats = statsAway;
            }

            _allPlayersInMatch = stats.StartingEleven.Concat(stats.Substitutes).ToList();

            flpPlayers.Controls.Clear();
            // Iterate through all players (starting eleven + substitutes)
            foreach (var player in _allPlayersInMatch)
            {
                // do not add player to the main list since it is in the facourite list
                bool isFavorite = _favoritePlayers.Any(p => p.Name == player.Name);
                if (isFavorite) continue;

                try
                {
                    // add player to the list of players
                    var playerControl = new PlayerControl(player, isFavorite);
                    playerControl.Margin = new Padding(5);
                    flpPlayers.Controls.Add(playerControl);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR {ex}");
                    // continue to the next player
                }
            }
        }

        //FOR CLOSING
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter || keyData == Keys.Escape)
            {
                AttemptClose(); // Show confirmation
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void AttemptClose()
        {
            if (ConfirmExit())
            {
                this.Close();
            }
            else
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private bool ConfirmExit()
        {
            var result = MessageBox.Show(
                _localizationService["exitQuestion"],
                "Exit",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1
            );

            return result == DialogResult.OK;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ConfirmExit())
            {
                e.Cancel = true;
            }
        }


        //FOR SETTINGS 
        /*
         Opens SettingsForm
        After closing, reloads config and UI localization
        Reloads team list
         */
        private async void btnSettings_Click(object sender, EventArgs e)
        {
            using (var form = new SettingsForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {


                    _configService.LoadConfig();
                    _localizationService = new LocalizationService();
                    _localizationService.LoadLanguage(_configService.Settings.Language);
                    ApplyLocalization();
                    await LoadTeams();
                }
            }
        }

        //LIST OF MATCHES

        private void lstMatches_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        //LIST OF FAV TEAMS AND PLAYERS
        private void lstFavTeam_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void flpPlayers_Paint(object sender, PaintEventArgs e)
        {

        }

        private void flpFavPlayers_Paint(object sender, PaintEventArgs e)
        {

        }


    }
}
