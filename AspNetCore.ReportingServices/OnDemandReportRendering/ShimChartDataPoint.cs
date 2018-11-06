using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimChartDataPoint : ChartDataPoint
	{
		private AspNetCore.ReportingServices.ReportRendering.ChartDataPoint m_renderDataPoint;

		private bool m_dataValueUpdateNeeded;

		private DataValueCollection m_dataValues;

		private ShimChartMember m_seriesParentMember;

		private ShimChartMember m_categoryParentMember;

		private AspNetCore.ReportingServices.ReportProcessing.ChartDataPoint m_cachedDataPoint;

		public override string DataElementName
		{
			get
			{
				return this.CachedRenderDataPoint.DataElementName;
			}
		}

		public override DataElementOutputTypes DataElementOutput
		{
			get
			{
				if (this.CachedRenderDataPoint.DataElementOutput == AspNetCore.ReportingServices.ReportRendering.DataElementOutputTypes.Output)
				{
					return DataElementOutputTypes.ContentsOnly;
				}
				return DataElementOutputTypes.NoOutput;
			}
		}

		internal DataValueCollection DataValues
		{
			get
			{
				if (this.m_dataValues == null)
				{
					this.m_dataValues = new DataValueCollection(base.m_owner.RenderingContext, this.CachedRenderDataPoint);
				}
				else if (this.m_dataValueUpdateNeeded)
				{
					this.m_dataValueUpdateNeeded = false;
					this.m_dataValues.UpdateChartDataValues(this.CachedRenderDataPoint.DataValues);
				}
				return this.m_dataValues;
			}
		}

		public override ChartDataPointValues DataPointValues
		{
			get
			{
				if (base.m_dataPointValues == null)
				{
					base.m_dataPointValues = new ChartDataPointValues(this, base.ChartDef);
				}
				return base.m_dataPointValues;
			}
		}

		public override ChartItemInLegend ItemInLegend
		{
			get
			{
				return null;
			}
		}

		public override ActionInfo ActionInfo
		{
			get
			{
				if (base.m_actionInfo == null && this.CachedRenderDataPoint.ActionInfo != null)
				{
					base.m_actionInfo = new ActionInfo(base.m_owner.RenderingContext, this.CachedRenderDataPoint.ActionInfo);
				}
				return base.m_actionInfo;
			}
		}

		public override CustomPropertyCollection CustomProperties
		{
			get
			{
				if (base.m_customProperties == null && this.CachedRenderDataPoint.CustomProperties != null && 0 < this.CachedRenderDataPoint.CustomProperties.Count)
				{
					base.m_customProperties = new CustomPropertyCollection(base.m_owner.RenderingContext, this.CachedRenderDataPoint.CustomProperties);
				}
				return base.m_customProperties;
			}
		}

		public override Style Style
		{
			get
			{
				if (base.m_style == null)
				{
					base.m_style = new Style(this.CachedDataPoint.StyleClass, this.CachedRenderDataPoint.InstanceInfo.StyleAttributeValues, base.ChartDef.RenderingContext);
				}
				return base.m_style;
			}
		}

		public override ChartMarker Marker
		{
			get
			{
				if (base.m_marker == null)
				{
					base.m_marker = new ChartMarker(this, base.ChartDef);
				}
				return base.m_marker;
			}
		}

		public override ChartDataLabel DataLabel
		{
			get
			{
				if (base.m_dataLabel == null)
				{
					base.m_dataLabel = new ChartDataLabel(this, base.ChartDef);
				}
				return base.m_dataLabel;
			}
		}

		public override ReportVariantProperty AxisLabel
		{
			get
			{
				return null;
			}
		}

		public override ReportStringProperty ToolTip
		{
			get
			{
				return null;
			}
		}

		internal override AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint DataPointDef
		{
			get
			{
				return null;
			}
		}

		internal override AspNetCore.ReportingServices.ReportRendering.ChartDataPoint RenderItem
		{
			get
			{
				return this.CachedRenderDataPoint;
			}
		}

		internal override AspNetCore.ReportingServices.ReportProcessing.ChartDataPoint RenderDataPointDef
		{
			get
			{
				return this.CachedDataPoint;
			}
		}

		private AspNetCore.ReportingServices.ReportRendering.ChartDataPoint CachedRenderDataPoint
		{
			get
			{
				if (this.m_renderDataPoint == null)
				{
					int cachedMemberDataPointIndex = this.m_seriesParentMember.CurrentRenderChartMember.CachedMemberDataPointIndex;
					int cachedMemberDataPointIndex2 = this.m_categoryParentMember.CurrentRenderChartMember.CachedMemberDataPointIndex;
					this.m_renderDataPoint = base.m_owner.RenderChart.DataPointCollection[cachedMemberDataPointIndex, cachedMemberDataPointIndex2];
					if (base.m_actionInfo != null)
					{
						base.m_actionInfo.Update(this.m_renderDataPoint.ActionInfo);
					}
					if (base.m_customProperties != null)
					{
						base.m_customProperties.UpdateCustomProperties(this.m_renderDataPoint.CustomProperties);
					}
				}
				return this.m_renderDataPoint;
			}
		}

		private AspNetCore.ReportingServices.ReportProcessing.ChartDataPoint CachedDataPoint
		{
			get
			{
				if (this.m_cachedDataPoint == null)
				{
					int memberCellIndex = this.m_seriesParentMember.MemberCellIndex;
					int memberCellIndex2 = this.m_categoryParentMember.MemberCellIndex;
					this.m_cachedDataPoint = ((AspNetCore.ReportingServices.ReportProcessing.Chart)base.m_owner.RenderChart.ReportItemDef).GetDataPoint(memberCellIndex, memberCellIndex2);
				}
				return this.m_cachedDataPoint;
			}
		}

		internal ShimChartDataPoint(Chart owner, int rowIndex, int colIndex, ShimChartMember seriesParentMember, ShimChartMember categoryParentMember)
			: base(owner, rowIndex, colIndex)
		{
			this.m_dataValues = null;
			this.m_seriesParentMember = seriesParentMember;
			this.m_categoryParentMember = categoryParentMember;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (this.m_dataValues != null)
			{
				this.m_dataValues.SetNewContext();
			}
			this.m_renderDataPoint = null;
			this.m_dataValueUpdateNeeded = true;
			this.m_cachedDataPoint = null;
		}
	}
}
