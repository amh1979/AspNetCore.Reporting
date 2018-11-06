namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	internal sealed class GaugeMemberList : HierarchyNodeList
	{
		internal new GaugeMember this[int index]
		{
			get
			{
				return (GaugeMember)base[index];
			}
		}
	}
}
