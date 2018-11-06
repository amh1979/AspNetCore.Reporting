using AspNetCore.ReportingServices.RdlObjectModel;
using AspNetCore.ReportingServices.RdlObjectModel2005.Upgrade;

namespace AspNetCore.ReportingServices.RdlObjectModel2005
{
	internal class DataSource2005 : DataSource, IUpgradeable
	{
		public DataSource2005()
		{
		}

		public DataSource2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public void Upgrade(UpgradeImpl2005 upgrader)
		{
			upgrader.UpgradeDataSource(this);
		}
	}
}
