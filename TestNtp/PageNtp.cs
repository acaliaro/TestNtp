using System;
using Xamarin.Forms;
using System.Linq;
using System.Collections.ObjectModel;

namespace TestNtp
{
	public class PageNtp : ContentPage
	{
		Label lOk = new Label ();
		Label lBad = new Label ();

		public PageNtp ()
		{

			this.BindingContext = App.Lista;

			StackLayout sl = new StackLayout ();
			sl.Padding = new Thickness (0, 20, 0, 0);
			ListView lv = new ListView ();
			lv.ItemsSource = App.Lista;
			lv.ItemTemplate = new DataTemplate (typeof(ListCell));
			sl.Children.Add (lv);
			sl.Children.Add (lOk);
			sl.Children.Add (lBad);
			Content = sl;
		}

		protected override void OnAppearing ()
		{
			MessagingCenter.Subscribe<App, ObservableCollection<Model>> (this, "Lista", (sender, arg) => Device.BeginInvokeOnMainThread (() => {
				lOk.Text = "Ok " + App.Lista.Count (o => o.Timeout == false);
				lBad.Text = "Bad " + App.Lista.Count (o => o.Timeout == true);
			}));
			base.OnAppearing ();
		}

		protected override void OnDisappearing ()
		{
			MessagingCenter.Unsubscribe<App, ObservableCollection<Model>> (this, "Lista");
			base.OnDisappearing ();
		}
	}
}

