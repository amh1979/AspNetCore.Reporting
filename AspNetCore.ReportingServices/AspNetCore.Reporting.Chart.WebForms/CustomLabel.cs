using AspNetCore.Reporting.Chart.WebForms.Design;
using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeCustomLabel_CustomLabel")]
	[DefaultProperty("Text")]
	internal class CustomLabel
	{
		internal Axis axis;

		private string name = "Custom Label";

		private double from;

		private double to;

		private string text = "";

		private LabelMark labelMark;

		private Color textColor = Color.Empty;

		private Color markColor = Color.Empty;

		private int labelRowIndex;

		private GridTicks gridTick;

		internal bool customLabel = true;

		private object tag;

		private string image = string.Empty;

		private Color imageTranspColor = Color.Empty;

		private string tooltip = string.Empty;

		private string imageHref = string.Empty;

		private string imageMapAreaAttributes = string.Empty;

		private string href = string.Empty;

		private string mapAreaAttributes = string.Empty;

		[DefaultValue("")]
		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeCustomLabel_ToolTip")]
		public string ToolTip
		{
			get
			{
				return this.tooltip;
			}
			set
			{
				this.tooltip = value;
			}
		}

		[SRCategory("CategoryAttributeMapArea")]
		[DefaultValue("")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeCustomLabel_Href")]
		public string Href
		{
			get
			{
				return this.href;
			}
			set
			{
				this.href = value;
			}
		}

		[DefaultValue("")]
		[SRCategory("CategoryAttributeMapArea")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeCustomLabel_MapAreaAttributes")]
		public string MapAreaAttributes
		{
			get
			{
				return this.mapAreaAttributes;
			}
			set
			{
				this.mapAreaAttributes = value;
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeMapArea")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeCustomLabel_ImageHref")]
		public string ImageHref
		{
			get
			{
				return this.imageHref;
			}
			set
			{
				this.imageHref = value;
			}
		}

		[SRCategory("CategoryAttributeMapArea")]
		[DefaultValue("")]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeCustomLabel_ImageMapAreaAttributes")]
		public string ImageMapAreaAttributes
		{
			get
			{
				return this.imageMapAreaAttributes;
			}
			set
			{
				this.imageMapAreaAttributes = value;
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeCustomLabel_Tag")]
		[DefaultValue(null)]
		[Browsable(false)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
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

		[SRCategory("CategoryAttributeAppearance")]
		[NotifyParentProperty(true)]
		[Bindable(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeCustomLabel_Image")]
		public string Image
		{
			get
			{
				return this.image;
			}
			set
			{
				this.image = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeCustomLabel_ImageTransparentColor")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		public Color ImageTransparentColor
		{
			get
			{
				return this.imageTranspColor;
			}
			set
			{
				this.imageTranspColor = value;
				this.Invalidate();
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeCustomLabel_Name")]
		[DefaultValue("Custom Label")]
		[Browsable(false)]
		[DesignOnly(true)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeCustomLabel_GridTicks")]
		[Bindable(true)]
		[DefaultValue(GridTicks.None)]
		public GridTicks GridTicks
		{
			get
			{
				return this.gridTick;
			}
			set
			{
				this.gridTick = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[TypeConverter(typeof(AxisLabelDateValueConverter))]
		[Bindable(true)]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeCustomLabel_From")]
		public double From
		{
			get
			{
				return this.from;
			}
			set
			{
				this.from = value;
				this.Invalidate();
			}
		}

		[TypeConverter(typeof(AxisLabelDateValueConverter))]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(0.0)]
		[SRDescription("DescriptionAttributeCustomLabel_To")]
		public double To
		{
			get
			{
				return this.to;
			}
			set
			{
				this.to = value;
				this.Invalidate();
			}
		}

		[DefaultValue("")]
		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeCustomLabel_Text")]
		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeCustomLabel_TextColor")]
		[NotifyParentProperty(true)]
		public Color TextColor
		{
			get
			{
				return this.textColor;
			}
			set
			{
				this.textColor = value;
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "")]
		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeCustomLabel_MarkColor")]
		[NotifyParentProperty(true)]
		public Color MarkColor
		{
			get
			{
				return this.markColor;
			}
			set
			{
				this.markColor = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeCustomLabel_Row")]
		[Browsable(false)]
		[Bindable(false)]
		[DefaultValue(LabelRow.First)]
		[SRCategory("CategoryAttributeAppearance")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public LabelRow Row
		{
			get
			{
				if (this.axis != null && this.axis.chart != null && this.axis.chart.serializing)
				{
					return LabelRow.First;
				}
				if (this.labelRowIndex != 0)
				{
					return LabelRow.Second;
				}
				return LabelRow.First;
			}
			set
			{
				if (this.labelRowIndex == 0)
				{
					this.labelRowIndex = ((value != 0) ? 1 : 0);
					this.Invalidate();
				}
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeCustomLabel_RowIndex")]
		public int RowIndex
		{
			get
			{
				return this.labelRowIndex;
			}
			set
			{
				if (value < 0)
				{
					throw new InvalidOperationException(SR.ExceptionAxisLabelRowIndexIsNegative);
				}
				this.labelRowIndex = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(LabelMark.None)]
		[SRDescription("DescriptionAttributeCustomLabel_LabelMark")]
		public LabelMark LabelMark
		{
			get
			{
				return this.labelMark;
			}
			set
			{
				this.labelMark = value;
				this.Invalidate();
			}
		}

		public CustomLabel()
		{
		}

		public CustomLabel(double fromPosition, double toPosition, string text, int labelRow, LabelMark mark)
		{
			this.from = fromPosition;
			this.to = toPosition;
			this.text = text;
			this.RowIndex = labelRow;
			this.labelMark = mark;
			this.gridTick = GridTicks.None;
		}

		public CustomLabel(double fromPosition, double toPosition, string text, int labelRow, LabelMark mark, GridTicks gridTick)
		{
			this.from = fromPosition;
			this.to = toPosition;
			this.text = text;
			this.RowIndex = labelRow;
			this.labelMark = mark;
			this.gridTick = gridTick;
		}

		public CustomLabel(double fromPosition, double toPosition, string text, LabelRow row, LabelMark mark)
		{
			this.from = fromPosition;
			this.to = toPosition;
			this.text = text;
			this.RowIndex = ((row != 0) ? 1 : 0);
			this.labelMark = mark;
			this.gridTick = GridTicks.None;
		}

		public CustomLabel(double fromPosition, double toPosition, string text, LabelRow row, LabelMark mark, GridTicks gridTick)
		{
			this.from = fromPosition;
			this.to = toPosition;
			this.text = text;
			this.RowIndex = ((row != 0) ? 1 : 0);
			this.labelMark = mark;
			this.gridTick = gridTick;
		}

		public CustomLabel Clone()
		{
			CustomLabel customLabel = new CustomLabel();
			customLabel.From = this.From;
			customLabel.To = this.To;
			customLabel.Text = this.Text;
			customLabel.TextColor = this.TextColor;
			customLabel.MarkColor = this.MarkColor;
			customLabel.RowIndex = this.RowIndex;
			customLabel.LabelMark = this.LabelMark;
			customLabel.GridTicks = this.GridTicks;
			customLabel.ToolTip = this.ToolTip;
			customLabel.Tag = this.Tag;
			customLabel.Image = this.Image;
			customLabel.ImageTransparentColor = this.ImageTransparentColor;
			customLabel.Href = this.Href;
			customLabel.MapAreaAttributes = this.MapAreaAttributes;
			customLabel.ImageHref = this.ImageHref;
			customLabel.ImageMapAreaAttributes = this.ImageMapAreaAttributes;
			return customLabel;
		}

		public Axis GetAxis()
		{
			return this.axis;
		}

		private void Invalidate()
		{
		}
	}
}
