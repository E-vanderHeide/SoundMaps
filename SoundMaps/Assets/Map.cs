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
		public const float gridResolution = 25000;
		private List<List<GridPoint>> grid;
		private Mesh mesh;

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
			GetComponent<MeshFilter>().mesh = mesh = new Mesh();
			mesh.name = "Procedural Grid";

			grid = new List<List<GridPoint>>();

			List<Vector3> vertices = new List<Vector3>();
			Vector2[] uv = new Vector2[(xSize + 1) * (zSize + 1)];
		
			for (int i = 0, z = 0; z <= zSize; z++)
			{
				List<GridPoint> row = new List<GridPoint>();
				for (int x = 0; x <= xSize; x++, i++)
				{
					Vector3 vector = new Vector3(x, 0, z);
                    row.Add(new GridPoint(vector));
					vertices.Add(vector);
					uv[i] = new Vector2(x / xSize, z / zSize);

				}
				
				grid.Add(row);
			}

			mesh.vertices = vertices.ToArray();
			mesh.uv = uv;
			int[] triangles = new int[xSize * zSize * 6];
			for (int ti = 0, vi = 0, y = 0; y < zSize; y++, vi++)
			{
				for (int x = 0; x < xSize; x++, ti += 6, vi++)
				{
					triangles[ti] = vi;
					triangles[ti + 3] = triangles[ti + 2] = vi + 1;
					triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
					triangles[ti + 5] = vi + xSize + 2;
				}
			}
			mesh.triangles = triangles;
			
			mesh.RecalculateNormals();
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
						if (nd.Attributes[0].Name == "k")
						{
							switch (nd.Attributes["k"].InnerText)
							{
								case "building":
									type = "Building";
									//TODO: add outer way test to make better buildings.
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
									if (nd.Attributes["v"].InnerText == "motorWay")
									{
										type = "MotorWay";
									}
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

			grid.Reverse();
			int index = 0;
			Color[] colors = new Color[mesh.vertices.Length];
			
			foreach (List<GridPoint> row in grid)
			{
				foreach (GridPoint point in row)
				{
					Color color = Color.grey;
					switch (point.type)
					{
						case "Building":
							color = Color.red;
							break;
						case "Road":
							color = Color.black;
							break;
						case "Nature":
							color = Color.green;
							break;
						case "Railway":
							color = Color.yellow;
							break;
						case "Motorway":
							color = Color.blue;
							break;
						default:
							break;
					}

					colors[index] = color;
					index++;
					//Gizmos.DrawSphere(transform.TransformPoint(point.location), 0.1f);
				}
			}
			mesh.colors = colors;
		}

		private void OnDrawGizmos()
		{
			if (grid == null)
			{
				return;
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
			int z = GetGridLocationY(longitude);
			GridPoint point = null;
          
			List<GridPoint> row = grid[z];
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
