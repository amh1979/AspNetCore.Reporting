namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class FTestResult
	{
		internal double firstSeriesMean;

		internal double secondSeriesMean;

		internal double firstSeriesVariance;

		internal double secondSeriesVariance;

		internal double fValue;

		internal double probabilityFOneTail;

		internal double fCriticalValueOneTail;

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

		public double FValue
		{
			get
			{
				return this.fValue;
			}
		}

		public double ProbabilityFOneTail
		{
			get
			{
				return this.probabilityFOneTail;
			}
		}

		public double FCriticalValueOneTail
		{
			get
			{
				return this.fCriticalValueOneTail;
			}
		}
	}
}
