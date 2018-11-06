using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class ProjectionCenter : MapObject
	{
		private bool xIsNaN = true;

		private double x = double.NaN;

		private bool yIsNaN = true;

		private double y = double.NaN;

		[SRDescription("DescriptionAttributeProjectionCenter_X")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Coordinates")]
		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(double.NaN)]
		public double X
		{
			get
			{
				return this.x;
			}
			set
			{
				this.x = value;
				this.xIsNaN = double.IsNaN(value);
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Coordinates")]
		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeProjectionCenter_Y")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public double Y
		{
			get
			{
				return this.y;
			}
			set
			{
				this.y = value;
				this.yIsNaN = double.IsNaN(value);
				this.Invalidate();
			}
		}

		public ProjectionCenter()
			: this(null)
		{
		}

		internal ProjectionCenter(object parent)
			: base(parent)
		{
		}

		internal override void Invalidate()
		{
			MapCore mapCore = (MapCore)this.Parent;
			if (mapCore != null)
			{
				mapCore.InvalidateCachedPaths();
				mapCore.ResetCachedBoundsAfterProjection();
				mapCore.InvalidateDistanceScalePanel();
				mapCore.InvalidateViewport();
			}
		}

		internal bool IsXNaN()
		{
			return this.xIsNaN;
		}

		internal bool IsYNaN()
		{
			return this.yIsNaN;
		}
	}
}
