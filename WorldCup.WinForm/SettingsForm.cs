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

            cmbGender.SelectedItem = _configService.Settings.Gender;
            cmbLanguage.SelectedItem = _configService.Settings.Language;
        }


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

            }
        }

        private void lblLanguage_Click(object sender, EventArgs e)
        {

        }




        private void btnSave_Click(object sender, EventArgs e)
        {
            var gender = cmbGender.SelectedItem?.ToString();
            var language = cmbLanguage.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(gender) || string.IsNullOrEmpty(language))
            {
                MessageBox.Show(_localizationService["selGenderAndLang"], "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _configService.Settings.Gender = gender;
            _configService.Settings.Language = language;
            _configService.Save();


            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {

        }

        private void lblGender_Click(object sender, EventArgs e)
        {

        }

    }
}
