using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;
using System.Management;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace project_laty
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
        }

        //body
        public void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            //display
            var Display = DisplayArea.Primary;
            var DisplayBounds = Display.OuterBounds;
            var DisplayHeight = DisplayBounds.Height;
            var DisplayWidth = DisplayBounds.Width;
            // Screen.Text = DisplayWidth.ToString() + " x " + DisplayHeight.ToString();

            //get wallpaper
            string UserFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string Path = UserFolder + "\\Microsoft\\Windows\\Themes\\TranscodedWallpaper";
            float Ratio = DisplayWidth / 144;
            Frame.Height = DisplayHeight / Ratio;
            Wallpaper.ImageSource = new BitmapImage(new Uri(Path));

            //device
            var DeviceInformation = new EasClientDeviceInformation();
            var SystemInfoKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\OEMInformation");

            DeviceName.Text = (DeviceInformation.FriendlyName);

            if (SystemInfoKey.GetValue("Manufacturer", null) == null)
            {
                DeviceManufacturer.Text = (DeviceInformation.SystemManufacturer);
            }
            else
            {
                DeviceManufacturer.Text = (SystemInfoKey.GetValue("Manufacturer").ToString());
            }

            if (SystemInfoKey.GetValue("Model", null) == null)
            {
                DeviceModel.Text = (DeviceInformation.SystemProductName);
            }
            else
            {
                DeviceModel.Text = (SystemInfoKey.GetValue("Model").ToString());
            }

            //about windows
            string DeviceFamilyVersion = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong Version = ulong.Parse(DeviceFamilyVersion);
            ulong Build = (Version & 0x00000000FFFF0000L) >> 16;
            ulong Revision = (Version & 0x000000000000FFFFL);
            //WindowsRevision.Text = ($"{Build}.{Revision}");

            var WindowsInfoKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            //WindowsVersion.Text = (WindowsInfoKey.GetValue("DisplayVersion").ToString());

            string Windows = (WindowsInfoKey.GetValue("ProductName").ToString());
            OSVersion.Text = Windows;
            //WindowsEdition.Text = Windows;

            if (Build > 21995)
            {
                string Windows11 = Regex.Replace(Windows, "0", "1");
                OSVersion.Text = Windows11;
                //WindowsEdition.Text = Windows11;
            }

            //processor
            var ProcessorInfo = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0\");
            //CPU.Text = (ProcessorInfo.GetValue("ProcessorNameString").ToString());

            //memory
            ManagementObjectSearcher MemoryInfo = new ManagementObjectSearcher(@"select * from Win32_OperatingSystem");
            string Memory = String.Empty;
            string MemoryFree = String.Empty;

            foreach (ManagementObject Result in MemoryInfo.Get())
            {
                Memory = Result["TotalVisibleMemorySize"].ToString();
                float MemoryInKB = Int32.Parse(Memory);
                double MemoryInGB = MemoryInKB / 1048576;
                //RAM.Text = MemoryInGB.ToString("F1") + " GB";
            }

            //storage
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            string Storage = String.Empty;

            foreach (System.IO.DriveInfo drive in drives)
            {
                if (drive.RootDirectory.ToString() == "C:\\")
                {
                    Storage = drive.TotalSize.ToString();
                    float StorageInKB = Int64.Parse(Storage);
                    double StorageInGB = StorageInKB / 1073741824;
                    //Disk.Text = StorageInGB.ToString("F1") + " GB";
                }
            }

            //gpu
            ManagementObjectSearcher GraphicsInfo = new ManagementObjectSearcher("select * from Win32_VideoController ");
            string Graphics = String.Empty;

            foreach (ManagementObject Result in GraphicsInfo.Get())
            {

                if ((String)Result["DeviceID"] == "VideoController1")
                {
                    Graphics = Result["Description"].ToString();
                    //GPU.Text = Graphics;
                }

                if ((String)Result["DeviceID"] == "VideoController2")
                {
                    Graphics = Result["Description"].ToString();
                    //GPU.Text = Graphics;
                }

            }

        }

        //update button
        private async void Update_Click(object sender, RoutedEventArgs e)
        {
            var UpdateUri = new Uri(@"ms-settings:windowsupdate");
            var success = await Windows.System.Launcher.LaunchUriAsync(UpdateUri);
        }
    }
}
