namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class TTestResult
	{
		internal double firstSeriesMean;

		internal double secondSeriesMean;

		internal double firstSeriesVariance;

		internal double secondSeriesVariance;

		internal double tValue;

		internal double degreeOfFreedom;

		internal double probabilityTOneTail;

		internal double tCriticalValueOneTail;

		internal double probabilityTTwoTail;

		internal double tCriticalValueTwoTail;

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

		public double TValue
		{
			get
			{
				return this.tValue;
			}
		}

		public double DegreeOfFreedom
		{
			get
			{
				return this.degreeOfFreedom;
			}
		}

		public double ProbabilityTOneTail
		{
			get
			{
				return this.probabilityTOneTail;
			}
		}

		public double TCriticalValueOneTail
		{
			get
			{
				return this.tCriticalValueOneTail;
			}
		}

		public double ProbabilityTTwoTail
		{
			get
			{
				return this.probabilityTTwoTail;
			}
		}

		public double TCriticalValueTwoTail
		{
			get
			{
				return this.tCriticalValueTwoTail;
			}
		}
	}
}
