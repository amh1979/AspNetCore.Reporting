namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class GaugeCellList : CellList
	{
		internal new GaugeCell this[int index]
		{
			get
			{
				return (GaugeCell)base[index];
			}
		}
	}
}
