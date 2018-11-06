namespace AspNetCore.ReportingServices.Interfaces
{
	internal interface IWindowsAuthenticationExtension : IAuthenticationExtension, IExtension
	{
		byte[] PrincipalNameToSid(string name);

		string SidToPrincipalName(byte[] sid);
	}
}
