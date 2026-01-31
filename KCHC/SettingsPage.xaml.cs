using KCHC.Properties;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KCHC
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        private const string DarkFile = "kchc.ico";
        private const string LightFile = "whitelogo.png";

        public SettingsPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            // Init controls from stored settings
            IntroMusicSwitch.IsToggled = Settings.IntroMusicEnabled;

            var stored = Settings.BackgroundImage ?? string.Empty;
            if (!string.IsNullOrEmpty(stored))
            {
                ModeSwitch.IsToggled = string.Equals(stored, DarkFile, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                // default to dark
                ModeSwitch.IsToggled = true;
                Settings.BackgroundImage = DarkFile;
            }

            ModeSwitch.Toggled += ModeSwitch_Toggled;
        }

        private void ModeSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            // true => Dark (kchc.ico), false => Light (whitelogo.png)
            Settings.BackgroundImage = e.Value ? DarkFile : LightFile;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // Persist values
            Settings.IntroMusicEnabled = IntroMusicSwitch.IsToggled;
            Settings.BackgroundImage = ModeSwitch.IsToggled ? DarkFile : LightFile;

            try
            {
                await App.Current.SavePropertiesAsync();
            }
            catch
            {
                // ignore save failure or log if needed
            }

            await DisplayAlert("Saved", "Settings saved.", "OK");
        }
    }
}