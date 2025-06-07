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

            cmbLang.Items.AddRange(new[] { "hr", "hr" });
            cmbLang.SelectedItem = _configService.Settings.Language;


            //LoadLang creates a dictionary based on lang
            _localizationService.LoadLanguage(_configService.Settings.Language);
            ApplyLocalization();


            //loading teams in combo box Country
            await LoadTeams();

        }


        //changes the names of lables,buttons etc based on lang
        private void ApplyLocalization()
        {
            System.Diagnostics.Debug.WriteLine("ApplyLocalization: _localizationService: " + _localizationService);

            if (_localizationService == null) return;

            this.Text = _localizationService["title"];
            lblGender.Text = _localizationService["gender"];
            lblLang.Text = _localizationService["language"];
            System.Diagnostics.Debug.WriteLine("ApplyLocalization: title: " + _localizationService["title"]);

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

        private void cmbGender_SelectedIndexChanged(object sender, EventArgs e)
        {
            // if (_suppressGenderSave) return;

            var selectedGender = cmbGender.SelectedItem?.ToString();
            System.Diagnostics.Debug.WriteLine("selectedGender: " + selectedGender);

            if (!string.IsNullOrEmpty(selectedGender))
            {
                _configService.Settings.Gender = selectedGender;
                _configService.Save(); // persist to settings.json
            }
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
