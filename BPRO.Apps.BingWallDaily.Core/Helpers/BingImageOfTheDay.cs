using System.Collections.Generic;

namespace BPRO.Apps.BingWallDaily.Core
{
    public class BingImageOfTheDay
    {
        public string HD_Suffix
        {
            get
            {
                return "_1920x1080.jpg";
            }
        }

        public string hdImageUrl { get; set; }

        public string imageFilename { get; set; }

        public string imageFilename_wm { get; set; }

        public string copyRight { get; set; }

        public bool processImage { get; set; } = true;

        
    }
}
