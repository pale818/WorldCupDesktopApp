using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using WorldCup.Data.Models;
using WorldCup.Data.Services;
using System.IO;
using System.Windows.Media.Animation;
using System.Text.Json;
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

        // Context for menues
        private ContextMenu _favoritePlayerRemoveContextMenu;
        private ContextMenu _favoritePlayerAddContextMenu;
        private ContextMenu _favoriteTeamRemoveContextMenu;
        private ContextMenu _favoriteTeamInfoContextMenu;

        private TeamStatistics statsHome;
        private TeamStatistics statsAway;

        private List<Player> _currentHomePlayers = new();
        private List<Player> _currentAwayPlayers = new();

        // Context menues
        MenuItem teamInfo;
        MenuItem removeTeam;
        MenuItem addPlayer;
        MenuItem removePlayer;

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
            cmbLanguage.SelectionChanged += CmbLanguage_SelectionChanged;

            btnHomeStat.Click += BtnLoadHomeStat_Click;
            btnAwayStat.Click += BtnLoadAwayStat_Click;

            // Instantiate the ContextMenu
            _favoritePlayerRemoveContextMenu = new ContextMenu();
            _favoritePlayerAddContextMenu = new ContextMenu();
            _favoriteTeamRemoveContextMenu = new ContextMenu();
            _favoriteTeamInfoContextMenu = new ContextMenu();

            teamInfo = new MenuItem();
            removeTeam = new MenuItem();
            addPlayer = new MenuItem();
            removePlayer = new MenuItem();


            // Menu item made via ContextMenu for removing favourite player
            removePlayer.Header = "Remove from Favorites"; // Set the text displayed in the menu
            removePlayer.Click += BtnRemoveFavoritePlayer_Click; // Attach the event handler for when the item is clicked
            _favoritePlayerRemoveContextMenu.Items.Add(removePlayer);

            // Menu item made via ContextMenu for adding favourite player
            addPlayer.Header = "Add to Favorites"; // Set the text displayed in the menu
            addPlayer.Click += BtnAddToFavoritesPlayer_Click; // Attach the event handler for when the item is clicked
            _favoritePlayerAddContextMenu.Items.Add(addPlayer);

            // Menu item made via ContextMenu for removing favourite team
            removeTeam.Header = "Remove from Favourites"; // Set the text displayed in the menu
            removeTeam.Click += BtnRemoveFavoriteTeam_Click; // Attach the event handler for when the item is clicked
            _favoriteTeamRemoveContextMenu.Items.Add(removeTeam);

            // Menu item made via ContextMenu for presenting team info
            teamInfo.Header = "Info"; // Set the text displayed in the menu
            teamInfo.Click += BtnTeamInfo_Click; // Attach the event handler for when the item is clicked
            _favoriteTeamInfoContextMenu.Items.Add(teamInfo);




            this.Closing += MainWindow_Closing;
            this.Loaded += MainWindow_Loaded;

            // when canvasPlayers detect change in height or width calls DisplayPlayersOnField
            canvasPlayers.SizeChanged += (s, e) => DisplayPlayersOnField();

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

        // STATISTIC WINDOW
        private void BtnLoadHomeStat_Click(object sender, RoutedEventArgs e)
        {
            if (statsHome == null)
            {
                MessageBox.Show("No info.");
                return;
            }

            try
            {
                var statWindow = new Statistics(statsHome);
                statWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void BtnLoadAwayStat_Click(object sender, RoutedEventArgs e)
        {
            if (statsAway == null)
            {
                MessageBox.Show("No info.");
                return;
            }

            try
            {
                var statWindow = new Statistics(statsAway);
                statWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
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
            btnHomeStat.Content = _localizationService["stat"];
            btnAwayStat.Content = _localizationService["stat"];
            miTeamInfo.Header = _localizationService["teamInfo"];
            miRemoveTeam.Header = _localizationService["remove"];
            miAddPlayer.Header = _localizationService["addToFavorites"];
            miRemovePayer.Header = _localizationService["removeFavoritePlayer"];

            // Use the correct x:Name prefixes from your XAML (grp instead of gb)
            grpFavoriteTeams.Header = _localizationService["favoriteTeamsHeader"];
            grpFavoritePlayers.Header = _localizationService["favoritePlayersHeader"];
            grpMatches.Header = _localizationService["matchesHeader"];
            grpPlayers.Header = _localizationService["playersHeader"];
            grpPlayerLayout.Header = _localizationService["playerLayoutHeader"];
            grpPlayerLayoutTitle.Header = _localizationService["groupInfo"];
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
            statsHome = selectedMatch.HomeTeamStatistics;
            statsAway = selectedMatch.AwayTeamStatistics;
            var fifaCode = cmbFavoriteTeam.SelectedItem?.ToString().Split('-')[0].Trim();
            System.Diagnostics.Debug.WriteLine($"fifaCode ${fifaCode}");

            if (selectedMatch.AwayTeam.Code == fifaCode)
            {
                statsHome = selectedMatch.AwayTeamStatistics;
                statsAway = selectedMatch.HomeTeamStatistics;
            }

            if (statsHome == null)
            {
                MessageBox.Show("No statistics available.");
                return;
            }

            List<Player> _allPlayers = new();

            // for debuging
            var homeTeam = JsonSerializer.Serialize(statsHome, new JsonSerializerOptions { WriteIndented = true });
            var awayTeam = JsonSerializer.Serialize(statsAway, new JsonSerializerOptions { WriteIndented = true });
            System.Diagnostics.Debug.WriteLine($"statsHome: {homeTeam}");
            System.Diagnostics.Debug.WriteLine($"statsAway: {awayTeam}");

            lblHomeTeam.Content = statsHome.Country;
            lblAwayTeam.Content = statsAway.Country;

            _allPlayersInMatch = statsHome.StartingEleven.Concat(statsHome.Substitutes).ToList();

            // list of teams on the field
            _currentHomePlayers = statsHome.StartingEleven.ToList();
            _currentAwayPlayers = statsAway.StartingEleven.ToList();

            lstPlayers.Items.Clear();
            // Iterate through all players (starting eleven + substitutes)
            foreach (var player in _allPlayersInMatch)
            {
                // do not add player to the main list since it is in the facourite list
                bool isFavorite = _favoritePlayers.Any(p => p.Name == player.Name);
                if (isFavorite) continue;

                // Format the player information as a string
                string playerInfo = $"{player.ShirtNumber} - {player.Name} ({player.Position})";
                if (player.Captain)
                {
                    playerInfo += " 🧢"; // Add captain emoji if applicable
                }
                lstPlayers.Items.Add(playerInfo);
            }


            // Display on field
            DisplayPlayersOnField();
        }


        private void DisplayPlayersOnField()
        {
            canvasPlayers.Children.Clear();

            double fieldWidth = canvasPlayers.ActualWidth;
            double fieldHeight = canvasPlayers.ActualHeight;

            if (fieldWidth == 0 || fieldHeight == 0)
                return;

            List<Point> homeLayout = new()
           {
               new Point(0.0, 0.5), new Point(0.1, 0.2), new Point(0.1, 0.4),
               new Point(0.1, 0.6), new Point(0.1, 0.8), new Point(0.2, 0.25),
               new Point(0.2, 0.5), new Point(0.2, 0.75), new Point(0.30, 0.2),
               new Point(0.30, 0.5), new Point(0.30, 0.8)
           };

            List<Point> awayLayout = new()
           {
               new Point(0.9, 0.5), new Point(0.8, 0.2), new Point(0.8, 0.4),
               new Point(0.8, 0.6), new Point(0.8, 0.8), new Point(0.7, 0.25),
               new Point(0.7, 0.5), new Point(0.7, 0.75), new Point(0.6, 0.2),
               new Point(0.6, 0.5), new Point(0.6, 0.8)
           };

            void AddPlayers(List<Player> players, List<Point> layout)
            {
                for (int i = 0; i < players.Count && i < layout.Count; i++)
                {
                    var player = players[i];
                    string playerName = $"{player.Name}";
                    string playerShirt = $"{player.ShirtNumber}";
                    var control = new PlayerControl(playerName, playerShirt) { Opacity = 0 };

                    double x = layout[i].X * fieldWidth;
                    double y = layout[i].Y * fieldHeight;

                    Canvas.SetLeft(control, x);
                    Canvas.SetTop(control, y);
                    canvasPlayers.Children.Add(control);

                    var fadeIn = new DoubleAnimation
                    {
                        From = 0,
                        To = 1,
                        Duration = TimeSpan.FromSeconds(0.5),
                        BeginTime = TimeSpan.FromSeconds(i * 0.2)
                    };
                    Storyboard.SetTarget(fadeIn, control);
                    Storyboard.SetTargetProperty(fadeIn, new PropertyPath("Opacity"));

                    var sb = new Storyboard();
                    sb.Children.Add(fadeIn);
                    sb.Begin();
                }
            }

            AddPlayers(_currentHomePlayers, homeLayout);
            AddPlayers(_currentAwayPlayers, awayLayout);
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
            System.Diagnostics.Debug.WriteLine($"playerName: {playerName}");
            if (playerName == null) return;

            var player = _allPlayersInMatch.FirstOrDefault(p => playerName.Contains(p.Name));
            System.Diagnostics.Debug.WriteLine($"player: {player}");

            if (player == null || _favoritePlayers.Any(p => p.Name == player.Name))
            {
                MessageBox.Show("This player is already in the favorite list.");
                return;

            }

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