using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class MapLocation : MapObject, ICloneable
	{
		private PointF point = new PointF(0f, 0f);

		private bool docked;

		private bool defaultValues;

		[SRDescription("DescriptionAttributeMapLocation_X")]
		[SRCategory("CategoryAttribute_Values")]
		[RefreshProperties(RefreshProperties.All)]
		[NotifyParentProperty(true)]
		public float X
		{
			get
			{
				return this.point.X;
			}
			set
			{
				if (!((double)value > 100000000.0) && !((double)value < -100000000.0))
				{
					float x = this.point.X;
					this.point.X = value;
					this.DefaultValues = false;
					this.Invalidate();
					if (this.point.X != x)
					{
						Panel panel = this.Parent as Panel;
						if (panel != null)
						{
							panel.LocationChanged(this);
						}
					}
					return;
				}
				throw new ArgumentOutOfRangeException(SR.out_of_range(-100000000.0, 100000000.0));
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Values")]
		[SRDescription("DescriptionAttributeMapLocation_Y")]
		[RefreshProperties(RefreshProperties.All)]
		public float Y
		{
			get
			{
				return this.point.Y;
			}
			set
			{
				if (!((double)value > 100000000.0) && !((double)value < -100000000.0))
				{
					float y = this.point.Y;
					this.point.Y = value;
					this.DefaultValues = false;
					this.Invalidate();
					if (this.point.Y != y)
					{
						Panel panel = this.Parent as Panel;
						if (panel != null)
						{
							panel.LocationChanged(this);
						}
					}
					return;
				}
				throw new ArgumentOutOfRangeException(SR.out_of_range(-100000000.0, 100000000.0));
			}
		}

		internal bool Docked
		{
			get
			{
				return this.docked;
			}
			set
			{
				this.docked = value;
			}
		}

		internal bool DefaultValues
		{
			get
			{
				return this.defaultValues;
			}
			set
			{
				this.defaultValues = value;
			}
		}

		private MapLocation DefaultLocation
		{
			get
			{
				IDefaultValueProvider defaultValueProvider = this.Parent as IDefaultValueProvider;
				if (defaultValueProvider == null)
				{
					return new MapLocation(null, 0f, 0f);
				}
				return (MapLocation)defaultValueProvider.GetDefaultValue("Location", this);
			}
		}

		public MapLocation()
			: this((object)null)
		{
		}

		internal MapLocation(object parent)
			: base(parent)
		{
		}

		internal MapLocation(object parent, float x, float y)
			: this(parent)
		{
			this.point.X = x;
			this.point.Y = y;
		}

		internal MapLocation(MapLocation location)
			: this(location.Parent, location.X, location.Y)
		{
		}

		protected void ResetX()
		{
			this.X = this.DefaultLocation.X;
		}

		protected bool ShouldSerializeX()
		{
			return true;
		}

		protected void ResetY()
		{
			this.Y = this.DefaultLocation.Y;
		}

		protected bool ShouldSerializeY()
		{
			return true;
		}

		public override string ToString()
		{
			return this.point.X.ToString(CultureInfo.CurrentCulture) + ", " + this.point.Y.ToString(CultureInfo.CurrentCulture);
		}

		public PointF ToPoint()
		{
			return new PointF(this.point.X, this.point.Y);
		}

		public static implicit operator PointF(MapLocation location)
		{
			return location.GetPointF();
		}

		public object Clone()
		{
			MapLocation mapLocation = new MapLocation(this.Parent, this.X, this.Y);
			mapLocation.Docked = this.Docked;
			return mapLocation;
		}

		internal PointF GetPointF()
		{
			return this.point;
		}
	}
}
