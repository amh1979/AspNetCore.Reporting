using System;
using System.Collections;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeLegendCollection_LegendCollection")]
	internal class LegendCollection : IList, ICollection, IEnumerable
	{
		private ArrayList array = new ArrayList();

		internal CommonElements common;

		[SRDescription("DescriptionAttributeLegendCollection_Item")]
		public Legend this[object parameter]
		{
			get
			{
				if (parameter is int)
				{
					return (Legend)this.array[(int)parameter];
				}
				if (parameter is string)
				{
					foreach (Legend item in this.array)
					{
						if (item.Name == (string)parameter)
						{
							return item;
						}
					}
					if ((string)parameter == "Default" && this.array.Count > 0)
					{
						return (Legend)this.array[0];
					}
					throw new ArgumentException(SR.ExceptionLegendNotFound((string)parameter));
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgumentType);
			}
			set
			{
				int index = this.GetIndex(value.Name);
				if (parameter is int)
				{
					if (index != -1 && index != (int)parameter)
					{
						throw new ArgumentException(SR.ExceptionLegendAddedIsNotUnique(value.Name));
					}
					this.array[(int)parameter] = value;
					goto IL_00d6;
				}
				if (parameter is string)
				{
					int num = 0;
					foreach (Legend item in this.array)
					{
						if (item.Name == (string)parameter)
						{
							if (index != -1 && index != num)
							{
								throw new ArgumentException(SR.ExceptionLegendAddedIsNotUnique(value.Name));
							}
							this.array[num] = value;
							break;
						}
						num++;
					}
					goto IL_00d6;
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgumentType);
				IL_00d6:
				this.Invalidate();
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

		public bool IsFixedSize
		{
			get
			{
				return this.array.IsFixedSize;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.array.IsReadOnly;
			}
		}

		public int Count
		{
			get
			{
				return this.array.Count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return this.array.IsSynchronized;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this.array.SyncRoot;
			}
		}

		public LegendCollection()
		{
		}

		public LegendCollection(CommonElements common)
		{
			this.common = common;
		}

		public int Add(string name)
		{
			Legend value = new Legend(this.common, name);
			int result = this.Add(value);
			this.Invalidate();
			return result;
		}

		public void Insert(int index, string name)
		{
			Legend value = new Legend(this.common, name);
			this.Insert(index, value);
			this.Invalidate();
		}

		public void Clear()
		{
			this.array.Clear();
			this.Invalidate();
		}

		bool IList.Contains(object value)
		{
			return this.array.Contains(value);
		}

		public bool Contains(Legend value)
		{
			return this.array.Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return this.array.IndexOf(value);
		}

		public int IndexOf(Legend value)
		{
			return this.array.IndexOf(value);
		}

		void IList.Remove(object value)
		{
			this.array.Remove(value);
			this.Invalidate();
		}

		public void Remove(Legend value)
		{
			((IList)this).Remove((object)value);
		}

		public void RemoveAt(int index)
		{
			this.array.RemoveAt(index);
			this.Invalidate();
		}

		public int Add(object value)
		{
			if (!(value is Legend))
			{
				throw new ArgumentException(SR.ExceptionLegendAddedHasWrongType);
			}
			if (((Legend)value).Name.Length == 0)
			{
				string text = this.CreateName(null);
				if (text == null)
				{
					throw new ArgumentException(SR.ExceptionLegendAddedIsNotUnique(text));
				}
				((Legend)value).Name = text;
			}
			bool flag = true;
			int result = 0;
			if (((Legend)value).Name == "Default")
			{
				int index = this.GetIndex("Default");
				if (index >= 0)
				{
					this.array[index] = value;
					flag = false;
					result = index;
				}
			}
			if (flag && this.GetIndex(((Legend)value).Name) != -1)
			{
				throw new ArgumentException(SR.ExceptionLegendAddedIsNotUnique(((Legend)value).Name));
			}
			((Legend)value).Common = this.common;
			this.Invalidate();
			if (flag)
			{
				result = this.array.Add(value);
			}
			return result;
		}

		public void Insert(int index, Legend value)
		{
			value.Common = this.common;
			this.Insert(index, (object)value);
			this.Invalidate();
		}

		public void Insert(int index, object value)
		{
			if (!(value is Legend))
			{
				throw new ArgumentException(SR.ExceptionLegendInsertedHasWrongType);
			}
			if (((Legend)value).Name.Length == 0)
			{
				string text = this.CreateName(null);
				if (text == null)
				{
					throw new ArgumentException(SR.ExceptionLegendAddedIsNotUnique(text));
				}
				((Legend)value).Name = text;
			}
			bool flag = true;
			if (((Legend)value).Name == "Default")
			{
				int index2 = this.GetIndex("Default");
				if (index2 >= 0)
				{
					this.array[index2] = value;
					flag = false;
				}
			}
			if (flag && this.GetIndex(((Legend)value).Name) != -1)
			{
				throw new ArgumentException(SR.ExceptionLegendAddedIsNotUnique(((Legend)value).Name));
			}
			((Legend)value).Common = this.common;
			if (flag)
			{
				this.array.Insert(index, value);
			}
			this.Invalidate();
		}

		public void CopyTo(Array array, int index)
		{
			this.array.CopyTo(array, index);
			this.Invalidate();
		}

		public IEnumerator GetEnumerator()
		{
			return this.array.GetEnumerator();
		}

		private bool UniqueName(string name)
		{
			foreach (Legend item in this.array)
			{
				if (item.Name == name)
				{
					return false;
				}
			}
			return true;
		}

		private string CreateName(string Name)
		{
			if (Name != null && this.UniqueName(Name))
			{
				return Name;
			}
			int num = 1;
			while (num < 2147483647)
			{
				string text = "Legend" + num.ToString(CultureInfo.InvariantCulture);
				num++;
				if (this.UniqueName(text))
				{
					return text;
				}
			}
			return null;
		}

		public int GetIndex(string name)
		{
			int result = -1;
			int num = 0;
			while (num < this.array.Count)
			{
				if (!(this[num].Name == name))
				{
					num++;
					continue;
				}
				result = num;
				break;
			}
			return result;
		}

		private void Invalidate()
		{
		}

		internal void CalcLegendPosition(ChartGraphics chartGraph, ref RectangleF chartAreasRectangle, float maxLegendSize, float elementSpacing)
		{
			foreach (Legend item in this.array)
			{
				if (item.IsEnabled() && item.DockToChartArea == "NotSet" && item.Position.Auto)
				{
					item.CalcLegendPosition(chartGraph, ref chartAreasRectangle, maxLegendSize, elementSpacing);
				}
			}
		}

		internal void CalcOutsideLegendPosition(ChartGraphics chartGraph, ChartArea area, ref RectangleF chartAreasRectangle, float maxLegendSize, float elementSpacing)
		{
			if (this.common != null && this.common.ChartPicture != null)
			{
				float num = Math.Min((float)(chartAreasRectangle.Height / 100.0 * elementSpacing), (float)(chartAreasRectangle.Width / 100.0 * elementSpacing));
				foreach (Legend item in this.array)
				{
					if (item.DockToChartArea != "NotSet")
					{
						try
						{
							ChartArea chartArea = this.common.ChartPicture.ChartAreas[item.DockToChartArea];
						}
						catch
						{
							throw new ArgumentException(SR.ExceptionLegendDockedChartAreaIsMissing(item.DockToChartArea));
						}
					}
					if (item.IsEnabled() && !item.DockInsideChartArea && item.DockToChartArea == area.Name && item.Position.Auto)
					{
						item.CalcLegendPosition(chartGraph, ref chartAreasRectangle, maxLegendSize, num);
						RectangleF rectangleF = item.Position.ToRectangleF();
						if (item.Docking == LegendDocking.Top)
						{
							rectangleF.Y -= num;
							if (!area.Position.Auto)
							{
								rectangleF.Y -= rectangleF.Height;
							}
						}
						else if (item.Docking == LegendDocking.Bottom)
						{
							rectangleF.Y += num;
							if (!area.Position.Auto)
							{
								rectangleF.Y = area.Position.Bottom() + num;
							}
						}
						if (item.Docking == LegendDocking.Left)
						{
							rectangleF.X -= num;
							if (!area.Position.Auto)
							{
								rectangleF.X -= rectangleF.Width;
							}
						}
						if (item.Docking == LegendDocking.Right)
						{
							rectangleF.X += num;
							if (!area.Position.Auto)
							{
								rectangleF.X = area.Position.Right() + num;
							}
						}
						item.Position.SetPositionNoAuto(rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height);
					}
				}
			}
		}

		internal void CalcInsideLegendPosition(ChartGraphics chartGraph, float maxLegendSize, float elementSpacing)
		{
			if (this.common != null && this.common.ChartPicture != null)
			{
				foreach (Legend item in this.array)
				{
					if (item.DockToChartArea != "NotSet")
					{
						try
						{
							ChartArea chartArea2 = this.common.ChartPicture.ChartAreas[item.DockToChartArea];
						}
						catch
						{
							throw new ArgumentException(SR.ExceptionLegendDockedChartAreaIsMissing(item.DockToChartArea));
						}
					}
				}
				foreach (ChartArea chartArea in this.common.ChartPicture.ChartAreas)
				{
					if (chartArea.Visible)
					{
						RectangleF rectangleF = chartArea.PlotAreaPosition.ToRectangleF();
						float elementSpacing2 = Math.Min((float)(rectangleF.Height / 100.0 * elementSpacing), (float)(rectangleF.Width / 100.0 * elementSpacing));
						foreach (Legend item2 in this.array)
						{
							if (item2.IsEnabled() && item2.DockInsideChartArea && item2.DockToChartArea == chartArea.Name && item2.Position.Auto)
							{
								item2.CalcLegendPosition(chartGraph, ref rectangleF, maxLegendSize, elementSpacing2);
							}
						}
					}
				}
			}
		}
	}
}
