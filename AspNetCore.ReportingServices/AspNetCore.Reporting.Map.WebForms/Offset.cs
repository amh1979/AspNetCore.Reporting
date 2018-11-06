using System;
using System.ComponentModel;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class Offset : MapObject, ICloneable
	{
		private MapPoint point = new MapPoint(0.0, 0.0);

		[RefreshProperties(RefreshProperties.All)]
		[NotifyParentProperty(true)]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeOffset_X")]
		[SRCategory("CategoryAttribute_Values")]
		public double X
		{
			get
			{
				return this.point.X;
			}
			set
			{
				if (!(value > 100000000.0) && !(value < -100000000.0))
				{
					this.point.X = value;
					this.ResetCachedPaths();
					this.InvalidateCachedBounds();
					this.InvalidateViewport();
					return;
				}
				throw new ArgumentOutOfRangeException(SR.out_of_range(-100000000.0, 100000000.0));
			}
		}

		[NotifyParentProperty(true)]
		[DefaultValue(0.0)]
		[SRCategory("CategoryAttribute_Values")]
		[SRDescription("DescriptionAttributeOffset_Y")]
		[RefreshProperties(RefreshProperties.All)]
		public double Y
		{
			get
			{
				return this.point.Y;
			}
			set
			{
				if (!(value > 100000000.0) && !(value < -100000000.0))
				{
					this.point.Y = value;
					this.ResetCachedPaths();
					this.InvalidateCachedBounds();
					this.InvalidateViewport();
					return;
				}
				throw new ArgumentOutOfRangeException(SR.out_of_range(-100000000.0, 100000000.0));
			}
		}

		public Offset()
			: this(null)
		{
		}

		internal Offset(object parent)
			: base(parent)
		{
		}

		internal Offset(object parent, double x, double y)
			: this(parent)
		{
			this.point.X = x;
			this.point.Y = y;
		}

		public override string ToString()
		{
			return this.point.X.ToString(CultureInfo.CurrentCulture) + ", " + this.point.Y.ToString(CultureInfo.CurrentCulture);
		}

		public override bool Equals(object obj)
		{
			if (obj is Offset)
			{
				Offset offset = (Offset)obj;
				if (offset.X == this.X && offset.Y == this.Y)
				{
					return true;
				}
				return false;
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public object Clone()
		{
			return new Offset(this.Parent, this.X, this.Y);
		}

		internal void InvalidateCachedBounds()
		{
			if (this.Parent is Shape)
			{
				((Shape)this.Parent).InvalidateCachedBounds();
			}
			else if (this.Parent is Path)
			{
				((Path)this.Parent).InvalidateCachedBounds();
			}
			else if (this.Parent is Symbol)
			{
				((Symbol)this.Parent).InvalidateCachedBounds();
			}
		}

		internal void ResetCachedPaths()
		{
			if (this.Parent is Shape)
			{
				((Shape)this.Parent).ResetCachedPaths();
			}
			else if (this.Parent is Path)
			{
				((Path)this.Parent).ResetCachedPaths();
			}
			else if (this.Parent is Symbol)
			{
				((Symbol)this.Parent).ResetCachedPaths();
			}
		}
	}
}
