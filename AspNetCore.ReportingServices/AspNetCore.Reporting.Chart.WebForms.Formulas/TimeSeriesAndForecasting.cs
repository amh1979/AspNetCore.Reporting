using System;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.Formulas
{
	internal class TimeSeriesAndForecasting : IFormula
	{
		private enum RegressionType
		{
			Polynomial,
			Logarithmic,
			Power,
			Exponential
		}

		public virtual string Name
		{
			get
			{
				return SR.FormulaNameTimeSeriesAndForecasting;
			}
		}

		public virtual void Formula(string formulaName, double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			string text = formulaName.ToUpper(CultureInfo.InvariantCulture);
			outLabels = null;
			try
			{
				string a;
				if ((a = text) != null && a == "FORECASTING")
				{
					this.Forecasting(inputValues, out outputValues, parameterList, extraParameterList);
				}
				else
				{
					outputValues = null;
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

		private void Forecasting(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			RegressionType regressionType = RegressionType.Polynomial;
			int num;
			try
			{
				if (parameterList[0] == "Exponential")
				{
					regressionType = RegressionType.Exponential;
					num = 2;
				}
				else if (parameterList[0] == "Linear")
				{
					regressionType = RegressionType.Polynomial;
					num = 2;
				}
				else if (parameterList[0] == "Logarithmic")
				{
					regressionType = RegressionType.Logarithmic;
					num = 2;
				}
				else if (parameterList[0] == "Power")
				{
					regressionType = RegressionType.Power;
					num = 2;
				}
				else
				{
					num = int.Parse(parameterList[0], CultureInfo.InvariantCulture);
				}
			}
			catch (Exception)
			{
				num = 2;
			}
			if (num <= 5 && num >= 1)
			{
				if (num > inputValues[0].Length)
				{
					throw new InvalidOperationException(SR.ExceptionForecastingNotEnoughDataPoints(num.ToString(CultureInfo.InvariantCulture)));
				}
				int forecastingPeriod;
				try
				{
					forecastingPeriod = int.Parse(parameterList[1], CultureInfo.InvariantCulture);
				}
				catch (Exception)
				{
					forecastingPeriod = inputValues[0].Length / 2;
				}
				bool flag;
				try
				{
					flag = bool.Parse(parameterList[2]);
				}
				catch (Exception)
				{
					flag = true;
				}
				bool flag2;
				try
				{
					flag2 = bool.Parse(parameterList[3]);
				}
				catch (Exception)
				{
					flag2 = true;
				}
				double[][] array = default(double[][]);
				this.Regression(regressionType, inputValues, out array, num, forecastingPeriod);
				if (!flag2 && !flag)
				{
					outputValues = array;
				}
				else
				{
					double[][] array2 = new double[2][]
					{
						new double[inputValues[0].Length / 2],
						new double[inputValues[0].Length / 2]
					};
					for (int i = 0; i < inputValues[0].Length / 2; i++)
					{
						array2[0][i] = inputValues[0][i];
						array2[1][i] = inputValues[1][i];
					}
					double[][] array3 = default(double[][]);
					this.Regression(regressionType, array2, out array3, num, inputValues[0].Length / 2);
					double num2 = 0.0;
					for (int j = inputValues[0].Length / 2; j < array3[1].Length; j++)
					{
						num2 += (array3[1][j] - inputValues[1][j]) * (array3[1][j] - inputValues[1][j]);
					}
					num2 /= (double)(inputValues[0].Length - inputValues[0].Length / 2);
					num2 = Math.Sqrt(num2);
					num2 /= (double)(inputValues[0].Length / 4);
					double num3 = 0.0;
					for (int k = 0; k < inputValues[0].Length; k++)
					{
						num3 += (array[1][k] - inputValues[1][k]) * (array[1][k] - inputValues[1][k]);
					}
					num3 /= (double)inputValues[0].Length;
					num3 = Math.Sqrt(num3);
					outputValues = new double[4][];
					outputValues[0] = array[0];
					outputValues[1] = array[1];
					outputValues[2] = new double[array[0].Length];
					outputValues[3] = new double[array[0].Length];
					if (!flag)
					{
						num3 = 0.0;
					}
					if (!flag2)
					{
						num2 = 0.0;
					}
					for (int l = 0; l < inputValues[0].Length; l++)
					{
						outputValues[2][l] = array[1][l] + 2.0 * num3;
						outputValues[3][l] = array[1][l] - 2.0 * num3;
					}
					double num4 = 0.0;
					for (int m = inputValues[0].Length; m < array[0].Length; m++)
					{
						num4 += num2;
						outputValues[2][m] = array[1][m] + num4 + 2.0 * num3;
						outputValues[3][m] = array[1][m] - num4 - 2.0 * num3;
					}
				}
				return;
			}
			throw new InvalidOperationException(SR.ExceptionForecastingDegreeInvalid);
		}

		private void Regression(RegressionType regressionType, double[][] inputValues, out double[][] outputValues, int polynomialDegree, int forecastingPeriod)
		{
			switch (regressionType)
			{
			case RegressionType.Exponential:
			{
				double[] array2 = new double[inputValues[1].Length];
				for (int m = 0; m < inputValues[1].Length; m++)
				{
					array2[m] = inputValues[1][m];
					if (inputValues[1][m] <= 0.0)
					{
						throw new InvalidOperationException(SR.ExceptionForecastingExponentialRegressionHasZeroYValues);
					}
					inputValues[1][m] = Math.Log(inputValues[1][m]);
				}
				this.PolynomialRegression(regressionType, inputValues, out outputValues, 2, forecastingPeriod, 0.0);
				inputValues[1] = array2;
				break;
			}
			case RegressionType.Logarithmic:
			{
				double num3 = inputValues[0][0];
				double num4 = Math.Abs(inputValues[0][0] - inputValues[0][inputValues[0].Length - 1]) / (double)(inputValues[0].Length - 1);
				if (num4 <= 0.0)
				{
					num4 = 1.0;
				}
				for (int k = 0; k < inputValues[0].Length; k++)
				{
					inputValues[0][k] = Math.Log(inputValues[0][k]);
				}
				this.PolynomialRegression(regressionType, inputValues, out outputValues, 2, forecastingPeriod, num4);
				for (int l = 0; l < outputValues[0].Length; l++)
				{
					outputValues[0][l] = num3 + (double)l * num4;
				}
				break;
			}
			case RegressionType.Power:
			{
				double[] array = new double[inputValues[1].Length];
				double num = inputValues[0][0];
				for (int i = 0; i < inputValues[1].Length; i++)
				{
					array[i] = inputValues[1][i];
					if (inputValues[1][i] <= 0.0)
					{
						throw new InvalidOperationException(SR.ExceptionForecastingPowerRegressionHasZeroYValues);
					}
				}
				double num2 = Math.Abs(inputValues[0][0] - inputValues[0][inputValues[0].Length - 1]) / (double)(inputValues[0].Length - 1);
				if (num2 <= 0.0)
				{
					num2 = 1.0;
				}
				this.PolynomialRegression(regressionType, inputValues, out outputValues, 2, forecastingPeriod, num2);
				inputValues[1] = array;
				for (int j = 0; j < outputValues[0].Length; j++)
				{
					outputValues[0][j] = num + (double)j * num2;
				}
				break;
			}
			default:
				this.PolynomialRegression(regressionType, inputValues, out outputValues, polynomialDegree, forecastingPeriod, 0.0);
				break;
			}
		}

		private void PolynomialRegression(RegressionType regressionType, double[][] inputValues, out double[][] outputValues, int polynomialDegree, int forecastingPeriod, double logInterval)
		{
			double[] array = new double[polynomialDegree];
			int num = inputValues[0].Length;
			double num2 = 1.7976931348623157E+308;
			double num3 = 1.0;
			num3 = Math.Abs(inputValues[0][0] - inputValues[0][inputValues[0].Length - 1]) / (double)(inputValues[0].Length - 1);
			if (num3 <= 0.0)
			{
				num3 = 1.0;
			}
			if (regressionType != RegressionType.Logarithmic)
			{
				for (int i = 0; i < inputValues[0].Length; i++)
				{
					if (num2 > inputValues[0][i])
					{
						num2 = inputValues[0][i];
					}
				}
				for (int j = 0; j < inputValues[0].Length; j++)
				{
					inputValues[0][j] -= num2 - 1.0;
				}
			}
			if (regressionType == RegressionType.Power)
			{
				for (int k = 0; k < inputValues[0].Length; k++)
				{
					inputValues[0][k] = Math.Log(inputValues[0][k]);
					inputValues[1][k] = Math.Log(inputValues[1][k]);
				}
			}
			double[][] array2 = new double[polynomialDegree][];
			for (int l = 0; l < polynomialDegree; l++)
			{
				array2[l] = new double[polynomialDegree];
			}
			for (int m = 0; m < polynomialDegree; m++)
			{
				for (int n = 0; n < polynomialDegree; n++)
				{
					array2[n][m] = 0.0;
					for (int num4 = 0; num4 < inputValues[0].Length; num4++)
					{
						array2[n][m] += Math.Pow(inputValues[0][num4], (double)(n + m));
					}
				}
			}
			double num5 = this.Determinant(array2);
			for (int num6 = 0; num6 < polynomialDegree; num6++)
			{
				double[][] array3 = this.CopyDeterminant(array2);
				for (int num7 = 0; num7 < polynomialDegree; num7++)
				{
					array3[num6][num7] = 0.0;
					for (int num8 = 0; num8 < inputValues[0].Length; num8++)
					{
						array3[num6][num7] += inputValues[1][num8] * Math.Pow(inputValues[0][num8], (double)num7);
					}
				}
				array[num6] = this.Determinant(array3) / num5;
			}
			outputValues = new double[2][];
			outputValues[0] = new double[num + forecastingPeriod];
			outputValues[1] = new double[num + forecastingPeriod];
			switch (regressionType)
			{
			case RegressionType.Polynomial:
				for (int num12 = 0; num12 < num + forecastingPeriod; num12++)
				{
					outputValues[0][num12] = inputValues[0][0] + (double)num12 * num3;
					outputValues[1][num12] = 0.0;
					for (int num13 = 0; num13 < polynomialDegree; num13++)
					{
						outputValues[1][num12] += array[num13] * Math.Pow(outputValues[0][num12], (double)num13);
					}
				}
				break;
			case RegressionType.Exponential:
				for (int num10 = 0; num10 < num + forecastingPeriod; num10++)
				{
					outputValues[0][num10] = inputValues[0][0] + (double)num10 * num3;
					outputValues[1][num10] = Math.Exp(array[0]) * Math.Exp(array[1] * outputValues[0][num10]);
				}
				break;
			case RegressionType.Logarithmic:
				for (int num11 = 0; num11 < num + forecastingPeriod; num11++)
				{
					outputValues[0][num11] = Math.Exp(inputValues[0][0]) + (double)num11 * logInterval;
					outputValues[1][num11] = array[1] * Math.Log(outputValues[0][num11]) + array[0];
				}
				break;
			case RegressionType.Power:
				for (int num9 = 0; num9 < num + forecastingPeriod; num9++)
				{
					outputValues[0][num9] = Math.Exp(inputValues[0][0]) + (double)num9 * logInterval;
					outputValues[1][num9] = Math.Exp(array[0]) * Math.Pow(outputValues[0][num9], array[1]);
				}
				break;
			}
			if (regressionType != RegressionType.Logarithmic)
			{
				for (int num14 = 0; num14 < num + forecastingPeriod; num14++)
				{
					outputValues[0][num14] += num2 - 1.0;
				}
			}
		}

		private double Determinant(double[][] inputDeterminant)
		{
			double num = 0.0;
			double num2 = 1.0;
			if (inputDeterminant.Length == 2)
			{
				return inputDeterminant[0][0] * inputDeterminant[1][1] - inputDeterminant[0][1] * inputDeterminant[1][0];
			}
			for (int i = 0; i < inputDeterminant.GetLength(0); i++)
			{
				double[][] inputDeterminant2 = this.MakeSubDeterminant(inputDeterminant, i);
				num += num2 * this.Determinant(inputDeterminant2) * inputDeterminant[i][0];
				num2 *= -1.0;
			}
			return num;
		}

		private double[][] MakeSubDeterminant(double[][] inputDeterminant, int columnPos)
		{
			int length = inputDeterminant.GetLength(0);
			double[][] array = new double[length - 1][];
			for (int i = 0; i < length - 1; i++)
			{
				array[i] = new double[length - 1];
			}
			int num = 0;
			for (int j = 0; j < length; j++)
			{
				if (j != columnPos)
				{
					for (int k = 1; k < length; k++)
					{
						array[num][k - 1] = inputDeterminant[j][k];
					}
					num++;
				}
			}
			return array;
		}

		private double[][] CopyDeterminant(double[][] inputDeterminant)
		{
			int length = inputDeterminant.GetLength(0);
			double[][] array = new double[length][];
			for (int i = 0; i < length; i++)
			{
				array[i] = new double[length];
			}
			for (int j = 0; j < length; j++)
			{
				for (int k = 0; k < length; k++)
				{
					array[j][k] = inputDeterminant[j][k];
				}
			}
			return array;
		}
	}
}
