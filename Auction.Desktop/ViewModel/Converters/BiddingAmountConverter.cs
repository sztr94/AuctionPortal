using System;
using System.Windows;
using System.Windows.Data;

namespace Auction.Desktop.ViewModel
{
    public class BiddingAmountConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is Int32)) 
                return Binding.DoNothing; 

            Int32 price = (Int32)value;

            return price + " Ft";
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is String))
                return DependencyProperty.UnsetValue; 

            try
            {
                String priceString = value as String;
                Int32 price;

                price = Int32.Parse(priceString.Substring(0, priceString.Length - 2));

                if (price < 0)
                    return DependencyProperty.UnsetValue;

                return (Int32)price;
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
