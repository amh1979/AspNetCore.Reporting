using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeRow : IDataRegionRow
	{
		private GaugePanel m_gaugePanel;

		private GaugeCell m_cell;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeRow m_rowDef;

		public GaugeCell GaugeCell
		{
			get
			{
				if (this.m_cell == null && this.m_rowDef.GaugeCell != null)
				{
					this.m_cell = new GaugeCell(this.m_gaugePanel, this.m_rowDef.GaugeCell);
				}
				return this.m_cell;
			}
		}

		int IDataRegionRow.Count
		{
			get
			{
				return 1;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeRow GaugeRowDef
		{
			get
			{
				return this.m_rowDef;
			}
		}

		internal GaugePanel GaugePanelDef
		{
			get
			{
				return this.m_gaugePanel;
			}
		}

		internal GaugeRow(GaugePanel gaugePanel)
		{
			this.m_gaugePanel = gaugePanel;
		}

		internal GaugeRow(GaugePanel gaugePanel, AspNetCore.ReportingServices.ReportIntermediateFormat.GaugeRow rowDef)
		{
			this.m_gaugePanel = gaugePanel;
			this.m_rowDef = rowDef;
		}

		IDataRegionCell IDataRegionRow.GetIfExists(int columnIndex)
		{
			if (columnIndex == 0)
			{
				return this.GaugeCell;
			}
			return null;
		}

		internal void SetNewContext()
		{
			if (this.m_cell != null)
			{
				this.m_cell.SetNewContext();
			}
		}
	}
}
