using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SubTwitr;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SubTwitr
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        TwitterContext ctx;
        SingleUserAuthorizer userAuth;
        UniversalAuthorizer uniAuth;
        
        public Settings()
        {
            this.InitializeComponent();

        }
        private void backButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage), null);
        }

        private async void clearTwitter_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile oAuthFile = await storageFolder.GetFileAsync(@"secrets.txt");
            await oAuthFile.DeleteAsync();
        }

        private async void setTwitter_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Windows.Storage.StorageFile secretsFile = null;

            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            try { secretsFile = await storageFolder.CreateFileAsync(@"secrets.txt", Windows.Storage.CreationCollisionOption.FailIfExists); }
            catch { secretsFile = await storageFolder.GetFileAsync(@"secrets.txt"); }

            string readFile = await FileIO.ReadTextAsync(secretsFile);

            // If secrets file exists
            if (readFile != "")
            {
                var dialog = new MessageDialog("Twitter credentials are already set.");
                await dialog.ShowAsync();
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
            }

        }
    }
}
