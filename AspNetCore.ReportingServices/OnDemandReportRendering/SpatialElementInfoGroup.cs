using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class SpatialElementInfoGroup
	{
		public List<SpatialElementInfo> Elements = new List<SpatialElementInfo>();

		public bool BoundToData;
	}
}
