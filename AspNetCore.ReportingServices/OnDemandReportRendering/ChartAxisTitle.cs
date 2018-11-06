using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartAxisTitle : ChartObjectCollectionItem<ChartAxisTitleInstance>, IROMStyleDefinitionContainer
	{
		private Chart m_chart;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxisTitle m_chartAxisTitleDef;

		private Style m_style;

		private ReportStringProperty m_caption;

		private AspNetCore.ReportingServices.ReportProcessing.ChartTitle m_renderChartTitleDef;

		private AspNetCore.ReportingServices.ReportProcessing.ChartTitleInstance m_renderChartTitleInstance;

		private ReportEnumProperty<ChartAxisTitlePositions> m_position;

		private ReportEnumProperty<TextOrientations> m_textOrientation;

		public ReportStringProperty Caption
		{
			get
			{
				if (this.m_caption == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						if (this.m_renderChartTitleDef.Caption != null)
						{
							this.m_caption = new ReportStringProperty(this.m_renderChartTitleDef.Caption);
						}
					}
					else if (this.m_chartAxisTitleDef.Caption != null)
					{
						this.m_caption = new ReportStringProperty(this.m_chartAxisTitleDef.Caption);
					}
				}
				return this.m_caption;
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
						this.m_style = new Style(this.m_renderChartTitleDef.StyleClass, this.m_renderChartTitleInstance.StyleAttributeValues, this.m_chart.RenderingContext);
					}
					else if (this.m_chartAxisTitleDef.StyleClass != null)
					{
						this.m_style = new Style(this.m_chart, this.m_chart, this.m_chartAxisTitleDef, this.m_chart.RenderingContext);
					}
				}
				return this.m_style;
			}
		}

		public ReportEnumProperty<ChartAxisTitlePositions> Position
		{
			get
			{
				if (this.m_position == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						ChartAxisTitlePositions value = ChartAxisTitlePositions.Center;
						switch (this.m_renderChartTitleDef.Position)
						{
						case AspNetCore.ReportingServices.ReportProcessing.ChartTitle.Positions.Center:
							value = ChartAxisTitlePositions.Center;
							break;
						case AspNetCore.ReportingServices.ReportProcessing.ChartTitle.Positions.Near:
							value = ChartAxisTitlePositions.Near;
							break;
						case AspNetCore.ReportingServices.ReportProcessing.ChartTitle.Positions.Far:
							value = ChartAxisTitlePositions.Far;
							break;
						}
						this.m_position = new ReportEnumProperty<ChartAxisTitlePositions>(value);
					}
					else if (this.m_chartAxisTitleDef.Position != null)
					{
						this.m_position = new ReportEnumProperty<ChartAxisTitlePositions>(this.m_chartAxisTitleDef.Position.IsExpression, this.m_chartAxisTitleDef.Position.OriginalText, EnumTranslator.TranslateChartAxisTitlePosition(this.m_chartAxisTitleDef.Position.StringValue, null));
					}
				}
				return this.m_position;
			}
		}

		public ReportEnumProperty<TextOrientations> TextOrientation
		{
			get
			{
				if (this.m_textOrientation == null && !this.m_chart.IsOldSnapshot && this.m_chartAxisTitleDef.TextOrientation != null)
				{
					this.m_textOrientation = new ReportEnumProperty<TextOrientations>(this.m_chartAxisTitleDef.TextOrientation.IsExpression, this.m_chartAxisTitleDef.TextOrientation.OriginalText, EnumTranslator.TranslateTextOrientations(this.m_chartAxisTitleDef.TextOrientation.StringValue, null));
				}
				return this.m_textOrientation;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_chart;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxisTitle ChartAxisTitleDef
		{
			get
			{
				return this.m_chartAxisTitleDef;
			}
		}

		internal AspNetCore.ReportingServices.ReportProcessing.ChartTitleInstance RenderChartTitleInstance
		{
			get
			{
				return this.m_renderChartTitleInstance;
			}
		}

		public ChartAxisTitleInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					base.m_instance = new ChartAxisTitleInstance(this);
				}
				return base.m_instance;
			}
		}

		internal ChartAxisTitle(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartAxisTitle chartAxisTitleDef, Chart chart)
		{
			this.m_chart = chart;
			this.m_chartAxisTitleDef = chartAxisTitleDef;
		}

		internal ChartAxisTitle(AspNetCore.ReportingServices.ReportProcessing.ChartTitle renderChartTitleDef, AspNetCore.ReportingServices.ReportProcessing.ChartTitleInstance renderChartTitleInstance, Chart chart)
		{
			this.m_chart = chart;
			this.m_renderChartTitleDef = renderChartTitleDef;
			this.m_renderChartTitleInstance = renderChartTitleInstance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
		}
	}
}
