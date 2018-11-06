namespace AspNetCore.ReportingServices.Interfaces
{
	internal interface IWindowsAuthenticationExtension2 : IAuthenticationExtension2, IExtension
	{
		byte[] PrincipalNameToSid(string name);

		string SidToPrincipalName(byte[] sid);
	}
}
