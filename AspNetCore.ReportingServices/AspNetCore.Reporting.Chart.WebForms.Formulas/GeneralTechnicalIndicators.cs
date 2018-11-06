using System;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.Formulas
{
	internal class GeneralTechnicalIndicators : PriceIndicators
	{
		public override string Name
		{
			get
			{
				return SR.FormulaNameGeneralTechnicalIndicators;
			}
		}

		private void StandardDeviation(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			int num = inputValues.Length;
			int num2;
			try
			{
				num2 = int.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception ex)
			{
				if (ex.Message == SR.ExceptionObjectReferenceIsNull)
				{
					throw new InvalidOperationException(SR.ExceptionPriceIndicatorsPeriodMissing);
				}
				throw new InvalidOperationException(SR.ExceptionPriceIndicatorsPeriodMissing + ex.Message);
			}
			if (num2 <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodParameterIsNegative);
			}
			bool flag = bool.Parse(extraParameterList[0]);
			if (num != 2)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresOneArray);
			}
			if (inputValues[0].Length != inputValues[1].Length)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsSameXYNumber);
			}
			if (inputValues[0].Length < num2)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsNotEnoughPoints);
			}
			outputValues = new double[2][];
			base.StandardDeviation(inputValues[1], out outputValues[1], num2, flag);
			outputValues[0] = new double[outputValues[1].Length];
			for (int i = 0; i < outputValues[1].Length; i++)
			{
				if (flag)
				{
					outputValues[0][i] = inputValues[0][i];
				}
				else
				{
					outputValues[0][i] = inputValues[0][i + num2 - 1];
				}
			}
		}

		private void AverageTrueRange(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 4)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresThreeArrays);
			}
			base.CheckNumOfValues(inputValues, 3);
			int num;
			try
			{
				num = int.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num = 14;
			}
			if (num <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodParameterIsNegative);
			}
			double[] array = new double[inputValues[0].Length - 1];
			for (int i = 1; i < inputValues[0].Length; i++)
			{
				double val = Math.Abs(inputValues[1][i] - inputValues[2][i]);
				double val2 = Math.Abs(inputValues[3][i - 1] - inputValues[1][i]);
				double val3 = Math.Abs(inputValues[3][i - 1] - inputValues[2][i]);
				array[i - 1] = Math.Max(Math.Max(val, val2), val3);
			}
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length - num];
			base.MovingAverage(array, out outputValues[1], num, false);
			for (int j = num; j < inputValues[0].Length; j++)
			{
				outputValues[0][j - num] = inputValues[0][j];
			}
		}

		private void EaseOfMovement(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 4)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresThreeArrays);
			}
			base.CheckNumOfValues(inputValues, 3);
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length - 1];
			outputValues[1] = new double[inputValues[0].Length - 1];
			for (int i = 1; i < inputValues[0].Length; i++)
			{
				outputValues[0][i - 1] = inputValues[0][i];
				double num = (inputValues[1][i] + inputValues[2][i]) / 2.0 - (inputValues[1][i - 1] + inputValues[2][i - 1]) / 2.0;
				double num2 = inputValues[3][i] / (inputValues[1][i] - inputValues[2][i]);
				outputValues[1][i - 1] = num / num2;
			}
		}

		private void MassIndex(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 3)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresTwoArrays);
			}
			base.CheckNumOfValues(inputValues, 2);
			int num;
			try
			{
				num = int.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num = 25;
			}
			if (num <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodParameterIsNegative);
			}
			int num2;
			try
			{
				num2 = int.Parse(parameterList[1], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num2 = 9;
			}
			if (num <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodAverageParameterIsNegative);
			}
			double[] array = new double[inputValues[0].Length];
			for (int i = 0; i < inputValues[0].Length; i++)
			{
				array[i] = inputValues[1][i] - inputValues[2][i];
			}
			double[] array2 = default(double[]);
			base.ExponentialMovingAverage(array, out array2, num2, false);
			double[] array3 = default(double[]);
			base.ExponentialMovingAverage(array2, out array3, num2, false);
			outputValues = new double[2][];
			outputValues[0] = new double[array3.Length - num + 1];
			outputValues[1] = new double[array3.Length - num + 1];
			int num3 = 0;
			double num4 = 0.0;
			for (int j = 2 * num2 - 3 + num; j < inputValues[0].Length; j++)
			{
				outputValues[0][num3] = inputValues[0][j];
				num4 = 0.0;
				for (int k = j - num + 1; k <= j; k++)
				{
					num4 += array2[k - num2 + 1] / array3[k - 2 * num2 + 2];
				}
				outputValues[1][num3] = num4;
				num3++;
			}
		}

		private void Performance(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 2)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresOneArray);
			}
			base.CheckNumOfValues(inputValues, 1);
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length];
			outputValues[1] = new double[inputValues[0].Length];
			for (int i = 0; i < inputValues[0].Length; i++)
			{
				outputValues[0][i] = inputValues[0][i];
				outputValues[1][i] = (inputValues[1][i] - inputValues[1][0]) / inputValues[1][0] * 100.0;
			}
		}

		private void RateOfChange(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 2)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresOneArray);
			}
			base.CheckNumOfValues(inputValues, 1);
			int num;
			try
			{
				num = int.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num = 10;
			}
			if (num <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodParameterIsNegative);
			}
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length - num];
			outputValues[1] = new double[inputValues[0].Length - num];
			for (int i = num; i < inputValues[0].Length; i++)
			{
				outputValues[0][i - num] = inputValues[0][i];
				outputValues[1][i - num] = (inputValues[1][i] - inputValues[1][i - num]) / inputValues[1][i - num] * 100.0;
			}
		}

		private void RelativeStrengthIndex(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 2)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresOneArray);
			}
			base.CheckNumOfValues(inputValues, 1);
			int num;
			try
			{
				num = int.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num = 10;
			}
			if (num <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodParameterIsNegative);
			}
			double[] array = new double[inputValues[0].Length - 1];
			double[] array2 = new double[inputValues[0].Length - 1];
			for (int i = 1; i < inputValues[0].Length; i++)
			{
				if (inputValues[1][i - 1] < inputValues[1][i])
				{
					array[i - 1] = inputValues[1][i] - inputValues[1][i - 1];
					array2[i - 1] = 0.0;
				}
				if (inputValues[1][i - 1] > inputValues[1][i])
				{
					array[i - 1] = 0.0;
					array2[i - 1] = inputValues[1][i - 1] - inputValues[1][i];
				}
			}
			double[] array3 = new double[inputValues[0].Length];
			double[] array4 = new double[inputValues[0].Length];
			base.ExponentialMovingAverage(array2, out array4, num, false);
			base.ExponentialMovingAverage(array, out array3, num, false);
			outputValues = new double[2][];
			outputValues[0] = new double[array4.Length];
			outputValues[1] = new double[array4.Length];
			for (int j = 0; j < array4.Length; j++)
			{
				outputValues[0][j] = inputValues[0][j + num];
				outputValues[1][j] = 100.0 - 100.0 / (1.0 + array3[j] / array4[j]);
			}
		}

		private void Trix(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 2)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresOneArray);
			}
			base.CheckNumOfValues(inputValues, 1);
			int num;
			try
			{
				num = int.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num = 12;
			}
			if (num <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodParameterIsNegative);
			}
			double[] inputValues2 = default(double[]);
			base.ExponentialMovingAverage(inputValues[1], out inputValues2, num, false);
			double[] inputValues3 = default(double[]);
			base.ExponentialMovingAverage(inputValues2, out inputValues3, num, false);
			double[] array = default(double[]);
			base.ExponentialMovingAverage(inputValues3, out array, num, false);
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length - num * 3 + 2];
			outputValues[1] = new double[inputValues[0].Length - num * 3 + 2];
			int num2 = 0;
			for (int i = num * 3 - 2; i < inputValues[0].Length; i++)
			{
				outputValues[0][num2] = inputValues[0][i];
				outputValues[1][num2] = (array[num2 + 1] - array[num2]) / array[num2];
				num2++;
			}
		}

		private void Macd(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 2)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresOneArray);
			}
			base.CheckNumOfValues(inputValues, 1);
			int num;
			try
			{
				num = int.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num = 12;
			}
			if (num <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodShortParameterIsNegative);
			}
			int num2;
			try
			{
				num2 = int.Parse(parameterList[1], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num2 = 26;
			}
			if (num2 <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodLongParameterIsNegative);
			}
			if (num2 <= num)
			{
				throw new InvalidOperationException(SR.ExceptionIndicatorsLongPeriodLessThenShortPeriod);
			}
			double[] array = default(double[]);
			base.ExponentialMovingAverage(inputValues[1], out array, num2, false);
			double[] array2 = default(double[]);
			base.ExponentialMovingAverage(inputValues[1], out array2, num, false);
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length - num2 + 1];
			outputValues[1] = new double[inputValues[0].Length - num2 + 1];
			int num3 = 0;
			for (int i = num2 - 1; i < inputValues[0].Length; i++)
			{
				outputValues[0][num3] = inputValues[0][i];
				outputValues[1][num3] = array2[num3 + num2 - num] - array[num3];
				num3++;
			}
		}

		private void CommodityChannelIndex(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 4)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresThreeArrays);
			}
			base.CheckNumOfValues(inputValues, 3);
			int num;
			try
			{
				num = int.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num = 10;
			}
			if (num <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodParameterIsNegative);
			}
			double[] array = new double[inputValues[0].Length];
			for (int i = 0; i < inputValues[0].Length; i++)
			{
				array[i] = (inputValues[1][i] + inputValues[2][i] + inputValues[3][i]) / 3.0;
			}
			double[] array2 = default(double[]);
			base.MovingAverage(array, out array2, num, false);
			double[] array3 = new double[array2.Length];
			double num2 = 0.0;
			for (int j = 0; j < array2.Length; j++)
			{
				num2 = 0.0;
				for (int k = j; k < j + num; k++)
				{
					num2 += Math.Abs(array2[j] - array[k]);
				}
				array3[j] = num2 / (double)num;
			}
			outputValues = new double[2][];
			outputValues[0] = new double[array3.Length];
			outputValues[1] = new double[array3.Length];
			for (int l = 0; l < array3.Length; l++)
			{
				outputValues[0][l] = inputValues[0][l + num - 1];
				outputValues[1][l] = (array[l + num - 1] - array2[l]) / (0.015 * array3[l]);
			}
		}

		public override void Formula(string formulaName, double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			outputValues = null;
			string text = formulaName.ToUpper(CultureInfo.InvariantCulture);
			outLabels = null;
			try
			{
				switch (text)
				{
				case "STANDARDDEVIATION":
					this.StandardDeviation(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "AVERAGETRUERANGE":
					this.AverageTrueRange(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "EASEOFMOVEMENT":
					this.EaseOfMovement(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "MASSINDEX":
					this.MassIndex(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "PERFORMANCE":
					this.Performance(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "RATEOFCHANGE":
					this.RateOfChange(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "RELATIVESTRENGTHINDEX":
					this.RelativeStrengthIndex(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "TRIX":
					this.Trix(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "MACD":
					this.Macd(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "COMMODITYCHANNELINDEX":
					this.CommodityChannelIndex(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				default:
					outputValues = null;
					break;
				}
			}
			catch (IndexOutOfRangeException)
			{
				throw new InvalidOperationException(SR.ExceptionFormulaInvalidPeriod(text));
			}
			catch (OverflowException)
			{
				throw new InvalidOperationException(SR.ExceptionFormulaNotEnoughDataPoints(text));
			}
		}
	}
}
