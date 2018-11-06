using System.ComponentModel;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class ChartLicense : License
	{
		private ChartLicenseProvider owner;

		private string key;

		public override string LicenseKey
		{
			get
			{
				return this.key;
			}
		}

		public ChartLicense(ChartLicenseProvider owner, string key)
		{
			this.owner = owner;
			this.key = key;
		}

		public override void Dispose()
		{
		}
	}
}
