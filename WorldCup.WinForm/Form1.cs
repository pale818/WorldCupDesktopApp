using System.Diagnostics.Metrics;
using System.IO;
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


        // introduced because gender was overwritten 
        // so to solve this raise condition this var is introduced
        private bool _suppressGenderSave = false;

        public Form1()
        {
            InitializeComponent();
            _localizationService = new LocalizationService();

            flpPlayers.DragEnter += Panel_DragEnter;
            flpFavPlayers.DragEnter += Panel_DragEnter;

            flpPlayers.DragDrop += PanelPlayers_DragDrop;
            flpFavPlayers.DragDrop += PanelFavoritePlayers_DragDrop;

        }


        //first called func after Form1
        private async void Form1_Load(object sender, EventArgs e)
        {

            //OBJECTS OF DIFF SERVICES

            //_confServ works with public "Settings" variable from CongifServ class
            _configService = new ConfigService();
            _settingsService = new SettingsService();


            //use the values from "Settings" to change gender
            _teamService = new TeamService(_configService);
            _matchService = new MatchService(_configService);



            cmbGender.Items.AddRange(new[] { "men", "women" });
            cmbGender.SelectedItem = _configService.Settings.Gender;

            cmbLang.Items.AddRange(new[] { "en", "hr" });
            cmbLang.SelectedItem = _configService.Settings.Language;


            //LoadLang creates a dictionary based on lang
            _localizationService.LoadLanguage(_configService.Settings.Language);
            ApplyLocalization();


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

            //
            foreach(var p in _favoritePlayers)
            {

                var pControl = new PlayerControl(p,true);
                pControl.Margin = new Padding(5);
                pControl.ContextMenuStrip = _favoritePlayerContextMenu; 


                flpFavPlayers.Controls.Add(pControl);

                
            }



        }


        //panel drag and drop functions

        
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
                    MessageBox.Show("This player doesn't belong to the current match.");
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





        //changes the names of lables,buttons etc based on lang
        private void ApplyLocalization()
        {

            if (_localizationService == null) return;

            this.Text = _localizationService["title"];
            lblGender.Text = _localizationService["gender"];
            lblLang.Text = _localizationService["language"];
            lblFavTeam.Text = _localizationService["favoriteTeam"];
            lblFavPlayers.Text = _localizationService["favoritePlayer"];
            btnMatches.Text = _localizationService["loadMatches"];
            btnAddTeam.Text = _localizationService["addToFavorites"];
            btnRemoveTeam.Text = _localizationService["remove"];
            lblCountry.Text = _localizationService["country"];
            lblMatchesList.Text = _localizationService["listOfMatches"];
            lblPlayers.Text = _localizationService["listOfPlayers"];


            this.Invalidate();
            this.Refresh();
        }



        //based on choosen gender "Country" combo box is filled with teams 
        private async Task LoadTeams()
        {
            var gender = cmbGender.SelectedItem?.ToString() ?? "men";

            try
            {
                _teams = await _teamService.GetTeamsAsync(gender);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); // user-friendly error shown in UI
                return;
            }

            cmbCountry.Items.Clear();
            foreach (var team in _teams)
            {
                cmbCountry.Items.Add($"{team.FifaCode} - {team.Country}");
            }
        }



        //************************************************************************************//


        //GENDER
        private void lblGender_Click(object sender, EventArgs e)
        {

        }

        private async void cmbGender_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if (_suppressGenderSave) return;

            var selectedGender = cmbGender.SelectedItem?.ToString();
            System.Diagnostics.Debug.WriteLine("selectedGender: " + selectedGender);

            if (!string.IsNullOrEmpty(selectedGender))
            {
                _configService.Settings.Gender = selectedGender;
                _configService.Save(); //saves last value of gender to  file
            }

            // call backend now
            await LoadTeams();
        }




        //COUNTRY
        private void cmbCountry_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lblCountry_Click(object sender, EventArgs e)
        {

        }


        //LANGUAGE
        private void cmbLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedLanguage = cmbLang.SelectedItem?.ToString();
            System.Diagnostics.Debug.WriteLine("selectedLanguage: " + selectedLanguage);

            if (!string.IsNullOrEmpty(selectedLanguage))
            {
                _configService.Settings.Language = selectedLanguage;
                _configService.Save(); // Saves to settings.json

                _localizationService.LoadLanguage(selectedLanguage);
                System.Diagnostics.Debug.WriteLine("_localizationService: " + _localizationService);
                ApplyLocalization();

            }
        }
        private void lblLang_Click(object sender, EventArgs e)
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
                MessageBox.Show("This team is already in the favorite list.");
            }
        }


        //REMOVE FAV TEAMS
        private void btnRemoveTeam_Click(object sender, EventArgs e)
        {


            var selectedIndex = lstFavTeam.SelectedIndex;


            //-1 is if the list is empty
            if (selectedIndex == -1)
            {
                MessageBox.Show("Select a team to remove.");
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
            var gender = cmbGender.SelectedItem?.ToString() ?? "men";


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
            var selectedIndex = lstMatches.SelectedIndex;
            if (selectedIndex == -1) return;

            var selectedMatch = _matches[selectedIndex];

            TeamStatistics stats = null;

            stats = selectedMatch.HomeTeamStatistics;
            /*
            if (cmbTeamSide.SelectedItem?.ToString() == "Home")
            {
                stats = selectedMatch.HomeTeamStatistics;
            }
            else if (cmbTeamSide.SelectedItem?.ToString() == "Away")
            {
                stats = selectedMatch.AwayTeamStatistics;
            }
            */

            if (stats == null)
            {
                MessageBox.Show("No statistics available for this match/side.");
                return;
            }

            // Store full list for later comparisons
            // to do check ?
            _allPlayersInMatch = stats.StartingEleven.Concat(stats.Substitutes).ToList();

            // DO NOT clear panelFavoritePlayers — it contains your favorites already
            flpPlayers.Controls.Clear();

            foreach (var player in stats.StartingEleven.Concat(stats.Substitutes))
            {
                bool isFavorite = _favoritePlayers.Any(p => p.Name == player.Name);

                // Skip adding to main list if it's already in favorites
                if (isFavorite) continue;

                var playerControl = new PlayerControl(player, isFavorite);
                playerControl.Margin = new Padding(5);
                flpPlayers.Controls.Add(playerControl);
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
