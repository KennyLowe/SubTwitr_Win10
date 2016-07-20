using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
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

namespace SubTwitr
{
    
    public sealed partial class MainPage : Page
    {
        public StorageFile file;
        public FileInfo fileInfo;
        public MainPage()
        {

            this.InitializeComponent();
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
                playPause.Text = "Tap to Play";
                playPauseBorder.Visibility = Visibility.Visible;
                playPause.Visibility = Visibility.Visible;
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
    }
}
