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
    /// Interaction logic for StatupWindow.xaml
    /// </summary>
    public partial class StartupWindow : Window
    {
        public StartupWindow()
        {
            InitializeComponent();
            cmbResolution.SelectedIndex = 0; // Default to 1024x768

        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            string selected = ((ComboBoxItem)cmbResolution.SelectedItem).Content.ToString();

            var main = new MainWindow();

            switch (selected)
            {
                case "1024x768":
                    main.Width = 1024;
                    main.Height = 768;
                    main.WindowState = WindowState.Normal;
                    break;
                case "1280x800":
                    main.Width = 1280;
                    main.Height = 800;
                    main.WindowState = WindowState.Normal;
                    break;
                case "Fullscreen":
                    main.WindowState = WindowState.Maximized;
                    main.WindowStyle = WindowStyle.None;
                    break;
            }

            main.Show();
            this.Close();
        }

    }
}
