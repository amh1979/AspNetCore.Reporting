using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class GaugeRowCollection : IDataRegionRowCollection
	{
		private GaugePanel m_owner;

		private GaugeRow m_gaugeRow;

		private GaugeRowList m_gaugeRowCollectionDefs;

		public GaugeRow GaugeRow
		{
			get
			{
				if (this.m_gaugeRow == null && this.m_gaugeRowCollectionDefs.Count == 1)
				{
					this.m_gaugeRow = new GaugeRow(this.m_owner, this.m_gaugeRowCollectionDefs[0]);
				}
				return this.m_gaugeRow;
			}
		}

		public int Count
		{
			get
			{
				return 1;
			}
		}

		internal GaugeRowCollection(GaugePanel owner, GaugeRowList gaugeRowCollectionDefs)
		{
			this.m_owner = owner;
			this.m_gaugeRowCollectionDefs = gaugeRowCollectionDefs;
		}

		internal GaugeRowCollection(GaugePanel owner)
		{
			this.m_owner = owner;
		}

		internal void SetNewContext()
		{
			if (this.m_gaugeRow != null)
			{
				this.m_gaugeRow.SetNewContext();
			}
		}

		IDataRegionRow IDataRegionRowCollection.GetIfExists(int rowIndex)
		{
			if (rowIndex == 0)
			{
				return this.GaugeRow;
			}
			return null;
		}
	}
}
