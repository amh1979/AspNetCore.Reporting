namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class RDLUpgradeResult
	{
		private bool hasUnsupportedDundasChartFeatures;

		private bool hasUnsupportedDundasGaugeFeatures;

		public bool HasUnsupportedDundasCRIFeatures
		{
			get
			{
				if (!this.HasUnsupportedDundasChartFeatures)
				{
					return this.HasUnsupportedDundasGaugeFeatures;
				}
				return true;
			}
		}

		public bool HasUnsupportedDundasChartFeatures
		{
			get
			{
				return this.hasUnsupportedDundasChartFeatures;
			}
			internal set
			{
				this.hasUnsupportedDundasChartFeatures = value;
			}
		}

		public bool HasUnsupportedDundasGaugeFeatures
		{
			get
			{
				return this.hasUnsupportedDundasGaugeFeatures;
			}
			internal set
			{
				this.hasUnsupportedDundasGaugeFeatures = value;
			}
		}
	}
}
