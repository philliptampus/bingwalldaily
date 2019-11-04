using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPRO.Apps.BingWallDaily.Core;

namespace BingWallDailyRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var bingWallpaperProcessor = new BingImageProcessor();
                bingWallpaperProcessor.Init();
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }
    }
}
