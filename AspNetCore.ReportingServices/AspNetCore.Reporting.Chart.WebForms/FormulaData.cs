using System;
using System.ComponentModel;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class FormulaData : ChartElement
	{
		internal const string IndexedSeriesLabelsSourceAttr = "__IndexedSeriesLabelsSource__";

		private bool ignoreEmptyPoints = true;

		private string[] extraParameters;

		internal FinancialMarkersCollection markers;

		private bool zeroXValues;

		private Statistics statistics;

		public bool IgnoreEmptyPoints
		{
			get
			{
				return this.ignoreEmptyPoints;
			}
			set
			{
				this.ignoreEmptyPoints = value;
			}
		}

		public bool StartFromFirst
		{
			get
			{
				return bool.Parse(this.extraParameters[0]);
			}
			set
			{
				if (value)
				{
					this.extraParameters[0] = true.ToString((IFormatProvider)CultureInfo.InvariantCulture);
				}
				else
				{
					this.extraParameters[0] = false.ToString((IFormatProvider)CultureInfo.InvariantCulture);
				}
			}
		}

		public Statistics Statistics
		{
			get
			{
				return this.statistics;
			}
		}

		public FormulaData()
		{
			if (this.markers == null)
			{
				this.markers = new FinancialMarkersCollection();
			}
			this.statistics = new Statistics(this);
			this.extraParameters = new string[1];
			this.extraParameters[0] = false.ToString((IFormatProvider)CultureInfo.InvariantCulture);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Formula(string formulaName, string parameters, string inputSeries, string outputSeries)
		{
			bool flag = false;
			double[][] array = null;
			string[][] outputLabels = null;
			string[] parameterList = default(string[]);
			this.SplitParameters(parameters, out parameterList);
			Series[] array2 = default(Series[]);
			int[] valueIndex = default(int[]);
			this.ConvertToArrays(inputSeries, out array2, out valueIndex, true);
			Series[] array3 = default(Series[]);
			int[] valueIndex2 = default(int[]);
			this.ConvertToArrays(outputSeries, out array3, out valueIndex2, false);
			Series[] array4 = array3;
			foreach (Series series in array4)
			{
				if (array2[0] != null)
				{
					series.XValueType = array2[0].XValueType;
				}
			}
			double[][] array5 = default(double[][]);
			this.GetDoubleArray(array2, valueIndex, out array5);
			double[][] inputValues = default(double[][]);
			if (!this.DifferentNumberOfSeries(array5))
			{
				this.RemoveEmptyValues(array5, out inputValues);
			}
			else
			{
				inputValues = array5;
			}
			string text = null;
			for (int j = 0; j < base.Common.FormulaRegistry.Count; j++)
			{
				text = base.Common.FormulaRegistry.GetModuleName(j);
				base.Common.FormulaRegistry.GetFormulaModule(text).Formula(formulaName, inputValues, out array, parameterList, this.extraParameters, out outputLabels);
				if (array != null)
				{
					if (text == SR.FormulaNameStatisticalAnalysis)
					{
						flag = true;
					}
					break;
				}
			}
			if (array == null)
			{
				throw new ArgumentException(SR.ExceptionFormulaNotFound(formulaName));
			}
			if (!flag)
			{
				this.InsertEmptyDataPoints(array5, array, out array);
			}
			this.SetDoubleArray(array3, valueIndex2, array, outputLabels);
			if (this.zeroXValues)
			{
				Series[] array6 = array3;
				foreach (Series series2 in array6)
				{
					if (series2.Points.Count > 0)
					{
						double xValue = series2.Points[series2.Points.Count - 1].XValue;
						base.Common.Chart.DataManipulator.InsertEmptyPoints(1.0, IntervalType.Number, 0.0, IntervalType.Number, 1.0, xValue, series2);
						foreach (DataPoint point in series2.Points)
						{
							point.XValue = 0.0;
						}
					}
				}
			}
			this.CopyAxisLabels(array2, array3);
		}

		private void CopyAxisLabels(Series[] inSeries, Series[] outSeries)
		{
			for (int i = 0; i < inSeries.Length && i < outSeries.Length; i++)
			{
				Series series = inSeries[i];
				Series series2 = outSeries[i];
				if (this.zeroXValues)
				{
					((DataPointAttributes)series2)["__IndexedSeriesLabelsSource__"] = series.Name;
				}
				else
				{
					int num = 0;
					foreach (DataPoint point in series.Points)
					{
						if (!string.IsNullOrEmpty(point.AxisLabel))
						{
							if (num < series2.Points.Count && point.XValue == series2.Points[num].XValue)
							{
								series2.Points[num].AxisLabel = point.AxisLabel;
							}
							else
							{
								num = 0;
								foreach (DataPoint point2 in series2.Points)
								{
									if (point.XValue == point2.XValue)
									{
										point2.AxisLabel = point.AxisLabel;
										break;
									}
									num++;
								}
							}
						}
						num++;
					}
				}
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Formula(string formulaName, Series inputSeries)
		{
			this.Formula(formulaName, "", inputSeries, inputSeries);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Formula(string formulaName, Series inputSeries, Series outputSeries)
		{
			this.Formula(formulaName, "", inputSeries, outputSeries);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Formula(string formulaName, string parameters, Series inputSeries, Series outputSeries)
		{
			this.Formula(formulaName, parameters, inputSeries.Name, outputSeries.Name);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Formula(string formulaName, string inputSeries)
		{
			this.Formula(formulaName, "", inputSeries, inputSeries);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Formula(string formulaName, string inputSeries, string outputSeries)
		{
			this.Formula(formulaName, "", inputSeries, outputSeries);
		}

		private void SetDoubleArray(Series[] outputSeries, int[] valueIndex, double[][] outputValues, string[][] outputLabels)
		{
			if (outputSeries.Length != valueIndex.Length)
			{
				throw new ArgumentException(SR.ExceptionFormulaDataItemsNumberMismatch);
			}
			if (outputSeries.Length < outputValues.Length - 1)
			{
				throw new ArgumentException(SR.ExceptionFormulaDataOutputSeriesNumberYValuesIncorrect);
			}
			int num = 0;
			foreach (Series series in outputSeries)
			{
				if (num + 1 > outputValues.Length - 1)
				{
					break;
				}
				if (series.Points.Count != outputValues[num].Length)
				{
					series.Points.Clear();
				}
				if (series.YValuesPerPoint < valueIndex[num])
				{
					series.YValuesPerPoint = valueIndex[num];
				}
				for (int j = 0; j < outputValues[0].Length; j++)
				{
					if (series.Points.Count != outputValues[num].Length)
					{
						series.Points.AddXY(outputValues[0][j], 0.0);
						if (outputLabels != null)
						{
							series.Points[j].Label = outputLabels[num][j];
						}
						if (double.IsNaN(outputValues[num + 1][j]))
						{
							series.Points[j].Empty = true;
						}
						else
						{
							series.Points[j].YValues[valueIndex[num] - 1] = outputValues[num + 1][j];
						}
					}
					else
					{
						if (series.Points[j].XValue != outputValues[0][j] && !this.zeroXValues)
						{
							throw new InvalidOperationException(SR.ExceptionFormulaXValuesNotAligned);
						}
						if (double.IsNaN(outputValues[num + 1][j]))
						{
							series.Points[j].Empty = true;
						}
						else
						{
							series.Points[j].YValues[valueIndex[num] - 1] = outputValues[num + 1][j];
							if (outputLabels != null)
							{
								series.Points[j].Label = outputLabels[num][j];
							}
						}
					}
				}
				num++;
			}
		}

		private void ConvertToArrays(string inputString, out Series[] seiesArray, out int[] valueArray, bool inputSeries)
		{
			string[] array = inputString.Split(',');
			seiesArray = new Series[array.Length];
			valueArray = new int[array.Length];
			int num = 0;
			string[] array2 = array;
			int num2 = 0;
			string text;
			while (true)
			{
				if (num2 < array2.Length)
				{
					text = array2[num2];
					string[] array3 = text.Split(':');
					if (array3.Length < 1 && array3.Length > 2)
					{
						throw new ArgumentException(SR.ExceptionFormulaDataFormatInvalid(text));
					}
					int num3 = 1;
					if (array3.Length == 2)
					{
						if (!array3[1].StartsWith("Y", StringComparison.Ordinal))
						{
							break;
						}
						array3[1] = array3[1].TrimStart('Y');
						if (array3[1].Length == 0)
						{
							num3 = 1;
						}
						else
						{
							try
							{
								num3 = int.Parse(array3[1], CultureInfo.InvariantCulture);
							}
							catch (Exception)
							{
								throw new ArgumentException(SR.ExceptionFormulaDataFormatInvalid(text));
							}
						}
					}
					valueArray[num] = num3;
					try
					{
						seiesArray[num] = base.Common.DataManager.Series[array3[0].Trim()];
					}
					catch (Exception)
					{
						if (!inputSeries)
						{
							base.Common.DataManager.Series.Add(array3[0]);
							seiesArray[num] = base.Common.DataManager.Series[array3[0]];
							goto end_IL_0103;
						}
						throw new ArgumentException(SR.ExceptionFormulaDataSeriesNameNotFoundInCollection(text));
						end_IL_0103:;
					}
					num++;
					num2++;
					continue;
				}
				return;
			}
			throw new ArgumentException(SR.ExceptionFormulaDataSeriesNameNotFound(text));
		}

		private void GetDoubleArray(Series[] inputSeries, int[] valueIndex, out double[][] output)
		{
			this.GetDoubleArray(inputSeries, valueIndex, out output, false);
		}

		private void GetDoubleArray(Series[] inputSeries, int[] valueIndex, out double[][] output, bool ignoreZeroX)
		{
			output = new double[inputSeries.Length + 1][];
			if (inputSeries.Length != valueIndex.Length)
			{
				throw new ArgumentException(SR.ExceptionFormulaDataItemsNumberMismatch2);
			}
			int num = -2147483648;
			Series series = null;
			foreach (Series series2 in inputSeries)
			{
				if (num < series2.Points.Count)
				{
					num = series2.Points.Count;
					series = series2;
				}
			}
			foreach (DataPoint point in inputSeries[0].Points)
			{
				this.zeroXValues = true;
				if (point.XValue != 0.0)
				{
					this.zeroXValues = false;
					break;
				}
			}
			if (this.zeroXValues && !ignoreZeroX)
			{
				this.CheckXValuesAlignment(inputSeries);
			}
			int num2 = 0;
			output[0] = new double[num];
			foreach (DataPoint point2 in series.Points)
			{
				if (this.zeroXValues)
				{
					output[0][num2] = (double)num2 + 1.0;
				}
				else
				{
					output[0][num2] = point2.XValue;
				}
				num2++;
			}
			int num3 = 1;
			foreach (Series series3 in inputSeries)
			{
				output[num3] = new double[series3.Points.Count];
				num2 = 0;
				foreach (DataPoint point3 in series3.Points)
				{
					if (point3.Empty)
					{
						output[num3][num2] = double.NaN;
					}
					else
					{
						try
						{
							output[num3][num2] = point3.YValues[valueIndex[num3 - 1] - 1];
						}
						catch (Exception)
						{
							throw new ArgumentException(SR.ExceptionFormulaYIndexInvalid);
						}
					}
					num2++;
				}
				num3++;
			}
		}

		public void CopySeriesValues(string inputSeries, string outputSeries)
		{
			Series[] array = default(Series[]);
			int[] valueIndex = default(int[]);
			this.ConvertToArrays(inputSeries, out array, out valueIndex, true);
			Series[] array2 = default(Series[]);
			int[] valueIndex2 = default(int[]);
			this.ConvertToArrays(outputSeries, out array2, out valueIndex2, false);
			if (array.Length != array2.Length)
			{
				throw new ArgumentException(SR.ExceptionFormulaInputOutputSeriesMismatch);
			}
			for (int i = 0; i < array.Length; i++)
			{
				Series[] array3 = new Series[2]
				{
					array[i],
					array2[i]
				};
				if (array3[1].Points.Count == 0)
				{
					foreach (DataPoint point in array3[0].Points)
					{
						DataPoint dataPoint2 = point.Clone();
						dataPoint2.series = array3[1];
						array3[1].Points.Add(dataPoint2);
					}
				}
			}
			for (int j = 0; j < array.Length; j++)
			{
				this.CheckXValuesAlignment(new Series[2]
				{
					array[j],
					array2[j]
				});
			}
			double[][] array4 = default(double[][]);
			this.GetDoubleArray(array, valueIndex, out array4, true);
			double[][] array5 = new double[array4.Length][];
			for (int k = 0; k < array4.Length; k++)
			{
				array5[k] = new double[array4[k].Length];
				for (int l = 0; l < array4[k].Length; l++)
				{
					array5[k][l] = array4[k][l];
				}
			}
			int num;
			for (num = 0; num < array.Length; num++)
			{
				if (array2[num].XValueType == ChartValueTypes.Auto)
				{
					array2[num].XValueType = array[num].XValueType;
					array2[num].autoXValueType = array[num].autoXValueType;
				}
				if (array2[num].YValueType == ChartValueTypes.Auto)
				{
					array2[num].YValueType = array[num].YValueType;
					array2[num].autoYValueType = array[num].autoYValueType;
				}
				num++;
			}
			this.SetDoubleArray(array2, valueIndex2, array5, null);
		}

		private void RemoveEmptyValues(double[][] input, out double[][] output)
		{
			output = new double[input.Length][];
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < input[0].Length; i++)
			{
				bool flag = false;
				for (num = 0; num < input.Length; num++)
				{
					if (num < input[num].Length && double.IsNaN(input[num][i]))
					{
						flag = true;
					}
				}
				if (!flag)
				{
					num2++;
				}
				if (flag)
				{
					for (num = 1; num < input.Length; num++)
					{
						input[num][i] = double.NaN;
					}
				}
			}
			for (num = 0; num < input.Length; num++)
			{
				output[num] = new double[num2];
				int num3 = 0;
				for (int j = 0; j < input[0].Length; j++)
				{
					if (j < input[num].Length && !double.IsNaN(input[1][j]))
					{
						output[num][num3] = input[num][j];
						num3++;
					}
				}
			}
		}

		private void InsertEmptyDataPoints(double[][] input, double[][] inputWithoutEmpty, out double[][] output)
		{
			output = inputWithoutEmpty;
		}

		private void SplitParameters(string parameters, out string[] parameterList)
		{
			parameterList = parameters.Split(',');
			for (int i = 0; i < parameterList.Length; i++)
			{
				parameterList[i] = parameterList[i].Trim();
			}
		}

		private bool DifferentNumberOfSeries(double[][] input)
		{
			for (int i = 0; i < input.Length - 1; i++)
			{
				if (input[i].Length != input[i + 1].Length)
				{
					return true;
				}
			}
			return false;
		}

		internal void CheckXValuesAlignment(Series[] series)
		{
			if (series.Length <= 1)
			{
				return;
			}
			int num = 0;
			while (true)
			{
				if (num < series.Length - 1)
				{
					if (series[num].Points.Count == series[num + 1].Points.Count)
					{
						for (int i = 0; i < series[num].Points.Count; i++)
						{
							if (series[num].Points[i].XValue != series[num + 1].Points[i].XValue)
							{
								throw new ArgumentException(SR.ExceptionFormulaDataSeriesAreNotAlignedDifferentXValues(series[num].Name, series[num + 1].Name));
							}
						}
						num++;
						continue;
					}
					break;
				}
				return;
			}
			throw new ArgumentException(SR.ExceptionFormulaDataSeriesAreNotAlignedDifferentDataPoints(series[num].Name, series[num + 1].Name));
		}

		public void FormulaFinancial(FinancialFormula formulaName, string parameters, string inputSeries, string outputSeries)
		{
			this.Formula(formulaName.ToString(), parameters, inputSeries, outputSeries);
		}

		public void FormulaFinancial(FinancialFormula formulaName, Series inputSeries)
		{
			this.Formula(formulaName.ToString(), "", inputSeries, inputSeries);
		}

		public void FormulaFinancial(FinancialFormula formulaName, Series inputSeries, Series outputSeries)
		{
			this.Formula(formulaName.ToString(), "", inputSeries, outputSeries);
		}

		public void FormulaFinancial(FinancialFormula formulaName, string parameters, Series inputSeries, Series outputSeries)
		{
			this.Formula(formulaName.ToString(), parameters, inputSeries.Name, outputSeries.Name);
		}

		public void FormulaFinancial(FinancialFormula formulaName, string inputSeries)
		{
			this.Formula(formulaName.ToString(), "", inputSeries, inputSeries);
		}

		public void FormulaFinancial(FinancialFormula formulaName, string inputSeries, string outputSeries)
		{
			this.Formula(formulaName.ToString(), "", inputSeries, outputSeries);
		}
	}
}
