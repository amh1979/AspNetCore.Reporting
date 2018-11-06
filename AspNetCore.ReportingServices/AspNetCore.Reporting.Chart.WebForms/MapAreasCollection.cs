using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeMapAreasCollection_MapAreasCollection")]
	internal class MapAreasCollection : IList, ICollection, IEnumerable
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
				throw new ArgumentException(SR.ExceptionImageMapAddedHasWrongType);
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
				throw new ArgumentException(SR.ExceptionImageMapInsertedHasWrongType);
			}
			this.array.Insert(index, value);
		}

		public bool Contains(MapArea value)
		{
			return this.array.Contains(value);
		}

		public int IndexOf(MapArea value)
		{
			return this.array.IndexOf(value);
		}

		public void Remove(MapArea value)
		{
			this.array.Remove(value);
		}

		public void CopyTo(Array array, int index)
		{
			this.array.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return this.array.GetEnumerator();
		}

		internal int Add(string toolTip, string href, string attr, GraphicsPath path, object tag)
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
				return this.Add(MapAreaShape.Polygon, toolTip, href, attr, array, tag);
			}
			return -1;
		}

		internal int Add(string toolTip, string href, string attr, RectangleF rect, object tag)
		{
			return this.Add(MapAreaShape.Rectangle, toolTip, href, attr, new float[4]
			{
				rect.X,
				rect.Y,
				rect.Right,
				rect.Bottom
			}, tag);
		}

		internal int Add(MapAreaShape shape, string toolTip, string href, string attr, float[] coordinates, object tag)
		{
			if (shape == MapAreaShape.Circle && coordinates.Length != 3)
			{
				throw new InvalidOperationException(SR.ExceptionImageMapCircleShapeInvalid);
			}
			if (shape == MapAreaShape.Rectangle && coordinates.Length != 4)
			{
				throw new InvalidOperationException(SR.ExceptionImageMapRectangleShapeInvalid);
			}
			if (shape == MapAreaShape.Polygon && (float)coordinates.Length % 2.0 != 0.0)
			{
				throw new InvalidOperationException(SR.ExceptionImageMapPolygonShapeInvalid);
			}
			MapArea mapArea = new MapArea();
			mapArea.Custom = false;
			mapArea.ToolTip = toolTip;
			mapArea.Href = href;
			mapArea.MapAreaAttributes = attr;
			mapArea.Shape = shape;
			mapArea.Coordinates = new float[coordinates.Length];
			coordinates.CopyTo(mapArea.Coordinates, 0);
			((IMapAreaAttributes)mapArea).Tag = tag;
			return this.Add(mapArea);
		}

		internal void Insert(int index, string toolTip, string href, string attr, GraphicsPath path, object tag)
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
				this.Insert(index, MapAreaShape.Polygon, toolTip, href, attr, array, tag);
			}
		}

		internal void Insert(int index, string toolTip, string href, string attr, GraphicsPath path, bool absCoordinates, ChartGraphics graph)
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

		private void InsertSubpath(int index, string toolTip, string href, string attr, GraphicsPath path, bool absCoordinates, ChartGraphics graph)
		{
			if (path.PointCount > 0)
			{
				path.Flatten();
				PointF[] pathPoints = path.PathPoints;
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
				this.Insert(index, MapAreaShape.Polygon, toolTip, href, attr, array, null);
			}
		}

		internal void Insert(int index, string toolTip, string href, string attr, RectangleF rect, object tag)
		{
			this.Insert(index, MapAreaShape.Rectangle, toolTip, href, attr, new float[4]
			{
				rect.X,
				rect.Y,
				rect.Right,
				rect.Bottom
			}, tag);
		}

		internal void Insert(int index, MapAreaShape shape, string toolTip, string href, string attr, float[] coordinates, object tag)
		{
			if (shape == MapAreaShape.Circle && coordinates.Length != 3)
			{
				throw new InvalidOperationException(SR.ExceptionImageMapCircleShapeInvalid);
			}
			if (shape == MapAreaShape.Rectangle && coordinates.Length != 4)
			{
				throw new InvalidOperationException(SR.ExceptionImageMapRectangleShapeInvalid);
			}
			if (shape == MapAreaShape.Polygon && (float)coordinates.Length % 2.0 != 0.0)
			{
				throw new InvalidOperationException(SR.ExceptionImageMapPolygonShapeInvalid);
			}
			MapArea mapArea = new MapArea();
			mapArea.Custom = false;
			mapArea.ToolTip = toolTip;
			mapArea.Href = href;
			mapArea.MapAreaAttributes = attr;
			mapArea.Shape = shape;
			mapArea.Coordinates = new float[coordinates.Length];
			coordinates.CopyTo(mapArea.Coordinates, 0);
			((IMapAreaAttributes)mapArea).Tag = tag;
			this.Insert(index, mapArea);
		}

		public int Add(string href, GraphicsPath path)
		{
			return this.Add("", href, "", path, null);
		}

		public int Add(string href, RectangleF rect)
		{
			return this.Add("", href, "", rect, null);
		}

		public int Add(MapAreaShape shape, string href, float[] coordinates)
		{
			return this.Add(shape, "", href, "", coordinates, null);
		}

		public void Insert(int index, string href, GraphicsPath path)
		{
			this.Insert(index, "", href, "", path, null);
		}

		public void Insert(int index, string href, RectangleF rect)
		{
			this.Insert(index, "", href, "", rect, null);
		}

		public void Insert(int index, MapAreaShape shape, string href, float[] coordinates)
		{
			this.Insert(index, shape, "", href, "", coordinates, null);
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
