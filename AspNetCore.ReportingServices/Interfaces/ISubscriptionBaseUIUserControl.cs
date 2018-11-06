namespace AspNetCore.ReportingServices.Interfaces
{
	internal interface ISubscriptionBaseUIUserControl : IExtension
	{
		Setting[] UserData
		{
			get;
			set;
		}

		string Description
		{
			get;
		}

		IDeliveryReportServerInformation ReportServerInformation
		{
			set;
		}

		bool IsPrivilegedUser
		{
			set;
		}

		bool Validate();
	}
}
