using System;
using Xamarin.Forms;

namespace TestNtp
{
	public class ListCell : ViewCell
	{
		public ListCell(){

			Label l = new Label ();
			l.SetBinding (Label.TextProperty,"DataOra");
			l.SetBinding (Label.TextColorProperty,"Timeout", BindingMode.Default, new BoolToColorConverter());
			View = l;
		}
	}

}

