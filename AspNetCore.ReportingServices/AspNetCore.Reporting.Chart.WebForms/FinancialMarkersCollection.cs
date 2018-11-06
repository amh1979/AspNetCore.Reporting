using AspNetCore.Reporting.Chart.WebForms.Formulas;
using System;
using System.Collections;
using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class FinancialMarkersCollection : IList, ICollection, IEnumerable
	{
		private ArrayList array = new ArrayList();

		internal Series series;

		private FinancialMarkers markers;

		public bool IsReadOnly
		{
			get
			{
				return this.array.IsReadOnly;
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return this.array.IsFixedSize;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return this.array.IsSynchronized;
			}
		}

		public int Count
		{
			get
			{
				return this.array.Count;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this.array.SyncRoot;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this.array[index];
			}
			set
			{
				this.array[index] = value;
			}
		}

		[SRDescription("DescriptionAttributeFinancialMarkersCollection_Item")]
		public FinancialMarker this[object parameter]
		{
			get
			{
				if (parameter is int)
				{
					return (FinancialMarker)this.array[(int)parameter];
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgumentType);
			}
			set
			{
				if (parameter is int)
				{
					this.array[(int)parameter] = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgumentType);
			}
		}

		public int Add(FinancialMarkerType markerName, int firstPointIndex, int secondPointIndex, int firstYIndex, int secondYIndex, Color lineColor, int lineWidth, Color textColor, Font textFont)
		{
			FinancialMarker value = new FinancialMarker(markerName, firstPointIndex, secondPointIndex, firstYIndex, secondYIndex, lineColor, lineWidth, textColor, textFont);
			int result = this.array.Add(value);
			this.Invalidate();
			return result;
		}

		public int Add(FinancialMarkerType markerName, int firstPointIndex, int secondPointIndex, int firstYIndex, int secondYIndex, Color lineColor, int lineWidth)
		{
			return this.Add(markerName, firstPointIndex, secondPointIndex, firstYIndex, secondYIndex, lineColor, lineWidth, Color.Black, new Font(ChartPicture.GetDefaultFontFamilyName(), 8f));
		}

		public int Add(FinancialMarkerType markerName, int firstPointIndex, int secondPointIndex, int firstYIndex, int secondYIndex)
		{
			return this.Add(markerName, firstPointIndex, secondPointIndex, firstYIndex, secondYIndex, Color.Gray, 1, Color.Black, new Font(ChartPicture.GetDefaultFontFamilyName(), 8f));
		}

		public int Add(FinancialMarkerType markerName, int firstPointIndex, int secondPointIndex)
		{
			return this.Add(markerName, firstPointIndex, secondPointIndex, 0, 0, Color.Gray, 1, Color.Black, new Font(ChartPicture.GetDefaultFontFamilyName(), 8f));
		}

		public int Add(object value)
		{
			if (!(value is FinancialMarker))
			{
				throw new ArgumentException(SR.ExceptionFinancialMarkerObjectRequired);
			}
			this.Invalidate();
			return this.array.Add(value);
		}

		public void Insert(int index, FinancialMarkerType markerName, int firstPointIndex, int secondPointIndex, int firstYIndex, int secondYIndex, Color lineColor, int lineWidth, Color textColor, Font textFont)
		{
			FinancialMarker value = new FinancialMarker(markerName, firstPointIndex, secondPointIndex, firstYIndex, secondYIndex, lineColor, lineWidth, textColor, textFont);
			this.array.Insert(index, value);
		}

		public void Insert(int index, FinancialMarkerType markerName, int firstPointIndex, int secondPointIndex, int firstYIndex, int secondYIndex, Color lineColor, int lineWidth)
		{
			this.Insert(index, markerName, firstPointIndex, secondPointIndex, firstYIndex, secondYIndex, lineColor, lineWidth, Color.Black, new Font(ChartPicture.GetDefaultFontFamilyName(), 8f));
		}

		public void Insert(int index, FinancialMarkerType markerName, int firstPointIndex, int secondPointIndex, int firstYIndex, int secondYIndex)
		{
			this.Insert(index, markerName, firstPointIndex, secondPointIndex, firstYIndex, secondYIndex, Color.Gray, 1, Color.Black, new Font(ChartPicture.GetDefaultFontFamilyName(), 8f));
		}

		public void Insert(int index, FinancialMarkerType markerName, int firstPointIndex, int secondPointIndex)
		{
			this.Insert(index, markerName, firstPointIndex, secondPointIndex, 0, 0, Color.Gray, 1, Color.Black, new Font(ChartPicture.GetDefaultFontFamilyName(), 8f));
		}

		public void Insert(int index, FinancialMarker value)
		{
			this.Insert(index, (object)value);
		}

		public void Insert(int index, object value)
		{
			if (!(value is FinancialMarker))
			{
				throw new ArgumentException(SR.ExceptionFinancialMarkerObjectRequired);
			}
			this.Invalidate();
			this.array.Insert(index, value);
		}

		int IList.IndexOf(object value)
		{
			return this.array.IndexOf(value);
		}

		bool IList.Contains(object value)
		{
			return this.array.Contains(value);
		}

		void IList.Remove(object value)
		{
			this.array.Remove(value);
			this.Invalidate();
		}

		public int IndexOf(FinancialMarker value)
		{
			return this.array.IndexOf(value);
		}

		public bool Contains(FinancialMarker value)
		{
			return this.array.Contains(value);
		}

		public void Remove(FinancialMarker value)
		{
			this.array.Remove(value);
			this.Invalidate();
		}

		public void RemoveAt(int index)
		{
			this.array.RemoveAt(index);
			this.Invalidate();
		}

		public void Clear()
		{
			this.array.Clear();
			this.Invalidate();
		}

		public IEnumerator GetEnumerator()
		{
			return this.array.GetEnumerator();
		}

		public void CopyTo(Array array, int index)
		{
			this.array.CopyTo(array, index);
			this.Invalidate();
		}

		public FinancialMarkersCollection()
		{
			this.markers = new FinancialMarkers();
		}

		internal void DrawMarkers(ChartGraphics graph, ChartPicture chart)
		{
			if (this.array != null && this.array.Count != 0)
			{
				foreach (FinancialMarker item in this.array)
				{
					this.markers.DrawMarkers(graph, chart, item.MarkerType, this.series, item.FirstPointIndex, item.FirstYIndex, item.SecondPointIndex, item.SecondYIndex, item.LineColor, item.LineWidth, item.LineStyle, item.TextColor, item.Font);
				}
			}
		}

		private void Invalidate()
		{
		}
	}
}
