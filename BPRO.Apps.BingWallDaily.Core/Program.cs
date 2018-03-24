using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Win32;
using System.Drawing;
using System.Drawing.Imaging;


/* 
 *	Bing Wall Daily Beta
 *	Version: 1.1
 *
 *	Copyright (c) 2015 Phillip John Tampus (http://www.codetampus.com/)
 *
 *	Dual licensed under the MIT (http://www.opensource.org/licenses/mit-license.php) 
 *	and GPL (http://opensource.org/licenses/GPL-3.0) licenses.
 *
 */
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BPRO.Apps.BingWallDaily.Core
{
    class Program
    {
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;
        static BingImageOfTheDay bingImage = new BingImageOfTheDay();
        static Form pleaseWaitForm = new Form();
        static Label namelabel = new Label();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        static void Main(string[] args)
        {
            try
            {
                ShowLoading();

            }
            catch (Exception e)
            {
                MessageBox.Show("Oops, something went wrong: \n\nMessage: \n" + e.Message + "\n\nStack Trace: \n" + e.StackTrace);
            }

        }

        public static void StartGettingWallpaper()
        {
            WebRequest request = WebRequest.Create("http://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1");

            using (WebResponse response = request.GetResponse())
            {

                Stream dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    string responseFromServer = reader.ReadToEnd();
                    bingImage = (BingImageOfTheDay)JsonConvert.DeserializeObject(responseFromServer, typeof(BingImageOfTheDay));
                    bingImage.PullImage();
                    if (bingImage.processImage)
                        AddWaterMarkText();
                    UseImage();
                }
            }
            File.Delete(bingImage.imageFilename);
        }

        public static void ShowLoading()
        {
            pleaseWaitForm.StartPosition = FormStartPosition.CenterScreen;
            pleaseWaitForm.Opacity = 0.8;
            pleaseWaitForm.FormBorderStyle = FormBorderStyle.None;
            pleaseWaitForm.Height = 15;

            namelabel.AutoSize = true;

            namelabel.Text = "Please wait while I'm getting Bing's latest wallpaper...\nThis should only take a couple of seconds...\n-PJT";
            pleaseWaitForm.Controls.Add(namelabel);

            pleaseWaitForm.Show();
            pleaseWaitForm.Refresh();
            StartGettingWallpaper();
        }

        public static void UseImage()
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

        /// <summary>
        /// Add a text watermark to an image
        /// </summary>
        /// <param name="sourceImage">path to source image</param>
        /// <param name="text">text to add as a watermark</param>
        /// <param name="targetImage">path to the modified image</param>
        /// <param name="fmt">ImageFormat type</param>
        public static void AddWaterMarkText()
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

                int lineHeight = 10;
                int topHeight = 0;


                graphics.DrawString(bingImage.copyRight, font, bgbrush, new Point(img.Size.Width - ((int)copyRightStrSize.Width + 3) - 1, (int)copyRightStrSize.Height + 1));
                graphics.DrawString(bingImage.copyRight, font, brush, new Point(img.Size.Width - ((int)copyRightStrSize.Width + 3), (int)copyRightStrSize.Height));

                topHeight += (int)copyRightStrSize.Height + 1 + lineHeight;

                string disclaimerText = "Image was acquired using a public URL. BingWallDaily® is a free software by BPRO Consuting LLP.";

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
                MessageBox.Show(ex.Message);
            }
        }


    }
}
