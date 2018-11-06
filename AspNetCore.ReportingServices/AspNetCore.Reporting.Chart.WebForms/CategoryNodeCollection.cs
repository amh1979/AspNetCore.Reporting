using System;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal sealed class CategoryNodeCollection : IEnumerable<CategoryNode>, IEnumerable
	{
		private readonly List<CategoryNode> _nodes;

		public CategoryNode Parent
		{
			get;
			private set;
		}

		public bool AllNodesEmpty
		{
			get;
			private set;
		}

		public CategoryNodeCollection(CategoryNode parent)
		{
			this.Parent = parent;
			this._nodes = new List<CategoryNode>();
			this.AllNodesEmpty = true;
		}

		public IEnumerator<CategoryNode> GetEnumerator()
		{
			return (IEnumerator<CategoryNode>)(object)this._nodes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Add(CategoryNode node)
		{
			this.AllNodesEmpty &= node.Empty;
			this._nodes.Add(node);
		}

		public int GetDepth()
		{
			int num = 0;
			foreach (CategoryNode node in this._nodes)
			{
				num = Math.Max(num, node.GetDepth());
			}
			return num;
		}

		public void Calculate(List<Series> seriesCollection)
		{
			int num = -1;
			this.CalculateIndices(ref num);
			this.CalculateValues(seriesCollection);
		}

		private void CalculateValues(List<Series> seriesCollection)
		{
			foreach (CategoryNode node in this._nodes)
			{
				node.CalculateValues(seriesCollection);
			}
		}

		public void CalculateIndices(ref int dataPointIndex)
		{
			foreach (CategoryNode node in this._nodes)
			{
				node.CalculateIndices(ref dataPointIndex);
			}
		}

		public double GetTotalAbsoluetValue()
		{
			double num = 0.0;
			foreach (CategoryNode node in this._nodes)
			{
				num += node.GetTotalAbsoluteValue();
			}
			return num;
		}

		public void SortByAbsoluteValue(Series series)
		{
			this._nodes.Sort((CategoryNode node1, CategoryNode node2) => node2.GetValues(series).AbsoluteValue.CompareTo(node1.GetValues(series).AbsoluteValue));
		}

		public double GetTotalAbsoluteValue(Series series)
		{
			double num = 0.0;
			foreach (CategoryNode node in this._nodes)
			{
				num += node.GetValues(series).AbsoluteValue;
			}
			return num;
		}

		public bool AreAllNodesEmpty(Series series)
		{
			if (this.AllNodesEmpty)
			{
				return true;
			}
			foreach (CategoryNode node in this._nodes)
			{
				if (!node.Empty && node.GetValues(series).AbsoluteValue != 0.0)
				{
					return false;
				}
			}
			return true;
		}

		public CategoryNode GetEmptyNode()
		{
			foreach (CategoryNode node in this._nodes)
			{
				if (node.Empty)
				{
					return node;
				}
			}
			return null;
		}
	}
}
