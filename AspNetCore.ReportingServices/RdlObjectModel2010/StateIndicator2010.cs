using AspNetCore.ReportingServices.RdlObjectModel;
using AspNetCore.ReportingServices.RdlObjectModel2010.Upgrade;

namespace AspNetCore.ReportingServices.RdlObjectModel2010
{
	internal class StateIndicator2010 : StateIndicator, IUpgradeable2010
	{
		public void Upgrade(UpgradeImpl2010 upgrader)
		{
			if (base.Style != null)
			{
				base.Style.Border = new Border();
				base.Style.Border.Style = BorderStyles.Solid;
			}
		}
	}
}
