using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class ShapeData
	{
		private MapPoint[] points;

		private ShapeSegment[] segments;

		private MapPoint minimumExtent = default(MapPoint);

		private MapPoint maximumExtent = default(MapPoint);

		private int largestSegmentIndex;

		private bool multiPolygonWithHoles;

		public MapPoint[] Points
		{
			get
			{
				return this.points;
			}
			set
			{
				this.points = value;
			}
		}

		public ShapeSegment[] Segments
		{
			get
			{
				return this.segments;
			}
			set
			{
				this.segments = value;
			}
		}

		public MapPoint MinimumExtent
		{
			get
			{
				return this.minimumExtent;
			}
			set
			{
				this.minimumExtent = value;
			}
		}

		public MapPoint MaximumExtent
		{
			get
			{
				return this.maximumExtent;
			}
			set
			{
				this.maximumExtent = value;
			}
		}

		internal int LargestSegmentIndex
		{
			get
			{
				return this.largestSegmentIndex;
			}
			set
			{
				this.largestSegmentIndex = value;
			}
		}

		internal bool MultiPolygonWithHoles
		{
			get
			{
				return this.multiPolygonWithHoles;
			}
			set
			{
				this.multiPolygonWithHoles = value;
			}
		}

		internal bool IsEmpty
		{
			get
			{
				if (this.Segments != null && this.Segments.Length != 0 && this.Points != null)
				{
					return this.Points.Length == 0;
				}
				return true;
			}
		}

		internal void UpdateStoredParameters()
		{
			if (this.Segments != null)
			{
				MapPoint mapPoint = new MapPoint(double.PositiveInfinity, double.PositiveInfinity);
				MapPoint mapPoint2 = new MapPoint(double.NegativeInfinity, double.NegativeInfinity);
				int num = 0;
				for (int i = 0; i < this.Segments.Length; i++)
				{
					MapPoint mapPoint3 = new MapPoint(double.PositiveInfinity, double.PositiveInfinity);
					MapPoint mapPoint4 = new MapPoint(double.NegativeInfinity, double.NegativeInfinity);
					double num2 = 0.0;
					double num3 = 0.0;
					MapPoint polygonCentroid = new MapPoint(0.0, 0.0);
					for (int j = 0; j < this.Segments[i].Length; j++)
					{
						int num4 = num - j;
						mapPoint3.X = Math.Min(mapPoint3.X, this.Points[num].X);
						mapPoint3.Y = Math.Min(mapPoint3.Y, this.Points[num].Y);
						mapPoint4.X = Math.Max(mapPoint4.X, this.Points[num].X);
						mapPoint4.Y = Math.Max(mapPoint4.Y, this.Points[num].Y);
						int num5 = (j + 1) % this.Segments[i].Length;
						num2 += this.Points[j + num4].X * this.Points[num5 + num4].Y;
						num2 -= this.Points[j + num4].Y * this.Points[num5 + num4].X;
						num3 = this.Points[j + num4].X * this.Points[num5 + num4].Y - this.Points[num5 + num4].X * this.Points[j + num4].Y;
						polygonCentroid.X += (this.Points[j + num4].X + this.Points[num5 + num4].X) * num3;
						polygonCentroid.Y += (this.Points[j + num4].Y + this.Points[num5 + num4].Y) * num3;
						num++;
					}
					num2 /= 2.0;
					this.Segments[i].PolygonSignedArea = num2;
					num2 *= 6.0;
					if (num2 != 0.0)
					{
						num3 = 1.0 / num2;
						polygonCentroid.X *= num3;
						polygonCentroid.Y *= num3;
					}
					else
					{
						polygonCentroid = mapPoint3;
						polygonCentroid.X += (mapPoint4.X - mapPoint3.X) / 2.0;
						polygonCentroid.Y += (mapPoint4.Y - mapPoint3.Y) / 2.0;
					}
					this.Segments[i].PolygonCentroid = polygonCentroid;
					this.Segments[i].MinimumExtent = mapPoint3;
					this.Segments[i].MaximumExtent = mapPoint4;
					mapPoint.X = Math.Min(mapPoint.X, mapPoint3.X);
					mapPoint.Y = Math.Min(mapPoint.Y, mapPoint3.Y);
					mapPoint2.X = Math.Max(mapPoint2.X, mapPoint4.X);
					mapPoint2.Y = Math.Max(mapPoint2.Y, mapPoint4.Y);
				}
				this.MinimumExtent = mapPoint;
				this.MaximumExtent = mapPoint2;
				this.UpdateLargestSegmentIndex();
				this.UpdateMultiPolygonWithHoles();
			}
		}

		private void UpdateLargestSegmentIndex()
		{
			double num = 1.7976931348623157E+308;
			this.LargestSegmentIndex = 0;
			for (int i = 0; i < this.Segments.Length; i++)
			{
				if (this.Segments[i].Type == SegmentType.Polygon)
				{
					double polygonSignedArea = this.Segments[i].PolygonSignedArea;
					if (polygonSignedArea < num)
					{
						num = polygonSignedArea;
						this.LargestSegmentIndex = i;
					}
				}
			}
		}

		private void UpdateMultiPolygonWithHoles()
		{
			int num = 0;
			int num2 = 0;
			ShapeSegment[] array = this.Segments;
			for (int i = 0; i < array.Length; i++)
			{
				ShapeSegment shapeSegment = array[i];
				if (shapeSegment.Type == SegmentType.Polygon)
				{
					if (shapeSegment.PolygonSignedArea <= 0.0)
					{
						num++;
					}
					else
					{
						num2++;
					}
				}
				else if (shapeSegment.Type == SegmentType.StartFigure)
				{
					this.MultiPolygonWithHoles = true;
					return;
				}
			}
			this.MultiPolygonWithHoles = (num > 1 && num2 > 0);
		}

		internal void LoadFromStream(Stream stream)
		{
			byte[] array = new byte[Marshal.SizeOf(typeof(byte))];
			byte[] array2 = new byte[Marshal.SizeOf(typeof(int))];
			byte[] array3 = new byte[Marshal.SizeOf(typeof(double))];
			stream.Read(array2, 0, array2.Length);
			int num = BitConverter.ToInt32(array2, 0);
			this.Segments = new ShapeSegment[num];
			for (int i = 0; i < num; i++)
			{
				stream.Read(array, 0, array.Length);
				this.Segments[i].Type = (SegmentType)array[0];
				stream.Read(array2, 0, array2.Length);
				this.Segments[i].Length = BitConverter.ToInt32(array2, 0);
			}
			stream.Read(array2, 0, array2.Length);
			int num2 = BitConverter.ToInt32(array2, 0);
			this.Points = new MapPoint[num2];
			for (int j = 0; j < num2; j++)
			{
				stream.Read(array3, 0, array3.Length);
				this.Points[j].X = BitConverter.ToDouble(array3, 0);
				stream.Read(array3, 0, array3.Length);
				this.Points[j].Y = BitConverter.ToDouble(array3, 0);
			}
		}

		internal void SaveToStream(Stream stream)
		{
			if (this.Segments != null && this.Points != null)
			{
				byte[] bytes = BitConverter.GetBytes(this.Segments.Length);
				stream.Write(bytes, 0, bytes.Length);
				ShapeSegment[] array = this.Segments;
				for (int i = 0; i < array.Length; i++)
				{
					ShapeSegment shapeSegment = array[i];
					bytes = new byte[Marshal.SizeOf(typeof(byte))];
					bytes[0] = (byte)shapeSegment.Type;
					stream.Write(bytes, 0, bytes.Length);
					bytes = BitConverter.GetBytes(shapeSegment.Length);
					stream.Write(bytes, 0, bytes.Length);
				}
				bytes = BitConverter.GetBytes(this.Points.Length);
				stream.Write(bytes, 0, bytes.Length);
				MapPoint[] array2 = this.Points;
				for (int j = 0; j < array2.Length; j++)
				{
					MapPoint mapPoint = array2[j];
					bytes = BitConverter.GetBytes(mapPoint.X);
					stream.Write(bytes, 0, bytes.Length);
					bytes = BitConverter.GetBytes(mapPoint.Y);
					stream.Write(bytes, 0, bytes.Length);
				}
			}
		}

		internal static string ShapeDataToString(ShapeData shapeData)
		{
			MemoryStream memoryStream = new MemoryStream();
			shapeData.SaveToStream(memoryStream);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			byte[] inArray = memoryStream.ToArray();
			memoryStream.Close();
			return Convert.ToBase64String(inArray);
		}

		internal static ShapeData ShapeDataFromString(string data)
		{
			byte[] array = new byte[1000];
			MemoryStream memoryStream = new MemoryStream();
			XmlTextReader xmlTextReader = new XmlTextReader(new StringReader("<base64>" + data + "</base64>"));
			xmlTextReader.Read();
			int num = 0;
			while ((num = xmlTextReader.ReadBase64(array, 0, 1000)) > 0)
			{
				memoryStream.Write(array, 0, num);
			}
			xmlTextReader.Read();
			memoryStream.Seek(0L, SeekOrigin.Begin);
			ShapeData shapeData = new ShapeData();
			shapeData.LoadFromStream(memoryStream);
			xmlTextReader.Close();
			memoryStream.Close();
			return shapeData;
		}

		internal double CalculateSignedPolygonArea(MapPoint[] polygon)
		{
			double num = 0.0;
			for (int i = 0; i < polygon.Length; i++)
			{
				int num2 = (i + 1) % polygon.Length;
				num += polygon[i].X * polygon[num2].Y;
				num -= polygon[i].Y * polygon[num2].X;
			}
			return num / 2.0;
		}

		internal MapPoint FindPolygonCentroid(MapPoint[] polygon)
		{
			double num = 0.0;
			double num2 = 0.0;
			double num3 = this.CalculateSignedPolygonArea(polygon);
			MapPoint result = default(MapPoint);
			double num4 = 0.0;
			for (int i = 0; i < polygon.Length; i++)
			{
				int num5 = (i + 1) % polygon.Length;
				num4 = polygon[i].X * polygon[num5].Y - polygon[num5].X * polygon[i].Y;
				num += (polygon[i].X + polygon[num5].X) * num4;
				num2 += (polygon[i].Y + polygon[num5].Y) * num4;
			}
			num3 *= 6.0;
			num4 = 1.0 / num3;
			num *= num4;
			num2 *= num4;
			result.X = num;
			result.Y = num2;
			return result;
		}
	}
}
