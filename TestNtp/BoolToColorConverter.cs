using System;
using Xamarin.Forms;

namespace TestNtp
{
	public class BoolToColorConverter : IValueConverter
	{

		#region IValueConverter implementation

		public object Convert (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is bool)  {
				if ((bool)value)
					return Color.Red;
				else {
					if (Device.OS == TargetPlatform.Android)
						return Color.White;
					else
						return Color.Black;
				}
				//return !(bool)value;
			}
			return Color.Red;
		}

		public object ConvertBack (object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException ();
		}

		#endregion
	}


}

