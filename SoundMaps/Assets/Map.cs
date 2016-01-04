using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Xml;

namespace Assets
{
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class Map:MonoBehaviour
	{
		XmlDocument doc = new XmlDocument();
		public int xSize, zSize;
		public float minlat, minlon, maxlat, maxlon;
		public const float gridResolution = 20000;
		private List<List<GridPoint>> grid;

		public struct Way
		{
			public string id;
			public List<string> wnodes;
			public string type;
			public Way(string ID)
			{
				id = ID;
				wnodes = new List<string>();
				type = "";
			}
		}
		public Dictionary<string, Node> nodes = new Dictionary<string, Node>();

		private void Generate()
		{
			grid = new List<List<GridPoint>>();
			for (int i = 0, z = 0; z <= zSize; z++)
			{
				List<GridPoint> row = new List<GridPoint>();
				for (int x = 0; x <= xSize; x++, i++)
				{
					row.Add(new GridPoint(new Vector3(x,0, z)));
					
				}
				grid.Add(row);
			}
		}

		private void Awake()
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(new XmlTextReader("Maps/KTH2.osm"));
			//Grid
			XmlNode bounds = doc.GetElementsByTagName("bounds")[0];
			minlat = float.Parse(bounds.Attributes["minlat"].InnerText);
			minlon = float.Parse(bounds.Attributes["minlon"].InnerText);
			maxlat = float.Parse(bounds.Attributes["maxlat"].InnerText);
			maxlon = float.Parse(bounds.Attributes["maxlon"].InnerText);

			xSize = (int)(gridResolution * (maxlat - minlat));
			zSize = (int)(gridResolution * (maxlon - minlon));


			XmlNodeList elemList = doc.GetElementsByTagName("node");

			foreach (XmlNode attr in elemList)
			{
				nodes.Add(attr.Attributes["id"].InnerText, new Node(attr.Attributes["id"].InnerText, float.Parse(attr.Attributes["lat"].InnerText), float.Parse(attr.Attributes["lon"].InnerText)));
			}

			XmlNodeList wayList = doc.GetElementsByTagName("way");
			List<Way> ways = new List<Way>();
			foreach (XmlNode node in wayList)
			{

				XmlNodeList wayNodes = node.ChildNodes;
				Way wayNode = new Way(node.Attributes["id"].InnerText);

				string type = "";
				foreach (XmlNode nd in wayNodes)
				{
					if (nd.Attributes[0].Name == "ref")
					{
						wayNode.wnodes.Add(nd.Attributes["ref"].InnerText);
					}
					else
					{
						//Check if this group of waynodes form a building
						if (nd.Attributes[0].Name == "k")
						{
							switch (nd.Attributes["k"].InnerText)
							{
								case "building":
									type = "Building";
									break;
								case "natural":
									type = "Nature";
									break;
								case "leisure":
									if (nd.Attributes["v"].InnerText == "park")
									{
										type = "Nature";
									}
									break;
								case "landuse":
									if (nd.Attributes["v"].InnerText == "grass")
									{
										type = "Nature";
									}
									if (nd.Attributes["v"].InnerText == "railway")
									{
										type = "Railway";
									}
									break;
								case "highway":
									type = "Road";
									break;
								case "railway":
									type = "Railway";
									break;
								default:
									break;
							}
						}
					}
					
				}
				wayNode.type = type;
				ways.Add(wayNode);
			}

			Generate();
			AddType(ways);
		}

		private void OnDrawGizmos()
		{
			if (grid == null)
			{
				return;
			}
			
			
			foreach (List<GridPoint> row in grid)
			{
				foreach(GridPoint point in row)
				{
					switch(point.type)
					{
						case "Building":
							Gizmos.color = Color.red;
							break;
						case "Road":
							Gizmos.color = Color.gray;
							break;
						case "Nature":
							Gizmos.color = Color.green;
							break;
						case "Railway":
							Gizmos.color = Color.yellow;
							break;
						default:
							Gizmos.color = Color.white;
							break;
					}					
					Gizmos.DrawSphere(transform.TransformPoint(point.location), 0.5f);
				}
			}
		}

		public GridPoint GetGridPoint(float latitude, float longitude)
		{
		
			if (latitude > maxlat)
			{
				return null;
			}
			if(latitude<minlat)
			{
				return null;
			}
			if (longitude > maxlon)
			{
				return null;
			}
			if (longitude < minlon)
			{
				return null;
			}
			
			int x = GetGridLocationX(latitude);
			int y = GetGridLocationY(longitude);
			GridPoint point = null;
          
			List<GridPoint> row = grid[y];
			point = row[x];

			
			return point;
		}

		/// <summary>
		/// Returns grid coordinate based on gps coordinate
		/// </summary>
		/// <param name="gridSizeX">size of one grid</param>
		/// <param name="minlat">smallest latitud on map</param>
		/// <param name="coordinateX">GPS latitude</param>
		/// <returns></returns>
		public int GetGridLocationX(float coordinateX)
		{
			return (int)(gridResolution * (coordinateX - minlat));
		}

		/// <summary>
		/// Returns grid coordinate based on gps coordinate
		/// </summary>
		/// <param name="gridSizeX">size of one grid</param>
		/// <param name="minlon">smallest longitude on map</param>
		/// <param name="coordinateX">GPS latitude</param>
		/// <returns></returns>
		public int GetGridLocationY(float coordinateY)
		{
            return (int)(gridResolution * (coordinateY - minlon));
		}

		public void AddType(List<Way> waypoints)
		{
			foreach (Way track in waypoints)
			{
				
				GridPoint oldPoint = null;
				foreach (string nodeKey in track.wnodes)
				{
					Node node;
					GridPoint newPoint;
					if (nodes.TryGetValue(nodeKey, out node))
					{
						newPoint = GetGridPoint(node.lat, node.lon);
						if(newPoint != null)
						{
							newPoint.type = track.type;
							if (oldPoint != null)
							{
								float dX = Math.Abs(oldPoint.location.x - newPoint.location.x);
								float dZ = Math.Abs(oldPoint.location.z - newPoint.location.z);
								float tangent = dZ / dX;
								if (tangent > 0)
								{
									if (oldPoint.location.x > newPoint.location.x)
									{
										float z = oldPoint.location.z;
										for (int x = (int)oldPoint.location.x; x > newPoint.location.x; x--)
										{
											if (oldPoint.location.z > newPoint.location.z)
											{
												z = z - tangent;
											}
											else
											{
												z = z + tangent;
											}
											int zint = (int)Math.Round(z, 0);
											grid[zint][x].type = track.type;
										}
									}
									else
									{
										float z = oldPoint.location.z;
										for (int x = (int)oldPoint.location.x; x < newPoint.location.x; x++)
										{
											if (oldPoint.location.z > newPoint.location.z)
											{
												z = z - tangent;
											}
											else
											{
												z = z + tangent;
											}
											int zint = (int)Math.Round(z, 0);
											grid[zint][x].type = track.type;
										}
									}
								}
							}
							oldPoint = newPoint;
						}
					}
				}

			}
		}
	}

	public class GridPoint
	{
		public Vector3 location;

		public string type;

		public GridPoint(Vector3 vector)
		{
			location = vector;
			type = "";
		}

	}

	public class Node
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
}
