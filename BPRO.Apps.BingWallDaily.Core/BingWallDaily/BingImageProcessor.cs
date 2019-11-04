using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;

namespace BPRO.Apps.BingWallDaily.Core
{
    public class BingImageProcessor
    {
        private BingImageOfTheDay bingImage = new BingImageOfTheDay();

        public void Init()
        {
            WebRequest request = WebRequest.Create("http://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1");

            using (WebResponse response = request.GetResponse())
            {
                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    string responseFromServer = reader.ReadToEnd();
                    var imagesContainer = (BingImagesContainer)JsonConvert.DeserializeObject(responseFromServer, typeof(BingImagesContainer));
                    bingImage = ProcessImageFile(imagesContainer);
                    if (bingImage.processImage)
                        AddWaterMarkText();
                    SetImageAsWallpaper();
                }
            }
            File.Delete(bingImage.imageFilename);
        }

        public BingImageOfTheDay GetBingImageofTheDay()
        {
            return bingImage;
        }

        private BingImageOfTheDay ProcessImageFile(BingImagesContainer imagesContainer)
        {
            var bingImageContainer = new BingImageOfTheDay();

            if (!imagesContainer.images.Any())
            {
                bingImageContainer.processImage = false;
                return bingImageContainer;
            }
            BingImage bingImage = imagesContainer.images[0];

            bingImageContainer.hdImageUrl = "http://www.bing.com/" + bingImage.urlbase + bingImageContainer.HD_Suffix;
            string dir = @"C:\BingWallDaily\wallpapers\";

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            bingImageContainer.imageFilename = dir + "bingwallpaper_" + bingImage.hsh + " .jpg";
            bingImageContainer.imageFilename_wm = dir + "bingwallpaper_" + bingImage.hsh + "_watermarked" + ".jpg";
            bingImageContainer.copyRight = bingImage.copyright;

            if (File.Exists(bingImageContainer.imageFilename_wm))
            {
                //meaning the image is the latest image.
                bingImageContainer.processImage = false;
                return bingImageContainer;
            }

            try
            {
                HttpWebRequest lxRequest = (HttpWebRequest)WebRequest.Create(bingImageContainer.hdImageUrl);

                // returned values are returned as a stream, then read into a string
                String lsResponse = string.Empty;
                using (HttpWebResponse lxResponse = (HttpWebResponse)lxRequest.GetResponse())
                {
                    using (BinaryReader reader = new BinaryReader(lxResponse.GetResponseStream()))
                    {
                        Byte[] lnByte = reader.ReadBytes(1 * 1024 * 1024 * 10);
                        using (FileStream lxFS = new FileStream(bingImageContainer.imageFilename, FileMode.Create))
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

            return bingImageContainer;
        }

        private void AddWaterMarkText()
        {
            try
            {
                //open source image as stream and create a memorystream for output
                FileStream source = new FileStream(bingImage.imageFilename, FileMode.Open);
                Stream output = new MemoryStream();
                Image img = Image.FromStream(source);

                // choose font for text
                Font font = new Font("Tahoma", 16, FontStyle.Regular, GraphicsUnit.Pixel);
                Font discFont = new Font("Arial", 11, FontStyle.Regular, GraphicsUnit.Pixel);

                //choose color and transparency
                Color bgcolor = Color.DarkSlateGray;
                Color color = Color.GhostWhite;

                SolidBrush brush = new SolidBrush(color);
                SolidBrush bgbrush = new SolidBrush(bgcolor);

                //draw text on image
                Graphics graphics = Graphics.FromImage(img);

                SizeF copyRightStrSize = graphics.MeasureString(bingImage.copyRight, font);

                int lineHeight = 8;
                int topHeight = 0;


                graphics.DrawString(bingImage.copyRight, font, bgbrush, new Point(img.Size.Width - ((int)copyRightStrSize.Width + 3) - 1, (int)copyRightStrSize.Height + 1));
                graphics.DrawString(bingImage.copyRight, font, brush, new Point(img.Size.Width - ((int)copyRightStrSize.Width + 3), (int)copyRightStrSize.Height));

                topHeight += (int)copyRightStrSize.Height + 1 + lineHeight;

                string disclaimerText = "Image date: " + DateTime.Now.ToShortDateString() + ". Image was acquired using a public URL. BingWallDaily® is a free software by BPRO Consuting LLP.";

                SizeF disclaimerStrSize = graphics.MeasureString(disclaimerText, discFont);

                graphics.DrawString(disclaimerText, discFont, bgbrush, new Point(img.Size.Width - ((int)disclaimerStrSize.Width + 20) - 1, topHeight + ((int)disclaimerStrSize.Height) + 1));
                graphics.DrawString(disclaimerText, discFont, brush, new Point(img.Size.Width - ((int)disclaimerStrSize.Width + 20), topHeight + ((int)disclaimerStrSize.Height)));

                topHeight += (int)disclaimerStrSize.Height + 2;

                string disclaimer2Text = "All rights reserved to the owner of the image.";

                SizeF disclaimer2StrSize = graphics.MeasureString(disclaimer2Text, discFont);

                graphics.DrawString(disclaimer2Text, discFont, bgbrush, new Point(img.Size.Width - ((int)disclaimer2StrSize.Width + 21) - 1, topHeight + ((int)disclaimer2StrSize.Height) + 1));
                graphics.DrawString(disclaimer2Text, discFont, brush, new Point(img.Size.Width - ((int)disclaimer2StrSize.Width + 21), topHeight + (int)disclaimer2StrSize.Height));

                graphics.Dispose();

                //update image memorystream
                img.Save(output, ImageFormat.Jpeg);
                Image imgFinal = Image.FromStream(output);

                //write modified image to file
                Bitmap bmp = new System.Drawing.Bitmap(img.Width, img.Height, img.PixelFormat);
                Graphics graphics2 = Graphics.FromImage(bmp);
                graphics2.DrawImage(imgFinal, new Point(0, 0));
                bmp.Save(bingImage.imageFilename_wm, ImageFormat.Jpeg);

                imgFinal.Dispose();
                img.Dispose();
                source.Dispose();
                output.Dispose();

            }
            catch (Exception ex)
            {
                //
            }
        }


        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        private void SetImageAsWallpaper()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

                //Fit style only
                key.SetValue(@"WallpaperStyle", "6");
                key.SetValue(@"TileWallpaper", "0");

                SystemParametersInfo(SPI_SETDESKWALLPAPER,
                   0,
                   bingImage.imageFilename_wm,
                   SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
            }
            catch (Exception e)
            {
                throw new Exception("There was a problem on using the image: \n" + e.Message + "\nStack Trace: " + e.StackTrace);
            }

        }
    }
}
