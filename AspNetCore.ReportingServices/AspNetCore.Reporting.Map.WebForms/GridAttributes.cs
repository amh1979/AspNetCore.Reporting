using System;
using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class GridAttributes : MapObject, ISelectable
	{
		private GridLine[] gridLines;

		private bool visible = true;

		private Color lineColor = Color.FromArgb(128, 128, 128, 255);

		private int lineWidth = 1;

		private double interval = double.NaN;

		private MapDashStyle lineStyle = MapDashStyle.Solid;

		private Font font = new Font("Microsoft Sans Serif", 8.25f);

		private bool showLabels = true;

		private Color labelColor = Color.FromArgb(192, 128, 128, 255);

		private LabelPosition labelPosition;

		private string labelFormatString = "#°E;#°W;0°";

		internal GridLine[] GridLines
		{
			get
			{
				return this.gridLines;
			}
			set
			{
				this.gridLines = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeGridAttributes_Visible")]
		[ParenthesizePropertyName(true)]
		[NotifyParentProperty(true)]
		public bool Visible
		{
			get
			{
				return this.visible;
			}
			set
			{
				this.visible = value;
				this.InvalidateViewport();
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeGridAttributes_LineColor")]
		[DefaultValue(typeof(Color), "128, 128, 128, 255")]
		[SRCategory("CategoryAttribute_Appearance")]
		public Color LineColor
		{
			get
			{
				return this.lineColor;
			}
			set
			{
				this.lineColor = value;
				this.InvalidateViewport();
			}
		}

		[DefaultValue(1)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeGridAttributes_LineWidth")]
		[SRCategory("CategoryAttribute_Appearance")]
		public int LineWidth
		{
			get
			{
				return this.lineWidth;
			}
			set
			{
				this.lineWidth = value;
				this.InvalidateViewport();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Interval")]
		[DefaultValue(double.NaN)]
		[SRDescription("DescriptionAttributeGridAttributes_Interval")]
		[TypeConverter(typeof(DoubleAutoValueConverter))]
		public double Interval
		{
			get
			{
				return this.interval;
			}
			set
			{
				if (value <= 0.0)
				{
					throw new ArgumentException(SR.ExceptionIntervalGraterThanZero);
				}
				this.interval = value;
				this.InvalidateViewport();
			}
		}

		[SRDescription("DescriptionAttributeGridAttributes_LineStyle")]
		[NotifyParentProperty(true)]
		[DefaultValue(MapDashStyle.Solid)]
		[SRCategory("CategoryAttribute_Appearance")]
		public MapDashStyle LineStyle
		{
			get
			{
				return this.lineStyle;
			}
			set
			{
				this.lineStyle = value;
				this.InvalidateViewport();
			}
		}

		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8.25pt")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeGridAttributes_Font")]
		public Font Font
		{
			get
			{
				return this.font;
			}
			set
			{
				this.font = value;
				this.InvalidateViewport();
			}
		}

		[NotifyParentProperty(true)]
		[DefaultValue(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeGridAttributes_ShowLabels")]
		public bool ShowLabels
		{
			get
			{
				return this.showLabels;
			}
			set
			{
				this.showLabels = value;
				this.InvalidateViewport();
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeGridAttributes_LabelColor")]
		[DefaultValue(typeof(Color), "192, 128, 128, 255")]
		[SRCategory("CategoryAttribute_Appearance")]
		public Color LabelColor
		{
			get
			{
				return this.labelColor;
			}
			set
			{
				this.labelColor = value;
				this.InvalidateViewport();
			}
		}

		[NotifyParentProperty(true)]
		[DefaultValue(LabelPosition.Near)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeGridAttributes_LabelPosition")]
		public LabelPosition LabelPosition
		{
			get
			{
				return this.labelPosition;
			}
			set
			{
				this.labelPosition = value;
				this.InvalidateViewport();
			}
		}

		[DefaultValue("#°E;#°W;0°")]
		[SRDescription("DescriptionAttributeGridAttributes_LabelFormatString")]
		[SRCategory("CategoryAttribute_Appearance")]
		public string LabelFormatString
		{
			get
			{
				return this.labelFormatString;
			}
			set
			{
				this.labelFormatString = value;
				this.InvalidateViewport();
			}
		}

		public GridAttributes()
			: this(null, true)
		{
		}

		public GridAttributes(object parent, bool parallels)
			: base(parent)
		{
			if (parallels)
			{
				this.LabelFormatString = "#°N;#°S;0°";
			}
		}

		internal MapCore GetMapCore()
		{
			return (MapCore)this.Parent;
		}

		internal Pen GetPen()
		{
			Pen pen = new Pen(this.LineColor, (float)this.LineWidth);
			pen.Width = (float)this.LineWidth;
			pen.DashStyle = MapGraphics.GetPenStyle(this.LineStyle);
			return pen;
		}

		void ISelectable.DrawSelection(MapGraphics g, RectangleF clipRect, bool designTimeSelection)
		{
			MapCore mapCore = this.GetMapCore();
			GridLine[] array = this.GridLines;
			for (int i = 0; i < array.Length; i++)
			{
				GridLine gridLine = array[i];
				g.DrawSelectionMarkers(gridLine.SelectionMarkerPositions, designTimeSelection, mapCore.SelectionBorderColor, mapCore.SelectionMarkerColor);
			}
		}

		bool ISelectable.IsSelected()
		{
			return false;
		}

		bool ISelectable.IsVisible()
		{
			return true;
		}

		RectangleF ISelectable.GetSelectionRectangle(MapGraphics g, RectangleF clipRect)
		{
			return RectangleF.Empty;
		}
	}
}
