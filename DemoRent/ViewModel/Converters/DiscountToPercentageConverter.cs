using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ViewModel.Converters
{
    public class DiscountToPercentageConverter : IValueConverter
    {        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is decimal))
                throw new InvalidOperationException("Only decimal values can be converted to percentage.");

            return string.Format("{0:0.00} %", ((decimal)value - 1) * 100);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal percentage;
            string valueString = (value as string).Replace("%", string.Empty).Replace(',','.').Trim();
            
            if (decimal.TryParse(valueString, NumberStyles.Any, CultureInfo.InvariantCulture, out percentage))
                return (percentage / 100M) + 1;            
            else            
                return 1M;            
        }
    }
}
