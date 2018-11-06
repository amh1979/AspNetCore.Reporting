using System.IO;

namespace AspNetCore.ReportingServices.ReportPublishing
{
	internal abstract class ReportUpgradeStrategy
	{
		internal abstract Stream Upgrade(Stream definitionStream);
	}
}
