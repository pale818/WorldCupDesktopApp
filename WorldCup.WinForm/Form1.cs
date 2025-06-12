using System.Diagnostics.Metrics;
using System.IO;
using System.Text.Json;
using WorldCup.Data.Models;
using WorldCup.Data.Services;

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
        private AppConfig _appConfig;

        private TeamStatistics statsHome;
        private TeamStatistics statsAway;
       

        public Form1()
        {
            InitializeComponent();
            _localizationService = new LocalizationService();

            //_confServ works with public "Settings" variable from CongifServ class
            _configService = new ConfigService();
            _settingsService = new SettingsService();


            //LoadLang creates a dictionary based on lang
            _localizationService.LoadLanguage(_configService.Settings.Language);


            // REMOVES FROM FAV LIST PLAYERS BY CONTEXT MENU
            _favoritePlayerContextMenu = new ContextMenuStrip();
            _favoritePlayerContextMenu.Items.Add(_localizationService["removeFavoritePlayer"], null, RemoveFromFavorites_Click);

            ApplyLocalization();


            flpPlayers.DragEnter += Panel_DragEnter;
            flpFavPlayers.DragEnter += Panel_DragEnter;

            flpPlayers.DragDrop += PanelPlayers_DragDrop;
            flpFavPlayers.DragDrop += PanelFavoritePlayers_DragDrop;


            this.DragEnter += MainForm_DragEnter;
            this.DragDrop += MainForm_DragDrop;
        }


        //first called func after Form1
        private async void Form1_Load(object sender, EventArgs e)
        {

            //OBJECTS OF DIFF SERVICES
            //use the values from "Settings" to change gender
            _teamService = new TeamService(_configService);
            _matchService = new MatchService(_configService);



            //loading teams in combo box Country
            await LoadTeams();



            // ensure to load favourite teams from txt file into the list of fav teams
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


        //DRAG AND DROP FUNCTIONS:


        //when you first click on the panel
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

                // Update favorite list
                var player = control.PlayerData;
                if (!_favoritePlayers.Any(p => p.Name == player.Name))
                    _favoritePlayers.Add(player);

                _settingsService.SaveFavoritePlayers(_favoritePlayers);
            }
        }


        //drag from favourite and return to original box
        private void PanelPlayers_DragDrop(object sender, DragEventArgs e)
        {
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


        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(PlayerControl)))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetData(typeof(PlayerControl)) is PlayerControl control)
            {
                var player = control.PlayerData;

                // Check if it's currently in favorites
                if (control.IsFavourite)
                {
                    flpFavPlayers.Controls.Remove(control);

                    control.SetFavourite(false);
                    control.ContextMenuStrip = null;

                    _favoritePlayers.RemoveAll(p => p.Name == player.Name);
                    _settingsService.SaveFavoritePlayers(_favoritePlayers);

                    MessageBox.Show($"{player.Name} {_localizationService["playerRemovedFromFavouite"]}");
                    loadPlayers();
                }
            }
        }




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
        private async Task LoadTeams()
        {
            _configService = new ConfigService();
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
        private async void btnMatches_Click(object sender, EventArgs e)
        {
            // fetch from cmbGender value, converts into string and in case nothing is found takes "man"
            _configService = new ConfigService();
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
        private void btnPlayers_Click(object sender, EventArgs e)
        {
            loadPlayers();
        }

        private void loadPlayers()
        {

            var selectedIndex = lstMatches.SelectedIndex;
            if (selectedIndex == -1) return;

            var selectedMatch = _matches[selectedIndex];
            statsHome = selectedMatch.HomeTeamStatistics;
            statsAway = selectedMatch.AwayTeamStatistics;

            // set fifa code in 
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

            var homeTeam = JsonSerializer.Serialize(statsHome, new JsonSerializerOptions { WriteIndented = true });
            var awayTeam = JsonSerializer.Serialize(statsAway, new JsonSerializerOptions { WriteIndented = true });

            System.Diagnostics.Debug.WriteLine($"statsHome: {homeTeam}");
            System.Diagnostics.Debug.WriteLine($"statsAway: {awayTeam}");


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
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show(_localizationService["exitQuestion"], "OK",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (result == DialogResult.No)
            {
                e.Cancel = true; // prevent closing
            }
        }


        private async void btnSettings_Click(object sender, EventArgs e)
        {
            using (var form = new SettingsForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // refresh UI
                    System.Diagnostics.Debug.WriteLine("REFRESH UI");


                    _configService = new ConfigService();
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
