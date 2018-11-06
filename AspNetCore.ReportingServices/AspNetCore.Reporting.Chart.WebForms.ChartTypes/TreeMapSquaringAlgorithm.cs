using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AspNetCore.Reporting.Chart.WebForms.ChartTypes
{
	internal sealed class TreeMapSquaringAlgorithm
	{
		private sealed class TreeMapDataPointInfo
		{
			public RectangleF Rect
			{
				get;
				set;
			}

			public TreeMapNode Node
			{
				get;
				set;
			}

			public TreeMapDataPointInfo(RectangleF rect, TreeMapNode dataPoint)
			{
				this.Rect = rect;
				this.Node = dataPoint;
			}
		}

		private IList<TreeMapNode> dataPoint;

		private RectangleF currentRectangle;

		private int currentStart;

		private double factor;

		private IEnumerable<TreeMapDataPointInfo> Split(RectangleF parentRectangle, double totalValue, IEnumerable<TreeMapNode> children)
		{
			if (children != null && children.Any() && totalValue != 0.0)
			{
				if (!(parentRectangle.Width <= 0.0) && !(parentRectangle.Height <= 0.0))
				{
					this.currentRectangle = new RectangleF(parentRectangle.X, parentRectangle.Y, parentRectangle.Width, parentRectangle.Height);
					this.dataPoint = (from child in children
					where child.Value != 0.0
					orderby child.Value descending
					select child).ToArray();
					this.factor = (double)(this.currentRectangle.Width * this.currentRectangle.Height) / totalValue;
					return this.BuildTreeMap().ToArray();
				}
				return from child in children
				select new TreeMapDataPointInfo(new RectangleF(0f, 0f, 0f, 0f), child);
			}
			return Enumerable.Empty<TreeMapDataPointInfo>();
		}

		private IEnumerable<TreeMapDataPointInfo> BuildTreeMap()
		{
			this.currentStart = 0;
			while (this.currentStart < this.dataPoint.Count)
			{
				foreach (TreeMapDataPointInfo item in this.BuildTreeMapStep())
				{
					yield return item;
				}
			}
		}

		private IEnumerable<TreeMapDataPointInfo> BuildTreeMapStep()
		{
			int last = this.currentStart;
			double total = 0.0;
			double prevAspect = double.PositiveInfinity;
			double widthOrHeight = 0.0;
			bool horizontal = this.currentRectangle.Width > this.currentRectangle.Height;
			for (; last < this.dataPoint.Count; last++)
			{
				total += this.GetArea(last);
				widthOrHeight = total / (double)(horizontal ? this.currentRectangle.Height : this.currentRectangle.Width);
				double num = Math.Max(this.GetAspect(this.currentStart, widthOrHeight), this.GetAspect(last, widthOrHeight));
				if (num > prevAspect)
				{
					total -= this.GetArea(last);
					widthOrHeight = total / (double)(horizontal ? this.currentRectangle.Height : this.currentRectangle.Width);
					last--;
					break;
				}
				prevAspect = num;
			}
			if (last == this.dataPoint.Count)
			{
				last--;
			}
			double x = (double)this.currentRectangle.Left;
			double y = (double)this.currentRectangle.Top;
			for (int i = this.currentStart; i <= last; i++)
			{
				if (horizontal)
				{
					double h = this.GetArea(i) / widthOrHeight;
					RectangleF rect2 = new RectangleF((float)x, (float)y, (float)widthOrHeight, (float)h);
					yield return new TreeMapDataPointInfo(rect2, this.dataPoint[i]);
					y += h;
				}
				else
				{
					double w = this.GetArea(i) / widthOrHeight;
					RectangleF rect = new RectangleF((float)x, (float)y, (float)w, (float)widthOrHeight);
					yield return new TreeMapDataPointInfo(rect, this.dataPoint[i]);
					x += w;
				}
			}
			this.currentStart = last + 1;
			if (horizontal)
			{
				this.currentRectangle = new RectangleF((float)((double)this.currentRectangle.Left + widthOrHeight), this.currentRectangle.Top, (float)Math.Max(0.0, (double)this.currentRectangle.Width - widthOrHeight), this.currentRectangle.Height);
			}
			else
			{
				this.currentRectangle = new RectangleF(this.currentRectangle.Left, (float)((double)this.currentRectangle.Top + widthOrHeight), this.currentRectangle.Width, (float)Math.Max(0.0, (double)this.currentRectangle.Height - widthOrHeight));
			}
		}

		private double GetArea(int i)
		{
			return this.dataPoint[i].Value * this.factor;
		}

		private double GetAspect(int i, double width)
		{
			double num = this.GetArea(i) / (width * width);
			if (num < 1.0)
			{
				num = 1.0 / num;
			}
			return num;
		}

		public static void CalculateRectangles(RectangleF containerRect, IEnumerable<TreeMapNode> treeMapNodes, double value)
		{
			TreeMapSquaringAlgorithm treeMapSquaringAlgorithm = new TreeMapSquaringAlgorithm();
			IEnumerable<TreeMapDataPointInfo> enumerable = treeMapSquaringAlgorithm.Split(containerRect, value, treeMapNodes);
			foreach (TreeMapDataPointInfo item in enumerable)
			{
				RectangleF rect = item.Rect;
				TreeMapNode node = item.Node;
				node.Rectangle = rect;
				if (node.Children != null)
				{
					TreeMapSquaringAlgorithm.CalculateRectangles(node.Rectangle, node.Children, node.Value);
				}
			}
		}
	}
}
