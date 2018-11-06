using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class MapSize : MapObject, ICloneable
	{
		private SizeF size = new SizeF(100f, 100f);

		private bool autoSize;

		private bool defaultValues;

		[SRCategory("CategoryAttribute_Values")]
		[SRDescription("DescriptionAttributeMapSize_Width")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public float Width
		{
			get
			{
				return this.size.Width;
			}
			set
			{
				if (!((double)value > 100000000.0) && !((double)value < -100000000.0))
				{
					float width = this.size.Width;
					this.size.Width = Math.Max(value, 0f);
					this.DefaultValues = false;
					this.Invalidate();
					if (this.size.Width != width)
					{
						Panel panel = this.Parent as Panel;
						if (panel != null)
						{
							panel.SizeChanged(this);
						}
					}
					return;
				}
				throw new ArgumentOutOfRangeException(SR.out_of_range(-100000000.0, 100000000.0));
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[SRCategory("CategoryAttribute_Values")]
		[SRDescription("DescriptionAttributeMapSize_Height")]
		[NotifyParentProperty(true)]
		public float Height
		{
			get
			{
				return this.size.Height;
			}
			set
			{
				if (!((double)value > 100000000.0) && !((double)value < -100000000.0))
				{
					float height = this.size.Height;
					this.size.Height = Math.Max(value, 0f);
					this.DefaultValues = false;
					this.Invalidate();
					if (this.size.Height != height)
					{
						Panel panel = this.Parent as Panel;
						if (panel != null)
						{
							panel.SizeChanged(this);
						}
					}
					return;
				}
				throw new ArgumentOutOfRangeException(SR.out_of_range(-100000000.0, 100000000.0));
			}
		}

		internal bool AutoSize
		{
			get
			{
				return this.autoSize;
			}
			set
			{
				this.autoSize = value;
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

		private MapSize DefaultSize
		{
			get
			{
				IDefaultValueProvider defaultValueProvider = this.Parent as IDefaultValueProvider;
				if (defaultValueProvider == null)
				{
					return new MapSize(null, 0f, 0f);
				}
				return (MapSize)defaultValueProvider.GetDefaultValue("Size", this);
			}
		}

		public MapSize()
			: this((object)null)
		{
		}

		internal MapSize(object parent)
			: base(parent)
		{
		}

		internal MapSize(object parent, float width, float height)
			: this(parent)
		{
			this.size.Width = Math.Max(width, 0f);
			this.size.Height = Math.Max(height, 0f);
		}

		internal MapSize(object parent, SizeF size)
			: this(parent)
		{
			this.size.Width = size.Width;
			this.size.Height = size.Height;
		}

		internal MapSize(MapSize size)
			: this(size.Parent, size.Width, size.Height)
		{
		}

		protected void ResetWidth()
		{
			this.Width = this.DefaultSize.Width;
		}

		protected bool ShouldSerializeWidth()
		{
			return true;
		}

		protected void ResetHeight()
		{
			this.Height = this.DefaultSize.Height;
		}

		protected bool ShouldSerializeHeight()
		{
			return true;
		}

		public override string ToString()
		{
			return this.size.Width.ToString(CultureInfo.CurrentCulture) + ", " + this.size.Height.ToString(CultureInfo.CurrentCulture);
		}

		public SizeF ToSize()
		{
			return new SizeF(this.size);
		}

		public static implicit operator SizeF(MapSize size)
		{
			return size.ToSize();
		}

		internal SizeF GetSizeF()
		{
			return this.size;
		}

		public object Clone()
		{
			return new MapSize(this.Parent, this.size);
		}
	}
}
