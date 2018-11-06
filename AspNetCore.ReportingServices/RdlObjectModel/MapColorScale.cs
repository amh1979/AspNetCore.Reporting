using System.Globalization;

namespace AspNetCore.ReportingServices.RdlObjectModel
{
	internal class MapColorScale : MapDockableSubItem
	{
		internal new class Definition : DefinitionStore<MapColorScale, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				MapLocation,
				MapSize,
				LeftMargin,
				RightMargin,
				TopMargin,
				BottomMargin,
				ZIndex,
				ActionInfo,
				MapPosition,
				DockOutsideViewport,
				Hidden,
				ToolTip,
				MapColorScaleTitle,
				TickMarkLength,
				ColorBarBorderColor,
				LabelInterval,
				LabelFormat,
				LabelPlacement,
				LabelBehavior,
				HideEndLabels,
				RangeGapColor,
				NoDataText,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public MapColorScaleTitle MapColorScaleTitle
		{
			get
			{
				return (MapColorScaleTitle)base.PropertyStore.GetObject(13);
			}
			set
			{
				base.PropertyStore.SetObject(13, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportSize), "2.25pt")]
		public ReportExpression<ReportSize> TickMarkLength
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportSize>>(14);
			}
			set
			{
				base.PropertyStore.SetObject(14, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportColor), "Black")]
		public ReportExpression<ReportColor> ColorBarBorderColor
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(15);
			}
			set
			{
				base.PropertyStore.SetObject(15, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(int), "1")]
		public ReportExpression<int> LabelInterval
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<int>>(16);
			}
			set
			{
				base.PropertyStore.SetObject(16, value);
			}
		}

		[ReportExpressionDefaultValue]
		public ReportExpression LabelFormat
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(17);
			}
			set
			{
				base.PropertyStore.SetObject(17, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(MapLabelPlacements), MapLabelPlacements.Alternate)]
		public ReportExpression<MapLabelPlacements> LabelPlacement
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<MapLabelPlacements>>(18);
			}
			set
			{
				base.PropertyStore.SetObject(18, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(MapLabelBehaviors), MapLabelBehaviors.Auto)]
		public ReportExpression<MapLabelBehaviors> LabelBehavior
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<MapLabelBehaviors>>(19);
			}
			set
			{
				base.PropertyStore.SetObject(19, value);
			}
		}

		public ReportExpression<bool> HideEndLabels
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<bool>>(20);
			}
			set
			{
				base.PropertyStore.SetObject(20, value);
			}
		}

		[ReportExpressionDefaultValue(typeof(ReportColor), "White")]
		public ReportExpression<ReportColor> RangeGapColor
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression<ReportColor>>(21);
			}
			set
			{
				base.PropertyStore.SetObject(21, value);
			}
		}

		public ReportExpression NoDataText
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(22);
			}
			set
			{
				base.PropertyStore.SetObject(22, value);
			}
		}

		public MapColorScale()
		{
		}

		internal MapColorScale(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			this.TickMarkLength = new ReportExpression<ReportSize>("2.25pt", CultureInfo.InvariantCulture);
			this.ColorBarBorderColor = new ReportExpression<ReportColor>("Black", CultureInfo.InvariantCulture);
			this.LabelInterval = 1;
			this.LabelFormat = "#,##0.##";
			this.LabelPlacement = MapLabelPlacements.Alternate;
			this.LabelBehavior = MapLabelBehaviors.Auto;
			this.RangeGapColor = new ReportExpression<ReportColor>("White", CultureInfo.InvariantCulture);
		}
	}
}
