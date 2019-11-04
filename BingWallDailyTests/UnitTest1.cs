using System;
using System.Runtime.InteropServices;
using BPRO.Apps.BingWallDaily.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;

namespace BingWallDailyTests
{
    [TestClass]
    public class UnitTest1
    {
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        //[TestMethod]
        //public void TestMethod1()
        //{
        //    //BingImageProcessor.Init();
        //}

        [TestMethod]
        public void TestSettingWallpaper()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

            //Fit style only
            key.SetValue(@"WallpaperStyle", "6");
            key.SetValue(@"TileWallpaper", "0");

            SystemParametersInfo(SPI_SETDESKWALLPAPER,
               0,
               "C:\\BingWallDaily\\wallpapers\\bingwallpaper_2605383e0bc32b217f4b01473c567d56_watermarked.jpg",
               SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}
