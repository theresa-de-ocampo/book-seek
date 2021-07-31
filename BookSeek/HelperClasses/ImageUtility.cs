using System;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BookSeek.HelperClasses
{
    public static class ImageUtility
    {
        public static Image GetImage(string source)
        {
            Image image = new Image();
            BitmapImage bitmapImage = GetBitmapImage(source);
            image.Source = bitmapImage;
            return image;
        }

        public static BitmapImage GetBitmapImage(string source)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(source);
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmapImage.EndInit();
            return bitmapImage;
        }
    }
}
