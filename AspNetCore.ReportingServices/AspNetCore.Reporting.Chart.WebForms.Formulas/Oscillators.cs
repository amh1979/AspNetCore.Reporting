using System;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms.Formulas
{
	internal class Oscillators : PriceIndicators
	{
		public override string Name
		{
			get
			{
				return "Oscillators";
			}
		}

		private void ChaikinOscillator(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 5)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresFourArrays);
			}
			base.CheckNumOfValues(inputValues, 4);
			int num;
			try
			{
				num = int.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num = 3;
			}
			int num2;
			try
			{
				num2 = int.Parse(parameterList[1], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num2 = 10;
			}
			if (num <= num2 && num2 > 0 && num > 0)
			{
				bool flag = bool.Parse(extraParameterList[0]);
				VolumeIndicators volumeIndicators = new VolumeIndicators();
				double[][] array = new double[2][];
				volumeIndicators.AccumulationDistribution(inputValues, out array, parameterList, extraParameterList);
				double[] array2 = default(double[]);
				base.ExponentialMovingAverage(array[1], out array2, num2, flag);
				double[] array3 = default(double[]);
				base.ExponentialMovingAverage(array[1], out array3, num, flag);
				outputValues = new double[2][];
				int num3 = Math.Min(array3.Length, array2.Length);
				outputValues[0] = new double[num3];
				outputValues[1] = new double[num3];
				int num4 = 0;
				for (int i = inputValues[1].Length - num3; i < inputValues[1].Length; i++)
				{
					outputValues[0][num4] = inputValues[0][i];
					if (flag)
					{
						outputValues[1][num4] = array3[num4] - array2[num4];
					}
					else if (num4 + num2 - num < array3.Length)
					{
						outputValues[1][num4] = array3[num4 + num2 - num] - array2[num4];
					}
					else
					{
						outputValues[1][num4] = double.NaN;
					}
					num4++;
				}
				return;
			}
			throw new ArgumentException(SR.ExceptionOscillatorObjectInvalidPeriod);
		}

		private void DetrendedPriceOscillator(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
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
			catch (Exception ex)
			{
				if (ex.Message == SR.ExceptionObjectReferenceIsNull)
				{
					throw new InvalidOperationException(SR.ExceptionPriceIndicatorsPeriodMissing);
				}
				throw new InvalidOperationException(SR.ExceptionPriceIndicatorsPeriodMissing + ex.Message);
			}
			if (num <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodParameterIsNegative);
			}
			double[] array = default(double[]);
			base.MovingAverage(inputValues[1], out array, num, false);
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length - num * 3 / 2];
			outputValues[1] = new double[inputValues[1].Length - num * 3 / 2];
			for (int i = 0; i < outputValues[1].Length; i++)
			{
				outputValues[0][i] = inputValues[0][i + num + num / 2];
				outputValues[1][i] = inputValues[1][i + num + num / 2] - array[i];
			}
		}

		private void VolatilityChaikins(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
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
				num = 10;
			}
			if (num <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionOscillatorNegativePeriodParameter);
			}
			int num2;
			try
			{
				num2 = int.Parse(parameterList[1], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num2 = 10;
			}
			if (num2 <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionOscillatorNegativeSignalPeriod);
			}
			double[] array = new double[inputValues[1].Length];
			for (int i = 0; i < inputValues[1].Length; i++)
			{
				array[i] = inputValues[1][i] - inputValues[2][i];
			}
			double[] array2 = default(double[]);
			base.ExponentialMovingAverage(array, out array2, num2, false);
			outputValues = new double[2][];
			outputValues[0] = new double[array2.Length - num];
			outputValues[1] = new double[array2.Length - num];
			for (int j = 0; j < outputValues[1].Length; j++)
			{
				outputValues[0][j] = inputValues[0][j + num + num2 - 1];
				if (array2[j] != 0.0)
				{
					outputValues[1][j] = (array2[j + num] - array2[j]) / array2[j] * 100.0;
				}
				else
				{
					outputValues[1][j] = 0.0;
				}
			}
		}

		private void VolumeOscillator(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
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
				num = 5;
			}
			int num2;
			try
			{
				num2 = int.Parse(parameterList[1], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num2 = 10;
			}
			if (num <= num2 && num2 > 0 && num > 0)
			{
				bool flag;
				try
				{
					flag = bool.Parse(parameterList[2]);
				}
				catch (Exception)
				{
					flag = true;
				}
				double[] array = default(double[]);
				base.MovingAverage(inputValues[1], out array, num, false);
				double[] array2 = default(double[]);
				base.MovingAverage(inputValues[1], out array2, num2, false);
				outputValues = new double[2][];
				outputValues[0] = new double[array2.Length];
				outputValues[1] = new double[array2.Length];
				for (int i = 0; i < array2.Length; i++)
				{
					outputValues[0][i] = inputValues[0][i + num2 - 1];
					outputValues[1][i] = array[i + num] - array2[i];
					if (flag)
					{
						if (array2[i] == 0.0)
						{
							outputValues[1][i] = 0.0;
						}
						else
						{
							outputValues[1][i] = outputValues[1][i] / array[i + num] * 100.0;
						}
					}
				}
				return;
			}
			throw new ArgumentException(SR.ExceptionOscillatorObjectInvalidPeriod);
		}

		internal void StochasticIndicator(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
		{
			if (inputValues.Length != 4)
			{
				throw new ArgumentException(SR.ExceptionPriceIndicatorsFormulaRequiresThreeArrays);
			}
			base.CheckNumOfValues(inputValues, 3);
			int num;
			try
			{
				num = int.Parse(parameterList[1], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num = 10;
			}
			if (num <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodParameterIsNegative);
			}
			int num2;
			try
			{
				num2 = int.Parse(parameterList[0], CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				num2 = 10;
			}
			if (num2 <= 0)
			{
				throw new InvalidOperationException(SR.ExceptionPeriodParameterIsNegative);
			}
			outputValues = new double[3][];
			outputValues[0] = new double[inputValues[0].Length - num2 - num + 2];
			outputValues[1] = new double[inputValues[0].Length - num2 - num + 2];
			outputValues[2] = new double[inputValues[0].Length - num2 - num + 2];
			double[] array = new double[inputValues[0].Length - num2 + 1];
			for (int i = num2 - 1; i < inputValues[0].Length; i++)
			{
				double num3 = 1.7976931348623157E+308;
				double num4 = -1.7976931348623157E+308;
				for (int j = i - num2 + 1; j <= i; j++)
				{
					if (num3 > inputValues[2][j])
					{
						num3 = inputValues[2][j];
					}
					if (num4 < inputValues[1][j])
					{
						num4 = inputValues[1][j];
					}
				}
				array[i - num2 + 1] = (inputValues[3][i] - num3) / (num4 - num3) * 100.0;
				if (i >= num2 + num - 2)
				{
					outputValues[0][i - num2 - num + 2] = inputValues[0][i];
					outputValues[1][i - num2 - num + 2] = array[i - num2 + 1];
				}
			}
			base.MovingAverage(array, out outputValues[2], num, false);
		}

		internal void WilliamsR(double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList)
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
			outputValues = new double[2][];
			outputValues[0] = new double[inputValues[0].Length - num + 1];
			outputValues[1] = new double[inputValues[0].Length - num + 1];
			for (int i = num - 1; i < inputValues[0].Length; i++)
			{
				double num2 = 1.7976931348623157E+308;
				double num3 = -1.7976931348623157E+308;
				for (int j = i - num + 1; j <= i; j++)
				{
					if (num2 > inputValues[2][j])
					{
						num2 = inputValues[2][j];
					}
					if (num3 < inputValues[1][j])
					{
						num3 = inputValues[1][j];
					}
				}
				outputValues[0][i - num + 1] = inputValues[0][i];
				outputValues[1][i - num + 1] = (num3 - inputValues[3][i]) / (num3 - num2) * -100.0;
			}
		}

		public override void Formula(string formulaName, double[][] inputValues, out double[][] outputValues, string[] parameterList, string[] extraParameterList, out string[][] outLabels)
		{
			outputValues = null;
			outLabels = null;
			string text = formulaName.ToUpper(CultureInfo.InvariantCulture);
			try
			{
				switch (text)
				{
				case "STOCHASTICINDICATOR":
					this.StochasticIndicator(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "CHAIKINOSCILLATOR":
					this.ChaikinOscillator(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "DETRENDEDPRICEOSCILLATOR":
					this.DetrendedPriceOscillator(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "VOLATILITYCHAIKINS":
					this.VolatilityChaikins(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "VOLUMEOSCILLATOR":
					this.VolumeOscillator(inputValues, out outputValues, parameterList, extraParameterList);
					break;
				case "WILLIAMSR":
					this.WilliamsR(inputValues, out outputValues, parameterList, extraParameterList);
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
