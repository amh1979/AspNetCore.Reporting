using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class ChartDataPoint : IReportScope, IDataRegionCell, IROMStyleDefinitionContainer
	{
		protected Chart m_owner;

		protected int m_rowIndex;

		protected int m_columnIndex;

		protected ChartDataPointValues m_dataPointValues;

		protected ActionInfo m_actionInfo;

		protected CustomPropertyCollection m_customProperties;

		protected bool m_customPropertiesReady;

		protected ChartDataPointInstance m_instance;

		protected Style m_style;

		protected ChartMarker m_marker;

		protected ChartDataLabel m_dataLabel;

		public abstract DataElementOutputTypes DataElementOutput
		{
			get;
		}

		public abstract string DataElementName
		{
			get;
		}

		public abstract ChartDataPointValues DataPointValues
		{
			get;
		}

		public abstract ChartItemInLegend ItemInLegend
		{
			get;
		}

		public abstract ActionInfo ActionInfo
		{
			get;
		}

		public abstract CustomPropertyCollection CustomProperties
		{
			get;
		}

		public abstract Style Style
		{
			get;
		}

		public abstract ChartMarker Marker
		{
			get;
		}

		public abstract ChartDataLabel DataLabel
		{
			get;
		}

		public abstract ReportVariantProperty AxisLabel
		{
			get;
		}

		public abstract ReportStringProperty ToolTip
		{
			get;
		}

		internal abstract AspNetCore.ReportingServices.ReportIntermediateFormat.ChartDataPoint DataPointDef
		{
			get;
		}

		internal abstract AspNetCore.ReportingServices.ReportRendering.ChartDataPoint RenderItem
		{
			get;
		}

		internal abstract AspNetCore.ReportingServices.ReportProcessing.ChartDataPoint RenderDataPointDef
		{
			get;
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_owner;
			}
		}

		public ChartDataPointInstance Instance
		{
			get
			{
				if (this.m_owner.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new ChartDataPointInstance(this);
				}
				return this.m_instance;
			}
		}

		IReportScopeInstance IReportScope.ReportScopeInstance
		{
			get
			{
				return this.Instance;
			}
		}

		IRIFReportScope IReportScope.RIFReportScope
		{
			get
			{
				return this.RIFReportScope;
			}
		}

		internal virtual IRIFReportScope RIFReportScope
		{
			get
			{
				return null;
			}
		}

		internal ChartDataPoint(Chart owner, int rowIndex, int colIndex)
		{
			this.m_owner = owner;
			this.m_rowIndex = rowIndex;
			this.m_columnIndex = colIndex;
			this.m_dataPointValues = null;
			this.m_actionInfo = null;
			this.m_customProperties = null;
		}

		void IDataRegionCell.SetNewContext()
		{
			this.SetNewContext();
		}

		internal virtual void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
			this.m_customPropertiesReady = false;
			if (this.m_actionInfo != null)
			{
				this.m_actionInfo.SetNewContext();
			}
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			if (this.m_marker != null)
			{
				this.m_marker.SetNewContext();
			}
			if (this.m_dataLabel != null)
			{
				this.m_dataLabel.SetNewContext();
			}
			if (this.m_dataPointValues != null)
			{
				this.m_dataPointValues.SetNewContext();
			}
		}
	}
}
