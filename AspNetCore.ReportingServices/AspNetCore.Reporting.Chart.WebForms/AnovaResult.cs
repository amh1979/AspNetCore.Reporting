namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class AnovaResult
	{
		internal double sumOfSquaresBetweenGroups;

		internal double sumOfSquaresWithinGroups;

		internal double sumOfSquaresTotal;

		internal double degreeOfFreedomBetweenGroups;

		internal double degreeOfFreedomWithinGroups;

		internal double degreeOfFreedomTotal;

		internal double meanSquareVarianceBetweenGroups;

		internal double meanSquareVarianceWithinGroups;

		internal double fRatio;

		internal double fCriticalValue;

		public double SumOfSquaresBetweenGroups
		{
			get
			{
				return this.sumOfSquaresBetweenGroups;
			}
		}

		public double SumOfSquaresWithinGroups
		{
			get
			{
				return this.sumOfSquaresWithinGroups;
			}
		}

		public double SumOfSquaresTotal
		{
			get
			{
				return this.sumOfSquaresTotal;
			}
		}

		public double DegreeOfFreedomBetweenGroups
		{
			get
			{
				return this.degreeOfFreedomBetweenGroups;
			}
		}

		public double DegreeOfFreedomWithinGroups
		{
			get
			{
				return this.degreeOfFreedomWithinGroups;
			}
		}

		public double DegreeOfFreedomTotal
		{
			get
			{
				return this.degreeOfFreedomTotal;
			}
		}

		public double MeanSquareVarianceBetweenGroups
		{
			get
			{
				return this.meanSquareVarianceBetweenGroups;
			}
		}

		public double MeanSquareVarianceWithinGroups
		{
			get
			{
				return this.meanSquareVarianceWithinGroups;
			}
		}

		public double FRatio
		{
			get
			{
				return this.fRatio;
			}
		}

		public double FCriticalValue
		{
			get
			{
				return this.fCriticalValue;
			}
		}
	}
}
