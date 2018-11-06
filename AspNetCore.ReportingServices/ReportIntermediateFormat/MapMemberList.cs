namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class MapMemberList : HierarchyNodeList
	{
		internal new MapMember this[int index]
		{
			get
			{
				return (MapMember)base[index];
			}
		}
	}
}
