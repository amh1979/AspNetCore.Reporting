using AspNetCore.Reporting.Chart.WebForms.Design;
using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeDataPointAttributes_DataPointAttributes")]
	[DefaultProperty("Label")]
	[TypeConverter(typeof(NoNameExpandableObjectConverter))]
	internal class DataPointAttributes : IMapAreaAttributes, ICustomTypeDescriptor
	{
		internal IServiceContainer serviceContainer;

		internal bool pointAttributes = true;

		internal Series series;

		internal Hashtable attributes = new Hashtable();

		internal static ColorConverter colorConverter = new ColorConverter();

		internal static FontConverter fontConverter = new FontConverter();

		internal bool tempColorIsSet;

		internal CustomAttributes customAttributes;

		internal bool emptyPoint;

		private object tag;

		private object mapAreaTag;

		private object mapAreaLegendTag;

		private object mapAreaLabelTag;

		public string this[int index]
		{
			get
			{
				int num = 0;
				foreach (object key in this.attributes.Keys)
				{
					if (num == index)
					{
						if (key is string)
						{
							return (string)key;
						}
						if (key is int)
						{
							return Enum.GetName(typeof(CommonAttributes), key);
						}
						return key.ToString();
					}
					num++;
				}
				throw new IndexOutOfRangeException();
			}
		}

		public string this[string name]
		{
			get
			{
				if (!this.IsAttributeSet(name) && this.pointAttributes)
				{
					if (this.emptyPoint)
					{
						return (string)this.series.EmptyPointStyle.attributes[name];
					}
					return (string)this.series.attributes[name];
				}
				return (string)this.attributes[name];
			}
			set
			{
				this.attributes[name] = value;
				this.Invalidate(true);
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeLabel")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeLabel")]
		public virtual string Label
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.Label))
					{
						return (string)this.GetAttributeObject(CommonAttributes.Label);
					}
					if (this.IsSerializing())
					{
						return "";
					}
					if (this.emptyPoint)
					{
						return (string)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.Label);
					}
					return this.series.label;
				}
				return this.series.label;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.Label, value);
				}
				else
				{
					this.series.label = value;
				}
				this.Invalidate(true);
			}
		}

		[DefaultValue("")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeAxisLabel")]
		[SRCategory("CategoryAttributeMisc")]
		public virtual string AxisLabel
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.AxisLabel))
					{
						return (string)this.GetAttributeObject(CommonAttributes.AxisLabel);
					}
					if (this.IsSerializing())
					{
						return "";
					}
					if (this.emptyPoint)
					{
						return (string)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.AxisLabel);
					}
					return this.series.axisLabel;
				}
				return this.series.axisLabel;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.AxisLabel, value);
				}
				else
				{
					this.series.axisLabel = value;
				}
				if (value.Length > 0 && this.series != null)
				{
					this.series.noLabelsInPoints = false;
				}
				this.Invalidate(false);
			}
		}

		[SRCategory("CategoryAttributeLabel")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLabelFormat")]
		[DefaultValue("")]
		[Browsable(false)]
		public string LabelFormat
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.LabelFormat))
					{
						return (string)this.GetAttributeObject(CommonAttributes.LabelFormat);
					}
					if (this.IsSerializing())
					{
						return "";
					}
					if (this.emptyPoint)
					{
						return (string)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.LabelFormat);
					}
					return this.series.labelFormat;
				}
				return this.series.labelFormat;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.LabelFormat, value);
				}
				else
				{
					this.series.labelFormat = value;
				}
				this.Invalidate(false);
			}
		}

		[SRDescription("DescriptionAttributeShowLabelAsValue")]
		[Browsable(false)]
		[DefaultValue(false)]
		[SRCategory("CategoryAttributeLabel")]
		[Bindable(true)]
		public bool ShowLabelAsValue
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.ShowLabelAsValue))
					{
						return (bool)this.GetAttributeObject(CommonAttributes.ShowLabelAsValue);
					}
					if (this.IsSerializing())
					{
						return false;
					}
					if (this.emptyPoint)
					{
						return (bool)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.ShowLabelAsValue);
					}
					return this.series.showLabelAsValue;
				}
				return this.series.showLabelAsValue;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.ShowLabelAsValue, value);
				}
				else
				{
					this.series.showLabelAsValue = value;
				}
				this.Invalidate(false);
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeColor4")]
		public Color Color
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.Color))
					{
						return (Color)this.GetAttributeObject(CommonAttributes.Color);
					}
					if (this.IsSerializing())
					{
						return Color.Empty;
					}
					if (this.emptyPoint)
					{
						return (Color)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.Color);
					}
					return this.series.color;
				}
				return this.series.color;
			}
			set
			{
				this.tempColorIsSet = false;
				if (value == Color.Empty && this.pointAttributes)
				{
					this.DeleteAttribute(CommonAttributes.Color);
				}
				else
				{
					if (this.pointAttributes)
					{
						this.SetAttributeObject(CommonAttributes.Color, value);
					}
					else
					{
						this.series.color = value;
					}
					this.Invalidate(true);
				}
			}
		}

		[DefaultValue(typeof(Color), "")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeBorderColor9")]
		[SRCategory("CategoryAttributeAppearance")]
		public Color BorderColor
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.BorderColor))
					{
						return (Color)this.GetAttributeObject(CommonAttributes.BorderColor);
					}
					if (this.IsSerializing())
					{
						return Color.Empty;
					}
					if (this.emptyPoint)
					{
						return (Color)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.BorderColor);
					}
					return this.series.borderColor;
				}
				return this.series.borderColor;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.BorderColor, value);
				}
				else
				{
					this.series.borderColor = value;
				}
				this.Invalidate(true);
			}
		}

		[DefaultValue(ChartDashStyle.Solid)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeBorderStyle3")]
		public ChartDashStyle BorderStyle
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.BorderDashStyle))
					{
						return (ChartDashStyle)this.GetAttributeObject(CommonAttributes.BorderDashStyle);
					}
					if (this.IsSerializing())
					{
						return ChartDashStyle.Solid;
					}
					if (this.emptyPoint)
					{
						return (ChartDashStyle)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.BorderDashStyle);
					}
					return this.series.borderStyle;
				}
				return this.series.borderStyle;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.BorderDashStyle, value);
				}
				else
				{
					this.series.borderStyle = value;
				}
				this.Invalidate(true);
			}
		}

		[DefaultValue(1)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeBorderWidth8")]
		public int BorderWidth
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.BorderWidth))
					{
						return (int)this.GetAttributeObject(CommonAttributes.BorderWidth);
					}
					if (this.IsSerializing())
					{
						return 1;
					}
					if (this.emptyPoint)
					{
						return (int)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.BorderWidth);
					}
					return this.series.borderWidth;
				}
				return this.series.borderWidth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionBorderWidthIsNotPositive);
				}
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.BorderWidth, value);
				}
				else
				{
					this.series.borderWidth = value;
				}
				this.Invalidate(true);
			}
		}

		[SRDescription("DescriptionAttributeBackImage10")]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue("")]
		[Bindable(true)]
		public string BackImage
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.BackImage))
					{
						return (string)this.GetAttributeObject(CommonAttributes.BackImage);
					}
					if (this.IsSerializing())
					{
						return "";
					}
					if (this.emptyPoint)
					{
						return (string)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.BackImage);
					}
					return this.series.backImage;
				}
				return this.series.backImage;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.BackImage, value);
				}
				else
				{
					this.series.backImage = value;
				}
				this.Invalidate(true);
			}
		}

		[DefaultValue(ChartImageWrapMode.Tile)]
		[SRDescription("DescriptionAttributeBackImageMode4")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		public ChartImageWrapMode BackImageMode
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.BackImageMode))
					{
						return (ChartImageWrapMode)this.GetAttributeObject(CommonAttributes.BackImageMode);
					}
					if (this.IsSerializing())
					{
						return ChartImageWrapMode.Tile;
					}
					if (this.emptyPoint)
					{
						return (ChartImageWrapMode)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.BackImageMode);
					}
					return this.series.backImageMode;
				}
				return this.series.backImageMode;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.BackImageMode, value);
				}
				else
				{
					this.series.backImageMode = value;
				}
				this.Invalidate(true);
			}
		}

		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeBackImageTransparentColor")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		public Color BackImageTransparentColor
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.BackImageTransparentColor))
					{
						return (Color)this.GetAttributeObject(CommonAttributes.BackImageTransparentColor);
					}
					if (this.IsSerializing())
					{
						return Color.Empty;
					}
					if (this.emptyPoint)
					{
						return (Color)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.BackImageTransparentColor);
					}
					return this.series.backImageTranspColor;
				}
				return this.series.backImageTranspColor;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.BackImageTransparentColor, value);
				}
				else
				{
					this.series.backImageTranspColor = value;
				}
				this.Invalidate(true);
			}
		}

		[Bindable(true)]
		[SRDescription("DescriptionAttributeBackImageAlign")]
		[SRCategory("CategoryAttributeAppearance")]
		[NotifyParentProperty(true)]
		[DefaultValue(ChartImageAlign.TopLeft)]
		public ChartImageAlign BackImageAlign
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.BackImageAlign))
					{
						return (ChartImageAlign)this.GetAttributeObject(CommonAttributes.BackImageAlign);
					}
					if (this.IsSerializing())
					{
						return ChartImageAlign.TopLeft;
					}
					if (this.emptyPoint)
					{
						return (ChartImageAlign)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.BackImageAlign);
					}
					return this.series.backImageAlign;
				}
				return this.series.backImageAlign;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.BackImageAlign, value);
				}
				else
				{
					this.series.backImageAlign = value;
				}
				this.Invalidate(true);
			}
		}

		[DefaultValue(GradientType.None)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeBackGradientType4")]
		public GradientType BackGradientType
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.BackGradientType))
					{
						return (GradientType)this.GetAttributeObject(CommonAttributes.BackGradientType);
					}
					if (this.IsSerializing())
					{
						return GradientType.None;
					}
					if (this.emptyPoint)
					{
						return (GradientType)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.BackGradientType);
					}
					return this.series.backGradientType;
				}
				return this.series.backGradientType;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.BackGradientType, value);
				}
				else
				{
					this.series.backGradientType = value;
				}
				this.Invalidate(true);
			}
		}

		[SRDescription("DescriptionAttributeBackGradientEndColor7")]
		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		public Color BackGradientEndColor
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.BackGradientEndColor))
					{
						return (Color)this.GetAttributeObject(CommonAttributes.BackGradientEndColor);
					}
					if (this.IsSerializing())
					{
						return Color.Empty;
					}
					if (this.emptyPoint)
					{
						return (Color)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.BackGradientEndColor);
					}
					return this.series.backGradientEndColor;
				}
				return this.series.backGradientEndColor;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.BackGradientEndColor, value);
				}
				else
				{
					this.series.backGradientEndColor = value;
				}
				this.Invalidate(true);
			}
		}

		[SRDescription("DescriptionAttributeBackHatchStyle9")]
		[DefaultValue(ChartHatchStyle.None)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		public ChartHatchStyle BackHatchStyle
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.BackHatchStyle))
					{
						return (ChartHatchStyle)this.GetAttributeObject(CommonAttributes.BackHatchStyle);
					}
					if (this.IsSerializing())
					{
						return ChartHatchStyle.None;
					}
					if (this.emptyPoint)
					{
						return (ChartHatchStyle)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.BackHatchStyle);
					}
					return this.series.backHatchStyle;
				}
				return this.series.backHatchStyle;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.BackHatchStyle, value);
				}
				else
				{
					this.series.backHatchStyle = value;
				}
				this.Invalidate(true);
			}
		}

		[SRCategory("CategoryAttributeLabelAppearance")]
		[SRDescription("DescriptionAttributeFont")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		[Bindable(true)]
		public Font Font
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.Font))
					{
						return (Font)this.GetAttributeObject(CommonAttributes.Font);
					}
					if (this.IsSerializing())
					{
						return new Font(ChartPicture.GetDefaultFontFamilyName(), 8f);
					}
					if (this.emptyPoint)
					{
						return (Font)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.Font);
					}
					return this.series.font;
				}
				return this.series.font;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.Font, value);
				}
				else
				{
					this.series.font = value;
				}
				this.Invalidate(false);
			}
		}

		[Bindable(true)]
		[SRDescription("DescriptionAttributeFontColor")]
		[DefaultValue(typeof(Color), "Black")]
		[SRCategory("CategoryAttributeLabelAppearance")]
		public Color FontColor
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.FontColor))
					{
						return (Color)this.GetAttributeObject(CommonAttributes.FontColor);
					}
					if (this.IsSerializing())
					{
						return Color.Black;
					}
					if (this.emptyPoint)
					{
						return (Color)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.FontColor);
					}
					return this.series.fontColor;
				}
				return this.series.fontColor;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.FontColor, value);
				}
				else
				{
					this.series.fontColor = value;
				}
				this.Invalidate(false);
			}
		}

		[SRCategory("CategoryAttributeLabelAppearance")]
		[DefaultValue(0)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeFontAngle3")]
		public int FontAngle
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.FontAngle))
					{
						return (int)this.GetAttributeObject(CommonAttributes.FontAngle);
					}
					if (this.IsSerializing())
					{
						return 0;
					}
					if (this.emptyPoint)
					{
						return (int)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.FontAngle);
					}
					return this.series.fontAngle;
				}
				return this.series.fontAngle;
			}
			set
			{
				if (value >= -90 && value <= 90)
				{
					if (this.pointAttributes)
					{
						this.SetAttributeObject(CommonAttributes.FontAngle, value);
					}
					else
					{
						this.series.fontAngle = value;
					}
					this.Invalidate(false);
					return;
				}
				throw new ArgumentOutOfRangeException("value", SR.ExceptionAngleRangeInvalid);
			}
		}

		[Bindable(true)]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(MarkerStyle.None)]
		[SRCategory("CategoryAttributeMarker")]
		[SRDescription("DescriptionAttributeMarkerStyle4")]
		public MarkerStyle MarkerStyle
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.MarkerStyle))
					{
						return (MarkerStyle)this.GetAttributeObject(CommonAttributes.MarkerStyle);
					}
					if (this.IsSerializing())
					{
						return MarkerStyle.None;
					}
					if (this.emptyPoint)
					{
						return (MarkerStyle)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.MarkerStyle);
					}
					return this.series.markerStyle;
				}
				return this.series.markerStyle;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.MarkerStyle, value);
				}
				else
				{
					this.series.markerStyle = value;
				}
				if (this is Series)
				{
					((Series)this).tempMarkerStyleIsSet = false;
				}
				this.Invalidate(true);
			}
		}

		[SRCategory("CategoryAttributeMarker")]
		[SRDescription("DescriptionAttributeMarkerSize")]
		[RefreshProperties(RefreshProperties.All)]
		[Bindable(true)]
		[DefaultValue(5)]
		public int MarkerSize
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.MarkerSize))
					{
						return (int)this.GetAttributeObject(CommonAttributes.MarkerSize);
					}
					if (this.IsSerializing())
					{
						return 5;
					}
					if (this.emptyPoint)
					{
						return (int)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.MarkerSize);
					}
					return this.series.markerSize;
				}
				return this.series.markerSize;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.MarkerSize, value);
				}
				else
				{
					this.series.markerSize = value;
				}
				this.Invalidate(true);
			}
		}

		[DefaultValue("")]
		[RefreshProperties(RefreshProperties.All)]
		[SRDescription("DescriptionAttributeMarkerImage10")]
		[SRCategory("CategoryAttributeMarker")]
		[Bindable(true)]
		public string MarkerImage
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.MarkerImage))
					{
						return (string)this.GetAttributeObject(CommonAttributes.MarkerImage);
					}
					if (this.IsSerializing())
					{
						return "";
					}
					if (this.emptyPoint)
					{
						return (string)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.MarkerImage);
					}
					return this.series.markerImage;
				}
				return this.series.markerImage;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.MarkerImage, value);
				}
				else
				{
					this.series.markerImage = value;
				}
				this.Invalidate(true);
			}
		}

		[SRDescription("DescriptionAttributeMarkerImageTransparentColor3")]
		[DefaultValue(typeof(Color), "")]
		[RefreshProperties(RefreshProperties.All)]
		[Bindable(true)]
		[SRCategory("CategoryAttributeMarker")]
		public Color MarkerImageTransparentColor
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.MarkerImageTransparentColor))
					{
						return (Color)this.GetAttributeObject(CommonAttributes.MarkerImageTransparentColor);
					}
					if (this.IsSerializing())
					{
						return Color.Empty;
					}
					if (this.emptyPoint)
					{
						return (Color)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.MarkerImageTransparentColor);
					}
					return this.series.markerImageTranspColor;
				}
				return this.series.markerImageTranspColor;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.MarkerImageTransparentColor, value);
				}
				else
				{
					this.series.markerImageTranspColor = value;
				}
				this.Invalidate(true);
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[SRCategory("CategoryAttributeMarker")]
		[DefaultValue(typeof(Color), "")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMarkerColor3")]
		public Color MarkerColor
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.MarkerColor))
					{
						return (Color)this.GetAttributeObject(CommonAttributes.MarkerColor);
					}
					if (this.IsSerializing())
					{
						return Color.Empty;
					}
					if (this.emptyPoint)
					{
						return (Color)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.MarkerColor);
					}
					return this.series.markerColor;
				}
				return this.series.markerColor;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.MarkerColor, value);
				}
				else
				{
					this.series.markerColor = value;
				}
				this.Invalidate(true);
			}
		}

		[DefaultValue(typeof(Color), "")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeMarkerBorderColor")]
		[RefreshProperties(RefreshProperties.All)]
		[SRCategory("CategoryAttributeMarker")]
		public Color MarkerBorderColor
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.MarkerBorderColor))
					{
						return (Color)this.GetAttributeObject(CommonAttributes.MarkerBorderColor);
					}
					if (this.IsSerializing())
					{
						return Color.Empty;
					}
					if (this.emptyPoint)
					{
						return (Color)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.MarkerBorderColor);
					}
					return this.series.markerBorderColor;
				}
				return this.series.markerBorderColor;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.MarkerBorderColor, value);
				}
				else
				{
					this.series.markerBorderColor = value;
				}
				this.Invalidate(true);
			}
		}

		[SRDescription("DescriptionAttributeMarkerBorderWidth3")]
		[SRCategory("CategoryAttributeMarker")]
		[Bindable(true)]
		[DefaultValue(1)]
		[Browsable(false)]
		public int MarkerBorderWidth
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.MarkerBorderWidth))
					{
						return (int)this.GetAttributeObject(CommonAttributes.MarkerBorderWidth);
					}
					if (this.IsSerializing())
					{
						return 1;
					}
					if (this.emptyPoint)
					{
						return (int)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.MarkerBorderWidth);
					}
					return this.series.markerBorderWidth;
				}
				return this.series.markerBorderWidth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionBorderWidthIsNotPositive);
				}
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.MarkerBorderWidth, value);
				}
				else
				{
					this.series.markerBorderWidth = value;
				}
				this.Invalidate(true);
			}
		}

		[NotifyParentProperty(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Bindable(false)]
		[SRDescription("DescriptionAttributeCustomAttributesExtended")]
		[DefaultValue(null)]
		[RefreshProperties(RefreshProperties.All)]
		[SRCategory("CategoryAttributeMisc")]
		[DesignOnly(true)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public CustomAttributes CustomAttributesExtended
		{
			get
			{
				return this.customAttributes;
			}
			set
			{
				this.customAttributes = value;
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Browsable(false)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeCustomAttributesExtended")]
		[DefaultValue("")]
		public string CustomAttributes
		{
			get
			{
				string text = "";
				string[] names = Enum.GetNames(typeof(CommonAttributes));
				for (int num = this.attributes.Count - 1; num >= 0; num--)
				{
					if (this[num] != null)
					{
						string text2 = this[num];
						bool flag = true;
						string[] array = names;
						foreach (string strB in array)
						{
							if (string.Compare(text2, strB, StringComparison.OrdinalIgnoreCase) == 0)
							{
								flag = false;
								break;
							}
						}
						if (flag && this.attributes[text2] != null)
						{
							if (text.Length > 0)
							{
								text += ", ";
							}
							string text3 = this.attributes[text2].ToString().Replace(",", "\\,");
							text3 = text3.Replace("=", "\\=");
							text = text + text2 + "=" + text3;
						}
					}
				}
				return text;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				Hashtable hashtable = new Hashtable();
				Array values = Enum.GetValues(typeof(CommonAttributes));
				foreach (object item in values)
				{
					if (this.IsAttributeSet((CommonAttributes)item))
					{
						hashtable[(int)item] = this.attributes[(int)item];
					}
				}
				if (value.Length > 0)
				{
					value = value.Replace("\\,", "\\x45");
					value = value.Replace("\\=", "\\x46");
					string[] array = value.Split(',');
					string[] array2 = array;
					foreach (string text in array2)
					{
						string[] array3 = text.Split('=');
						if (array3.Length != 2)
						{
							throw new FormatException(SR.ExceptionAttributeInvalidFormat);
						}
						array3[0] = array3[0].Trim();
						array3[1] = array3[1].Trim();
						if (array3[0].Length == 0)
						{
							throw new FormatException(SR.ExceptionAttributeInvalidFormat);
						}
						foreach (object key in hashtable.Keys)
						{
							if (key is string && string.Compare((string)key, array3[0], StringComparison.OrdinalIgnoreCase) == 0)
							{
								throw new FormatException(SR.ExceptionAttributeNameIsNotUnique(array3[0]));
							}
						}
						string text2 = array3[1].Replace("\\x45", ",");
						hashtable[array3[0]] = text2.Replace("\\x46", "=");
					}
				}
				this.attributes = hashtable;
				this.Invalidate(true);
			}
		}

		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		public object Tag
		{
			get
			{
				return this.tag;
			}
			set
			{
				this.tag = value;
			}
		}

		[SRCategory("CategoryAttributeMapArea")]
		[DefaultValue("")]
		[Bindable(true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeToolTip7")]
		public string ToolTip
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.ToolTip))
					{
						return (string)this.GetAttributeObject(CommonAttributes.ToolTip);
					}
					if (this.IsSerializing())
					{
						return "";
					}
					if (this.emptyPoint)
					{
						return (string)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.ToolTip);
					}
					return this.series.toolTip;
				}
				return this.series.toolTip;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.ToolTip, value);
				}
				else
				{
					this.series.toolTip = value;
				}
			}
		}

		object IMapAreaAttributes.Tag
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.mapAreaTag == null && this.series != null)
					{
						if (this.emptyPoint)
						{
							return this.series.EmptyPointStyle.mapAreaTag;
						}
						return this.series.mapAreaTag;
					}
					return this.mapAreaTag;
				}
				return this.series.mapAreaTag;
			}
			set
			{
				this.mapAreaTag = value;
			}
		}

		public object LegendTag
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.mapAreaLegendTag == null && this.series != null)
					{
						return this.series.mapAreaLegendTag;
					}
					return this.mapAreaLegendTag;
				}
				return this.series.mapAreaLegendTag;
			}
			set
			{
				this.mapAreaLegendTag = value;
			}
		}

		public object LabelTag
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.mapAreaLabelTag == null && this.series != null)
					{
						if (this.emptyPoint)
						{
							return this.series.EmptyPointStyle.mapAreaLabelTag;
						}
						return this.series.mapAreaLabelTag;
					}
					return this.mapAreaLabelTag;
				}
				return this.series.mapAreaLabelTag;
			}
			set
			{
				this.mapAreaLabelTag = value;
			}
		}

		[SRDescription("DescriptionAttributeHref7")]
		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[DefaultValue("")]
		public string Href
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.Href))
					{
						return (string)this.GetAttributeObject(CommonAttributes.Href);
					}
					if (this.IsSerializing())
					{
						return "";
					}
					if (this.emptyPoint)
					{
						return (string)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.Href);
					}
					return this.series.href;
				}
				return this.series.href;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.Href, value);
				}
				else
				{
					this.series.href = value;
				}
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeMapArea")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeMapAreaAttributes9")]
		public string MapAreaAttributes
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.MapAreaAttributes))
					{
						return (string)this.GetAttributeObject(CommonAttributes.MapAreaAttributes);
					}
					if (this.IsSerializing())
					{
						return "";
					}
					if (this.emptyPoint)
					{
						return (string)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.MapAreaAttributes);
					}
					return this.series.mapAreaAttributes;
				}
				return this.series.mapAreaAttributes;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.MapAreaAttributes, value);
				}
				else
				{
					this.series.mapAreaAttributes = value;
				}
			}
		}

		[DefaultValue(true)]
		[SRCategory("CategoryAttributeLegend")]
		[Browsable(false)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeShowInLegend")]
		public bool ShowInLegend
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.ShowInLegend))
					{
						return (bool)this.GetAttributeObject(CommonAttributes.ShowInLegend);
					}
					if (this.IsSerializing())
					{
						return true;
					}
					if (this.emptyPoint)
					{
						return (bool)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.ShowInLegend);
					}
					return this.series.showInLegend;
				}
				return this.series.showInLegend;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.ShowInLegend, value);
				}
				else
				{
					this.series.showInLegend = value;
				}
				this.Invalidate(true);
			}
		}

		[SRDescription("DescriptionAttributeLegendText")]
		[DefaultValue("")]
		[SRCategory("CategoryAttributeLegend")]
		[Bindable(true)]
		public string LegendText
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.LegendText))
					{
						return (string)this.GetAttributeObject(CommonAttributes.LegendText);
					}
					if (this.IsSerializing())
					{
						return "";
					}
					if (this.emptyPoint)
					{
						return (string)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.LegendText);
					}
					return this.series.legendText;
				}
				return this.series.legendText;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.LegendText, value);
				}
				else
				{
					this.series.legendText = value;
				}
				this.Invalidate(true);
			}
		}

		[SRDescription("DescriptionAttributeLegendToolTip")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAttributeLegend")]
		[Bindable(true)]
		[DefaultValue("")]
		public string LegendToolTip
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.LegendToolTip))
					{
						return (string)this.GetAttributeObject(CommonAttributes.LegendToolTip);
					}
					if (this.IsSerializing())
					{
						return "";
					}
					if (this.emptyPoint)
					{
						return (string)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.LegendToolTip);
					}
					return this.series.legendToolTip;
				}
				return this.series.legendToolTip;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.LegendToolTip, value);
				}
				else
				{
					this.series.legendToolTip = value;
				}
			}
		}

		[SRCategory("CategoryAttributeLabelAppearance")]
		[DefaultValue(typeof(Color), "")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLabelBackColor")]
		public Color LabelBackColor
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.LabelBackColor))
					{
						return (Color)this.GetAttributeObject(CommonAttributes.LabelBackColor);
					}
					if (this.IsSerializing())
					{
						return Color.Empty;
					}
					if (this.emptyPoint)
					{
						return (Color)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.LabelBackColor);
					}
					return this.series.labelBackColor;
				}
				return this.series.labelBackColor;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.LabelBackColor, value);
				}
				else
				{
					this.series.labelBackColor = value;
				}
				this.Invalidate(true);
			}
		}

		[DefaultValue(typeof(Color), "")]
		[SRCategory("CategoryAttributeLabelAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLabelBorderColor")]
		public Color LabelBorderColor
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.LabelBorderColor))
					{
						return (Color)this.GetAttributeObject(CommonAttributes.LabelBorderColor);
					}
					if (this.IsSerializing())
					{
						return Color.Empty;
					}
					if (this.emptyPoint)
					{
						return (Color)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.LabelBorderColor);
					}
					return this.series.labelBorderColor;
				}
				return this.series.labelBorderColor;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.LabelBorderColor, value);
				}
				else
				{
					this.series.labelBorderColor = value;
				}
				this.Invalidate(true);
			}
		}

		[SRDescription("DescriptionAttributeLabelBorderStyle")]
		[DefaultValue(ChartDashStyle.Solid)]
		[SRCategory("CategoryAttributeLabelAppearance")]
		[Bindable(true)]
		public ChartDashStyle LabelBorderStyle
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.LabelBorderDashStyle))
					{
						return (ChartDashStyle)this.GetAttributeObject(CommonAttributes.LabelBorderDashStyle);
					}
					if (this.IsSerializing())
					{
						return ChartDashStyle.Solid;
					}
					if (this.emptyPoint)
					{
						return (ChartDashStyle)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.LabelBorderDashStyle);
					}
					return this.series.labelBorderStyle;
				}
				return this.series.labelBorderStyle;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.LabelBorderDashStyle, value);
				}
				else
				{
					this.series.labelBorderStyle = value;
				}
				this.Invalidate(true);
			}
		}

		[DefaultValue(1)]
		[SRCategory("CategoryAttributeLabelAppearance")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLabelBorderWidth")]
		public int LabelBorderWidth
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.LabelBorderWidth))
					{
						return (int)this.GetAttributeObject(CommonAttributes.LabelBorderWidth);
					}
					if (this.IsSerializing())
					{
						return 1;
					}
					if (this.emptyPoint)
					{
						return (int)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.LabelBorderWidth);
					}
					return this.series.labelBorderWidth;
				}
				return this.series.labelBorderWidth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionLabelBorderIsNotPositive);
				}
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.LabelBorderWidth, value);
				}
				else
				{
					this.series.labelBorderWidth = value;
				}
				this.Invalidate(true);
			}
		}

		[Bindable(true)]
		[DefaultValue("")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAttributeLabel")]
		[SRDescription("DescriptionAttributeLabelToolTip")]
		public string LabelToolTip
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.LabelToolTip))
					{
						return (string)this.GetAttributeObject(CommonAttributes.LabelToolTip);
					}
					if (this.IsSerializing())
					{
						return "";
					}
					if (this.emptyPoint)
					{
						return (string)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.LabelToolTip);
					}
					return this.series.labelToolTip;
				}
				return this.series.labelToolTip;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.LabelToolTip, value);
				}
				else
				{
					this.series.labelToolTip = value;
				}
			}
		}

		[Bindable(true)]
		[DefaultValue("")]
		[SRCategory("CategoryAttributeLegend")]
		[SRDescription("DescriptionAttributeLegendHref")]
		public string LegendHref
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.LegendHref))
					{
						return (string)this.GetAttributeObject(CommonAttributes.LegendHref);
					}
					if (this.IsSerializing())
					{
						return "";
					}
					if (this.emptyPoint)
					{
						return (string)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.LegendHref);
					}
					return this.series.legendHref;
				}
				return this.series.legendHref;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.LegendHref, value);
				}
				else
				{
					this.series.legendHref = value;
				}
			}
		}

		[SRCategory("CategoryAttributeLegend")]
		[SRDescription("DescriptionAttributeLegendMapAreaAttributes")]
		[DefaultValue("")]
		[Bindable(true)]
		public string LegendMapAreaAttributes
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.LegendMapAreaAttributes))
					{
						return (string)this.GetAttributeObject(CommonAttributes.LegendMapAreaAttributes);
					}
					if (this.IsSerializing())
					{
						return "";
					}
					if (this.emptyPoint)
					{
						return (string)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.LegendMapAreaAttributes);
					}
					return this.series.legendMapAreaAttributes;
				}
				return this.series.legendMapAreaAttributes;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.LegendMapAreaAttributes, value);
				}
				else
				{
					this.series.legendMapAreaAttributes = value;
				}
			}
		}

		[DefaultValue("")]
		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeLabelHref")]
		public string LabelHref
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.LabelHref))
					{
						return (string)this.GetAttributeObject(CommonAttributes.LabelHref);
					}
					if (this.IsSerializing())
					{
						return "";
					}
					if (this.emptyPoint)
					{
						return (string)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.LabelHref);
					}
					return this.series.labelHref;
				}
				return this.series.labelHref;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.LabelHref, value);
				}
				else
				{
					this.series.labelHref = value;
				}
			}
		}

		[SRCategory("CategoryAttributeLabel")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeLabelMapAreaAttributes")]
		[Bindable(true)]
		public string LabelMapAreaAttributes
		{
			get
			{
				if (this.pointAttributes)
				{
					if (this.attributes.Count != 0 && this.IsAttributeSet(CommonAttributes.LabelMapAreaAttributes))
					{
						return (string)this.GetAttributeObject(CommonAttributes.LabelMapAreaAttributes);
					}
					if (this.IsSerializing())
					{
						return "";
					}
					if (this.emptyPoint)
					{
						return (string)this.series.EmptyPointStyle.GetAttributeObject(CommonAttributes.LabelMapAreaAttributes);
					}
					return this.series.labelMapAreaAttributes;
				}
				return this.series.labelMapAreaAttributes;
			}
			set
			{
				if (this.pointAttributes)
				{
					this.SetAttributeObject(CommonAttributes.LabelMapAreaAttributes, value);
				}
				else
				{
					this.series.labelMapAreaAttributes = value;
				}
			}
		}

		[Bindable(false)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeEmptyX")]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool EmptyX
		{
			get
			{
				if (this.IsAttributeSet("EmptyX"))
				{
					object attributeObject = this.GetAttributeObject(CommonAttributes.EmptyX);
					if (attributeObject is bool)
					{
						return (bool)attributeObject;
					}
					if (attributeObject is string)
					{
						return bool.Parse((string)attributeObject);
					}
				}
				return false;
			}
			set
			{
				this.SetAttributeObject(CommonAttributes.EmptyX, value);
			}
		}

		[SRDescription("DescriptionAttributeElementId")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Browsable(false)]
		[Bindable(false)]
		public int ElementId
		{
			get
			{
				if (this.IsAttributeSet(CommonAttributes.ElementID))
				{
					object attributeObject = this.GetAttributeObject(CommonAttributes.ElementID);
					if (attributeObject is int)
					{
						return (int)attributeObject;
					}
					if (attributeObject is string)
					{
						return int.Parse((string)attributeObject, CultureInfo.InvariantCulture);
					}
				}
				return 0;
			}
			set
			{
				this.SetAttributeObject(CommonAttributes.ElementID, value);
			}
		}

		public DataPointAttributes()
		{
			this.series = null;
			this.customAttributes = new CustomAttributes(this);
		}

		public DataPointAttributes(Series series, bool pointAttributes)
		{
			this.series = series;
			this.pointAttributes = pointAttributes;
			this.customAttributes = new CustomAttributes(this);
		}

		public virtual bool IsAttributeSet(string name)
		{
			return this.attributes.ContainsKey(name);
		}

		internal bool IsAttributeSet(CommonAttributes attrib)
		{
			return this.attributes.ContainsKey((int)attrib);
		}

		public virtual void DeleteAttribute(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(SR.ExceptionAttributeNameIsEmpty);
			}
			string[] names = Enum.GetNames(typeof(CommonAttributes));
			string[] array = names;
			foreach (string text in array)
			{
				if (name == text)
				{
					this.DeleteAttribute((CommonAttributes)Enum.Parse(typeof(CommonAttributes), text));
				}
			}
			this.attributes.Remove(name);
		}

		internal void DeleteAttribute(CommonAttributes attrib)
		{
			if (!this.pointAttributes)
			{
				throw new ArgumentException(SR.ExceptionAttributeUnableToDelete);
			}
			this.attributes.Remove((int)attrib);
		}

		public virtual string GetAttribute(string name)
		{
			if (!this.IsAttributeSet(name) && this.pointAttributes)
			{
				bool flag = false;
				if (this.series.chart == null && this.series.serviceContainer != null)
				{
					this.series.chart = (Chart)this.series.serviceContainer.GetService(typeof(Chart));
				}
				if (this.series.chart != null && this.series.chart.serializing)
				{
					flag = true;
				}
				if (!flag)
				{
					if (this.emptyPoint)
					{
						return (string)this.series.EmptyPointStyle.attributes[name];
					}
					return (string)this.series.attributes[name];
				}
				return Series.defaultAttributes[name];
			}
			return (string)this.attributes[name];
		}

		internal bool IsSerializing()
		{
			if (this.series == null)
			{
				return true;
			}
			if (this.series.chart == null)
			{
				if (this.series.serviceContainer != null)
				{
					this.series.chart = (Chart)this.series.serviceContainer.GetService(typeof(Chart));
					if (this.series.chart != null)
					{
						return this.series.chart.serializing;
					}
				}
				return false;
			}
			return this.series.chart.serializing;
		}

		internal object GetAttributeObject(CommonAttributes attrib)
		{
			if (this.pointAttributes && this.series != null)
			{
				if (this.attributes.Count != 0 && this.IsAttributeSet(attrib))
				{
					return this.attributes[(int)attrib];
				}
				bool flag = false;
				if (this.series.chart == null)
				{
					if (this.series.serviceContainer != null)
					{
						this.series.chart = (Chart)this.series.serviceContainer.GetService(typeof(Chart));
						if (this.series.chart != null)
						{
							flag = this.series.chart.serializing;
						}
					}
				}
				else
				{
					flag = this.series.chart.serializing;
				}
				if (!flag)
				{
					if (this.emptyPoint)
					{
						return this.series.EmptyPointStyle.attributes[(int)attrib];
					}
					return this.series.attributes[(int)attrib];
				}
				return Series.defaultAttributes.attributes[(int)attrib];
			}
			return this.attributes[(int)attrib];
		}

		public virtual void SetAttribute(string name, string attributeValue)
		{
			this.attributes[name] = attributeValue;
		}

		internal void SetAttributeObject(CommonAttributes attrib, object attributeValue)
		{
			this.attributes[(int)attrib] = attributeValue;
		}

		public virtual void SetDefault(bool clearAll)
		{
			if (!this.pointAttributes)
			{
				if (clearAll)
				{
					this.attributes.Clear();
				}
				if (!this.IsAttributeSet(CommonAttributes.ToolTip))
				{
					this.SetAttributeObject(CommonAttributes.ToolTip, "");
				}
				if (!this.IsAttributeSet(CommonAttributes.LegendToolTip))
				{
					this.SetAttributeObject(CommonAttributes.LegendToolTip, "");
				}
				if (!this.IsAttributeSet(CommonAttributes.Color))
				{
					this.SetAttributeObject(CommonAttributes.Color, Color.Empty);
				}
				if (!this.IsAttributeSet(CommonAttributes.ShowLabelAsValue))
				{
					this.SetAttributeObject(CommonAttributes.ShowLabelAsValue, false);
				}
				if (!this.IsAttributeSet(CommonAttributes.MarkerStyle))
				{
					this.SetAttributeObject(CommonAttributes.MarkerStyle, MarkerStyle.None);
				}
				if (!this.IsAttributeSet(CommonAttributes.MarkerSize))
				{
					this.SetAttributeObject(CommonAttributes.MarkerSize, 5);
				}
				if (!this.IsAttributeSet(CommonAttributes.MarkerImage))
				{
					this.SetAttributeObject(CommonAttributes.MarkerImage, "");
				}
				if (!this.IsAttributeSet(CommonAttributes.Label))
				{
					this.SetAttributeObject(CommonAttributes.Label, "");
				}
				if (!this.IsAttributeSet(CommonAttributes.BorderWidth))
				{
					this.SetAttributeObject(CommonAttributes.BorderWidth, 1);
				}
				if (!this.IsAttributeSet(CommonAttributes.BorderDashStyle))
				{
					this.SetAttributeObject(CommonAttributes.BorderDashStyle, ChartDashStyle.Solid);
				}
				if (!this.IsAttributeSet(CommonAttributes.AxisLabel))
				{
					this.SetAttributeObject(CommonAttributes.AxisLabel, "");
				}
				if (!this.IsAttributeSet(CommonAttributes.LabelFormat))
				{
					this.SetAttributeObject(CommonAttributes.LabelFormat, "");
				}
				if (!this.IsAttributeSet(CommonAttributes.BorderColor))
				{
					this.SetAttributeObject(CommonAttributes.BorderColor, Color.Empty);
				}
				if (!this.IsAttributeSet(CommonAttributes.BackImage))
				{
					this.SetAttributeObject(CommonAttributes.BackImage, "");
				}
				if (!this.IsAttributeSet(CommonAttributes.BackImageMode))
				{
					this.SetAttributeObject(CommonAttributes.BackImageMode, ChartImageWrapMode.Tile);
				}
				if (!this.IsAttributeSet(CommonAttributes.BackImageAlign))
				{
					this.SetAttributeObject(CommonAttributes.BackImageAlign, ChartImageAlign.TopLeft);
				}
				if (!this.IsAttributeSet(CommonAttributes.BackImageTransparentColor))
				{
					this.SetAttributeObject(CommonAttributes.BackImageTransparentColor, Color.Empty);
				}
				if (!this.IsAttributeSet(CommonAttributes.BackGradientType))
				{
					this.SetAttributeObject(CommonAttributes.BackGradientType, GradientType.None);
				}
				if (!this.IsAttributeSet(CommonAttributes.BackGradientEndColor))
				{
					this.SetAttributeObject(CommonAttributes.BackGradientEndColor, Color.Empty);
				}
				if (!this.IsAttributeSet(CommonAttributes.BackHatchStyle))
				{
					this.SetAttributeObject(CommonAttributes.BackHatchStyle, ChartHatchStyle.None);
				}
				if (!this.IsAttributeSet(CommonAttributes.Font))
				{
					this.SetAttributeObject(CommonAttributes.Font, new Font(ChartPicture.GetDefaultFontFamilyName(), 8f));
				}
				if (!this.IsAttributeSet(CommonAttributes.FontColor))
				{
					this.SetAttributeObject(CommonAttributes.FontColor, Color.Black);
				}
				if (!this.IsAttributeSet(CommonAttributes.FontAngle))
				{
					this.SetAttributeObject(CommonAttributes.FontAngle, 0);
				}
				if (!this.IsAttributeSet(CommonAttributes.MarkerImageTransparentColor))
				{
					this.SetAttributeObject(CommonAttributes.MarkerImageTransparentColor, Color.Empty);
				}
				if (!this.IsAttributeSet(CommonAttributes.MarkerColor))
				{
					this.SetAttributeObject(CommonAttributes.MarkerColor, Color.Empty);
				}
				if (!this.IsAttributeSet(CommonAttributes.MarkerBorderColor))
				{
					this.SetAttributeObject(CommonAttributes.MarkerBorderColor, Color.Empty);
				}
				if (!this.IsAttributeSet(CommonAttributes.MarkerBorderWidth))
				{
					this.SetAttributeObject(CommonAttributes.MarkerBorderWidth, 1);
				}
				if (!this.IsAttributeSet(CommonAttributes.MapAreaAttributes))
				{
					this.SetAttributeObject(CommonAttributes.MapAreaAttributes, "");
				}
				if (!this.IsAttributeSet(CommonAttributes.LabelToolTip))
				{
					this.SetAttributeObject(CommonAttributes.LabelToolTip, "");
				}
				if (!this.IsAttributeSet(CommonAttributes.LabelHref))
				{
					this.SetAttributeObject(CommonAttributes.LabelHref, "");
				}
				if (!this.IsAttributeSet(CommonAttributes.LabelMapAreaAttributes))
				{
					this.SetAttributeObject(CommonAttributes.LabelMapAreaAttributes, "");
				}
				if (!this.IsAttributeSet(CommonAttributes.LabelBackColor))
				{
					this.SetAttributeObject(CommonAttributes.LabelBackColor, Color.Empty);
				}
				if (!this.IsAttributeSet(CommonAttributes.LabelBorderWidth))
				{
					this.SetAttributeObject(CommonAttributes.LabelBorderWidth, 1);
				}
				if (!this.IsAttributeSet(CommonAttributes.LabelBorderDashStyle))
				{
					this.SetAttributeObject(CommonAttributes.LabelBorderDashStyle, ChartDashStyle.Solid);
				}
				if (!this.IsAttributeSet(CommonAttributes.LabelBorderColor))
				{
					this.SetAttributeObject(CommonAttributes.LabelBorderColor, Color.Empty);
				}
				if (!this.IsAttributeSet(CommonAttributes.MapAreaID))
				{
					this.SetAttributeObject(CommonAttributes.MapAreaID, 0);
				}
				if (!this.IsAttributeSet(CommonAttributes.ElementID))
				{
					this.SetAttributeObject(CommonAttributes.ElementID, 0);
				}
				if (!this.IsAttributeSet(CommonAttributes.EmptyX))
				{
					this.SetAttributeObject(CommonAttributes.EmptyX, false);
				}
				if (!this.IsAttributeSet(CommonAttributes.Href))
				{
					this.SetAttributeObject(CommonAttributes.Href, "");
				}
				if (!this.IsAttributeSet(CommonAttributes.LegendHref))
				{
					this.SetAttributeObject(CommonAttributes.LegendHref, "");
				}
				if (!this.IsAttributeSet(CommonAttributes.LegendText))
				{
					this.SetAttributeObject(CommonAttributes.LegendText, "");
				}
				if (!this.IsAttributeSet(CommonAttributes.LegendMapAreaAttributes))
				{
					this.SetAttributeObject(CommonAttributes.LegendMapAreaAttributes, "");
				}
				if (!this.IsAttributeSet(CommonAttributes.ShowInLegend))
				{
					this.SetAttributeObject(CommonAttributes.ShowInLegend, true);
				}
			}
			else
			{
				this.attributes.Clear();
			}
		}

		internal void Invalidate(bool invalidateLegend)
		{
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public PropertyDescriptorCollection GetProperties()
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this, true);
			Series series = (this is Series) ? ((Series)this) : this.series;
			if (series != null && series.chart != null)
			{
				PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
				{
					foreach (PropertyDescriptor item in properties)
					{
						if (item.Name == "CustomAttributesEx")
						{
							DynamicPropertyDescriptor value = new DynamicPropertyDescriptor(item, "CustomAttributes");
							propertyDescriptorCollection.Add(value);
						}
						else
						{
							propertyDescriptorCollection.Add(item);
						}
					}
					return propertyDescriptorCollection;
				}
			}
			return properties;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			return this.GetProperties();
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		public DataPointAttributes CloneAttributes()
		{
			DataPointAttributes dataPointAttributes = new DataPointAttributes();
			dataPointAttributes.pointAttributes = this.pointAttributes;
			dataPointAttributes.emptyPoint = this.emptyPoint;
			foreach (object key in this.attributes.Keys)
			{
				dataPointAttributes.attributes.Add(key, this.attributes[key]);
			}
			return dataPointAttributes;
		}
	}
}
