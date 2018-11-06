using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartDataLabelInstance : BaseInstance
	{
		private ChartDataLabel m_chartDataLabelDef;

		private StyleInstance m_style;

		private string m_formattedValue;

		private AspNetCore.ReportingServices.RdlExpressions.VariantResult? m_originalValue = null;

		private bool? m_useValueAsLabel;

		private ChartDataLabelPositions? m_position;

		private int? m_rotation;

		private bool? m_visible;

		private string m_toolTip;

		public StyleInstance Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new StyleInstance(this.m_chartDataLabelDef, this.m_chartDataLabelDef.ReportScope, this.m_chartDataLabelDef.ChartDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public object OriginalValue
		{
			get
			{
				return this.GetOriginalValue().Value;
			}
		}

		public string Label
		{
			get
			{
				if (this.m_formattedValue == null)
				{
					if (this.m_chartDataLabelDef.ChartDef.IsOldSnapshot)
					{
						this.m_formattedValue = (this.GetOriginalValue().Value as string);
					}
					else
					{
						this.m_formattedValue = this.m_chartDataLabelDef.ChartDataLabelDef.GetFormattedValue(this.GetOriginalValue(), this.ReportScopeInstance, this.m_chartDataLabelDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return this.m_formattedValue;
			}
		}

		public bool UseValueAsLabel
		{
			get
			{
				if (!this.m_useValueAsLabel.HasValue && !this.m_chartDataLabelDef.ChartDef.IsOldSnapshot)
				{
					this.m_useValueAsLabel = this.m_chartDataLabelDef.ChartDataLabelDef.EvaluateUseValueAsLabel(this.ReportScopeInstance, this.m_chartDataLabelDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_useValueAsLabel.Value;
			}
		}

		public string ToolTip
		{
			get
			{
				if (this.m_toolTip == null)
				{
					this.m_toolTip = this.m_chartDataLabelDef.ChartDataLabelDef.EvaluateToolTip(this.ReportScopeInstance, this.m_chartDataLabelDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_toolTip;
			}
		}

		public ChartDataLabelPositions Position
		{
			get
			{
				if (!this.m_position.HasValue && !this.m_chartDataLabelDef.ChartDef.IsOldSnapshot)
				{
					this.m_position = this.m_chartDataLabelDef.ChartDataLabelDef.EvaluatePosition(this.ReportScopeInstance, this.m_chartDataLabelDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_position.Value;
			}
		}

		public int Rotation
		{
			get
			{
				if (!this.m_rotation.HasValue && !this.m_chartDataLabelDef.ChartDef.IsOldSnapshot)
				{
					this.m_rotation = this.m_chartDataLabelDef.ChartDataLabelDef.EvaluateRotation(this.ReportScopeInstance, this.m_chartDataLabelDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_rotation.Value;
			}
		}

		public bool Visible
		{
			get
			{
				if (!this.m_visible.HasValue && !this.m_chartDataLabelDef.ChartDef.IsOldSnapshot)
				{
					this.m_visible = this.m_chartDataLabelDef.ChartDataLabelDef.EvaluateVisible(this.ReportScopeInstance, this.m_chartDataLabelDef.ChartDef.RenderingContext.OdpContext);
				}
				return this.m_visible.Value;
			}
		}

		internal ChartDataLabelInstance(ChartDataLabel chartDataLabelDef)
			: base(chartDataLabelDef.ReportScope)
		{
			this.m_chartDataLabelDef = chartDataLabelDef;
		}

		private AspNetCore.ReportingServices.RdlExpressions.VariantResult GetOriginalValue()
		{
			if (!this.m_originalValue.HasValue)
			{
				if (this.m_chartDataLabelDef.ChartDef.IsOldSnapshot)
				{
					AspNetCore.ReportingServices.ReportProcessing.ChartDataLabel dataLabel = this.m_chartDataLabelDef.ChartDataPoint.RenderDataPointDef.DataLabel;
					if (dataLabel != null)
					{
						if (dataLabel.Value != null && !dataLabel.Value.IsExpression)
						{
							string value = dataLabel.Value.Value;
						}
						else
						{
							string dataLabelValue = this.m_chartDataLabelDef.ChartDataPoint.RenderItem.InstanceInfo.DataLabelValue;
						}
					}
					this.m_originalValue = new AspNetCore.ReportingServices.RdlExpressions.VariantResult(false, dataLabel);
				}
				else
				{
					this.m_originalValue = this.m_chartDataLabelDef.ChartDataLabelDef.EvaluateLabel(this.ReportScopeInstance, this.m_chartDataLabelDef.ChartDef.RenderingContext.OdpContext);
				}
			}
			return this.m_originalValue.Value;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_originalValue = null;
			this.m_formattedValue = null;
			this.m_useValueAsLabel = null;
			this.m_position = null;
			this.m_rotation = null;
			this.m_visible = null;
			this.m_toolTip = null;
		}
	}
}
