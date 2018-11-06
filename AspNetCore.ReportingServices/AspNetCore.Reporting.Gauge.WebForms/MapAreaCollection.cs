using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[SRDescription("DescriptionAttributeMapAreaCollection_MapAreaCollection")]
	internal class MapAreaCollection : IList, ICollection, IEnumerable
	{
		internal ArrayList array = new ArrayList();

		public MapArea this[int index]
		{
			get
			{
				return (MapArea)this.array[index];
			}
			set
			{
				this.array[index] = value;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this.array[index];
			}
			set
			{
				this.array[index] = value;
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return this.array.IsFixedSize;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.array.IsReadOnly;
			}
		}

		public int Count
		{
			get
			{
				return this.array.Count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return this.array.IsSynchronized;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this.array.SyncRoot;
			}
		}

		public void Clear()
		{
			this.array.Clear();
		}

		public bool Contains(object value)
		{
			return this.array.Contains(value);
		}

		public int IndexOf(object value)
		{
			return this.array.IndexOf(value);
		}

		public void Remove(object value)
		{
			this.array.Remove(value);
		}

		public void RemoveAt(int index)
		{
			this.array.RemoveAt(index);
		}

		public int Add(object value)
		{
			if (!(value is MapArea))
			{
				throw new ArgumentException(Utils.SRGetStr("ExceptionImagemapInvalidObject"));
			}
			return this.array.Add(value);
		}

		public void Insert(int index, MapArea value)
		{
			this.Insert(index, (object)value);
		}

		public void Insert(int index, object value)
		{
			if (!(value is MapArea))
			{
				throw new ArgumentException(Utils.SRGetStr("ExceptionImagemapInvalidObject"));
			}
			this.array.Insert(index, value);
		}

		public void CopyTo(Array array, int index)
		{
			array.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return this.array.GetEnumerator();
		}

		public int Add(string toolTip, string href, string attr, GraphicsPath path, object tag)
		{
			int num = this.Add(toolTip, href, attr, path);
			if (num >= 0)
			{
				MapArea mapArea = this[num];
				mapArea.Tag = tag;
			}
			return num;
		}

		public int Add(string toolTip, string href, string attr, GraphicsPath path)
		{
			if (path.PointCount > 0)
			{
				path.Flatten();
				PointF[] pathPoints = path.PathPoints;
				float[] array = new float[pathPoints.Length * 2];
				int num = 0;
				PointF[] array2 = pathPoints;
				for (int i = 0; i < array2.Length; i++)
				{
					PointF pointF = array2[i];
					array[num++] = pointF.X;
					array[num++] = pointF.Y;
				}
				return this.Add(MapAreaShape.Polygon, toolTip, href, attr, array);
			}
			return -1;
		}

		public int Add(string toolTip, string href, string attr, RectangleF rect)
		{
			return this.Add(MapAreaShape.Rectangle, toolTip, href, attr, new float[4]
			{
				rect.X,
				rect.Y,
				rect.Right,
				rect.Bottom
			});
		}

		public int Add(MapAreaShape shape, string toolTip, string href, string attr, float[] coordinates)
		{
			if (shape == MapAreaShape.Circle && coordinates.Length != 3)
			{
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionImagemapInvalidCircle"));
			}
			if (shape == MapAreaShape.Rectangle && coordinates.Length != 4)
			{
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionImagemapInvalidRectangle"));
			}
			if (shape == MapAreaShape.Polygon && (float)coordinates.Length % 2.0 != 0.0)
			{
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionImagemapInvalidPolygon"));
			}
			MapArea mapArea = new MapArea();
			mapArea.Custom = false;
			mapArea.ToolTip = toolTip;
			mapArea.Href = href;
			mapArea.MapAreaAttributes = attr;
			mapArea.Shape = shape;
			mapArea.Coordinates = new float[coordinates.Length];
			coordinates.CopyTo(mapArea.Coordinates, 0);
			return this.Add(mapArea);
		}

		public void Insert(int index, string toolTip, string href, string attr, GraphicsPath path)
		{
			if (path.PointCount > 0)
			{
				path.Flatten();
				PointF[] pathPoints = path.PathPoints;
				float[] array = new float[pathPoints.Length * 2];
				int num = 0;
				PointF[] array2 = pathPoints;
				for (int i = 0; i < array2.Length; i++)
				{
					PointF pointF = array2[i];
					array[num++] = pointF.X;
					array[num++] = pointF.Y;
				}
				this.Insert(index, MapAreaShape.Polygon, toolTip, href, attr, array);
			}
		}

		public void Insert(int index, string toolTip, string href, string attr, RectangleF rect)
		{
			this.Insert(index, MapAreaShape.Rectangle, toolTip, href, attr, new float[4]
			{
				rect.X,
				rect.Y,
				rect.Right,
				rect.Bottom
			});
		}

		public void Insert(int index, MapAreaShape shape, string toolTip, string href, string attr, float[] coordinates)
		{
			if (shape == MapAreaShape.Circle && coordinates.Length != 3)
			{
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionImagemapInvalidCircle"));
			}
			if (shape == MapAreaShape.Rectangle && coordinates.Length != 4)
			{
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionImagemapInvalidRectangle"));
			}
			if (shape == MapAreaShape.Polygon && (float)coordinates.Length % 2.0 != 0.0)
			{
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionImagemapInvalidPolygon"));
			}
			MapArea mapArea = new MapArea();
			mapArea.Custom = false;
			mapArea.ToolTip = toolTip;
			mapArea.Href = href;
			mapArea.MapAreaAttributes = attr;
			mapArea.Shape = shape;
			mapArea.Coordinates = new float[coordinates.Length];
			coordinates.CopyTo(mapArea.Coordinates, 0);
			this.Insert(index, mapArea);
		}

		public int Add(MapArea value)
		{
			return this.array.Add(value);
		}

		public void Remove(MapArea value)
		{
			this.array.Remove(value);
		}

		public bool Contains(MapArea value)
		{
			return this.array.Contains(value);
		}

		public int IndexOf(MapArea value)
		{
			return this.array.IndexOf(value);
		}

		internal void RemoveNonCustom()
		{
			for (int i = 0; i < this.Count; i++)
			{
				if (!this[i].Custom)
				{
					this.RemoveAt(i);
					i--;
				}
			}
		}
	}
}
