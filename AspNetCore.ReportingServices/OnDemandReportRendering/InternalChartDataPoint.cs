using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalChartDataPoint : ChartDataPoint, IROMActionOwner
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint m_dataPointDef;

		private ReportVariantProperty m_axisLabel;

		private ChartItemInLegend m_itemInLegend;

		private ReportStringProperty m_toolTip;

		public override string DataElementName
		{
			get
			{
				return this.m_dataPointDef.DataElementName;
			}
		}

		public override DataElementOutputTypes DataElementOutput
		{
			get
			{
				return this.m_dataPointDef.DataElementOutput;
			}
		}

		public override ChartDataPointValues DataPointValues
		{
			get
			{
				if (base.m_dataPointValues == null && this.m_dataPointDef.DataPointValues != null)
				{
					base.m_dataPointValues = new ChartDataPointValues(this, this.m_dataPointDef.DataPointValues, base.m_owner);
				}
				return base.m_dataPointValues;
			}
		}

		public override ChartItemInLegend ItemInLegend
		{
			get
			{
				if (this.m_dataPointDef.ItemInLegend != null)
				{
					this.m_itemInLegend = new ChartItemInLegend(this, this.m_dataPointDef.ItemInLegend, base.m_owner);
				}
				return this.m_itemInLegend;
			}
		}

		public string UniqueName
		{
			get
			{
				return this.m_dataPointDef.UniqueName;
			}
		}

		public override ActionInfo ActionInfo
		{
			get
			{
				if (base.m_actionInfo == null && this.m_dataPointDef.Action != null)
				{
					base.m_actionInfo = new ActionInfo(base.m_owner.RenderingContext, this, this.m_dataPointDef.Action, this.m_dataPointDef, base.m_owner, ObjectType.Chart, this.m_dataPointDef.Name, this);
				}
				return base.m_actionInfo;
			}
		}

		public List<string> FieldsUsedInValueExpression
		{
			get
			{
				return null;
			}
		}

		public override CustomPropertyCollection CustomProperties
		{
			get
			{
				if (base.m_customProperties == null)
				{
					base.m_customPropertiesReady = true;
					base.m_customProperties = new CustomPropertyCollection(base.Instance, base.m_owner.RenderingContext, null, this.m_dataPointDef, ObjectType.Chart, base.m_owner.Name);
				}
				else if (!base.m_customPropertiesReady)
				{
					base.m_customPropertiesReady = true;
					base.m_customProperties.UpdateCustomProperties(base.Instance, this.m_dataPointDef, base.m_owner.RenderingContext.OdpContext, ObjectType.Chart, base.m_owner.Name);
				}
				return base.m_customProperties;
			}
		}

		public override Style Style
		{
			get
			{
				if (base.m_style == null && this.m_dataPointDef.StyleClass != null)
				{
					base.m_style = new Style(base.m_owner, this, this.m_dataPointDef, base.ChartDef.RenderingContext);
				}
				return base.m_style;
			}
		}

		public override ChartMarker Marker
		{
			get
			{
				if (base.m_marker == null && this.m_dataPointDef.Marker != null)
				{
					base.m_marker = new ChartMarker(this, this.m_dataPointDef.Marker, base.m_owner);
				}
				return base.m_marker;
			}
		}

		public override ChartDataLabel DataLabel
		{
			get
			{
				if (base.m_dataLabel == null && this.m_dataPointDef.DataLabel != null)
				{
					base.m_dataLabel = new ChartDataLabel(this, this.m_dataPointDef.DataLabel, base.m_owner);
				}
				return base.m_dataLabel;
			}
		}

		public override ReportVariantProperty AxisLabel
		{
			get
			{
				if (this.m_axisLabel == null && this.m_dataPointDef.AxisLabel != null)
				{
					this.m_axisLabel = new ReportVariantProperty(this.m_dataPointDef.AxisLabel);
				}
				return this.m_axisLabel;
			}
		}

		public override ReportStringProperty ToolTip
		{
			get
			{
				if (this.m_toolTip == null && this.m_dataPointDef.ToolTip != null)
				{
					this.m_toolTip = new ReportStringProperty(this.m_dataPointDef.ToolTip);
				}
				return this.m_toolTip;
			}
		}

		internal override AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint DataPointDef
		{
			get
			{
				return this.m_dataPointDef;
			}
		}

		internal override AspNetCore.ReportingServices.ReportRendering.ChartDataPoint RenderItem
		{
			get
			{
				return null;
			}
		}

		internal override AspNetCore.ReportingServices.ReportProcessing.ChartDataPoint RenderDataPointDef
		{
			get
			{
				return null;
			}
		}

		internal override IRIFReportScope RIFReportScope
		{
			get
			{
				return this.m_dataPointDef;
			}
		}

		internal InternalChartDataPoint(Chart owner, int rowIndex, int colIndex, AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint dataPointDef)
			: base(owner, rowIndex, colIndex)
		{
			this.m_dataPointDef = dataPointDef;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (this.m_itemInLegend != null)
			{
				this.m_itemInLegend.SetNewContext();
			}
			if (this.m_dataPointDef != null)
			{
				this.m_dataPointDef.ClearStreamingScopeInstanceBinding();
			}
		}
	}
}
