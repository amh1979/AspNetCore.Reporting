using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class Statistics
	{
		private string tempOutputSeriesName = "Statistical Analyses Formula Temporary Output Series 2552003";

		private FormulaData formulaData;

		public Statistics(FormulaData formulaData)
		{
			this.formulaData = formulaData;
		}

		public ZTestResult ZTest(double hypothesizedMeanDifference, double varianceFirstGroup, double varianceSecondGroup, double probability, string firstInputSeriesName, string secondInputSeriesName)
		{
			ZTestResult zTestResult = new ZTestResult();
			string str = hypothesizedMeanDifference.ToString(CultureInfo.InvariantCulture);
			str = str + "," + varianceFirstGroup.ToString(CultureInfo.InvariantCulture);
			str = str + "," + varianceSecondGroup.ToString(CultureInfo.InvariantCulture);
			str = str + "," + probability.ToString(CultureInfo.InvariantCulture);
			this.formulaData.Common.DataManager.Series.Add(this.tempOutputSeriesName);
			string inputSeries = firstInputSeriesName.ToString(CultureInfo.InvariantCulture) + "," + secondInputSeriesName.ToString(CultureInfo.InvariantCulture);
			this.formulaData.Formula("ZTest", str, inputSeries, this.tempOutputSeriesName);
			DataPointCollection points = this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName].Points;
			zTestResult.firstSeriesMean = points[0].YValues[0];
			zTestResult.secondSeriesMean = points[1].YValues[0];
			zTestResult.firstSeriesVariance = points[2].YValues[0];
			zTestResult.secondSeriesVariance = points[3].YValues[0];
			zTestResult.zValue = points[4].YValues[0];
			zTestResult.probabilityZOneTail = points[5].YValues[0];
			zTestResult.zCriticalValueOneTail = points[6].YValues[0];
			zTestResult.probabilityZTwoTail = points[7].YValues[0];
			zTestResult.zCriticalValueTwoTail = points[8].YValues[0];
			this.formulaData.Common.DataManager.Series.Remove(this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName]);
			return zTestResult;
		}

		public TTestResult TTestUnequalVariances(double hypothesizedMeanDifference, double probability, string firstInputSeriesName, string secondInputSeriesName)
		{
			TTestResult tTestResult = new TTestResult();
			string str = hypothesizedMeanDifference.ToString(CultureInfo.InvariantCulture);
			str = str + "," + probability.ToString(CultureInfo.InvariantCulture);
			this.formulaData.Common.DataManager.Series.Add(this.tempOutputSeriesName);
			string inputSeries = firstInputSeriesName.ToString(CultureInfo.InvariantCulture) + "," + secondInputSeriesName.ToString(CultureInfo.InvariantCulture);
			this.formulaData.Formula("TTestUnequalVariances", str, inputSeries, this.tempOutputSeriesName);
			DataPointCollection points = this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName].Points;
			tTestResult.firstSeriesMean = points[0].YValues[0];
			tTestResult.secondSeriesMean = points[1].YValues[0];
			tTestResult.firstSeriesVariance = points[2].YValues[0];
			tTestResult.secondSeriesVariance = points[3].YValues[0];
			tTestResult.tValue = points[4].YValues[0];
			tTestResult.degreeOfFreedom = points[5].YValues[0];
			tTestResult.probabilityTOneTail = points[6].YValues[0];
			tTestResult.tCriticalValueOneTail = points[7].YValues[0];
			tTestResult.probabilityTTwoTail = points[8].YValues[0];
			tTestResult.tCriticalValueTwoTail = points[9].YValues[0];
			this.formulaData.Common.DataManager.Series.Remove(this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName]);
			return tTestResult;
		}

		public TTestResult TTestEqualVariances(double hypothesizedMeanDifference, double probability, string firstInputSeriesName, string secondInputSeriesName)
		{
			TTestResult tTestResult = new TTestResult();
			string str = hypothesizedMeanDifference.ToString(CultureInfo.InvariantCulture);
			str = str + "," + probability.ToString(CultureInfo.InvariantCulture);
			this.formulaData.Common.DataManager.Series.Add(this.tempOutputSeriesName);
			string inputSeries = firstInputSeriesName.ToString(CultureInfo.InvariantCulture) + "," + secondInputSeriesName.ToString(CultureInfo.InvariantCulture);
			this.formulaData.Formula("TTestEqualVariances", str, inputSeries, this.tempOutputSeriesName);
			DataPointCollection points = this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName].Points;
			tTestResult.firstSeriesMean = points[0].YValues[0];
			tTestResult.secondSeriesMean = points[1].YValues[0];
			tTestResult.firstSeriesVariance = points[2].YValues[0];
			tTestResult.secondSeriesVariance = points[3].YValues[0];
			tTestResult.tValue = points[4].YValues[0];
			tTestResult.degreeOfFreedom = points[5].YValues[0];
			tTestResult.probabilityTOneTail = points[6].YValues[0];
			tTestResult.tCriticalValueOneTail = points[7].YValues[0];
			tTestResult.probabilityTTwoTail = points[8].YValues[0];
			tTestResult.tCriticalValueTwoTail = points[9].YValues[0];
			this.formulaData.Common.DataManager.Series.Remove(this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName]);
			return tTestResult;
		}

		public TTestResult TTestPaired(double hypothesizedMeanDifference, double probability, string firstInputSeriesName, string secondInputSeriesName)
		{
			TTestResult tTestResult = new TTestResult();
			string str = hypothesizedMeanDifference.ToString(CultureInfo.InvariantCulture);
			str = str + "," + probability.ToString(CultureInfo.InvariantCulture);
			this.formulaData.Common.DataManager.Series.Add(this.tempOutputSeriesName);
			string inputSeries = firstInputSeriesName.ToString(CultureInfo.InvariantCulture) + "," + secondInputSeriesName.ToString(CultureInfo.InvariantCulture);
			this.formulaData.Formula("TTestPaired", str, inputSeries, this.tempOutputSeriesName);
			DataPointCollection points = this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName].Points;
			tTestResult.firstSeriesMean = points[0].YValues[0];
			tTestResult.secondSeriesMean = points[1].YValues[0];
			tTestResult.firstSeriesVariance = points[2].YValues[0];
			tTestResult.secondSeriesVariance = points[3].YValues[0];
			tTestResult.tValue = points[4].YValues[0];
			tTestResult.degreeOfFreedom = points[5].YValues[0];
			tTestResult.probabilityTOneTail = points[6].YValues[0];
			tTestResult.tCriticalValueOneTail = points[7].YValues[0];
			tTestResult.probabilityTTwoTail = points[8].YValues[0];
			tTestResult.tCriticalValueTwoTail = points[9].YValues[0];
			this.formulaData.Common.DataManager.Series.Remove(this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName]);
			return tTestResult;
		}

		private void RemoveEmptyPoints(string seriesName)
		{
			Series series = this.formulaData.Common.DataManager.Series[seriesName];
			for (int i = 0; i < series.Points.Count; i++)
			{
				if (series.Points[i].Empty)
				{
					series.Points.RemoveAt(i--);
				}
			}
		}

		public FTestResult FTest(double probability, string firstInputSeriesName, string secondInputSeriesName)
		{
			FTestResult fTestResult = new FTestResult();
			string parameters = probability.ToString(CultureInfo.InvariantCulture);
			string inputSeries = firstInputSeriesName.ToString(CultureInfo.InvariantCulture) + "," + secondInputSeriesName.ToString(CultureInfo.InvariantCulture);
			this.formulaData.Common.DataManager.Series.Add(this.tempOutputSeriesName);
			this.RemoveEmptyPoints(firstInputSeriesName);
			this.RemoveEmptyPoints(secondInputSeriesName);
			this.formulaData.Formula("FTest", parameters, inputSeries, this.tempOutputSeriesName);
			DataPointCollection points = this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName].Points;
			fTestResult.firstSeriesMean = points[0].YValues[0];
			fTestResult.secondSeriesMean = points[1].YValues[0];
			fTestResult.firstSeriesVariance = points[2].YValues[0];
			fTestResult.secondSeriesVariance = points[3].YValues[0];
			fTestResult.fValue = points[4].YValues[0];
			fTestResult.probabilityFOneTail = points[5].YValues[0];
			fTestResult.fCriticalValueOneTail = points[6].YValues[0];
			this.formulaData.Common.DataManager.Series.Remove(this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName]);
			return fTestResult;
		}

		public AnovaResult Anova(double probability, string inputSeriesNames)
		{
			AnovaResult anovaResult = new AnovaResult();
			string parameters = probability.ToString(CultureInfo.InvariantCulture);
			this.formulaData.Common.DataManager.Series.Add(this.tempOutputSeriesName);
			this.formulaData.Formula("Anova", parameters, inputSeriesNames, this.tempOutputSeriesName);
			DataPointCollection points = this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName].Points;
			anovaResult.sumOfSquaresBetweenGroups = points[0].YValues[0];
			anovaResult.sumOfSquaresWithinGroups = points[1].YValues[0];
			anovaResult.sumOfSquaresTotal = points[2].YValues[0];
			anovaResult.degreeOfFreedomBetweenGroups = points[3].YValues[0];
			anovaResult.degreeOfFreedomWithinGroups = points[4].YValues[0];
			anovaResult.degreeOfFreedomTotal = points[5].YValues[0];
			anovaResult.meanSquareVarianceBetweenGroups = points[6].YValues[0];
			anovaResult.meanSquareVarianceWithinGroups = points[7].YValues[0];
			anovaResult.fRatio = points[8].YValues[0];
			anovaResult.fCriticalValue = points[9].YValues[0];
			this.formulaData.Common.DataManager.Series.Remove(this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName]);
			return anovaResult;
		}

		public double NormalDistribution(double zValue)
		{
			string parameters = zValue.ToString(CultureInfo.InvariantCulture);
			this.formulaData.Common.DataManager.Series.Add(this.tempOutputSeriesName);
			this.formulaData.Formula("NormalDistribution", parameters, this.tempOutputSeriesName, this.tempOutputSeriesName);
			DataPointCollection points = this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName].Points;
			double result = points[0].YValues[0];
			this.formulaData.Common.DataManager.Series.Remove(this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName]);
			return result;
		}

		public double InverseNormalDistribution(double probability)
		{
			string parameters = probability.ToString(CultureInfo.InvariantCulture);
			this.formulaData.Common.DataManager.Series.Add(this.tempOutputSeriesName);
			this.formulaData.Formula("InverseNormalDistribution", parameters, this.tempOutputSeriesName, this.tempOutputSeriesName);
			DataPointCollection points = this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName].Points;
			double result = points[0].YValues[0];
			this.formulaData.Common.DataManager.Series.Remove(this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName]);
			return result;
		}

		public double FDistribution(double value, int firstDegreeOfFreedom, int secondDegreeOfFreedom)
		{
			string str = value.ToString(CultureInfo.InvariantCulture);
			str = str + "," + firstDegreeOfFreedom.ToString(CultureInfo.InvariantCulture);
			str = str + "," + secondDegreeOfFreedom.ToString(CultureInfo.InvariantCulture);
			this.formulaData.Common.DataManager.Series.Add(this.tempOutputSeriesName);
			this.formulaData.Formula("FDistribution", str, this.tempOutputSeriesName, this.tempOutputSeriesName);
			DataPointCollection points = this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName].Points;
			double result = points[0].YValues[0];
			this.formulaData.Common.DataManager.Series.Remove(this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName]);
			return result;
		}

		public double InverseFDistribution(double probability, int firstDegreeOfFreedom, int secondDegreeOfFreedom)
		{
			string str = probability.ToString(CultureInfo.InvariantCulture);
			str = str + "," + firstDegreeOfFreedom.ToString(CultureInfo.InvariantCulture);
			str = str + "," + secondDegreeOfFreedom.ToString(CultureInfo.InvariantCulture);
			this.formulaData.Common.DataManager.Series.Add(this.tempOutputSeriesName);
			this.formulaData.Formula("InverseFDistribution", str, this.tempOutputSeriesName, this.tempOutputSeriesName);
			DataPointCollection points = this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName].Points;
			double result = points[0].YValues[0];
			this.formulaData.Common.DataManager.Series.Remove(this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName]);
			return result;
		}

		public double TDistribution(double value, int degreeOfFreedom, bool oneTail)
		{
			string str = value.ToString(CultureInfo.InvariantCulture);
			str = str + "," + degreeOfFreedom.ToString(CultureInfo.InvariantCulture);
			str = ((!oneTail) ? (str + ",2") : (str + ",1"));
			this.formulaData.Common.DataManager.Series.Add(this.tempOutputSeriesName);
			this.formulaData.Formula("TDistribution", str, this.tempOutputSeriesName, this.tempOutputSeriesName);
			DataPointCollection points = this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName].Points;
			double result = points[0].YValues[0];
			this.formulaData.Common.DataManager.Series.Remove(this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName]);
			return result;
		}

		public double InverseTDistribution(double probability, int degreeOfFreedom)
		{
			string str = probability.ToString(CultureInfo.InvariantCulture);
			str = str + "," + degreeOfFreedom.ToString(CultureInfo.InvariantCulture);
			this.formulaData.Common.DataManager.Series.Add(this.tempOutputSeriesName);
			this.formulaData.Formula("InverseTDistribution", str, this.tempOutputSeriesName, this.tempOutputSeriesName);
			DataPointCollection points = this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName].Points;
			double result = points[0].YValues[0];
			this.formulaData.Common.DataManager.Series.Remove(this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName]);
			return result;
		}

		public double Covariance(string firstInputSeriesName, string secondInputSeriesName)
		{
			this.formulaData.Common.DataManager.Series.Add(this.tempOutputSeriesName);
			string inputSeries = firstInputSeriesName.ToString(CultureInfo.InvariantCulture) + "," + secondInputSeriesName.ToString(CultureInfo.InvariantCulture);
			this.formulaData.Formula("Covariance", "", inputSeries, this.tempOutputSeriesName);
			DataPointCollection points = this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName].Points;
			double result = points[0].YValues[0];
			this.formulaData.Common.DataManager.Series.Remove(this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName]);
			return result;
		}

		public double Correlation(string firstInputSeriesName, string secondInputSeriesName)
		{
			this.formulaData.Common.DataManager.Series.Add(this.tempOutputSeriesName);
			string inputSeries = firstInputSeriesName + "," + secondInputSeriesName;
			this.formulaData.Formula("Correlation", "", inputSeries, this.tempOutputSeriesName);
			DataPointCollection points = this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName].Points;
			double result = points[0].YValues[0];
			this.formulaData.Common.DataManager.Series.Remove(this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName]);
			return result;
		}

		public double Mean(string inputSeriesName)
		{
			this.formulaData.Common.DataManager.Series.Add(this.tempOutputSeriesName);
			this.formulaData.Formula("Mean", "", inputSeriesName, this.tempOutputSeriesName);
			DataPointCollection points = this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName].Points;
			double result = points[0].YValues[0];
			this.formulaData.Common.DataManager.Series.Remove(this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName]);
			return result;
		}

		public double Median(string inputSeriesName)
		{
			this.formulaData.Common.DataManager.Series.Add(this.tempOutputSeriesName);
			this.formulaData.Formula("Median", "", inputSeriesName, this.tempOutputSeriesName);
			DataPointCollection points = this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName].Points;
			double result = points[0].YValues[0];
			this.formulaData.Common.DataManager.Series.Remove(this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName]);
			return result;
		}

		public double Variance(string inputSeriesName, bool sampleVariance)
		{
			this.formulaData.Common.DataManager.Series.Add(this.tempOutputSeriesName);
			string parameters = sampleVariance.ToString(CultureInfo.InvariantCulture);
			this.formulaData.Formula("Variance", parameters, inputSeriesName, this.tempOutputSeriesName);
			DataPointCollection points = this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName].Points;
			double result = points[0].YValues[0];
			this.formulaData.Common.DataManager.Series.Remove(this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName]);
			return result;
		}

		public double BetaFunction(double m, double n)
		{
			this.formulaData.Common.DataManager.Series.Add(this.tempOutputSeriesName);
			string str = m.ToString(CultureInfo.InvariantCulture);
			str = str + "," + n.ToString(CultureInfo.InvariantCulture);
			this.formulaData.Formula("BetaFunction", str, this.tempOutputSeriesName, this.tempOutputSeriesName);
			DataPointCollection points = this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName].Points;
			double result = points[0].YValues[0];
			this.formulaData.Common.DataManager.Series.Remove(this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName]);
			return result;
		}

		public double GammaFunction(double value)
		{
			this.formulaData.Common.DataManager.Series.Add(this.tempOutputSeriesName);
			string parameters = value.ToString(CultureInfo.InvariantCulture);
			this.formulaData.Formula("GammaFunction", parameters, this.tempOutputSeriesName, this.tempOutputSeriesName);
			DataPointCollection points = this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName].Points;
			double result = points[0].YValues[0];
			this.formulaData.Common.DataManager.Series.Remove(this.formulaData.Common.DataManager.Series[this.tempOutputSeriesName]);
			return result;
		}
	}
}
