using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartCustomPaletteColor : ChartObjectCollectionItem<ChartCustomPaletteColorInstance>
	{
		private Chart m_chart;

		private ReportColorProperty m_color;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartCustomPaletteColor m_chartCustomPaletteColorDef;

		public ReportColorProperty Color
		{
			get
			{
				if (this.m_color == null && !this.m_chart.IsOldSnapshot && this.m_chartCustomPaletteColorDef.Color != null)
				{
					ExpressionInfo color = this.m_chartCustomPaletteColorDef.Color;
					if (color != null)
					{
						this.m_color = new ReportColorProperty(color.IsExpression, color.OriginalText, color.IsExpression ? null : new ReportColor(color.StringValue.Trim(), true), color.IsExpression ? new ReportColor("", System.Drawing.Color.Empty, true) : null);
					}
				}
				return this.m_color;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_chart;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartCustomPaletteColor ChartCustomPaletteColorDef
		{
			get
			{
				return this.m_chartCustomPaletteColorDef;
			}
		}

		public ChartCustomPaletteColorInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					base.m_instance = new ChartCustomPaletteColorInstance(this);
				}
				return base.m_instance;
			}
		}

		internal ChartCustomPaletteColor(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartCustomPaletteColor chartCustomPaletteColorDef, Chart chart)
		{
			this.m_chart = chart;
			this.m_chartCustomPaletteColorDef = chartCustomPaletteColorDef;
		}
	}
}
