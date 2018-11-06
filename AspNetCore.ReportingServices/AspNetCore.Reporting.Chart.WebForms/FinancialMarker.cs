using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class FinancialMarker
	{
		private FinancialMarkerType markerType;

		private int firstPointIndex;

		private int secondPointIndex;

		private int firstYIndex;

		private int secondYIndex;

		private Color lineColor = Color.Gray;

		private int lineWidth = 1;

		private Color textColor = Color.Black;

		private Font textFont = new Font(ChartPicture.GetDefaultFontFamilyName(), 8f);

		private ChartDashStyle lineStyle = ChartDashStyle.Solid;

		[Bindable(true)]
		[ParenthesizePropertyName(true)]
		[DefaultValue(typeof(FinancialMarkerType), "FibonacciArcs")]
		[SRDescription("DescriptionAttributeFinancialMarker_MarkerType")]
		public FinancialMarkerType MarkerType
		{
			get
			{
				return this.markerType;
			}
			set
			{
				this.markerType = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue("FinancialMarker")]
		[SRDescription("DescriptionAttributeFinancialMarker_Name")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Bindable(false)]
		[Browsable(false)]
		public string Name
		{
			get
			{
				return "FinancialMarker";
			}
		}

		[ParenthesizePropertyName(true)]
		[Bindable(true)]
		[DefaultValue(0)]
		[SRDescription("DescriptionAttributeFinancialMarker_FirstPointIndex")]
		public int FirstPointIndex
		{
			get
			{
				return this.firstPointIndex;
			}
			set
			{
				this.firstPointIndex = value;
			}
		}

		[ParenthesizePropertyName(true)]
		[SRDescription("DescriptionAttributeFinancialMarker_SecondPointIndex")]
		[Bindable(true)]
		[DefaultValue(0)]
		public int SecondPointIndex
		{
			get
			{
				return this.secondPointIndex;
			}
			set
			{
				this.secondPointIndex = value;
			}
		}

		[DefaultValue(0)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeFinancialMarker_FirstYIndex")]
		public int FirstYIndex
		{
			get
			{
				return this.firstYIndex;
			}
			set
			{
				this.firstYIndex = value;
			}
		}

		[Bindable(true)]
		[SRDescription("DescriptionAttributeFinancialMarker_SecondYIndex")]
		[DefaultValue(0)]
		public int SecondYIndex
		{
			get
			{
				return this.secondYIndex;
			}
			set
			{
				this.secondYIndex = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeFinancialMarker_LineColor")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "Gray")]
		public Color LineColor
		{
			get
			{
				return this.lineColor;
			}
			set
			{
				this.lineColor = value;
			}
		}

		[SRDescription("DescriptionAttributeFinancialMarker_TextColor")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "Black")]
		public Color TextColor
		{
			get
			{
				return this.textColor;
			}
			set
			{
				this.textColor = value;
			}
		}

		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		[SRDescription("DescriptionAttributeFinancialMarker_Font")]
		[Bindable(true)]
		[SRCategory("CategoryAttributeAppearance")]
		public Font Font
		{
			get
			{
				return this.textFont;
			}
			set
			{
				this.textFont = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[SRDescription("DescriptionAttributeFinancialMarker_LineWidth")]
		[Bindable(true)]
		[DefaultValue(1)]
		public int LineWidth
		{
			get
			{
				return this.lineWidth;
			}
			set
			{
				this.lineWidth = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(ChartDashStyle.Solid)]
		[SRDescription("DescriptionAttributeFinancialMarker_LineStyle")]
		[Bindable(true)]
		public ChartDashStyle LineStyle
		{
			get
			{
				return this.lineStyle;
			}
			set
			{
				this.lineStyle = value;
			}
		}

		public FinancialMarker()
		{
		}

		public FinancialMarker(FinancialMarkerType markerType, int firstPointIndex, int secondPointIndex, int firstYIndex, int secondYIndex, Color lineColor, int lineWidth, Color textColor, Font textFont)
		{
			this.markerType = markerType;
			this.firstPointIndex = firstPointIndex;
			this.secondPointIndex = secondPointIndex;
			this.firstYIndex = firstYIndex;
			this.secondYIndex = secondYIndex;
			this.lineColor = lineColor;
			this.lineWidth = lineWidth;
			this.textColor = textColor;
			this.textFont = textFont;
		}
	}
}
