namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class GaugeRowList : RowList
	{
		internal new GaugeRow this[int index]
		{
			get
			{
				return (GaugeRow)base[index];
			}
		}

		public GaugeRowList()
		{
		}

		internal GaugeRowList(int capacity)
			: base(capacity)
		{
		}
	}
}
