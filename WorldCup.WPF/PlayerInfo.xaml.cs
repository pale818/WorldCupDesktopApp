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
using System.Windows.Shapes;

namespace WorldCup.WPF
{
    /// <summary>
    /// Interaction logic for PlayerInfo.xaml
    /// </summary>
    public partial class PlayerInfo : Window
    {
        public PlayerInfo(string playerName, string playerShirt, string playerPosition,bool isCaptain)
        {
            InitializeComponent();
            txtName.Text = $"Name: {playerName}";
            txtShirt.Text = $"Shirt Number: {playerShirt}";
            txtPosition.Text = $"Position: {playerPosition}";
            txtCaptain.Text =  isCaptain ? " 🧢" : "";

        }
    }
}
