namespace AspNetCore.ReportingServices.DataExtensions
{
	internal sealed class SecureStoreLookup
	{
		internal enum LookupContextOptions
		{
			AuthenticatedUser,
			Unattended
		}

		public readonly string m_targetApplicationId;

		public readonly LookupContextOptions m_lookUpContext;

		public LookupContextOptions LookupContext
		{
			get
			{
				return this.m_lookUpContext;
			}
		}

		public string TargetApplicationId
		{
			get
			{
				return this.m_targetApplicationId;
			}
		}

		internal SecureStoreLookup(LookupContextOptions lookupContext, string targetApplicationId)
		{
			this.m_lookUpContext = lookupContext;
			this.m_targetApplicationId = targetApplicationId;
		}
	}
}
