using System;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.Formulas
{
	internal class PriceIndicators : IFormula
	{
		public virtual string Name
		{
			get
			{
				return SR.FormulaNamePriceIndicators;
			}
		}

		internal void MovingAverage(double[] inputValues, out double[] outputValues, int period, bool FromFirst)
		{
			double[][] array = new double[2][];
			double[][] array2 = new double[2][];
			string[] array3 = new string[1];
			string[] array4 = new string[1];
			array3[0] = period.ToString(CultureInfo.InvariantCulture);
			array4[0] = FromFirst.ToString(CultureInfo.InvariantCulture);
			array[0] = new double[inputValues.Length];
			array[1] = inputValues;
			this.MovingAverage(array, out array2, array3, array4);
			outputValues = array2[1];
		}

		private void MovingAverage(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
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
			if (flag)
			{
				outputValues[0] = new double[inputValues[0].Length];
				outputValues[1] = new double[inputValues[1].Length];
				for (int i = 0; i < inputValues[0].Length; i++)
				{
					outputValues[0][i] = inputValues[0][i];
					double num3 = 0.0;
					int num4 = 0;
					if (i - num2 + 1 > 0)
					{
						num4 = i - num2 + 1;
					}
					for (int j = num4; j <= i; j++)
					{
						num3 += inputValues[1][j];
					}
					int num5 = num2;
					if (num2 > i + 1)
					{
						num5 = i + 1;
					}
					outputValues[1][i] = num3 / (double)num5;
				}
			}
			else
			{
				outputValues[0] = new double[inputValues[0].Length - num2 + 1];
				outputValues[1] = new double[inputValues[1].Length - num2 + 1];
				double num6 = 0.0;
				for (int k = 0; k < num2; k++)
				{
					num6 += inputValues[1][k];
				}
				for (int l = 0; l < outputValues[0].Length; l++)
				{
					outputValues[0][l] = inputValues[0][l + num2 - 1];
					outputValues[1][l] = num6 / (double)num2;
					if (l < outputValues[0].Length - 1)
					{
						num6 -= inputValues[1][l];
						num6 += inputValues[1][l + num2];
					}
				}
			}
		}

		internal void ExponentialMovingAverage(double[] inputValues, out double[] outputValues, int period, bool startFromFirst)
		{
			double[][] array = new double[2][];
			double[][] array2 = new double[2][];
			string[] array3 = new string[1];
			string[] array4 = new string[1];
			array3[0] = period.ToString(CultureInfo.InvariantCulture);
			array4[0] = startFromFirst.ToString(CultureInfo.InvariantCulture);
			array[0] = new double[inputValues.Length];
			array[1] = inputValues;
			this.ExponentialMovingAverage(array, out array2, array3, array4);
			outputValues = array2[1];
		}

		private void ExponentialMovingAverage(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
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
			double num3 = 2.0 / ((double)num2 + 1.0);
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
			if (flag)
			{
				outputValues[0] = new double[inputValues[0].Length];
				outputValues[1] = new double[inputValues[1].Length];
				for (int i = 0; i < inputValues[0].Length; i++)
				{
					outputValues[0][i] = inputValues[0][i];
					double num4 = 0.0;
					int num5 = 0;
					if (i - num2 + 1 > 0)
					{
						num5 = i - num2 + 1;
					}
					for (int j = num5; j < i; j++)
					{
						num4 += inputValues[1][j];
					}
					int num6 = num2;
					if (num2 > i + 1)
					{
						num6 = i + 1;
					}
					double num7 = (num6 > 1) ? (num4 / (double)(num6 - 1)) : 0.0;
					num3 = 2.0 / ((double)num6 + 1.0);
					outputValues[1][i] = num7 * (1.0 - num3) + inputValues[1][i] * num3;
				}
			}
			else
			{
				outputValues[0] = new double[inputValues[0].Length - num2 + 1];
				outputValues[1] = new double[inputValues[1].Length - num2 + 1];
				for (int k = 0; k < outputValues[0].Length; k++)
				{
					outputValues[0][k] = inputValues[0][k + num2 - 1];
					double num9;
					if (k == 0)
					{
						double num8 = 0.0;
						for (int l = k; l < k + num2; l++)
						{
							num8 += inputValues[1][l];
						}
						num9 = num8 / (double)num2;
					}
					else
					{
						num9 = outputValues[1][k - 1];
					}
					outputValues[1][k] = num9 * (1.0 - num3) + inputValues[1][k + num2 - 1] * num3;
				}
			}
		}

		private void TriangularMovingAverage(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
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
			double a = ((double)num2 + 1.0) / 2.0;
			a = Math.Round(a);
			double[] inputValues2 = inputValues[1];
			double[] array = default(double[]);
			this.MovingAverage(inputValues2, out array, (int)a, flag);
			this.MovingAverage(array, out array, (int)a, flag);
			outputValues[1] = array;
			outputValues[0] = new double[outputValues[1].Length];
			if (flag)
			{
				outputValues[0] = inputValues[0];
			}
			else
			{
				for (int i = 0; i < outputValues[1].Length; i++)
				{
					outputValues[0][i] = inputValues[0][((int)a - 1) * 2 + i];
				}
			}
		}

		private void WeightedMovingAverage(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
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
			double num12 = 2.0 / ((double)num2 + 1.0);
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
			if (flag)
			{
				outputValues[0] = new double[inputValues[0].Length];
				outputValues[1] = new double[inputValues[1].Length];
				for (int i = 0; i < inputValues[0].Length; i++)
				{
					outputValues[0][i] = inputValues[0][i];
					double num3 = 0.0;
					int num4 = 0;
					if (i - num2 + 1 > 0)
					{
						num4 = i - num2 + 1;
					}
					int num5 = 1;
					int num6 = 0;
					for (int j = num4; j <= i; j++)
					{
						num3 += inputValues[1][j] * (double)num5;
						num6 += num5;
						num5++;
					}
					double num7 = (i != 0) ? (num3 / (double)num6) : inputValues[1][0];
					outputValues[1][i] = num7;
				}
			}
			else
			{
				outputValues[0] = new double[inputValues[0].Length - num2 + 1];
				outputValues[1] = new double[inputValues[1].Length - num2 + 1];
				for (int k = 0; k < outputValues[0].Length; k++)
				{
					outputValues[0][k] = inputValues[0][k + num2 - 1];
					double num8 = 0.0;
					int num9 = 1;
					int num10 = 0;
					for (int l = k; l < k + num2; l++)
					{
						num8 += inputValues[1][l] * (double)num9;
						num10 += num9;
						num9++;
					}
					double num11 = num8 / (double)num10;
					outputValues[1][k] = num11;
				}
			}
		}

		private void BollingerBands(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
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
			double num3;
			try
			{
				num3 = double.Parse(parameterList[1], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionIndicatorsDeviationMissing);
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
			outputValues = new double[3][];
			if (flag)
			{
				outputValues[0] = new double[inputValues[0].Length];
				outputValues[1] = new double[inputValues[1].Length];
				outputValues[2] = new double[inputValues[1].Length];
				double[] array = new double[inputValues[1].Length];
				this.MovingAverage(inputValues[1], out array, num2, true);
				for (int i = 0; i < outputValues[0].Length; i++)
				{
					outputValues[0][i] = inputValues[0][i];
					double num4 = 0.0;
					int num5 = 0;
					if (i - num2 + 1 > 0)
					{
						num5 = i - num2 + 1;
					}
					for (int j = num5; j <= i; j++)
					{
						num4 += (inputValues[1][j] - array[i]) * (inputValues[1][j] - array[i]);
					}
					outputValues[1][i] = array[i] + Math.Sqrt(num4 / (double)num2) * num3;
					outputValues[2][i] = array[i] - Math.Sqrt(num4 / (double)num2) * num3;
				}
			}
			else
			{
				outputValues[0] = new double[inputValues[0].Length - num2 + 1];
				outputValues[1] = new double[inputValues[1].Length - num2 + 1];
				outputValues[2] = new double[inputValues[1].Length - num2 + 1];
				double[] array2 = new double[inputValues[1].Length - num2 + 1];
				this.MovingAverage(inputValues[1], out array2, num2, false);
				for (int k = 0; k < outputValues[0].Length; k++)
				{
					outputValues[0][k] = inputValues[0][k + num2 - 1];
					double num6 = 0.0;
					for (int l = k; l < k + num2; l++)
					{
						num6 += (inputValues[1][l] - array2[k]) * (inputValues[1][l] - array2[k]);
					}
					outputValues[1][k] = array2[k] + Math.Sqrt(num6 / (double)num2) * num3;
					outputValues[2][k] = array2[k] - Math.Sqrt(num6 / (double)num2) * num3;
				}
			}
		}

		private void TypicalPrice(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			int num = inputValues.Length;
			if (num != 4)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresThreeArrays);
			}
			this.CheckNumOfValues(inputValues, 3);
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length];
			outputValues[1] = new double[inputValues[1].Length];
			for (int i = 0; i < inputValues[1].Length; i++)
			{
				outputValues[0][i] = inputValues[0][i];
				outputValues[1][i] = (inputValues[1][i] + inputValues[2][i] + inputValues[3][i]) / 3.0;
			}
		}

		private void MedianPrice(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			int num = inputValues.Length;
			if (num != 3)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresTwoArrays);
			}
			this.CheckNumOfValues(inputValues, 2);
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length];
			outputValues[1] = new double[inputValues[1].Length];
			for (int i = 0; i < inputValues[1].Length; i++)
			{
				outputValues[0][i] = inputValues[0][i];
				outputValues[1][i] = (inputValues[1][i] + inputValues[2][i]) / 2.0;
			}
		}

		private void WeightedClose(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			int num = inputValues.Length;
			if (num != 4)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresThreeArrays);
			}
			this.CheckNumOfValues(inputValues, 3);
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length];
			outputValues[1] = new double[inputValues[1].Length];
			for (int i = 0; i < inputValues[1].Length; i++)
			{
				outputValues[0][i] = inputValues[0][i];
				outputValues[1][i] = (inputValues[1][i] + inputValues[2][i] + inputValues[3][i] * 2.0) / 4.0;
			}
		}

		private void Envelopes(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
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
			double num3;
			try
			{
				num3 = double.Parse(parameterList[1], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				throw new InvalidOperationException(SR.ExceptionPriceIndicatorsShiftParameterMissing);
			}
			bool.Parse(extraParameterList[0]);
			if (num != 2)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresOneArray);
			}
			if (inputValues[0].Length != inputValues[1].Length)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsSameXYNumber);
			}
			double[][] array = default(double[][]);
			this.MovingAverage(inputValues, out array, parameterList, extraParameterList);
			outputValues = new double[3][];
			outputValues[0] = new double[array[0].Length];
			outputValues[1] = new double[array[0].Length];
			outputValues[2] = new double[array[0].Length];
			for (int i = 0; i < array[0].Length; i++)
			{
				outputValues[0][i] = array[0][i];
				outputValues[1][i] = array[1][i] + num3 * array[1][i] / 100.0;
				outputValues[2][i] = array[1][i] - num3 * array[1][i] / 100.0;
			}
		}

		internal void StandardDeviation(double[] inputValues, out double[] outputValues, int period, bool startFromFirst)
		{
			double[] array = default(double[]);
			if (startFromFirst)
			{
				outputValues = new double[inputValues.Length];
				this.MovingAverage(inputValues, out array, period, startFromFirst);
				int num = 0;
				for (int i = 0; i < inputValues.Length; i++)
				{
					double num2 = 0.0;
					int num3 = 0;
					if (i - period + 1 > 0)
					{
						num3 = i - period + 1;
					}
					for (int j = num3; j <= i; j++)
					{
						num2 += (inputValues[j] - array[num]) * (inputValues[j] - array[num]);
					}
					outputValues[num] = Math.Sqrt(num2 / (double)period);
					num++;
				}
			}
			else
			{
				outputValues = new double[inputValues.Length - period + 1];
				this.MovingAverage(inputValues, out array, period, startFromFirst);
				int num4 = 0;
				for (int k = period - 1; k < inputValues.Length; k++)
				{
					double num5 = 0.0;
					for (int l = k - period + 1; l <= k; l++)
					{
						num5 += (inputValues[l] - array[num4]) * (inputValues[l] - array[num4]);
					}
					outputValues[num4] = Math.Sqrt(num5 / (double)period);
					num4++;
				}
			}
		}

		public void CheckNumOfValues(double[][] inputValues, int numOfYValues)
		{
			if (inputValues[0].Length != inputValues[1].Length)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsSameXYNumber);
			}
			int num = 1;
			while (true)
			{
				if (num < numOfYValues)
				{
					if (inputValues[num].Length == inputValues[num + 1].Length)
					{
						num++;
						continue;
					}
					break;
				}
				return;
			}
			throw new ArgumentException(SR.ExceptionPriceIndicatorsSameYNumber);
		}

		public virtual void Formula(string formulaName, double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			string text = formulaName.ToUpper(CultureInfo.InvariantCulture);
			outLabels = null;
			try
			{
				switch (text)
				{
				case "MOVINGAVERAGE":
					this.MovingAverage(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "EXPONENTIALMOVINGAVERAGE":
					this.ExponentialMovingAverage(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "TRIANGULARMOVINGAVERAGE":
					this.TriangularMovingAverage(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "WEIGHTEDMOVINGAVERAGE":
					this.WeightedMovingAverage(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "BOLLINGERBANDS":
					this.BollingerBands(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "MEDIANPRICE":
					this.MedianPrice(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "TYPICALPRICE":
					this.TypicalPrice(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "WEIGHTEDCLOSE":
					this.WeightedClose(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "ENVELOPES":
					this.Envelopes(inputValues, out outputValues, parameterList, extraParameterList);
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
