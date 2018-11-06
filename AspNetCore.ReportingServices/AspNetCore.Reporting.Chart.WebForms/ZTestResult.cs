namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class ZTestResult
	{
		internal double firstSeriesMean;

		internal double secondSeriesMean;

		internal double firstSeriesVariance;

		internal double secondSeriesVariance;

		internal double zValue;

		internal double probabilityZOneTail;

		internal double zCriticalValueOneTail;

		internal double probabilityZTwoTail;

		internal double zCriticalValueTwoTail;

		public double FirstSeriesMean
		{
			get
			{
				return this.firstSeriesMean;
			}
		}

		public double SecondSeriesMean
		{
			get
			{
				return this.secondSeriesMean;
			}
		}

		public double FirstSeriesVariance
		{
			get
			{
				return this.firstSeriesVariance;
			}
		}

		public double SecondSeriesVariance
		{
			get
			{
				return this.secondSeriesVariance;
			}
		}

		public double ZValue
		{
			get
			{
				return this.zValue;
			}
		}

		public double ProbabilityZOneTail
		{
			get
			{
				return this.probabilityZOneTail;
			}
		}

		public double ZCriticalValueOneTail
		{
			get
			{
				return this.zCriticalValueOneTail;
			}
		}

		public double ProbabilityZTwoTail
		{
			get
			{
				return this.probabilityZTwoTail;
			}
		}

		public double ZCriticalValueTwoTail
		{
			get
			{
				return this.zCriticalValueTwoTail;
			}
		}
	}
}
