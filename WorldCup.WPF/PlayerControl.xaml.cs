using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WorldCup.WPF
{
    /// <summary>
    /// Interaction logic for PlayerControl.xaml
    /// </summary>
    public partial class PlayerControl : UserControl
    {

        private string _playerName;
        private string _playerShirt;
        private string _playerPosition;
        private bool _isCaptain;
        public PlayerControl(string playerName, string playerShirt, string playerPosition, bool isCaptain)
        {
            InitializeComponent();
            txtPlayerName.Text = playerName;
            txtPlayerShirt.Text = playerShirt;

            _playerName = playerName;
            _playerShirt = playerShirt;
            _playerPosition = playerPosition;
            _isCaptain = isCaptain;


        }

        private void PlayerInfo_Click(object sender, MouseButtonEventArgs e)
        {
            var infoWindow = new PlayerInfo(_playerName, _playerShirt,_playerPosition,_isCaptain);
            infoWindow.Owner = Window.GetWindow(this);
            infoWindow.ShowDialog();
        } 

    }
}
