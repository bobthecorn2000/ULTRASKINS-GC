using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using UltraSkinsPacker;
using static System.Reflection.Metadata.BlobBuilder;
using System.Collections.ObjectModel;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace USutilsGUI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
         
        public MainPage()
        {
            this.InitializeComponent();
            Slots = new ObservableCollection<Slot>
            {
                new Slot { Image = "Assets/Image1.png", AssignCommand = new DelegateCommand(AssignImage) },
                new Slot { Image = "Assets/Image2.png", AssignCommand = new DelegateCommand(AssignImage) },
                // Add more slots here
            };

            // Set the DataContext
            this.DataContext = this;
        }
    }
        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Handle item click, e.g., show a larger view of the image.
            var clickedImage = e.ClickedItem as string;
        }
        /*        private async void PistolButton(object sender, RoutedEventArgs e)
                {
                    Image PistolImage = await FilePicker("pistol");
                }*/
        public async System.Threading.Tasks.Task<Windows.Storage.StorageFile> FilePicker(string guntype)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".png");
            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {

                return(file);
            }
            else
            {
                return(null);
            }
        }

    }
}
