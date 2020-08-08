using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace SAPPRemote
{
    internal class ComboBoxSelectedValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new ComboBoxItem() { Content = value, Tag = "REC" };
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ComboBoxItem item)
            {
                return item.Content.ToString();
            }
            else
            {
                return "";
            }
        }
    }
}