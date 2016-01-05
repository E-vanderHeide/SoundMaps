#define DEBUG
using UnityEngine;
using System.Collections;
using System.Net;
using System.IO;
using System;
using System.Text;
using System.Collections.Generic;
using OAuth;
using System.Xml;
using System.Xml.Serialization;

public class TerrainScript : MonoBehaviour {

	private const string URL = "http://api06.dev.openstreetmap.org/api/0.6/";


	private const string RequestTokenURL = "http://api06.dev.openstreetmap.org/oauth/request_token";
	private const string AccessTokenURL = "http://api06.dev.openstreetmap.org/oauth/access_token";
	private const string AuthorizeURL = "http://api06.dev.openstreetmap.org/oauth/authorize?oauth_token=";

	private const string ConsumerKey = "wM2JdDYX8yjwborxGwHGVGZPqUteOIrvsqtz0ZRV";
	private const string ConsumerSecret = "KkgbnkBClUXRpsRbBflMCi2UFQ9RYcYKQW6XisxJ";

	private const string UserName = "Ewoud@kth.se";
	private const string Password = "SoundMaps";

	private OAuth.Manager _oauth;
	private OAuth.Manager oauth
	{
		get
		{
			if (_oauth == null)
			{
				_oauth = new OAuth.Manager();
				_oauth["consumer_key"] = ConsumerKey;
				_oauth["consumer_secret"] = ConsumerSecret;
			}
			return _oauth;
		}
	}


	// Use this for initialization
	void Start () {
		OAuthResponse accessToken = null;
		try {
			accessToken = DeSerializeObject<OAuthResponse> ("AccessToken.xml");
		} catch (Exception e) {
			Console.WriteLine (e.Message);
		}

		if (accessToken == null) {
			OAuthResponse requestToken = oauth.AcquireRequestToken (RequestTokenURL, "POST");
			
			Application.OpenURL (AuthorizeURL + oauth ["token"]);
			
			string pin = string.Empty;
			Debug.Break ();
			accessToken = oauth.AcquireAccessToken (AccessTokenURL, "POST", pin);
			SerializeObject (accessToken, "AccessToken.xml");
		}

		float latitude = 59.42f;
		float longitude = 17.96f;

		string call = string.Format ("map?bbox={0},{1},{2},{3}", 17.8, 59, 18.3, 59.5); //longitude, latitude, longitude + 0.05, latitude + 0.05);

		Uri uri = new Uri (URL + call);
		string header = oauth.GenerateAuthzHeader (uri.ToString (), "GET");

		if (uri == null) {
			throw new ArgumentNullException ("RequestUri");
		}
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create (uri);
		request.Method = "GET";
		
		request.Headers ["Authorization"] = header;


		HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
		Stream receiveStream = response.GetResponseStream ();


		StreamReader oReader = new StreamReader (receiveStream, Encoding.ASCII);
		
		StreamWriter oWriter = new StreamWriter (string.Format ("Maps/Map-1412.osm", DateTime.Now.ToString ("ddMM-HH:mm")));
		oWriter.Write (oReader.ReadToEnd ());
		
		oWriter.Close ();
		oReader.Close ();
		response.Close ();

	}

	// Update is called once per frame
	void Update () {
	}

	/// <summary>
	/// Serializes an object.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="serializableObject"></param>
	/// <param name="fileName"></param>
	public void SerializeObject<T>(T serializableObject, string fileName)
	{
		if (serializableObject == null) { return; }
		
		try
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
			using (MemoryStream stream = new MemoryStream())
			{
				serializer.Serialize(stream, serializableObject);
				stream.Position = 0;
				xmlDocument.Load(stream);
				xmlDocument.Save(fileName);
				stream.Close();
			}
		}
		catch (Exception ex)
		{
			throw(ex);
		}
	}
	
	
	/// <summary>
	/// Deserializes an xml file into an object list
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="fileName"></param>
	/// <returns></returns>
	public T DeSerializeObject<T>(string fileName)
	{
		if (string.IsNullOrEmpty(fileName)) { return default(T); }
		
		T objectOut = default(T);
		
		try
		{
			string attributeXml = string.Empty;
			
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(fileName);
			string xmlString = xmlDocument.OuterXml;
			
			using (StringReader read = new StringReader(xmlString))
			{
				Type outType = typeof(T);
				
				XmlSerializer serializer = new XmlSerializer(outType);
				using (XmlReader reader = new XmlTextReader(read))
				{
					objectOut = (T)serializer.Deserialize(reader);
					reader.Close();
				}
				
				read.Close();
			}
		}
		catch (Exception ex)
		{
			throw(ex);
		}
		
		return objectOut;
	}
}
