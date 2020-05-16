using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Chatapp.Client.Utils
{
    public class ByteBitmapSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var img = value as byte[];
            if (img.Length == 0) return null;
            ImageSourceConverter converter = new ImageSourceConverter();
            var bmpSrc = (BitmapSource)converter.ConvertFrom(img);
            return bmpSrc;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
