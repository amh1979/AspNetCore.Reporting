namespace AspNetCore.ReportingServices.Interfaces
{
	internal interface IUserContext
	{
		string UserName
		{
			get;
		}

		object Token
		{
			get;
		}

		AuthenticationType AuthenticationType
		{
			get;
		}
	}
}
