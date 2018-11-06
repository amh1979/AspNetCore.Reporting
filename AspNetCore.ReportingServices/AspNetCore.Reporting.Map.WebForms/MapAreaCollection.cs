using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Map.WebForms
{
	[Description("Map areas collection.")]
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
				throw new ArgumentException("Object to be added is not a MapArea object.");
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
				throw new ArgumentException("Object to be inserted is not a MapArea object.");
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
				((IMapAreaAttributes)mapArea).Tag = tag;
			}
			return num;
		}

		public int Add(string toolTip, string href, string attr, GraphicsPath path)
		{
			PointF[] pathPoints = path.PathPoints;
			if (pathPoints.Length > 0)
			{
				int[] array = new int[pathPoints.Length * 2];
				int num = 0;
				PointF[] array2 = pathPoints;
				for (int i = 0; i < array2.Length; i++)
				{
					PointF pointF = array2[i];
					array[num++] = (int)Math.Round((double)pointF.X);
					array[num++] = (int)Math.Round((double)pointF.Y);
				}
				return this.Add(MapAreaShape.Polygon, toolTip, href, attr, array);
			}
			return -1;
		}

		public int Add(string toolTip, string href, string attr, Rectangle rect)
		{
			return this.Add(MapAreaShape.Rectangle, toolTip, href, attr, new int[4]
			{
				rect.X,
				rect.Y,
				rect.Right,
				rect.Bottom
			});
		}

		public int Add(MapAreaShape shape, string toolTip, string href, string attr, int[] coord)
		{
			if (shape == MapAreaShape.Circle && coord.Length != 3)
			{
				throw new InvalidOperationException("Invalid number of coordinates specified for the circle map area shape. Three coordinates must be provided: center of the circle (x,y) followed by the radius.");
			}
			if (shape == MapAreaShape.Rectangle && coord.Length != 4)
			{
				throw new InvalidOperationException("Invalid number of coordinates specified for the rectangle map area shape. Four coordinates must be provided: x,y coordinates for the upper-left corner, followed by x,y coordinates for the bottom-right corner.");
			}
			if (shape == MapAreaShape.Polygon && (float)coord.Length % 2.0 != 0.0)
			{
				throw new InvalidOperationException("Invalid number of coordinates specified for the polygon map area shape. \"x1,y1,x2,y2...xn,yn\" – Each x,y pair should contain the coordinates of one vertex of the polygon.");
			}
			MapArea mapArea = new MapArea();
			mapArea.Custom = false;
			mapArea.ToolTip = toolTip;
			mapArea.Href = href;
			mapArea.MapAreaAttributes = attr;
			mapArea.Shape = shape;
			mapArea.Coordinates = new int[coord.Length];
			coord.CopyTo(mapArea.Coordinates, 0);
			return this.Add(mapArea);
		}

		public void Insert(int index, string toolTip, string href, string attr, GraphicsPath path)
		{
			PointF[] pathPoints = path.PathPoints;
			if (pathPoints.Length > 0)
			{
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

		internal void Insert(int index, string toolTip, string href, string attr, GraphicsPath path, bool absCoordinates, MapGraphics graph)
		{
			GraphicsPathIterator graphicsPathIterator = new GraphicsPathIterator(path);
			if (graphicsPathIterator.SubpathCount > 1)
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				while (graphicsPathIterator.NextMarker(graphicsPath) > 0)
				{
					this.InsertSubpath(index, toolTip, href, attr, graphicsPath, absCoordinates, graph);
					graphicsPath.Reset();
				}
			}
			else
			{
				this.InsertSubpath(index, toolTip, href, attr, path, absCoordinates, graph);
			}
		}

		private void InsertSubpath(int index, string toolTip, string href, string attr, GraphicsPath path, bool absCoordinates, MapGraphics graph)
		{
			PointF[] pathPoints = path.PathPoints;
			if (pathPoints.Length > 0)
			{
				float[] array = new float[pathPoints.Length * 2];
				if (absCoordinates)
				{
					for (int i = 0; i < pathPoints.Length; i++)
					{
						pathPoints[i] = graph.GetRelativePoint(pathPoints[i]);
					}
				}
				int num = 0;
				PointF[] array2 = pathPoints;
				for (int j = 0; j < array2.Length; j++)
				{
					PointF pointF = array2[j];
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

		public void Insert(int index, MapAreaShape shape, string toolTip, string href, string attr, float[] coord)
		{
			if (shape == MapAreaShape.Circle && coord.Length != 3)
			{
				throw new InvalidOperationException("Invalid number of coordinates for the circle map area shape. Three coordinates must be provided: the center of the circle (x,y), followed by the radius.");
			}
			if (shape == MapAreaShape.Rectangle && coord.Length != 4)
			{
				throw new InvalidOperationException("Invalid number of coordinates for the rectangle map area shape. Four coordinates must be provided: x,y coordinates of the upper-left corner followed by x,y coordinates for the bottom-right corner.");
			}
			if (shape == MapAreaShape.Polygon && (float)coord.Length % 2.0 != 0.0)
			{
				throw new InvalidOperationException("Invalid number of coordinates for the polygon map area shape. \"x1,y1,x2,y2...xn,yn\" – Each x,y pair contains the coordinates of one vertex of the polygon.");
			}
			MapArea mapArea = new MapArea();
			mapArea.Custom = false;
			mapArea.ToolTip = toolTip;
			mapArea.Href = href;
			mapArea.MapAreaAttributes = attr;
			mapArea.Shape = shape;
			mapArea.Coordinates = new int[coord.Length];
			coord.CopyTo(mapArea.Coordinates, 0);
			this.Insert(index, mapArea);
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
