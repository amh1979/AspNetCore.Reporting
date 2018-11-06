using System.Drawing;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class HitTestResult
	{
		private PointF htPoint;

		private ObjectType objectType;

		private object obj;

		private HotRegion region;

		public ObjectType ObjectType
		{
			get
			{
				return this.objectType;
			}
		}

		public object Object
		{
			get
			{
				return this.obj;
			}
		}

		public bool Success
		{
			get
			{
				return this.obj != null;
			}
		}

		public string Name
		{
			get
			{
				if (this.obj is NamedElement)
				{
					return ((NamedElement)this.obj).Name;
				}
				return this.obj.ToString();
			}
		}

		internal HotRegion Region
		{
			get
			{
				return this.region;
			}
		}

		internal HitTestResult(HotRegion region, PointF hitTestPoint)
		{
			this.region = region;
			if (region != null)
			{
				this.obj = region.SelectedObject;
			}
			this.htPoint = hitTestPoint;
			if (this.Object is Group)
			{
				this.objectType = ObjectType.Group;
			}
			else if (this.Object is Shape)
			{
				this.objectType = ObjectType.Shape;
			}
			else if (this.Object is Path)
			{
				this.objectType = ObjectType.Path;
			}
			else if (this.Object is Symbol)
			{
				this.objectType = ObjectType.Symbol;
			}
			else if (this.Object is Viewport)
			{
				this.objectType = ObjectType.Viewport;
			}
			else if (this.Object is Legend)
			{
				this.objectType = ObjectType.Legend;
			}
			else if (this.Object is LegendCell)
			{
				this.objectType = ObjectType.LegendCell;
			}
			else if (this.Object is NavigationPanel)
			{
				this.objectType = ObjectType.NavigationPanel;
			}
			else if (this.Object is ZoomPanel)
			{
				this.objectType = ObjectType.ZoomPanel;
			}
			else if (this.Object is ColorSwatchPanel)
			{
				this.objectType = ObjectType.ColorSwatchPanel;
			}
			else if (this.Object is DistanceScalePanel)
			{
				this.objectType = ObjectType.DistanceScalePanel;
			}
			else if (this.Object is MapImage)
			{
				this.objectType = ObjectType.MapImage;
			}
			else if (this.Object is MapLabel)
			{
				this.objectType = ObjectType.MapLabel;
			}
			else
			{
				this.objectType = ObjectType.Unknown;
			}
		}
	}
}
