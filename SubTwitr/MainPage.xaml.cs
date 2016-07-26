using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Windows;
using LinqToTwitter;
using Windows.UI.Popups;
using Auth0.LoginClient;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.Background;
using System.Threading;

namespace SubTwitr
{

    public class checkForInternetConnection
    {
        [DllImport("wininet.dll")]

        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);
        public static bool IsConnectedToInternet()
        {
            int Desc;
            return InternetGetConnectedState(out Desc, 0);
        }
    }

    public sealed partial class MainPage : Page
    {
        public StorageFile file;
        public FileInfo fileInfo;
        TwitterContext ctx;
        SingleUserAuthorizer userAuth;
        UniversalAuthorizer uniAuth;
        CancellationTokenSource _cts;

        private async void RegisterBackgroundTask()
        {
            var taskName = "UpdateInternetConnection";

            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (backgroundAccessStatus == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity || backgroundAccessStatus == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
            {
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == taskName)
                    {
                        return;
                    }
                }

                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = taskName;
                taskBuilder.TaskEntryPoint = typeof(BackgroundTasks.ConnectionStatus).FullName;
                taskBuilder.SetTrigger(new TimeTrigger(120, false));

                var registration = taskBuilder.Register();
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            _cts = new CancellationTokenSource();
            RegisterBackgroundTask();
            runSetup();
        }



        public void runSetup()
        {


            if (checkForInternetConnection.IsConnectedToInternet())
            {
                twitterLogin();
            }
            else
            {
                string url = "ms-appx:///Images/NoConn.png";
                profilePic.Source = new BitmapImage(new Uri(url, UriKind.Absolute));
                userName.Text = "No Conn";
                refreshButton.Visibility = Visibility.Visible;
            }
        }



        public async void twitterLogin()
        {
            Windows.Storage.StorageFile secretsFile = null;

            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            try { secretsFile = await storageFolder.CreateFileAsync(@"secrets.txt", Windows.Storage.CreationCollisionOption.FailIfExists); }
            catch { secretsFile = await storageFolder.GetFileAsync(@"secrets.txt"); }

            string readFile = await FileIO.ReadTextAsync(secretsFile);

            // If secrets file exists
            if (readFile != "")
            {
                string readThis = await FileIO.ReadTextAsync(secretsFile);
                char[] delimiterChars = { ' ' };
                string[] words = readThis.Split(delimiterChars);
                string oauthToken = words[0];
                string oAuthTokenSecret = words[1];
                string oAuthUsername = words[2];
                ulong oauthUserID = Convert.ToUInt64(words[3]);
                userAuth = new SingleUserAuthorizer
                {
                    CredentialStore = new SingleUserInMemoryCredentialStore
                    {
                        ConsumerKey = "oG9yeSPTSRk6SrXvJdYzebTbq",
                        ConsumerSecret = "TpJH0SJeL3ruoQ7C9vWrLUP7X6ZxgdXlBQERxp3MNuT5xaHF9v",
                        AccessToken = oauthToken,
                        AccessTokenSecret = oAuthTokenSecret,
                        ScreenName = oAuthUsername,
                        UserID = oauthUserID
                    },
                    Callback = "http://www.subtwitr.com"
                };
                var credentials = userAuth.CredentialStore;
                userName.Text = credentials.ScreenName;
                ctx = new TwitterContext(userAuth);
                User currentUser = (from user in ctx.User
                                    where user.Type == UserType.Show && user.ScreenName == credentials.ScreenName
                                    select user).ToList<User>().SingleOrDefault();
                string profileImage = currentUser.ProfileImageUrl;
                string normalImage = profileImage.Remove(profileImage.Length - 11) + ".jpg";
                profilePic.Source = new BitmapImage(new Uri(normalImage));

            }
            else
            {
                uniAuth = new UniversalAuthorizer
                {
                    CredentialStore = new InMemoryCredentialStore
                    {
                        ConsumerKey = "oG9yeSPTSRk6SrXvJdYzebTbq",
                        ConsumerSecret = "TpJH0SJeL3ruoQ7C9vWrLUP7X6ZxgdXlBQERxp3MNuT5xaHF9v"
                    },
                    Callback = "http://www.subtwitr.com"
                };

                await uniAuth.AuthorizeAsync();
                ctx = new TwitterContext(uniAuth);

                var oauthToken = uniAuth.CredentialStore.OAuthToken;
                var oauthSecret = uniAuth.CredentialStore.OAuthTokenSecret;
                var oauthUsername = uniAuth.CredentialStore.ScreenName;
                var oauthID = uniAuth.CredentialStore.UserID;

                string oauthTokenStr = oauthToken.ToString();
                string oAuthSecretStr = oauthSecret.ToString();
                string oAuthUsernameStr = oauthUsername.ToString();
                string oAuthIDStr = oauthID.ToString();

                Windows.Storage.StorageFile createFile = await storageFolder.CreateFileAsync(@"secrets.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);

                await Windows.Storage.FileIO.WriteTextAsync(createFile, oauthTokenStr + " " + oAuthSecretStr + " " + oAuthUsernameStr + " " + oAuthIDStr);

                var credentials = uniAuth.CredentialStore;
                userName.Text = credentials.ScreenName;

                User currentUser = (from user in ctx.User
                                    where user.Type == UserType.Show && user.ScreenName == credentials.ScreenName
                                    select user).ToList<User>().SingleOrDefault();

                string profileImage = currentUser.ProfileImageUrl;
                string normalImage = profileImage.Remove(profileImage.Length - 11) + ".jpg";
                profilePic.Source = new BitmapImage(new Uri(normalImage));
            }
        }


        public static async Task<string> ReadTextFile(string _filename)
        {
            string contents = null;
            StorageFolder localfolder = ApplicationData.Current.LocalFolder;
            
            if (await localfolder.TryGetItemAsync(_filename) != null)
            {
                StorageFile textfile = await localfolder.GetFileAsync(_filename);
                contents = await FileIO.ReadTextAsync(textfile);
            }

            else
            {
                contents = null;
            }

            return contents;
        }

        private async void FilePicker_Click(object sender, RoutedEventArgs e)
        {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.Thumbnail;
                openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                openPicker.FileTypeFilter.Add(".mp4");
                openPicker.FileTypeFilter.Add(".avi");
                openPicker.FileTypeFilter.Add(".mpg");
                openPicker.FileTypeFilter.Add(".mpeg");

            PickFile.BorderBrush= new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));
            

            file = await openPicker.PickSingleFileAsync();

 
            if (file != null)
            {
                // Application now has read/write access to the picked file
                OutputTextBlock.Text = "File Selected: " + file.Name;
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                mediaElement.SetSource(stream, file.ContentType);
                if (mediaElement.NaturalDuration.HasTimeSpan)
                {
                    if (mediaElement.NaturalDuration.TimeSpan.TotalSeconds > 30)
                    {
                        OutputTextBlock.Text = "File is too long. 30 seconds max.";
                        mediaElement.Source = null;
                    }
                    else
                    {
                        playPause.Text = "Tap to Play";
                        playPauseBorder.Visibility = Visibility.Visible;
                        playPause.Visibility = Visibility.Visible;
                    }
                }
            }
            else
            {
                OutputTextBlock.Text = "Operation cancelled.";
            } 
        }

        private async void SendFile_Click(object sender, RoutedEventArgs e)
        {
            if (file != null)
            {
                if (checkForInternetConnection.IsConnectedToInternet())
                {

                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=subtwitr;AccountKey=y2FHEejelfEDF8w38kdYWSdpgBcJ39DsHU2fkjFbJT80fUPS8CYvg73QIUpO5pJYQZs8QImkQdN2s91cWvawzA==");
                    CloudFileClient fileClient = storageAccount.CreateCloudFileClient();
                    CloudFileShare share = fileClient.GetShareReference("subtwitr");
                    Guid g = Guid.NewGuid();

                    // Create folder name based on GUID
                    CloudFileDirectory dir = share.GetRootDirectoryReference();
                    CloudFileDirectory fileDirectory = null;
                    fileDirectory = dir.GetDirectoryReference(g.ToString());
                    await fileDirectory.CreateIfNotExistsAsync();

                    //Upload video file
                    CloudFile destFile = dir.GetFileReference(g.ToString() + @"\" + g.ToString() + ".mp4");
                    await destFile.UploadFromFileAsync(file);

                    //Upload Tweet
                    if ((tweetBox.Text != null) && (tweetBox.Text != "Enter Tweet"))
                    {
                        CloudFile tweetFile = dir.GetFileReference(g.ToString() + @"\" + g.ToString() + ".tweet");
                        await tweetFile.UploadTextAsync(tweetBox.Text);
                    }

                    if ((tweetBox.Text == null) || (tweetBox.Text == "Enter Tweet"))
                    {
                        CloudFile tweetFile = dir.GetFileReference(g.ToString() + @"\" + g.ToString() + ".tweet");
                        await tweetFile.UploadTextAsync("#subtwitr");
                    }

                    //upload oAuth Details

                    Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                    Windows.Storage.StorageFile oAuthFile = await storageFolder.GetFileAsync(@"secrets.txt");
                    CloudFile destoAuth = dir.GetFileReference(g.ToString() + "\\" + g.ToString() + ".oauth");


                    await destoAuth.UploadFromFileAsync(oAuthFile);
                }
                else
                {
                    string url = "ms-appx:///Images/NoConn.png";
                    profilePic.Source = new BitmapImage(new Uri(url, UriKind.Absolute));
                    userName.Text = "No Conn";
                    refreshButton.Visibility = Visibility.Visible;
                }
            }
        }

        private void TextBlock_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        { 
        }

        private void mediaElement_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
        }


        private void playPause_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((mediaElement.CurrentState.ToString() == "Stopped") || (mediaElement.CurrentState.ToString() == "Paused"))
            {
                playPauseBorder.Visibility = Visibility.Collapsed;
                playPause.Visibility = Visibility.Collapsed;
                mediaElement.Play();
            }
        }

        private void playPauseBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((mediaElement.CurrentState.ToString() == "Stopped") || (mediaElement.CurrentState.ToString() == "Paused"))
            {
                playPauseBorder.Visibility = Visibility.Collapsed;
                playPause.Visibility = Visibility.Collapsed;
                mediaElement.Play();
            }
        }

        private void PickFile_PointerPressed(object sender, PointerRoutedEventArgs e)
        {

        }

        private void mediaElement_PointerCanceled_1(object sender, PointerRoutedEventArgs e)
        {
            if (mediaElement.CurrentState.ToString() == "Playing")
            {
                mediaElement.Pause();
            }
            if ((mediaElement.CurrentState.ToString() == "Stopped") || (mediaElement.CurrentState.ToString() == "Paused"))
            {
                mediaElement.Play();
                playPauseBorder.Visibility = Visibility.Collapsed;
                playPause.Visibility = Visibility.Collapsed;
            }

        }

        private void mediaElement_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            if (mediaElement.CurrentState.ToString() == "Playing")
            {
                mediaElement.Pause();
            }
            if ((mediaElement.CurrentState.ToString() == "Stopped") || (mediaElement.CurrentState.ToString() == "Paused"))
            {
                mediaElement.Play();
                playPauseBorder.Visibility = Visibility.Collapsed;
                playPause.Visibility = Visibility.Collapsed;
            }
        }

        private void tweetBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tweetBox.Text == "Enter Tweet")
            {
                tweetBox.Text = "";
            }
        }


        private void settings_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Settings), null);
        }

        private void refreshButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (checkForInternetConnection.IsConnectedToInternet())
            {
                refreshButton.Visibility = Visibility.Collapsed;
                twitterLogin();
            }
            else
            {
                string url = "ms-appx:///Images/NoConn.png";
                profilePic.Source = new BitmapImage(new Uri(url, UriKind.Absolute));
                userName.Text = "No Conn";
                refreshButton.Visibility = Visibility.Visible;
            }
        }

        private void optionsButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Options), null);
        }
    }
}
