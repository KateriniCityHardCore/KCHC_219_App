using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Media;
using Android.OS;
using KCHC.Properties;

namespace KCHC.Droid
{
    [Activity(Label = "KCHC", Icon = "@drawable/kchc", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        MediaPlayer mediaPlayer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            // Initialize Xamarin.Forms application before reading Settings
            LoadApplication(new App());

            // Read the stored setting; default to true if anything goes wrong
            var playIntro = true;
            try
            {
                playIntro = Settings.IntroMusicEnabled;
            }
            catch
            {
                playIntro = true;
            }

            if (playIntro)
            {
                // Find the resource ID of your MP3 file
                int soundResourceId = Resource.Raw.Thanos;

                // Initialize MediaPlayer and start playing
                mediaPlayer = MediaPlayer.Create(this, soundResourceId);
                mediaPlayer.Start();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            mediaPlayer?.Release();
            mediaPlayer = null;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}