using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartArea : ChartObjectCollectionItem<ChartAreaInstance>, IROMStyleDefinitionContainer
	{
		private Chart m_chart;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartArea m_chartAreaDef;

		private Style m_style;

		private ChartAxisCollection m_categoryAxes;

		private ChartAxisCollection m_valueAxes;

		private ChartThreeDProperties m_threeDProperties;

		private ReportBoolProperty m_hidden;

		private ReportEnumProperty<ChartAreaAlignOrientations> m_alignOrientation;

		private ChartAlignType m_chartAlignType;

		private ReportBoolProperty m_equallySizedAxesFont;

		private ChartElementPosition m_chartElementPosition;

		private ChartElementPosition m_chartInnerPlotPosition;

		public string Name
		{
			get
			{
				if (this.m_chart.IsOldSnapshot)
				{
					return "Default";
				}
				return this.m_chartAreaDef.ChartAreaName;
			}
		}

		public Style Style
		{
			get
			{
				if (this.m_style == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						if (this.m_chart.RenderChartDef.PlotArea != null)
						{
							this.m_style = new Style(this.m_chart.RenderChartDef.PlotArea.StyleClass, this.m_chart.ChartInstanceInfo.PlotAreaStyleAttributeValues, this.m_chart.RenderingContext);
						}
					}
					else if (this.m_chartAreaDef.StyleClass != null)
					{
						this.m_style = new Style(this.m_chart, this.m_chart, this.m_chartAreaDef, this.m_chart.RenderingContext);
					}
				}
				return this.m_style;
			}
		}

		public ChartThreeDProperties ThreeDProperties
		{
			get
			{
				if (this.m_threeDProperties == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						if (this.m_chart.RenderChartDef.ThreeDProperties != null)
						{
							this.m_threeDProperties = new ChartThreeDProperties(this.m_chart.RenderChartDef.ThreeDProperties, this.m_chart);
						}
					}
					else if (this.ChartAreaDef.ThreeDProperties != null)
					{
						this.m_threeDProperties = new ChartThreeDProperties(this.ChartAreaDef.ThreeDProperties, this.m_chart);
					}
				}
				return this.m_threeDProperties;
			}
		}

		public ChartAxisCollection CategoryAxes
		{
			get
			{
				if (this.m_categoryAxes == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						if (this.m_chart.RenderChartDef.CategoryAxis != null)
						{
							this.m_categoryAxes = new ChartAxisCollection(this, this.m_chart, true);
						}
					}
					else if (this.m_chartAreaDef.CategoryAxes != null)
					{
						this.m_categoryAxes = new ChartAxisCollection(this, this.m_chart, true);
					}
				}
				return this.m_categoryAxes;
			}
		}

		public ChartAxisCollection ValueAxes
		{
			get
			{
				if (this.m_valueAxes == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						if (this.m_chart.RenderChartDef.ValueAxis != null)
						{
							this.m_valueAxes = new ChartAxisCollection(this, this.m_chart, false);
						}
					}
					else if (this.ChartAreaDef.ValueAxes != null)
					{
						this.m_valueAxes = new ChartAxisCollection(this, this.m_chart, false);
					}
				}
				return this.m_valueAxes;
			}
		}

		public ReportBoolProperty Hidden
		{
			get
			{
				if (this.m_hidden == null && !this.m_chart.IsOldSnapshot && this.m_chartAreaDef.Hidden != null)
				{
					this.m_hidden = new ReportBoolProperty(this.m_chartAreaDef.Hidden);
				}
				return this.m_hidden;
			}
		}

		public ReportEnumProperty<ChartAreaAlignOrientations> AlignOrientation
		{
			get
			{
				if (this.m_alignOrientation == null && !this.m_chart.IsOldSnapshot && this.m_chartAreaDef.AlignOrientation != null)
				{
					this.m_alignOrientation = new ReportEnumProperty<ChartAreaAlignOrientations>(this.m_chartAreaDef.AlignOrientation.IsExpression, this.m_chartAreaDef.AlignOrientation.OriginalText, EnumTranslator.TranslateChartAreaAlignOrientation(this.m_chartAreaDef.AlignOrientation.StringValue, null));
				}
				return this.m_alignOrientation;
			}
		}

		public ChartAlignType ChartAlignType
		{
			get
			{
				if (this.m_chartAlignType == null && !this.m_chart.IsOldSnapshot && this.m_chartAreaDef.ChartAlignType != null)
				{
					this.m_chartAlignType = new ChartAlignType(this.m_chartAreaDef.ChartAlignType, this.m_chart);
				}
				return this.m_chartAlignType;
			}
		}

		public string AlignWithChartArea
		{
			get
			{
				if (this.m_chart.IsOldSnapshot)
				{
					return null;
				}
				return this.m_chartAreaDef.AlignWithChartArea;
			}
		}

		public ReportBoolProperty EquallySizedAxesFont
		{
			get
			{
				if (this.m_equallySizedAxesFont == null && !this.m_chart.IsOldSnapshot && this.m_chartAreaDef.EquallySizedAxesFont != null)
				{
					this.m_equallySizedAxesFont = new ReportBoolProperty(this.m_chartAreaDef.EquallySizedAxesFont);
				}
				return this.m_equallySizedAxesFont;
			}
		}

		public ChartElementPosition ChartElementPosition
		{
			get
			{
				if (this.m_chartElementPosition == null && !this.m_chart.IsOldSnapshot && this.m_chartAreaDef.ChartElementPosition != null)
				{
					this.m_chartElementPosition = new ChartElementPosition(this.m_chartAreaDef.ChartElementPosition, this.m_chart);
				}
				return this.m_chartElementPosition;
			}
		}

		public ChartElementPosition ChartInnerPlotPosition
		{
			get
			{
				if (this.m_chartInnerPlotPosition == null && !this.m_chart.IsOldSnapshot && this.m_chartAreaDef.ChartInnerPlotPosition != null)
				{
					this.m_chartInnerPlotPosition = new ChartElementPosition(this.m_chartAreaDef.ChartInnerPlotPosition, this.m_chart);
				}
				return this.m_chartInnerPlotPosition;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartArea ChartAreaDef
		{
			get
			{
				return this.m_chartAreaDef;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_chart;
			}
		}

		public ChartAreaInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					base.m_instance = new ChartAreaInstance(this);
				}
				return base.m_instance;
			}
		}

		internal ChartArea(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartArea chartAreaDef, Chart chart)
		{
			this.m_chart = chart;
			this.m_chartAreaDef = chartAreaDef;
		}

		internal ChartArea(Chart chart)
		{
			this.m_chart = chart;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			if (this.m_categoryAxes != null)
			{
				this.m_categoryAxes.SetNewContext();
			}
			if (this.m_valueAxes != null)
			{
				this.m_valueAxes.SetNewContext();
			}
			if (this.m_threeDProperties != null)
			{
				this.m_threeDProperties.SetNewContext();
			}
			if (this.m_chartAlignType != null)
			{
				this.m_chartAlignType.SetNewContext();
			}
			if (this.m_chartElementPosition != null)
			{
				this.m_chartElementPosition.SetNewContext();
			}
			if (this.m_chartInnerPlotPosition != null)
			{
				this.m_chartInnerPlotPosition.SetNewContext();
			}
		}
	}
}
