using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System;
using KCHC.Models;
using KCHC.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.Design;

namespace KCHC
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ArtistPage : ContentPage
    {
        private Models.Artist ShownArtist { get; set; } = new Models.Artist();
        private string selectedSong = string.Empty;
        private bool isPlaying = false;
        private Button playButton;

        public ArtistPage()
        {
            InitializeComponent();
        }

        public ArtistPage(Models.Artist artist)
        {
            InitializeComponent();
            ShowPage(artist);
        }

        // Initialize common audio controls
        private void InitializeAudioControls(List<string> ArtistSongs)
        {
            GridForMusic.Children.Clear();
            int rowCounter = 0;

            foreach (string song in ArtistSongs)
            {
                RadioButton songRadioButt = new RadioButton();
                songRadioButt.Content = song;
                songRadioButt.CheckedChanged += (s, e) =>
                {
                    if (songRadioButt.IsChecked)
                    {
                        selectedSong = songRadioButt.ContentAsString();
                        DependencyService.Get<IAudio>().Reset();
                    }
                };

                // Set the row for the radio button
                Grid.SetRow(songRadioButt, rowCounter);

                // Increment the row counter for the next control
                rowCounter++;

                GridForMusic.Children.Add(songRadioButt);
            }

            if (GridForMusic.Children.Count > 0 && GridForMusic.Children[0] is RadioButton firstRadioButton)
            {
                firstRadioButton.IsChecked = true;
            }

            Slider positionSlider = new Slider
            {
                Minimum = 0,
                Value = 0
            };

            // Set the row for the slider
            Grid.SetRow(positionSlider, rowCounter);

            // Increment the row counter for the next control
            rowCounter++;

            Label durationLabel = new Label
            {
                Text = "0:00", // Initial text, you may update it dynamically
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };

            // Set the row for the duration label
            Grid.SetRow(durationLabel, rowCounter);

            // Increment the row counter for the next control
            rowCounter++;

            positionSlider.ValueChanged += async (sender, e) =>
            {
                await Task.Run(() =>
                {
                    // Update the UI on the main thread
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        // Update the duration label based on the slider position
                        TimeSpan timeSpan = TimeSpan.FromMilliseconds(e.NewValue);
                        durationLabel.Text = $"{(int)timeSpan.TotalMinutes}:{timeSpan.Seconds:D2}";
                    });
                });
            };

            positionSlider.DragStarted += async (sender, e) =>
            {
                await Task.Run(() =>
                {
                    // Update the UI on the main thread
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        isPlaying = false;
                    });
                });
            };
            positionSlider.DragCompleted += async (sender, e) =>
            {
                await Task.Run(() =>
                {
                    // Update the UI on the main thread
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        isPlaying = true;
                        DependencyService.Get<IAudio>().SeekTo((int)positionSlider.Value);

                        // Update the duration label based on the slider position
                        TimeSpan timeSpan = TimeSpan.FromMilliseconds(positionSlider.Value);
                        durationLabel.Text = $"{(int)timeSpan.TotalMinutes}:{timeSpan.Seconds:D2}";
                    });
                });
            };

            System.Threading.Timer updateTimer = new System.Threading.Timer(state =>
            {
                if (isPlaying)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        int currentPosition = DependencyService.Get<IAudio>().GetCurrentPosition();
                        positionSlider.Value = currentPosition;
                        // Update the duration label based on the slider position
                        TimeSpan timeSpan = TimeSpan.FromMilliseconds(currentPosition);
                        durationLabel.Text = $"{(int)timeSpan.TotalMinutes}:{timeSpan.Seconds:D2}";
                    });
                }
            }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(1000)); // Update every 1 second
            playButton = new Button
            {
                ImageSource = "play.png",
                Command = new Command(() =>
                {
                    if (!isPlaying)
                    {
                        // Resume playback from the last position if available
                        if (positionSlider.Value > 0)
                        {
                            DependencyService.Get<IAudio>().PlayFromSpecificTime(selectedSong, (int)positionSlider.Value);
                            positionSlider.Value = 0; // Reset the last playback position
                        }
                        else
                        {
                            DependencyService.Get<IAudio>().Reset();
                            DependencyService.Get<IAudio>().PlayAudioFile(selectedSong);

                            // Set the Maximum property after starting playback
                            positionSlider.Maximum = DependencyService.Get<IAudio>().GetDuration();
                        }

                        isPlaying = true; // Update the playback state
                        playButton.ImageSource = "Pause.png"; // Change button text to "Pause"
                    }
                    else
                    {
                        isPlaying = false;
                        DependencyService.Get<IAudio>().PauseAudio();
                        playButton.ImageSource = "play.png"; // Change button text to "Play"
                    }
                })
            };

            // Set the row for the play button
            Grid.SetRow(playButton, rowCounter);

            // Increment the row counter for the next control
            rowCounter++;
            GridForMusic.Children.Add(positionSlider);
            GridForMusic.Children.Add(durationLabel);
            GridForMusic.Children.Add(playButton);
        }

        // Helper method to create a social media button with an image
        private Button CreateSocialButton(string imageName, string url)
        {
            return new Button
            {
                HeightRequest = 45, // Set the height of the button
                WidthRequest = 45,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                ImageSource = imageName,
                Command = new Command(async () =>
                {
                    // Handle button click (e.g., open the URL)
                    if (!string.IsNullOrEmpty(url))
                    {
                        await Launcher.OpenAsync(new Uri(url));
                    }
                })
            };
        }
        public void LoadPagesDifferences()
        {
            switch (ShownArtist.Name)
            {
                case "Taratsa Paradeisou":
                    {
                        DependencyService.Get<IAudio>().Reset();
                        DependencyService.Get<IAudio>().PlayAudioFile("emo");
                        break;
                    }
                case "OLYMPUS":
                    {
                        DependencyService.Get<IAudio>().Reset();
                        List<string> artistsongs = new List<string>();
                        artistsongs.Add("olympus");
                        InitializeAudioControls(artistsongs);
                        break;
                    }
                case "Pizza Boston":
                    {
                        DependencyService.Get<IAudio>().Reset();
                        List<string> artistsongs = new List<string>();
                        artistsongs.Add("PizzaBoston_60K100");
                        artistsongs.Add("PizzaBoston_Denthaseswseikaneis");
                        InitializeAudioControls(artistsongs);
                        break;
                    }
                case "Words Of Hate":
                    {
                        DependencyService.Get<IAudio>().Reset();
                        List<string> artistsongs = new List<string>();
                        artistsongs.Add("WordsOfHate_Barricade_live");
                        artistsongs.Add("WordsOfHate_Siwph_live");
                        InitializeAudioControls(artistsongs);
                        break;
                    }
                case "Aχώνευτοι":
                    {
                        DependencyService.Get<IAudio>().Reset();
                        List<string> artistsongs = new List<string>();
                        artistsongs.Add("Axoneutoi_Cassetetape");
                        InitializeAudioControls(artistsongs);
                        break;
                    }
            }
        }
        public void ShowPage(Models.Artist artist)
        {
            ShownArtist = artist;
            LoadPagesDifferences();
            BindingContext = artist;
            ImageArtist.Source = artist.PhotoPath;
            if (artist.CreatedOn != null && artist.CreatedOn != System.DateTime.MinValue)
            {
                // Create a new Label for displaying "Created on: "
                Label createdOnLabel = new Label
                {
                    Text = "Created on: " + artist.CreatedOn.ToString("dd MMM yyyy"),
                    FontSize = 18,
                    HorizontalOptions = LayoutOptions.CenterAndExpand
                };
                GridForData.Children.Add(createdOnLabel);
            }
            
            GridForConnections.Children.Clear();
            int count = 0;
            if (!string.IsNullOrEmpty(artist.BandCampAccountUrl))
            {
                Button buttonBandCamp = CreateSocialButton("bandcamp.jpg", artist.BandCampAccountUrl);
                GridForConnections.Children.Add(buttonBandCamp, count++, 0);
            }

            if (!string.IsNullOrEmpty(artist.SpotifyAccountUrl))
            {
                Button buttonSpotify = CreateSocialButton("spotify.jpg", artist.SpotifyAccountUrl);
                GridForConnections.Children.Add(buttonSpotify, count++, 0);
            }

            if (!string.IsNullOrEmpty(artist.YoutubeAccountUrl))
            {
                Button buttonYouTube = CreateSocialButton("youtube.jpg", artist.YoutubeAccountUrl);
                GridForConnections.Children.Add(buttonYouTube, count++, 0);
            }

            if (!string.IsNullOrEmpty(artist.SoundcloudAccountUrl))
            {
                Button buttonSoundcloud = CreateSocialButton("soundcloud.png", artist.SoundcloudAccountUrl);
                GridForConnections.Children.Add(buttonSoundcloud, count++, 0);
            }
            if (!string.IsNullOrEmpty(artist.TwitchAccountUrl))
            {
                Button buttonTwitch = CreateSocialButton("twitch.png", artist.TwitchAccountUrl);
                GridForConnections.Children.Add(buttonTwitch, count++, 0);
            }
            if (!string.IsNullOrEmpty(artist.TrovoAccountUrl))
            {
                Button buttonTrovo = CreateSocialButton("trovo.png", artist.TrovoAccountUrl);
                GridForConnections.Children.Add(buttonTrovo, count++, 0);
            }
        }

        private void OnSwipe(object sender, SwipedEventArgs e)
        {
            if (e.Direction == SwipeDirection.Right)
            {
                if (App.Artists.IndexOf(ShownArtist) - 1 >= 0)
                {
                    ShowPage(App.Artists[App.Artists.IndexOf(ShownArtist) - 1]);
                    return;
                }
                ShowPage(App.Artists[App.Artists.Count - 1]);
                return;
            }
            if (e.Direction == SwipeDirection.Left)
            {
                if (App.Artists.IndexOf(ShownArtist) + 1 >= App.Artists.Count)
                {
                    ShowPage(App.Artists[0]);
                    return;
                }
                ShowPage(App.Artists[App.Artists.IndexOf(ShownArtist) + 1]);
                return;
            }
        }
    }
}