using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class HotRegionList : GaugeObject
	{
		private ArrayList list = new ArrayList();

		internal ArrayList List
		{
			get
			{
				return this.list;
			}
		}

		public HotRegionList(object parent)
			: base(parent)
		{
		}

		internal int LocateObject(object selectedObject)
		{
			for (int i = 0; i < this.list.Count; i++)
			{
				if (((HotRegion)this.list[i]).SelectedObject == selectedObject)
				{
					return i;
				}
			}
			return -1;
		}

		public void SetHotRegion(object selectedObject, params GraphicsPath[] paths)
		{
			this.SetHotRegion(selectedObject, PointF.Empty, paths);
		}

		public void SetHotRegion(object selectedObject, PointF pinPoint, params GraphicsPath[] paths)
		{
			int num = this.LocateObject(selectedObject);
			HotRegion hotRegion;
			if (num == -1)
			{
				hotRegion = new HotRegion();
				num = this.list.Add(hotRegion);
			}
			else
			{
				hotRegion = (HotRegion)this.list[num];
			}
			hotRegion.SelectedObject = selectedObject;
			Matrix transform = this.Common.Graph.Transform;
			if (transform != null)
			{
				for (int i = 0; i < paths.Length; i++)
				{
					if (paths[i] != null)
					{
						try
						{
							paths[i].Transform(transform);
						}
						catch
						{
							return;
						}
					}
				}
			}
			else
			{
				this.Common.Graph.Transform = new Matrix();
			}
			hotRegion.Paths = paths;
			if (!pinPoint.IsEmpty)
			{
				pinPoint.X += transform.OffsetX;
				pinPoint.Y += transform.OffsetY;
			}
			hotRegion.PinPoint = pinPoint;
			hotRegion.BuildMatrices(this.Common.Graph);
		}

		internal HotRegion[] CheckHotRegions(int x, int y, Type[] objectTypes)
		{
			ArrayList arrayList = new ArrayList();
			for (int num = this.list.Count - 1; num >= 0; num--)
			{
				HotRegion hotRegion = (HotRegion)this.list[num];
				if (this.IsOfType(objectTypes, hotRegion.SelectedObject))
				{
					GraphicsPath[] paths = ((HotRegion)this.list[num]).Paths;
					foreach (GraphicsPath graphicsPath in paths)
					{
						if (graphicsPath != null && graphicsPath.IsVisible((float)x, (float)y))
						{
							arrayList.Add(this.list[num]);
						}
					}
				}
			}
			if (arrayList.Count > 0)
			{
				return (HotRegion[])arrayList.ToArray(typeof(HotRegion));
			}
			return null;
		}

		internal bool IsOfType(Type[] objectTypes, object obj)
		{
			if (objectTypes.Length == 0)
			{
				return true;
			}
			foreach (Type type in objectTypes)
			{
				if (type.IsInstanceOfType(obj))
				{
					return true;
				}
			}
			return false;
		}

		internal void Clear()
		{
			foreach (HotRegion item in this.list)
			{
				item.Dispose();
			}
			this.list.Clear();
		}
	}
}
