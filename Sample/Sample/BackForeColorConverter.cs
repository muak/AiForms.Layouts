using System;
using System.Globalization;
using Xamarin.Forms;

namespace Sample
{
	public class BackForeColorConverter:IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			Color backColor;
			if (value == null) {
				return Color.Default;
			}
			
			else if (value is Color) {
				backColor = (Color)value;
			}
			else if (value is string) {
				backColor = Color.FromHex((string)value);
			}
			else {
				return Color.Default;
			}

			if (Color.Default == backColor) {
				return Color.Default;
			}

			var lightness = backColor.Luminosity;
			Color foreColor;

			if (lightness <= 0.5d) {
				foreColor = backColor.WithLuminosity(0.8d);
			}
			else {
				foreColor = backColor.WithLuminosity(0.2d);
			}

			//色がどぎつい時
			if (backColor.Saturation >= 0.70d && backColor.Luminosity >= 0.35d && backColor.Luminosity <= 0.6d ) {
				foreColor = backColor.WithLuminosity(0.1d);
				foreColor = foreColor.WithSaturation(0.2d);
			}

			return foreColor;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}

