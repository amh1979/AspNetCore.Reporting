using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeCell : IReportScope, IDataRegionCell
	{
		private GaugePanel m_owner;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeCell m_gaugeCellDef;

		private GaugeCellInstance m_instance;

		private List<GaugeInputValue> m_gaugeInputValues;

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeCell GaugeCellDef
		{
			get
			{
				return this.m_gaugeCellDef;
			}
		}

		internal GaugePanel GaugePanelDef
		{
			get
			{
				return this.m_owner;
			}
		}

		public GaugeCellInstance Instance
		{
			get
			{
				if (this.m_owner.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new GaugeCellInstance(this);
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
				return this.GaugeCellDef;
			}
		}

		internal GaugeCell(GaugePanel owner, AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeCell gaugeCellDef)
		{
			this.m_owner = owner;
			this.m_gaugeCellDef = gaugeCellDef;
		}

		void IDataRegionCell.SetNewContext()
		{
			this.SetNewContext();
		}

		internal void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
			List<GaugeInputValue> gaugeInputValues = this.GetGaugeInputValues();
			if (gaugeInputValues != null)
			{
				foreach (GaugeInputValue item in gaugeInputValues)
				{
					item.SetNewContext();
				}
			}
			if (this.m_gaugeCellDef != null)
			{
				this.m_gaugeCellDef.ClearStreamingScopeInstanceBinding();
			}
		}

		private List<GaugeInputValue> GetGaugeInputValues()
		{
			if (this.m_gaugeInputValues == null)
			{
				this.m_gaugeInputValues = this.m_owner.GetGaugeInputValues();
			}
			return this.m_gaugeInputValues;
		}
	}
}
