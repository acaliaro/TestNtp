using System;

using Xamarin.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Linq;

namespace TestNtp
{
	public class App : Application
	{
		public static CancellationTokenSource CancellationToken { get; set; }
		public static ObservableCollection<Model> Lista { get; set; }
//		public static int ListaBad {
//			get { 
//				if (Lista != null)
//					return Lista.Where (o => o.Timeout == true).Count ();
//			}
//		}

		public App ()
		{
			App.Lista = new ObservableCollection<Model> ();

			MainPage = new PageNtp ();
			return;

		}

		private async  Task timer(){

			bool first = true;


			App.CancellationToken = new CancellationTokenSource ();
			while (!App.CancellationToken.IsCancellationRequested)
			{
				try
				{

					App.CancellationToken.Token.ThrowIfCancellationRequested();
					await Task.Delay(10000, App.CancellationToken.Token).ContinueWith(async (arg) =>{

						if (!App.CancellationToken.Token.IsCancellationRequested) {
							App.CancellationToken.Token.ThrowIfCancellationRequested();
							/*
							 * HERE YOU CAN DO YOUR ACTION
							 */
							DateTime dt = DateTime.Now;
							Model m = new Model();

							try {
								
								dt = await NtpClient.GetNetworkTimeAsync ();
								m.DataOra = dt;
								m.Timeout = false;
							}
							catch{
								m.DataOra = dt;
								m.Timeout = true;
							}
							finally{
								Lista.Insert (0,m);
								MessagingCenter.Send<App, ObservableCollection<Model>> (this, "Lista", Lista);
							}
								

//							App.UpdateDateTime();
							//Device.BeginInvokeOnMainThread(()=> _label.Text = (++_counter).ToString());
							Debug.WriteLine ("TimerRunning ");// + watch.Elapsed.ToString ());
						}
					});
					//Debug.WriteLine (DateTime.Now.ToLocalTime ().ToString () + " DELAY: " + delay);

				}
				catch (Exception ex)
				{
					Debug.WriteLine ("EX 1: " + ex.Message);
				}

			}
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
			Task.Run(async()=> timer ());
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
			if (App.CancellationToken != null) {
				App.CancellationToken.Cancel ();
				//				App.CancellationToken = null;
			}
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
			Task.Run (async() => timer ());
		}
	}
}

