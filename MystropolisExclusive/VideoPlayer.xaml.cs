using FFmpegInteropX;
using MystropolisExclusive.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MystropolisExclusive
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VideoPlayer : Page
    {
        private const string CountdownText = "Video starter om ";

        private readonly string RootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), Settings.VideoFolderName);
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly DispatcherTimer stopEarlyTimer = new DispatcherTimer();
        private int _currentCount = 0;

        private MysticlusiveCode code = null;
        private FFmpegMediaSource FFmpegMSS;

        public VideoPlayer()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            code = (MysticlusiveCode)e.Parameter;

            if (code != null)
            {
                StartCountdown();
            }
            else
            {
                Frame.Navigate(typeof(MainPage));
            }
        }

        private void StartCountdown()
        {
            mediaPlayer.Stop();
            mediaPlayer.Visibility = Visibility.Collapsed;
            CountdownTextBlock.Visibility = Visibility.Visible;

            _currentCount = Settings.CountdownDuration;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;

            SetCountdownText();
            timer.Start();
        }

        private async void Timer_Tick(object sender, object e)
        {
            if (_currentCount == 0)
            {
                timer.Stop();

                await LoadAndPlayVideo(code.Video);
            }
            else
            {
                _currentCount--;
                SetCountdownText();
            }
        }

        private void SetCountdownText()
        {
            CountdownTextBlock.Text = CountdownText + TimeSpan.FromSeconds(_currentCount).ToString();
        }

        private async Task LoadAndPlayVideo(string filename)
        {
            var filePath = Path.Combine(RootPath, filename);
            var file = await StorageFile.GetFileFromPathAsync(filePath);

            if (file != null)
            {
                try
                {
                    var readStream = await file.OpenAsync(FileAccessMode.Read);
                    FFmpegMSS = await FFmpegMediaSource.CreateFromStreamAsync(readStream, new MediaSourceConfig
                    {
                    });

                    MediaStreamSource mss = FFmpegMSS.GetMediaStreamSource();

                    if (mss != null)
                    {
                        CountdownTextBlock.Visibility = Visibility.Collapsed;

                        mediaPlayer.Visibility = Visibility.Visible;
                        mediaPlayer.SetMediaStreamSource(mss);
                        mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;

                        if (code.MinimumDuration != null && code.MinimumDuration.Value > 0)
                        {
                            stopEarlyTimer.Interval = new TimeSpan(0, 0, code.MinimumDuration.Value);
                            stopEarlyTimer.Tick += StopEarlyTimer_Tick;
                            stopEarlyTimer.Start();
                        }
                        mediaPlayer.Play();
                    }

                }
                catch (Exception e)
                {
                    var messageBox = new MessageDialog(e.Message);
                    await messageBox.ShowAsync();
                }
            }
        }

        private async void StopEarlyTimer_Tick(object sender, object e)
        {
            StopEarlyButton.Visibility = Visibility.Visible;
            stopEarlyTimer.Stop();
        }

        private void StopEarlyButton_Click(object sender, RoutedEventArgs e)
        {
            StopEarly();
        }

        private async void StopEarly()
        {
            mediaPlayer.Pause();

            ContentDialog deletionPrompt = new ContentDialog
            {
                Title = "Er du sikker på du vil stoppe?",
                Content = "Det er ikke sikkert du kan se videoen igen.",
                CloseButtonText = "Nej, se videre.",
                PrimaryButtonText = "Ja, stop video"
            };

            ContentDialogResult result = await deletionPrompt.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                mediaPlayer.Stop();
                EndVideo();
            }
            else
            {
                mediaPlayer.Play();
            }
        }

        private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            EndVideo();
        }

        private void EndVideo()
        {
            DataAccess.DataAccess.MarkAsUsed(code.Code);
            Frame.Navigate(typeof(MainPage));
        }
    }
}
