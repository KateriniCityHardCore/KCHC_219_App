using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using KCHC.Models;
using Xamarin.Forms;
using Newtonsoft.Json;
using Xamarin.Essentials;
using System.Linq;
using KCHC.Properties;

namespace KCHC
{
    public partial class MainPage : ContentPage
    {
        private const string DefaultFont = "Default";
        private const string DarkFile = "kchc.ico";
        private const string LightFile = "whitelogo.png";

        public MainPage()
        {
            InitializeComponent();

            // Ensure the DynamicResource key exists with the stored value so XAML picks it up.
            ApplyFontFromSettings();

            // Apply theme (text color) and background from Settings
            ApplyThemeFromSettings();
            ApplyBackgroundFromSettings();

            GetUpdate();
            NavigationPage.SetHasNavigationBar(this, false);
            ArtistsCarousel.BindingContext = App.Artists;
            InitializeCustomIndicator(App.Artists.Count);
            ArtistsCarousel.PositionChanged += OnPositionSelected;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            // Re-apply theme and background in case Settings changed
            ApplyThemeFromSettings();
            ApplyBackgroundFromSettings();
        }

        private void ApplyThemeFromSettings()
        {
            var bgSetting = Settings.BackgroundImage ?? string.Empty;

            // Determine mode: dark when BackgroundImage equals DarkFile
            var isDark = string.Equals(bgSetting, DarkFile, StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(bgSetting);

            var color = isDark ? Color.White : Color.Black;

            // Store/replace resource so DynamicResource bindings update
            if (Application.Current != null)
            {
                if (Application.Current.Resources.ContainsKey("MainTextColor"))
                    Application.Current.Resources["MainTextColor"] = color;
                else
                    Application.Current.Resources.Add("MainTextColor", color);
            }
        }

        private void ApplyFontFromSettings()
        {
            var fontSetting = Settings.MainPageFont ?? DefaultFont;

            object valueToStore = fontSetting;
            if (string.Equals(fontSetting, DefaultFont, StringComparison.OrdinalIgnoreCase))
                valueToStore = null;

            if (Application.Current != null)
            {
                if (Application.Current.Resources.ContainsKey("MainFontFamily"))
                    Application.Current.Resources["MainFontFamily"] = valueToStore;
                else
                    Application.Current.Resources.Add("MainFontFamily", valueToStore);
            }
        }

        private void ApplyBackgroundFromSettings()
        {
            var bgSetting = Settings.BackgroundImage ?? string.Empty;
            ImageSource source = null;

            if (!string.IsNullOrWhiteSpace(bgSetting))
            {
                try
                {
                    source = ImageSource.FromFile(bgSetting);
                }
                catch
                {
                    source = null;
                }
            }

            if (source == null)
                source = ImageSource.FromFile(DarkFile);

            BackgroundImage.Source = source;
        }

        public async void GetUpdate()
        {
            if (IsDeviceOnline())
            {
                string owner = "GSmyrlis";
                string repo = "KCHC_219_App";
                // Create HttpClient instance
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    // Make the request to GitHub API to get tags
                    HttpResponseMessage response = await client.GetAsync($"https://api.github.com/repos/{owner}/{repo}/tags");

                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content
                        string responseBody = await response.Content.ReadAsStringAsync();

                        // Deserialize the JSON response to a list of Tag objects
                        List<Tag> tags = JsonConvert.DeserializeObject<List<Tag>>(responseBody);

                        if (tags.Count <= 3)
                        {
                            return;
                        }

                        string message = "New Edition Available. Trust me ;) download from:";
                        string url = "https://github.com/GSmyrlis/KCHC_219_App/tags";
                        bool closePopup = await DisplayAlert("Anakoinwsh", message + "\n\nURL: " + url, "Close", "Open URL");
                        if (!closePopup)
                        {
                            // Open the URL if the user chooses not to close the popup
                            await Launcher.OpenAsync(new System.Uri(url));
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Failed to fetch tags. Status code: {response.StatusCode}");
                    }
                }
            }
        }
        public class Tag
        {
            [JsonProperty("tag")]
            public string tag { get; set; }
        }

        private void InitializeCustomIndicator(int totalItems)
        {
            for (int i = 0; i < totalItems; i++)
            {
                var boxView = new BoxView
                {
                    WidthRequest = 5,
                    HeightRequest = 5,
                    BackgroundColor = Color.Gray,
                    Margin = new Thickness(5, 0)
                };
                CustomIndicator.Children.Add(boxView);
            }
            if (CustomIndicator.Children.Count > 0)
                CustomIndicator.Children[0].BackgroundColor = Color.DarkRed;
        }
        private void OnPositionSelected(object sender, PositionChangedEventArgs e)
        {
            // Update the custom indicator based on the selected position
            for (int i = 0; i < CustomIndicator.Children.Count; i++)
            {
                CustomIndicator.Children[i].BackgroundColor = (i == e.CurrentPosition) ? Color.DarkRed : Color.DarkGray;
            }
        }


        private async void OnKinimatoramaImageClicked(object sender, EventArgs e)
        {
            if (Application.Current.MainPage?.Navigation != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new KiminatoramaPage());
            }
        }

        private async void OnMenuButtonClicked(object sender, EventArgs e)
        {
            if (Application.Current.MainPage?.Navigation != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new SettingsPage());
            }
        }

        private async void OnImageTapped(object sender, EventArgs e)
        {
            if (sender is Image tappedImage && tappedImage.BindingContext is Models.Artist selectedArtist)
            {
                await Navigation.PushAsync(new ArtistPage(selectedArtist));
            }
        }

        public bool IsDeviceOnline()
        {
            var currentNetwork = Connectivity.NetworkAccess;

            if (currentNetwork == NetworkAccess.Internet)
            {
                // Device is connected to the internet
                return true;
            }
            else
            {
                // Device is not connected to the internet
                return false;
            }
        }
    }
}