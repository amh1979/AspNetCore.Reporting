using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlObjectModel
{
	internal interface IHierarchy
	{
		IEnumerable<IHierarchyMember> Members
		{
			get;
		}
	}
}
