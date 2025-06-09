using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using WorldCup.Data.Models;
using WorldCup.Data.Services;
using System.IO;
namespace WorldCup.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        private ConfigService _configService;
        private TeamService _teamService;
        private MatchService _matchService;
        private SettingsService _settingsService;
        private LocalizationService _localizationService;


        private List<Team> _teams = new();
        private List<Match> _matches = new();
        private List<Player> _favoritePlayers = new();
        private List<Player> _allPlayersInMatch = new();

        public MainWindow()
        {
            InitializeComponent();


            _configService = new ConfigService();

            //use Config because of gender change
            _teamService = new TeamService(_configService);
            _matchService = new MatchService(_configService);

            _settingsService = new SettingsService();
            _localizationService = new LocalizationService();


            cmbGender.Items.Add("men");
            cmbGender.Items.Add("women");
            cmbGender.SelectedItem = _configService.Settings.Gender; //from file saved to combo box selected gender


            cmbLanguage.Items.Add("en");
            cmbLanguage.Items.Add("hr");
            cmbLanguage.SelectedItem = _configService.Settings.Language;

            //changes the language on the label names, reads from files
            _localizationService.LoadLanguage(_configService.Settings.Language);
            ApplyLocalization();


            //when button LoadMathces is Clicked, BtnLoadMatches_Click function is called and so on
            cmbGender.SelectionChanged += CmbGender_SelectionChanged;
            btnLoadMatches.Click += BtnLoadMatches_Click;
            btnLoadPlayers.Click += BtnLoadPlayers_Click;
            btnAddFavoriteTeam.Click += BtnAddFavoriteTeam_Click;
            btnRemoveFavoriteTeam.Click += BtnRemoveFavoriteTeam_Click;
            btnAddToFavorites.Click += BtnAddToFavoritesPlayer_Click;
            btnRemoveFavoritePlayer.Click += BtnRemoveFavoritePlayer_Click;
            cmbLanguage.SelectionChanged += CmbLanguage_SelectionChanged;
            btnTeamInfo.Click += BtnTeamInfo_Click;

            this.Closing += MainWindow_Closing;
            this.Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadTeams();
            LoadFavoriteTeams();
            LoadFavoritePlayers();
        }
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Exit Confirmation",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }


        //CHANGES LABEL,BUTTON ETC. LANGUAGE
        public void ApplyLocalization()
        {
            this.Title = _localizationService["title"];
            lblGender.Content = _localizationService["gender"];
            lblLanguage.Content = _localizationService["language"];
            lblFavoriteTeam.Content = _localizationService["favoriteTeam"];

            btnLoadMatches.Content = _localizationService["loadMatches"];
            btnLoadPlayers.Content = _localizationService["loadPlayers"];
            btnAddFavoriteTeam.Content = _localizationService["addFavoriteTeam"];
            btnRemoveFavoriteTeam.Content = _localizationService["remove"];
            btnRemoveFavoritePlayer.Content = _localizationService["removeFavoritePlayer"];
            btnAddToFavorites.Content = _localizationService["addToFavorites"];
            btnTeamInfo.Content = _localizationService["teamInfo"];

            // Use the correct x:Name prefixes from your XAML (grp instead of gb)
            grpFavoriteTeams.Header = _localizationService["favoriteTeamsHeader"];
            grpFavoritePlayers.Header = _localizationService["favoritePlayersHeader"];
            grpMatches.Header = _localizationService["matchesHeader"];
            grpPlayers.Header = _localizationService["playersHeader"];
            grpPlayerLayout.Header = _localizationService["playerLayoutHeader"];
        }


        //LOADS TEAMS WHEN GENDER IS SELECTED
        private void CmbGender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedGender = cmbGender.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedGender))
            {
                _configService.Settings.Gender = selectedGender;
                _configService.Save();
                lstPlayers.Items.Clear();
                LoadTeams();  
            }
        }

        private  async Task LoadTeams()
        {
            try
            {
                var gender = cmbGender.SelectedItem?.ToString() ?? "men";
                System.Diagnostics.Debug.WriteLine(gender);

                _teams = await _teamService.GetTeamsAsync(gender);

                System.Diagnostics.Debug.WriteLine(_teams);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }

            cmbFavoriteTeam.Items.Clear();
            foreach (var team in _teams)
            {
                cmbFavoriteTeam.Items.Add($"{team.FifaCode} - {team.Country}");
            }

            //  Ensure something is selected by default
            if (cmbFavoriteTeam.Items.Count > 0 && cmbFavoriteTeam.SelectedIndex == -1)
                cmbFavoriteTeam.SelectedIndex = 0;
        }



        //LOAD MATCHES AFTER TEAM IS SELECTED 
        private async void BtnLoadMatches_Click(object sender, RoutedEventArgs e)
        {
            var gender = cmbGender.SelectedItem?.ToString() ?? "men";
            var selectedTeam = cmbFavoriteTeam.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedTeam)) return;

            var fifaCode = selectedTeam.Split('-')[0].Trim();
            try
            {
                _matches = await _matchService.GetMatchesForTeamAsync(gender, fifaCode);
            }
            catch
            {
                MessageBox.Show(_localizationService["matchLoadError"]);
                return;
            }

            lstMatches.Items.Clear();
            foreach (var match in _matches)
            {
                lstMatches.Items.Add($"{match.StageName}: {match.HomeTeamCountry} vs {match.AwayTeamCountry}");
            }
        }


        //LOADS PLAYERS AFTER CHOOSING A SPECIFIC MATCH
        private void BtnLoadPlayers_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = lstMatches.SelectedIndex;
            if (selectedIndex == -1) return;

            var selectedMatch = _matches[selectedIndex];
            var stats = selectedMatch.HomeTeamStatistics;
            if (stats == null)
            {
                MessageBox.Show("No statistics available.");
                return;
            }

            _allPlayersInMatch = stats.StartingEleven.Concat(stats.Substitutes).ToList();

            lstPlayers.Items.Clear();
            foreach (var player in _allPlayersInMatch)
            {
                lstPlayers.Items.Add(player.Name);
            }

            // Display on field
            DisplayPlayersOnField(_allPlayersInMatch);
        }
        //displays players on green field box
        private void DisplayPlayersOnField(List<Player> players)
        {
            canvasPlayers.Children.Clear();

            // Dummy layout: 4 defenders, 4 midfielders, 2 attackers, 1 goalie
            var layout = new List<Point>
            {
                new Point(220, 20),  // Goalie
                new Point(50, 80), new Point(150, 80), new Point(250, 80), new Point(350, 80), // Defenders
                new Point(50, 160), new Point(150, 160), new Point(250, 160), new Point(350, 160), // Midfield
                new Point(150, 240), new Point(250, 240), // Attackers
            };

            for (int i = 0; i < players.Count && i < layout.Count; i++)
            {
                var playerControl = new PlayerControl(players[i].Name);
                Canvas.SetLeft(playerControl, layout[i].X);
                Canvas.SetTop(playerControl, layout[i].Y);
                canvasPlayers.Children.Add(playerControl);
            }
        }


        //ADDS AND REMOVES FAVOURITE TEAM AND SAVES IT TO FILE
        private void BtnAddFavoriteTeam_Click(object sender, RoutedEventArgs e)
        {
            var selectedTeam = cmbFavoriteTeam.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedTeam)) return;

            if (!lstFavouriteTeams.Items.Contains(selectedTeam))
            {
                lstFavouriteTeams.Items.Add(selectedTeam);
                Directory.CreateDirectory("./Data");
                File.WriteAllLines("./Data/favourite_teams.txt", lstFavouriteTeams.Items.Cast<string>());
            }
            else
            {
                MessageBox.Show("This team is already in the favorite list.");
            }
        }

        private void BtnRemoveFavoriteTeam_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = lstFavouriteTeams.SelectedIndex;
            if (selectedIndex == -1)
            {
                MessageBox.Show("Please select a team to remove.");
                return;
            }

            lstFavouriteTeams.Items.RemoveAt(selectedIndex);

            // Save the updated list back to disk
            Directory.CreateDirectory("./Data");
            File.WriteAllLines("./Data/favourite_teams.txt", lstFavouriteTeams.Items.Cast<string>());
        }


        private void LoadFavoriteTeams()
        {
            lstFavouriteTeams.Items.Clear();
            var path = "./Data/favourite_teams.txt";
            if (File.Exists(path))
            {
                var teams = File.ReadAllLines(path);
                foreach (var t in teams)
                    lstFavouriteTeams.Items.Add(t);
            }
        }





        //ADDS AND REMOVES FAVOURITE PLAYERS AND SAVES THE STATE TO FILE
        private void BtnAddToFavoritesPlayer_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = lstPlayers.SelectedIndex;
            if (selectedIndex == -1) return;

            var playerName = lstPlayers.SelectedItem.ToString();
            var player = _allPlayersInMatch.FirstOrDefault(p => p.Name == playerName);
            if (player == null || _favoritePlayers.Any(p => p.Name == player.Name)) return;

            _favoritePlayers.Add(player);
            _settingsService.SaveFavoritePlayers(_favoritePlayers);
            LoadFavoritePlayers();
            MessageBox.Show($"Added {player.Name} to favorites.");
        }

        private void BtnRemoveFavoritePlayer_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = lstFavoritePlayers.SelectedIndex;
            if (selectedIndex == -1)
            {
                MessageBox.Show("Please select a player to remove.");
                return;
            }

            var playerName = lstFavoritePlayers.SelectedItem.ToString();
            var playerToRemove = _favoritePlayers.FirstOrDefault(p => p.Name == playerName);

            if (playerToRemove != null)
            {
                _favoritePlayers.Remove(playerToRemove);
                _settingsService.SaveFavoritePlayers(_favoritePlayers);
                LoadFavoritePlayers(); // Refresh UI
            }
        }

        private void LoadFavoritePlayers()
        {
            _favoritePlayers = _settingsService.LoadFavoritePlayers();
            lstFavoritePlayers.Items.Clear();
            foreach (var player in _favoritePlayers)
            {
                lstFavoritePlayers.Items.Add(player.Name);
            }
        }


        //FOR LANGUAGE CHANGE
        private void CmbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedLanguage = cmbLanguage.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedLanguage))
            {
                _configService.Settings.Language = selectedLanguage;
                _configService.Save();
                _localizationService.LoadLanguage(selectedLanguage);
                ApplyLocalization();
            }
        }



        //DISPLAYS TEAM STATISTICS IN A POP UP WINDOW
        private async void BtnTeamInfo_Click(object sender, RoutedEventArgs e)
        {
            var selectedIndex = cmbFavoriteTeam.SelectedIndex;
            if (selectedIndex == -1)
            {
                MessageBox.Show("No team selected.");
                return;
            }

            var selected = cmbFavoriteTeam.Items[selectedIndex].ToString();
            var fifaCode = selected.Split('-')[0].Trim();

            var team = _teams.FirstOrDefault(t => t.FifaCode == fifaCode);
            var results = await _teamService.GetTeamResultsAsync(cmbGender.SelectedItem?.ToString() ?? "men");
            var stats = results.FirstOrDefault(r => r.FifaCode == fifaCode);

            if (team != null && stats != null)
            {
                var infoWindow = new TeamInfo(team.Country, fifaCode, stats);
                infoWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Could not load team info.");
            }
        }

        private void cmbFavoriteTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}