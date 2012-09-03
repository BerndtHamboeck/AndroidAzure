using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Dialog;
using System.Threading;
using System.Net;
using System.IO;
using System.Json;

namespace AndroidAzure
{
	[Activity (Label = "AndroidAzure", MainLauncher = true)]
	public class Activity1 : Activity
	{
		ProgressDialog _pd;

string kGetAllUrl = @"https://yoursubdomain.azure-mobile.net/tables/TodoItem?$filter=(complete%20eq%20false)";
string kAddUrl = @"https://yoursubdomain.azure-mobile.net/tables/TodoItem";
string kUpdateUrl = @"https://yoursubdomain.azure-mobile.net/tables/TodoItem/";
string kMobileServiceAppId = @"yourappkey";




		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			_pd =	ProgressDialog.Show (this, "Please wait...", "Querying Azure!");
         
			ThreadPool.QueueUserWorkItem (delegate {

				var root = BuildRoot (kGetAllUrl);
				
				RunOnUiThread (delegate {
					var da = new DialogAdapter(this, root);
		            var lv = new ListView(this) {Adapter = da};

		            SetContentView(lv);
					_pd.Dismiss ();
					_pd.Dispose ();
				});
					
			});
		}

		private RootElement BuildRoot (string url)
		{
			var root = new RootElement (String.Empty);
			var section = new Section("TodoItems");
			root.Add (section);
			var result = GetAzureResult (url);

			foreach (JsonObject item in result) {
				section.Add (new StringElement(item["text"]));
			}

			return root;

		}


		public JsonArray GetAzureResult (string url) {

			var request = (HttpWebRequest) WebRequest.Create (url);
			request.Method = WebRequestMethods.Http.Get;
			request.Accept = "application/json";
			request.Headers.Add("X-ZUMO-APPLICATION", kMobileServiceAppId);
			request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

			using (var response = (HttpWebResponse) request.GetResponse ()) {
				using (var streamReader = new StreamReader (response.GetResponseStream ())) {
					JsonValue root = JsonValue.Load (streamReader);
					return ((JsonArray) root);
				}
			}
		}

	}
}


