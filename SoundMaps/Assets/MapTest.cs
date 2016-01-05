using System.Xml;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//Latitude = Horizontal
//Longitude = Vertical
using System;
using AssemblyCSharp;

public class Map : MonoBehaviour {
	
	XmlDocument doc = new XmlDocument();
	List<Transform> mapObjects = new List<Transform>();
	//public Node n;
	public float x;
	public float y;
	public float boundsX= 59;
	public float boundsY=18;
	
	public struct Node
	{
		
		public string id;
		public float lat, lon;
		
		public Node(string ID, float LAT, float LON)
		{
			id = ID;
			lat = LAT;
			lon = LON;
		}
	}
	
	
	public struct Way
	{
		public string id;
		public List<string> wnodes;
		
		public Way(string ID)
		{
			id = ID;
			wnodes = new List<string>();
		}
	}
	
	public Dictionary<string, Node> nodes = new Dictionary<string, Node>();
	public List<Way> roads = new List<Way>();

	public List<Way> buildings = new List<Way> ();
	
	/// <summary>
	/// Returns grid coordinate based on gps coordinate
	/// </summary>
	/// <param name="gridSizeX">size of one grid</param>
	/// <param name="minlat">smallest latitud on map</param>
	/// <param name="coordinateX">GPS latitude</param>
	/// <returns></returns>
	public int GetGridLocationX(int gridSizeX, int minlat, int coordinateX)
	{
		int locx =0;
		locx = (coordinateX - minlat) / gridSizeX;

		return locx;
	}

	/// <summary>
	/// Returns grid coordinate based on gps coordinate
	/// </summary>
	/// <param name="gridSizeX">size of one grid</param>
	/// <param name="minlon">smallest longitude on map</param>
	/// <param name="coordinateX">GPS latitude</param>
	/// <returns></returns>
	public int GetGridLocationY(int gridSizeY, int minlon, int coordinateY)
	{
		int locy = 0;
		locy = (coordinateY - minlon) / gridSizeY;

		return locy;
	}

	void Start () 
	{
		XmlDocument doc = new XmlDocument();
		doc.Load(new XmlTextReader("Maps/KTH2.osm"));
		//Grid
		XmlNode bounds = doc.GetElementsByTagName("bounds")[0];
		int minlat = int.Parse(bounds.Attributes["minlat"].InnerText);
		int minlon = int.Parse(bounds.Attributes["minlon"].InnerText);
		int maxlat = int.Parse(bounds.Attributes["maxlat"].InnerText);
		int maxlon = int.Parse(bounds.Attributes["maxlon"].InnerText);
		int gridResolution = 1000;

		int gridSizeX = (maxlat - minlat)/ 1000;
		int gridSizeY = (maxlon - minlon) / 1000;


	

		XmlNodeList elemList = doc.GetElementsByTagName("node");

		foreach (XmlNode attr in elemList)
		{
			nodes.Add(attr.Attributes["id"].InnerText, new Node(attr.Attributes["id"].InnerText, float.Parse(attr.Attributes["lat"].InnerText), float.Parse(attr.Attributes["lon"].InnerText)));
		}
		
		XmlNodeList wayList = doc.GetElementsByTagName("way");
		int ct = 0;
		foreach (XmlNode node in wayList)
		{
			
			XmlNodeList wayNodes = node.ChildNodes;
			Way wayNode = new Way(node.Attributes["id"].InnerText);

			bool building = false;
			foreach (XmlNode nd in wayNodes)
			{
				if (nd.Attributes[0].Name == "ref")
				{
					wayNode.wnodes.Add(nd.Attributes["ref"].InnerText);
				}
				//Check if this group of waynodes form a building
				if(nd.Attributes[0].Name == "k")
				{
					if(nd.Attributes["k"].InnerText == "building")
					{
						building = true;
					}
				}
			}
			if(!building)
			{
				roads.Add(wayNode);
			}
			else
			{
				buildings.Add(wayNode);
			}

			ct++;
		}
////		//draw roads
	
//		for (int i = 0; i < roads.Count; i++) 
//		{
//			GameObject gameObject = new GameObject("wayObject"+ roads[i].id);
//			gameObject.AddComponent<LineRenderer>();
//			gameObject.GetComponent<LineRenderer>().SetWidth(0.005f,0.005f);
//			gameObject.GetComponent<LineRenderer>().SetVertexCount(roads[i].wnodes.Count);
//			gameObject.AddComponent<MapObject>();

//			mapObjects.Add(gameObject.transform);

//			for (int j = 0; j < roads[i].wnodes.Count; j++)
//			{
//				Node node = new Node();
//				nodes.TryGetValue((string)roads[i].wnodes[j], out node);
//				Debug.Log("MATCH!");
//				x = node.lon;
//				y = node.lat;

//				gameObject.GetComponent<MapObject>().longitude = node.lon;
//				gameObject.GetComponent<MapObject>().latitude = node.lat;
//				mapObjects[i].GetComponent<LineRenderer>().SetPosition(j,new Vector3((x)*1000,0,(y)*1000));
//			}

//		}
	}
}
