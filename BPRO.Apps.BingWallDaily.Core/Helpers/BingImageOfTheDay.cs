using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace BPRO.Apps.BingWallDaily.Core
{
    public class BingImageOfTheDay
    {
        public const string HD_Suffix = "_1920x1080.jpg";
        public List<BingImage> images { get; set; }
        private string hdImageUrl;
        public string imageFilename { get; set; }
        public string imageFilename_wm { get; set; }
        public string copyRight { get; set; }
        public bool processImage { get; set; }

        public void PullImage()
        {
            try
            {
                processImage = true;
                if (!this.images.Any())
                {
                    processImage = false;
                    return;
                }
                BingImage bingImage = this.images[0];

                this.hdImageUrl = "http://www.bing.com/" + bingImage.urlbase + HD_Suffix;
                string dir = @"C:\BingWallDaily\wallpapers\";

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                this.imageFilename = dir + "bingwallpaper_" + bingImage.hsh + " .jpg";
                this.imageFilename_wm = dir + "bingwallpaper_" + bingImage.hsh + "_watermarked" + ".jpg";
                this.copyRight = bingImage.copyright;

                if (!GlobalSettings.Bypass_Img_Check && File.Exists(imageFilename_wm))
                {
                    //meaning the image is the latest image.
                    processImage = false;
                    return;
                }

                HttpWebRequest lxRequest = (HttpWebRequest)WebRequest.Create(hdImageUrl);

                // returned values are returned as a stream, then read into a string
                String lsResponse = string.Empty;
                using (HttpWebResponse lxResponse = (HttpWebResponse)lxRequest.GetResponse())
                {
                    using (BinaryReader reader = new BinaryReader(lxResponse.GetResponseStream()))
                    {
                        Byte[] lnByte = reader.ReadBytes(1 * 1024 * 1024 * 10);
                        using (FileStream lxFS = new FileStream(imageFilename, FileMode.Create))
                        {
                            lxFS.Write(lnByte, 0, lnByte.Length);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("There was a problem on pulling the image: \n" + e.Message + "\nStack Trace: " + e.StackTrace);
            }

        }
    }
}
