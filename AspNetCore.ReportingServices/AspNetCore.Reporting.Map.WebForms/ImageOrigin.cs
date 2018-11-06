using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.Reporting.Map.WebForms
{
	[TypeConverter(typeof(Converter))]
	internal class ImageOrigin : ICloneable
	{
		internal class Converter : ExpandableObjectConverter
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				if (sourceType == typeof(string))
				{
					return true;
				}
				return base.CanConvertFrom(context, sourceType);
			}

			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				if (destinationType == typeof(ImageOrigin))
				{
					return true;
				}
				return base.CanConvertTo(context, destinationType);
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				if (destinationType == typeof(string))
				{
					return ((ImageOrigin)value).ToString();
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				if (value is string)
				{
					string text = (string)value;
					string[] array = ((string)value).Split(',');
					if (array.Length == 2)
					{
						return new ImageOrigin(false, int.Parse(array[0], CultureInfo.CurrentCulture), int.Parse(array[1], CultureInfo.CurrentCulture));
					}
					throw new ArgumentException("ElementPositionConverter - Incorrect string format. The X, Y of the location must be specified e.g.  \"0,0\"");
				}
				return base.ConvertFrom(context, culture, value);
			}
		}

		private Point point = new Point(0, 0);

		private bool notSet = true;

		private bool defaultValues = true;

		[SRCategory("CategoryAttribute_Values")]
		[RefreshProperties(RefreshProperties.All)]
		[SRDescription("DescriptionAttributeImageOrigin_X")]
		[NotifyParentProperty(true)]
		public int X
		{
			get
			{
				return this.point.X;
			}
			set
			{
				this.point.X = value;
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeImageOrigin_Y")]
		[RefreshProperties(RefreshProperties.All)]
		[SRCategory("CategoryAttribute_Values")]
		public int Y
		{
			get
			{
				return this.point.Y;
			}
			set
			{
				this.point.Y = value;
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Values")]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeImageOrigin_NotSet")]
		[RefreshProperties(RefreshProperties.All)]
		public bool NotSet
		{
			get
			{
				return this.notSet;
			}
			set
			{
				this.notSet = value;
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

		public ImageOrigin()
		{
		}

		public ImageOrigin(bool notSet, int x, int y)
		{
			this.notSet = notSet;
			this.point.X = x;
			this.point.Y = y;
		}

		public override string ToString()
		{
			if (this.NotSet)
			{
				return "Not set";
			}
			return this.point.X.ToString(CultureInfo.CurrentCulture) + ", " + this.point.Y.ToString(CultureInfo.CurrentCulture);
		}

		public Point ToPoint()
		{
			return new Point(this.point.X, this.point.Y);
		}

		public object Clone()
		{
			return new ImageOrigin(this.notSet, this.point.X, this.point.Y);
		}
	}
}
