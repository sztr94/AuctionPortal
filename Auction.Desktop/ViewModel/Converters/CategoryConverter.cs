using Auction.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Auction.Desktop.ViewModel
{
    public class CategoryConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        { 
            if (value == null || !(value is Int32))
                return Binding.DoNothing; 

            Int32 id = (Int32)value;
            CategoryDTO category = (parameter as IEnumerable<CategoryDTO>).FirstOrDefault(cat => cat.Id == id);

            if (category == null)
                return Binding.DoNothing;

            return category;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is CategoryDTO))
                return DependencyProperty.UnsetValue;

            return ((CategoryDTO)value).Id;
        }
    }
}

