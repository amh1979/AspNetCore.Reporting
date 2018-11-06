using System;
using System.Collections.Generic;
using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal sealed class TreeMapNode
	{
		public double Value
		{
			get;
			set;
		}

		public RectangleF Rectangle
		{
			get;
			set;
		}

		public Series Series
		{
			get;
			set;
		}

		public DataPoint DataPoint
		{
			get;
			set;
		}

		public List<TreeMapNode> Children
		{
			get;
			set;
		}

		public TreeMapNode(Series series, double value)
		{
			this.Series = series;
			this.Value = Math.Abs(value);
		}

		public TreeMapNode(DataPoint dataPoint)
		{
			this.DataPoint = dataPoint;
			this.Value = Math.Abs(dataPoint.YValues[0]);
		}
	}
}
