//
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class GeoUtils
	{
		private class PolygonCutter
		{
			private List<PolygonPart> m_polygonPartsList;

			private bool m_polygonIsClosed;

			private bool m_polygonIsCut;

			public void ProcessShapeSegment(ShapeSegment segment, IList<MapPoint> points, int firstPointIndex, PolygonClosingPole closingPole, out List<ShapeSegment> segments, out List<MapPoint> segmentPoints, out bool isClosedAtPole)
			{
				isClosedAtPole = false;
				segments = new List<ShapeSegment>();
				segmentPoints = new List<MapPoint>();
				this.Load(points, firstPointIndex, segment.Length, closingPole);
				foreach (PolygonPart shape in this.GetShapes())
				{
					ShapeSegment item = default(ShapeSegment);
					item.Type = segment.Type;
					item.Length = shape.Points.Count;
					segments.Add(item);
					segmentPoints.AddRange(shape.Points);
					isClosedAtPole |= shape.isClosedAtPole;
				}
			}

			public void Load(IList<MapPoint> points, int firstPointIndex, int segmentLength, PolygonClosingPole closingPole)
			{
				MapPoint mapPoint = points[firstPointIndex];
				MapPoint mapPoint2 = points[firstPointIndex + segmentLength - 1];
				this.m_polygonIsClosed = mapPoint.Equals(mapPoint2);
				this.m_polygonIsCut = false;
				this.m_polygonPartsList = new List<PolygonPart>();
				PolygonPart polygonPart = new PolygonPart(closingPole);
				MapPoint mapPoint3 = default(MapPoint);
				for (int i = 0; i < segmentLength; i++)
				{
					MapPoint mapPoint4 = points[firstPointIndex + i];
					if (i == 0)
					{
						polygonPart.Points.Add(mapPoint4);
					}
					else
					{
						MapPoint mapPoint5 = mapPoint3;
						foreach (MapPoint item in GeoUtils.DensifyLine(mapPoint3, mapPoint4, GeoUtils.DEFAULT_DENSIFICATION_ANGLE))
						{
							if (Math.Abs(item.X - mapPoint5.X) <= 180.0)
							{
								polygonPart.Points.Add(item);
								mapPoint5 = item;
							}
							else if (Math.Abs(item.X) == 180.0)
							{
								MapPoint mapPoint6 = new MapPoint(0.0 - item.X, item.Y);
								polygonPart.Points.Add(mapPoint6);
								mapPoint5 = mapPoint6;
							}
							else if (Math.Abs(mapPoint5.X) == 180.0 && polygonPart.Points.Count == 1)
							{
								mapPoint5.X = 0.0 - mapPoint5.X;
								polygonPart.Points[polygonPart.Points.Count - 1] = mapPoint5;
								polygonPart.Points.Add(item);
								mapPoint5 = item;
							}
							else
							{
								MapPoint mapPoint7 = default(MapPoint);
								MapPoint mapPoint8 = default(MapPoint);
								this.FindIntersectionPoints(mapPoint5, item, out mapPoint7, out mapPoint8);
								if (!mapPoint5.Equals(mapPoint7))
								{
									polygonPart.Points.Add(mapPoint7);
								}
								this.m_polygonPartsList.Add(polygonPart);
								polygonPart = new PolygonPart(closingPole);
								if (!item.Equals(mapPoint8))
								{
									polygonPart.Points.Add(mapPoint8);
								}
								polygonPart.Points.Add(item);
								this.m_polygonIsCut = true;
								mapPoint5 = item;
							}
						}
						mapPoint4 = mapPoint5;
					}
					mapPoint3 = mapPoint4;
				}
				if (this.m_polygonIsCut && this.m_polygonIsClosed && polygonPart.LastPoint.Equals(this.m_polygonPartsList[0].FirstPoint))
				{
					polygonPart.Points.RemoveAt(polygonPart.Points.Count - 1);
					this.m_polygonPartsList[0].Points.InsertRange(0, polygonPart.Points);
				}
				else
				{
					this.m_polygonPartsList.Add(polygonPart);
				}
				if (!this.m_polygonIsCut && this.m_polygonIsClosed && Math.Abs(polygonPart.FirstPoint.X) == 180.0 && polygonPart.FirstPoint.X == 0.0 - polygonPart.LastPoint.X)
				{
					this.m_polygonIsCut = true;
				}
			}

			public List<PolygonPart> GetPaths()
			{
				return this.m_polygonPartsList;
			}

			public List<PolygonPart> GetShapes()
			{
				if (!this.m_polygonIsCut)
				{
					return this.m_polygonPartsList;
				}
				List<PolygonPart> list = new List<PolygonPart>();
				if (!this.m_polygonIsClosed)
				{
					{
						foreach (PolygonPart polygonParts in this.m_polygonPartsList)
						{
							list.Add(polygonParts);
						}
						return list;
					}
				}
				if (this.m_polygonPartsList.Count > 1)
				{
					this.m_polygonPartsList.Sort(new PolygonPart.Comparer());
					int num;
					for (num = 0; num < this.m_polygonPartsList.Count; num++)
					{
						PolygonPart polygonPart = this.m_polygonPartsList[num];
						polygonPart.CollectKids(this.m_polygonPartsList);
						num = this.m_polygonPartsList.IndexOf(polygonPart);
					}
				}
				foreach (PolygonPart polygonParts2 in this.m_polygonPartsList)
				{
					polygonParts2.SaveToResultsWithChildren(list);
				}
				return list;
			}

			private void FindIntersectionPoints(MapPoint point1, MapPoint point2, out MapPoint intersectionPoint1, out MapPoint intersectionPoint2)
			{
				double num = (double)((point1.X > 0.0) ? 180 : (-180));
				MapPoint mapPoint = default(MapPoint);
				mapPoint.X = point2.X + num * 2.0;
				mapPoint.Y = point2.Y;
				double num2 = mapPoint.X - point1.X;
				double num3 = mapPoint.Y - point1.Y;
				intersectionPoint1 = default(MapPoint);
				intersectionPoint1.X = num;
				if (num2 != 0.0)
				{
					intersectionPoint1.Y = point1.Y + (num - point1.X) * num3 / num2;
				}
				else
				{
					intersectionPoint1.Y = point1.Y + num3 / 2.0;
				}
				intersectionPoint2 = default(MapPoint);
				intersectionPoint2.X = 0.0 - num;
				intersectionPoint2.Y = intersectionPoint1.Y;
			}
		}

		private enum PolygonClosingPole
		{
			North,
			South
		}

		private class PolygonPart
		{
			internal class Comparer : IComparer<PolygonPart>
			{
				public int Compare(PolygonPart p1, PolygonPart p2)
				{
					double topPointY = p1.TopPointY;
					double topPointY2 = p2.TopPointY;
					if (topPointY == topPointY2)
					{
						return 0;
					}
					if (topPointY < topPointY2)
					{
						return -1;
					}
					return 1;
				}
			}

			internal class ReverseComparer : IComparer<PolygonPart>
			{
				public int Compare(PolygonPart p1, PolygonPart p2)
				{
					double topPointY = p1.TopPointY;
					double topPointY2 = p2.TopPointY;
					if (topPointY == topPointY2)
					{
						return 0;
					}
					if (topPointY > topPointY2)
					{
						return -1;
					}
					return 1;
				}
			}

			public List<MapPoint> Points = new List<MapPoint>();

			public List<PolygonPart> Kids = new List<PolygonPart>();

			private PolygonClosingPole closingPole;

			internal bool isClosedAtPole;

			public virtual MapPoint FirstPoint
			{
				get
				{
					return this.Points[0];
				}
			}

			public virtual MapPoint LastPoint
			{
				get
				{
					return this.Points[this.Points.Count - 1];
				}
			}

			public bool InNorth
			{
				get
				{
					return this.FirstPoint.Y > 0.0;
				}
			}

			public bool InWest
			{
				get
				{
					if (this.FirstPoint.X != -180.0)
					{
						return this.LastPoint.X == -180.0;
					}
					return true;
				}
			}

			public bool InEast
			{
				get
				{
					if (this.FirstPoint.X != 180.0)
					{
						return this.LastPoint.X == 180.0;
					}
					return true;
				}
			}

			public double TopPointY
			{
				get
				{
					if (this.InNorth && this.FirstPoint.X != this.LastPoint.X)
					{
						return 90.0;
					}
					return Math.Max(this.FirstPoint.Y, this.LastPoint.Y);
				}
			}

			public double BottomPointY
			{
				get
				{
					if (!this.InNorth && this.FirstPoint.X != this.LastPoint.X)
					{
						return -90.0;
					}
					return Math.Min(this.FirstPoint.Y, this.LastPoint.Y);
				}
			}

			public PolygonPart(PolygonClosingPole closingPole)
			{
				this.closingPole = closingPole;
			}

			public void CollectKids(List<PolygonPart> list)
			{
				if (!this.IsComplete())
				{
					int num = 0;
					while (num < list.Count)
					{
						PolygonPart polygonPart = list[num];
						if (this != polygonPart && this.IsOnTheSameSide(polygonPart) && polygonPart.TopPointY < this.TopPointY && polygonPart.BottomPointY > this.BottomPointY)
						{
							this.Kids.Add(polygonPart);
							polygonPart.CollectKids(list);
							list.Remove(polygonPart);
						}
						else
						{
							num++;
						}
					}
				}
			}

			public bool IsOnTheSameSide(PolygonPart other)
			{
				if (this.InWest && other.InWest)
				{
					return true;
				}
				if (this.InEast)
				{
					return other.InEast;
				}
				return false;
			}

			public bool IsComplete()
			{
				return this.FirstPoint.Equals(this.LastPoint);
			}

			public void ConnectByMeridianOrAroundTheGlobe(MapPoint point1, MapPoint point2)
			{
				if (point1.X == 180.0 && point2.X == -180.0)
				{
					goto IL_0048;
				}
				if (point1.X == -180.0 && point2.X == 180.0)
				{
					goto IL_0048;
				}
				if (point1.X != 180.0 || point2.X != 180.0)
				{
					if (point1.X != -180.0)
					{
						return;
					}
					if (point2.X != -180.0)
					{
						return;
					}
				}
				this.AddVerticalMidPoints(point1, point2);
				return;
				IL_0048:
				this.isClosedAtPole = true;
				double y = (double)((this.closingPole == PolygonClosingPole.North) ? 90 : (-90));
				MapPoint mapPoint = new MapPoint(point1.X, y);
				MapPoint mapPoint2 = new MapPoint(point2.X, y);
				this.AddVerticalMidPoints(point1, mapPoint);
				this.Points.Add(mapPoint);
				this.AddHorizontalMidPoints(mapPoint, mapPoint2);
				this.Points.Add(mapPoint2);
				this.AddVerticalMidPoints(mapPoint2, point2);
			}

			public void AddHorizontalMidPoints(MapPoint point1, MapPoint point2)
			{
				if (point1.X < point2.X)
				{
					for (double num = Math.Floor(point1.X + 1.0); num < point2.X; num += 1.0)
					{
						this.Points.Add(new MapPoint(num, point1.Y));
					}
				}
				else
				{
					for (double num2 = Math.Ceiling(point1.X - 1.0); num2 > point2.X; num2 -= 1.0)
					{
						this.Points.Add(new MapPoint(num2, point1.Y));
					}
				}
			}

			public void AddVerticalMidPoints(MapPoint point1, MapPoint point2)
			{
				if (point1.Y < point2.Y)
				{
					for (double num = Math.Floor(point1.Y + 1.0); num < point2.Y; num += 1.0)
					{
						this.Points.Add(new MapPoint(point1.X, num));
					}
				}
				else
				{
					for (double num2 = Math.Ceiling(point1.Y - 1.0); num2 > point2.Y; num2 -= 1.0)
					{
						this.Points.Add(new MapPoint(point1.X, num2));
					}
				}
			}

			public void SaveToResultsWithChildren(List<PolygonPart> results)
			{
				results.Add(this);
				if (!this.IsComplete())
				{
					foreach (PolygonPart kid in this.Kids)
					{
						this.ConnectByMeridianOrAroundTheGlobe(this.LastPoint, kid.FirstPoint);
						this.Points.AddRange(kid.Points);
					}
					this.ConnectByMeridianOrAroundTheGlobe(this.LastPoint, this.FirstPoint);
					this.Points.Add(this.Points[0]);
				}
				foreach (PolygonPart kid2 in this.Kids)
				{
					foreach (PolygonPart kid3 in kid2.Kids)
					{
						kid3.SaveToResultsWithChildren(results);
					}
				}
			}

			public override string ToString()
			{
				string text = string.Format(CultureInfo.CurrentCulture, "{0} - {1}. ", this.FirstPoint, this.LastPoint);
				foreach (MapPoint point in this.Points)
				{
					text += point.ToString();
				}
				return text;
			}
		}

		internal class Vector3
		{
			public double x;

			public double y;

			public double z;

			public double Longitude
			{
				get
				{
					return Math.Atan2(this.y, this.x);
				}
			}

			public double LongitudeDeg
			{
				get
				{
					return GeoUtils.ToDegrees(this.Longitude);
				}
			}

			public double Latitude
			{
				get
				{
					return Math.Asin(this.z);
				}
			}

			public double LatitudeDeg
			{
				get
				{
					return GeoUtils.ToDegrees(this.Latitude);
				}
			}

			public Vector3(double x, double y, double z)
			{
				this.x = x;
				this.y = y;
				this.z = z;
			}

			public static Vector3 operator +(Vector3 a, Vector3 b)
			{
				return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
			}

			public static Vector3 operator -(Vector3 a, Vector3 b)
			{
				return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
			}

			public static Vector3 operator *(Vector3 vector, double a)
			{
				return new Vector3(vector.x * a, vector.y * a, vector.z * a);
			}

			public static Vector3 operator /(Vector3 v, double a)
			{
				return v * (1.0 / a);
			}

			public Vector3 Unitize()
			{
				return this / this.VectorLength();
			}

			public static double operator *(Vector3 a, Vector3 b)
			{
				return b.x * a.x + b.y * a.y + b.z * a.z;
			}

			public double LengthSquared()
			{
				return this * this;
			}

			public double VectorLength()
			{
				return Math.Sqrt(this.LengthSquared());
			}

			public double DistanceSquared(Vector3 a)
			{
				return (this - a) * (this - a);
			}

			public double Distance(Vector3 a)
			{
				return Math.Sqrt(this.DistanceSquared(a));
			}

			public Vector3 CrossProduct(Vector3 a)
			{
				return new Vector3(this.y * a.z - this.z * a.y, this.z * a.x - this.x * a.z, this.x * a.y - this.y * a.x);
			}

			public double Angle(Vector3 a)
			{
				return 2.0 * Math.Asin(this.Distance(a) / (2.0 * a.VectorLength()));
			}

			public double AngleInDegrees(Vector3 a)
			{
				return GeoUtils.ToDegrees(this.Angle(a));
			}

			public static Vector3 FromLatLong(double latitudeDeg, double longitudeDeg)
			{
				double num = GeoUtils.ToRadians(latitudeDeg);
				double num2 = GeoUtils.ToRadians(longitudeDeg);
				double num3 = Math.Cos(num);
				return new Vector3(num3 * Math.Cos(num2), num3 * Math.Sin(num2), Math.Sin(num));
			}
		}

		private static double DEFAULT_DENSIFICATION_ANGLE = 0.1;

		public static void CutShapes(ref MapPoint[] points, ref ShapeSegment[] segments)
		{
			List<ShapeSegment> list = new List<ShapeSegment>();
			List<MapPoint> list2 = new List<MapPoint>();
			PolygonCutter polygonCutter = new PolygonCutter();
			int num = 0;
			ShapeSegment[] array = segments;
			for (int i = 0; i < array.Length; i++)
			{
				ShapeSegment segment = array[i];
				if (segment.Length > 0)
				{
					bool flag = false;
					List<ShapeSegment> list3 = default(List<ShapeSegment>);
					List<MapPoint> list4 = default(List<MapPoint>);
					polygonCutter.ProcessShapeSegment(segment, (IList<MapPoint>)points, num, PolygonClosingPole.North, out list3, out list4, out flag);
					if (flag)
					{
						List<ShapeSegment> list5 = default(List<ShapeSegment>);
						List<MapPoint> list6 = default(List<MapPoint>);
						polygonCutter.ProcessShapeSegment(segment, (IList<MapPoint>)points, num, PolygonClosingPole.South, out list5, out list6, out flag);
						ShapeSegment[] array2 = list3.ToArray();
						MapPoint[] array3 = list4.ToArray();
						GeoUtils.CalculateSignedArea(ref array3, ref array2);
						double num2 = 0.0;
						for (int j = 0; j < array2.Length; j++)
						{
							num2 += Math.Abs(array2[j].PolygonSignedArea);
						}
						ShapeSegment[] array4 = list5.ToArray();
						MapPoint[] array5 = list6.ToArray();
						GeoUtils.CalculateSignedArea(ref array5, ref array4);
						double num3 = 0.0;
						for (int k = 0; k < array4.Length; k++)
						{
							num3 += Math.Abs(array4[k].PolygonSignedArea);
						}
						if (num2 < num3)
						{
							list.AddRange(list3);
							list2.AddRange(list4);
						}
						else
						{
							list.AddRange(list5);
							list2.AddRange(list6);
						}
					}
					else
					{
						list.AddRange(list3);
						list2.AddRange(list4);
					}
				}
				num += segment.Length;
			}
			segments = list.ToArray();
			points = list2.ToArray();
		}

		public static void CutPaths(ref MapPoint[] points, ref PathSegment[] segments)
		{
			List<PathSegment> list = new List<PathSegment>();
			List<MapPoint> list2 = new List<MapPoint>();
			PolygonCutter polygonCutter = new PolygonCutter();
			int num = 0;
			PathSegment[] array = segments;
			for (int i = 0; i < array.Length; i++)
			{
				PathSegment pathSegment = array[i];
				if (pathSegment.Length > 0)
				{
					polygonCutter.Load(points, num, pathSegment.Length, PolygonClosingPole.North);
					foreach (PolygonPart path in polygonCutter.GetPaths())
					{
						PathSegment item = default(PathSegment);
						item.Type = pathSegment.Type;
						item.Length = path.Points.Count;
						list.Add(item);
						list2.AddRange(path.Points);
					}
				}
				num += pathSegment.Length;
			}
			segments = list.ToArray();
			points = list2.ToArray();
		}

		public static IEnumerable<MapPoint> DensifyLine(MapPoint prevPoint, MapPoint point, double maxAngle)
		{
			double dx = Math.Abs(point.X - prevPoint.X);
			double dy = Math.Abs(point.Y - prevPoint.Y);
			if (dx + dy > maxAngle)
			{
				double maxAngleRad = GeoUtils.ToRadians(maxAngle);
				Vector3 startPoint = Vector3.FromLatLong(prevPoint.Y, prevPoint.X);
				Vector3 endPoint = Vector3.FromLatLong(point.Y, point.X);
				double angle = endPoint.Angle(startPoint);
				if (angle > maxAngleRad)
				{
					Vector3 zAxis = (startPoint + endPoint).CrossProduct(startPoint - endPoint).Unitize();
					Vector3 yAxis = startPoint.CrossProduct(zAxis);
					int count = Convert.ToInt32(Math.Ceiling(angle / maxAngleRad));
					double exactAngle = angle / (double)count;
					double cosine = Math.Cos(exactAngle);
					double sine = Math.Sin(exactAngle);
					double x = cosine;
					double y = sine;
					for (int i = 0; i < count - 1; i++)
					{
						Vector3 newPoint = (startPoint * x + yAxis * y).Unitize();
						yield return new MapPoint(newPoint.LongitudeDeg, newPoint.LatitudeDeg);
						double r = x * cosine - y * sine;
						y = x * sine + y * cosine;
						x = r;
					}
				}
			}
			yield return point;
		}

		public static double ToRadians(double a)
		{
			return a / 180.0 * 3.1415926535897931;
		}

		public static double ToDegrees(double a)
		{
			return a * 180.0 / 3.1415926535897931;
		}

		public static void NormalizePointsLongigute(ref MapPoint[] points)
		{
			for (int i = 0; i < points.Length; i++)
			{
				if (points[i].X < -180.0)
				{
					points[i].X = (points[i].X - 180.0) % 360.0 + 180.0;
				}
				else if (points[i].X >= 180.0)
				{
					points[i].X = (points[i].X + 180.0) % 360.0 - 180.0;
				}
			}
		}

		internal static void CalculateSignedArea(ref MapPoint[] points, ref ShapeSegment[] segments)
		{
			int num = 0;
			for (int i = 0; i < segments.Length; i++)
			{
				segments[i].PolygonSignedArea = 0.0;
				for (int j = 0; j < segments[i].Length; j++)
				{
					int num2 = num - j;
					int num3 = (j + 1) % segments[i].Length;
					segments[i].PolygonSignedArea += points[j + num2].X * points[num3 + num2].Y;
					segments[i].PolygonSignedArea -= points[j + num2].Y * points[num3 + num2].X;
					num++;
				}
				segments[i].PolygonSignedArea /= 2.0;
			}
		}

		internal static void FixOrientationForGeometry(ref MapPoint[] points, ref ShapeSegment[] segments)
		{
			GeoUtils.CalculateSignedArea(ref points, ref segments);
			int i = 0;
			for (int j = 0; j < segments.Length; i += segments[j].Length, j++)
			{
				if (j == 0 && segments[j].PolygonSignedArea >= 0.0)
				{
					goto IL_0049;
				}
				if (j != 0 && segments[j].PolygonSignedArea < 0.0)
				{
					goto IL_0049;
				}
				continue;
				IL_0049:
				for (int k = i; k < i + segments[j].Length / 2; k++)
				{
					MapPoint mapPoint = points[k];
					points[k] = points[segments[j].Length + i - (k - i) - 1];
					points[segments[j].Length + i - (k - i) - 1] = mapPoint;
				}
			}
		}

		internal static void MoveLargestSegmentToFront(ref MapPoint[] points, ref ShapeSegment[] segments)
		{
			GeoUtils.CalculateSignedArea(ref points, ref segments);
			double num = 0.0;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < segments.Length; i++)
			{
				if (segments[i].PolygonSignedArea < num)
				{
					num = segments[i].PolygonSignedArea;
					num2 = i;
				}
			}
			if (num2 != 0)
			{
				List<MapPoint> list = new List<MapPoint>(points);
				num3 = 0;
				for (int j = 0; j < num2; j++)
				{
					num3 += segments[j].Length;
				}
				List<MapPoint> range = list.GetRange(num3, segments[num2].Length);
				list.RemoveRange(num3, segments[num2].Length);
				list.InsertRange(0, range);
				points = list.ToArray();
				ShapeSegment shapeSegment = segments[0];
				segments[0] = segments[num2];
				segments[num2] = shapeSegment;
			}
		}
        /*
		internal static SqlGeometry FlattenGeometry(SqlGeometry geometry)
		{
			List<SqlGeometry> list = new List<SqlGeometry>();
			GeoUtils.FlattenGeometryRec(geometry, list);
			if (list.Count > 1)
			{
				for (int i = 1; i < list.Count; i++)
				{
					list[0] = list[0].STUnion(list[i]);
				}
			}
			return list[0];
		}

		private static void FlattenGeometryRec(SqlGeometry geometry, List<SqlGeometry> basicGeometries)
		{
			string value = geometry.STGeometryType().Value;
			if (value == "GeometryCollection")
			{
				for (int i = 1; i <= geometry.STNumGeometries().Value; i++)
				{
					GeoUtils.FlattenGeometryRec(geometry.STGeometryN(i), basicGeometries);
				}
			}
			else
			{
				basicGeometries.Add(geometry);
			}
		}

		internal static SqlGeography FlattenGeography(SqlGeography geography)
		{
			List<SqlGeography> list = new List<SqlGeography>();
			GeoUtils.FlattenGeographyRec(geography, list);
			if (list.Count > 1)
			{
				for (int i = 1; i < list.Count; i++)
				{
					list[0] = list[0].STUnion(list[i]);
				}
			}
			return list[0];
		}

		private static void FlattenGeographyRec(SqlGeography geography, List<SqlGeography> basicGeographies)
		{
			string value = geography.STGeometryType().Value;
			if (value == "GeometryCollection")
			{
				for (int i = 1; i <= geography.STNumGeometries().Value; i++)
				{
					GeoUtils.FlattenGeographyRec(geography.STGeometryN(i), basicGeographies);
				}
			}
			else
			{
				basicGeographies.Add(geography);
			}
		}
        */
	}
}
