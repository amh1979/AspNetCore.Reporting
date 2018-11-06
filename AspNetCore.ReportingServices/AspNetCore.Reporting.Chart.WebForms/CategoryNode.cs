using System;
using System.Collections.Generic;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal sealed class CategoryNode
	{
		internal sealed class Values
		{
			public double Value
			{
				get;
				private set;
			}

			public double AbsoluteValue
			{
				get;
				private set;
			}

			public Values()
			{
				this.Value = 0.0;
				this.AbsoluteValue = 0.0;
			}

			public void AddValues(double value, double absoluteValue)
			{
				this.Value += value;
				this.AbsoluteValue += absoluteValue;
			}
		}

		private Dictionary<Series, DataPoint> _dataPoints;

		private Dictionary<Series, Values> _values;

		private CategoryNodeCollection _parentCollection;

		public int Index
		{
			get;
			private set;
		}

		public bool Empty
		{
			get;
			private set;
		}

		public string Label
		{
			get;
			private set;
		}

		public string ToolTip
		{
			get;
			set;
		}

		public string Href
		{
			get;
			set;
		}

		public string LabelToolTip
		{
			get;
			set;
		}

		public string LabelHref
		{
			get;
			set;
		}

		public CategoryNode Parent
		{
			get
			{
				return this._parentCollection.Parent;
			}
		}

		public CategoryNodeCollection Children
		{
			get;
			set;
		}

		public CategoryNode(CategoryNodeCollection parentCollection, bool empty, string label)
		{
			this._parentCollection = parentCollection;
			this.Empty = empty;
			this.Label = label;
			this.ToolTip = label;
			this.Href = string.Empty;
			this.LabelToolTip = string.Empty;
			this.LabelHref = string.Empty;
		}

		public void AddDataPoint(DataPoint dataPoint)
		{
			if (this._dataPoints == null)
			{
				this._dataPoints = new Dictionary<Series, DataPoint>();
			}
			this._dataPoints[dataPoint.series] = dataPoint;
		}

		public DataPoint GetDataPoint(Series series)
		{
			if (this._dataPoints == null)
			{
				return null;
			}
			DataPoint result = default(DataPoint);
			this._dataPoints.TryGetValue(series, out result);
			return result;
		}

		public CategoryNode GetDataPointNode(Series series)
		{
			if (this._dataPoints != null)
			{
				return this;
			}
			if (this.Children != null && this.Children.AreAllNodesEmpty(series))
			{
				CategoryNode emptyNode = this.Children.GetEmptyNode();
				if (emptyNode != null)
				{
					return emptyNode.GetDataPointNode(series);
				}
				return null;
			}
			return null;
		}

		public int GetDepth()
		{
			if (this.Empty)
			{
				return 0;
			}
			if (this.Children == null)
			{
				return 1;
			}
			return this.Children.GetDepth() + 1;
		}

		public void CalculateIndices(ref int dataPointIndex)
		{
			if (this.Children == null)
			{
				dataPointIndex++;
				this.Index = dataPointIndex;
			}
			else
			{
				this.Index = -1;
				this.Children.CalculateIndices(ref dataPointIndex);
			}
		}

		public void CalculateValues(List<Series> seriesCollection)
		{
			foreach (Series item in seriesCollection)
			{
				this.GetValues(item);
			}
		}

		public double GetTotalAbsoluteValue()
		{
			double num = 0.0;
			foreach (Series key in this._values.Keys)
			{
				num += this.GetValues(key).AbsoluteValue;
			}
			return num;
		}

		public Values GetValues(Series series)
		{
			if (this._values == null)
			{
				this._values = new Dictionary<Series, Values>();
			}
			Values values = default(Values);
			if (!this._values.TryGetValue(series, out values))
			{
				values = new Values();
				if (this.Children == null)
				{
					double value;
					double absoluteValue;
					DataPoint dataPoint = default(DataPoint);
					if (this._dataPoints == null)
					{
						value = (absoluteValue = 0.0);
					}
					else if (!this._dataPoints.TryGetValue(series, out dataPoint) || dataPoint.Empty)
					{
						value = (absoluteValue = 0.0);
					}
					else
					{
						value = dataPoint.YValues[0];
						absoluteValue = Math.Abs(value);
					}
					values.AddValues(value, absoluteValue);
				}
				else
				{
					foreach (CategoryNode child in this.Children)
					{
						Values values2 = child.GetValues(series);
						values.AddValues(values2.Value, values2.AbsoluteValue);
					}
				}
				this._values.Add(series, values);
			}
			return values;
		}
	}
}
