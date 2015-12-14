using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Net.HttpWebRequest;

namespace Objects
{
	class OSMLink
	{
		private const string URL = "http://api06.dev.openstreetmap.org/";
			
		public void ConnectToAPI()
		{
			//un = SoundMap
			//pw = DevSoundMap

			//
			HttpClient client = new HttpClient();
			client.BaseAddress = new Uri(URL);

			// Add an Accept header for JSON format.
			client.DefaultRequestHeaders.Accept.Add(
			new MediaTypeWithQualityHeaderValue("application/json"));

			// List data response.
			HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call!
			if (response.IsSuccessStatusCode)
			{
				// Parse the response body. Blocking!
				var dataObjects = response.Content.ReadAsAsync<IEnumerable<DataObject>>().Result;
				foreach (var d in dataObjects)
				{
					Console.WriteLine("{0}", d.Name);
				}
			}
			else
			{
				Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
			}



		}
	}
}
