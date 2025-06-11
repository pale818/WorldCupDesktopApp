using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WorldCup.Data.Models;

namespace WorldCup.WPF
{
    /// <summary>
    /// Interaction logic for Statistics.xaml
    /// </summary>
    public partial class Statistics : Window
    {
        public Statistics(TeamStatistics stats)
        {
            InitializeComponent();

            txtCountry.Text = $"{stats.Country}";
            txtTactics.Text = $"{stats.Tactics}";
            txtAttemptsOnGoal.Text = $"{stats.AttemptsOnGoal}";
            txtOnTarget.Text = $"{stats.OnTarget}";
            txtOffTarget.Text = $"{stats.OffTarget}";
            txtBlocked.Text = $"{stats.Blocked}";
            txtWoodwork.Text = $"{stats.Woodwork}";
            txtCorners.Text = $"{stats.Corners}";
            txtOffsides.Text = $"{stats.Offsides}";
            txtBallPossession.Text = $"{stats.BallPossession}%";
            txtPassAccuracy.Text = $" {stats.PassAccuracy}%";
            txtNumPasses.Text = $"{stats.NumPasses}";
            txtPassesCompleted.Text = $"{stats.PassesCompleted}";
            txtDistanceCovered.Text = $"{stats.DistanceCovered} km";
            txtBallsRecovered.Text = $"{stats.BallsRecovered}";
            txtTackles.Text = $"{stats.Tackles}";
            txtClearances.Text = $"{stats.Clearances}";
            txtYellowCards.Text = $"{stats.YellowCards}";
            txtRedCards.Text = $"{stats.RedCards}";
            txtFoulsCommitted.Text = $"{stats.FoulsCommitted}";
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
