using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WorldCup.Data.Services;

namespace WorldCup.WinForm
{
    public partial class SettingsForm : Form
    {
        private ConfigService _configService;
        private LocalizationService _localizationService;


        /*
         Initializes the form
        Loads localization service (for labels/buttons)
        Loads current saved settings from settings.json
         */
        public SettingsForm()
        {
            InitializeComponent();
            _localizationService = new LocalizationService();
            _configService = new ConfigService();
            LoadSettings();
        }

        private void LoadSettings()
        {
            cmbGender.Items.AddRange(new string[] { "men", "women" });
            cmbLanguage.Items.AddRange(new string[] { "hr", "en" });
            cmbData.Items.AddRange(new string[] { "api", "local" });


            cmbGender.SelectedItem = _configService.Settings.Gender;
            cmbLanguage.SelectedItem = _configService.Settings.Language;
            cmbData.SelectedItem = _configService.Settings.DataSource;

        }


        //Saves new gender when changed.
        private void cmbGender_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedGender = cmbGender.SelectedItem?.ToString();
            System.Diagnostics.Debug.WriteLine("selectedGender: " + selectedGender);

            if (!string.IsNullOrEmpty(selectedGender))
            {
                _configService.Settings.Gender = selectedGender;
                _configService.Save(); // persist to settings.json
            }
        }

        //Saves new language, reloads localization, and updates UI text

        private void cmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedLanguage = cmbLanguage.SelectedItem?.ToString();
            System.Diagnostics.Debug.WriteLine("selectedLanguage: " + selectedLanguage);

            if (!string.IsNullOrEmpty(selectedLanguage))
            {
                _configService.Settings.Language = selectedLanguage;
                _configService.Save(); // Saves to settings.json

                _localizationService.LoadLanguage(selectedLanguage);
                lblGender.Text = _localizationService["gender"];
                lblLanguage.Text = _localizationService["language"];
                btnCancel.Text = _localizationService["cancel"];
                btnSave.Text = _localizationService["save"];
                lblData.Text = _localizationService["lblData"];
            }
        }

        private void lblLanguage_Click(object sender, EventArgs e)
        {

        }



        //Reads values from dropdowns, saves to config
        private void btnSave_Click(object sender, EventArgs e)
        {
            var gender = cmbGender.SelectedItem?.ToString();
            var language = cmbLanguage.SelectedItem?.ToString();
            var dataSource = cmbData.SelectedItem?.ToString();


            if (string.IsNullOrEmpty(gender) || string.IsNullOrEmpty(language))
            {
                MessageBox.Show(_localizationService["selGenderAndLang"], "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _configService.Settings.Gender = gender;
            _configService.Settings.Language = language;
            _configService.Settings.DataSource = dataSource;

            _configService.Save();


            DialogResult = DialogResult.OK; // DialogResult.OK closes the form so the caller can refresh UI
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }


        //Saves "api" or "local" as the current data source
        //All changes are saved immediately to settings.json
        private void cmbData_SelectedIndexChanged(object sender, EventArgs e)
        {
            _configService.Settings.DataSource = cmbData.SelectedItem.ToString() ?? "api";
            _configService.Save();
        }



        //Keyboard shortcut handling
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                btnSave.PerformClick(); // Simulate Save button click
                return true; // Mark key as handled
            }
            else if (keyData == Keys.Escape)
            {
                btnCancel.PerformClick(); // Simulate Cancel button click
                return true; // Mark key as handled
            }

            return base.ProcessCmdKey(ref msg, keyData); // Default processing
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {

        }

        private void lblGender_Click(object sender, EventArgs e)
        {

        }


    }
}
