using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class MapLimits : MapObject
	{
		private bool minimumXIsNaN = true;

		private double minimumX = double.NaN;

		private bool minimumYIsNaN = true;

		private double minimumY = double.NaN;

		private bool maximumXIsNaN = true;

		private double maximumX = double.NaN;

		private bool maximumYIsNaN = true;

		private double maximumY = double.NaN;

		[SRDescription("DescriptionAttributeMapLimits_MinimumX")]
		[SRCategory("CategoryAttribute_Coordinates")]
		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[NotifyParentProperty(true)]
		[DefaultValue(double.NaN)]
		[RefreshProperties(RefreshProperties.All)]
		public double MinimumX
		{
			get
			{
				return this.minimumX;
			}
			set
			{
				this.minimumX = value;
				this.minimumXIsNaN = double.IsNaN(value);
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeMapLimits_MinimumY")]
		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[SRCategory("CategoryAttribute_Coordinates")]
		[DefaultValue(double.NaN)]
		[RefreshProperties(RefreshProperties.All)]
		public double MinimumY
		{
			get
			{
				return this.minimumY;
			}
			set
			{
				this.minimumY = value;
				this.minimumYIsNaN = double.IsNaN(value);
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeMapLimits_MaximumX")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[NotifyParentProperty(true)]
		[DefaultValue(double.NaN)]
		[SRCategory("CategoryAttribute_Coordinates")]
		public double MaximumX
		{
			get
			{
				return this.maximumX;
			}
			set
			{
				this.maximumX = value;
				this.maximumXIsNaN = double.IsNaN(value);
				this.Invalidate();
			}
		}

		[TypeConverter(typeof(DoubleAutoValueConverter))]
		[SRCategory("CategoryAttribute_Coordinates")]
		[NotifyParentProperty(true)]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeMapLimits_MaximumY")]
		[RefreshProperties(RefreshProperties.All)]
		public double MaximumY
		{
			get
			{
				return this.maximumY;
			}
			set
			{
				this.maximumY = value;
				this.maximumYIsNaN = double.IsNaN(value);
				this.Invalidate();
			}
		}

		public MapLimits()
			: this(null)
		{
		}

		internal MapLimits(object parent)
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

		internal bool IsMinimumXNaN()
		{
			return this.minimumXIsNaN;
		}

		internal bool IsMinimumYNaN()
		{
			return this.minimumYIsNaN;
		}

		internal bool IsMaximumXNaN()
		{
			return this.maximumXIsNaN;
		}

		internal bool IsMaximumYNaN()
		{
			return this.maximumYIsNaN;
		}
	}
}
