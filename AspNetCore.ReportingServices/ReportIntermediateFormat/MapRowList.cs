namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class MapRowList : RowList
	{
		internal new MapRow this[int index]
		{
			get
			{
				return (MapRow)base[index];
			}
		}

		public MapRowList()
		{
		}

		internal MapRowList(int capacity)
			: base(capacity)
		{
		}
	}
}
