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
using WorldCup.Data.Services;

namespace WorldCup.WPF
{
    /// <summary>
    /// Interaction logic for StatupWindow.xaml
    /// </summary>
    public partial class StartupWindow : Window
    {
        ConfigService configService;

        public StartupWindow()
        {
            InitializeComponent();
            configService = new();

            //cmbResolution.SelectedIndex = 0; // Default to 1024x768


            //Finds the item in cmbData that matches saved DataSource (e.g., "api"),preselects it
            foreach (ComboBoxItem item in cmbData.Items)
            {
                if ((string)item.Content == configService.Settings.DataSource)
                {
                    cmbData.SelectedItem = item;
                    break;
                }
            }


            System.Diagnostics.Debug.WriteLine($"SAVED RESOLUTION: {configService.Settings.Resolution}");

            foreach (ComboBoxItem item in cmbResolution.Items)
            {
                if ((string)item.Content == configService.Settings.Resolution)
                {
                    cmbResolution.SelectedItem = item;
                    break;
                }
            }

        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            //reads choosen values
            string selected = ((ComboBoxItem)cmbResolution.SelectedItem).Content.ToString();
            string selectedData = ((ComboBoxItem)cmbData.SelectedItem).Content.ToString();

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
                case "1366x768":
                    main.Width = 1366;
                    main.Height = 768;
                    main.WindowState = WindowState.Normal;
                    break;
                case "1600x900":
                    main.Width = 1600;
                    main.Height = 900;
                    main.WindowState = WindowState.Normal;
                    break;
                case "1920x1080":
                    main.Width = 1920;
                    main.Height = 1800;
                    main.WindowState = WindowState.Normal;
                    break;
                case "Fullscreen":
                    main.WindowState = WindowState.Maximized;
                    main.WindowStyle = WindowStyle.None;
                    break;
            }

            // save config for data usage
            if (selectedData != null)
            {
                configService.Settings.DataSource = selectedData;
                configService.Save();
            }

            main.Show();
            this.Close();
        }

        //When user changes api/local, it’s saved immediately to settings.json
        private void cmbData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedData = ((ComboBoxItem)cmbData.SelectedItem).Content.ToString();
            configService.Settings.DataSource = selectedData;
            configService.Save();
        }


        private void cmbResolution_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            string selected = ((ComboBoxItem)cmbResolution.SelectedItem).Content.ToString();
            System.Diagnostics.Debug.WriteLine($"SAVE RESOLUTION: {selected}");

            configService.Settings.Resolution = selected;
            configService.Save();
        }
    }
}
