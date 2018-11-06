using System;
using System.Collections;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeLegendCellCollection_LegendCellCollection")]
	internal class LegendCellCollection : CollectionBase
	{
		private LegendItem legendItem;

		[SRDescription("DescriptionAttributeLegendCellCollection_Item")]
		public LegendCell this[object parameter]
		{
			get
			{
				if (parameter is int)
				{
					return (LegendCell)base.List[(int)parameter];
				}
				if (parameter is string)
				{
					foreach (LegendCell item in base.List)
					{
						if (item.Name == (string)parameter)
						{
							return item;
						}
					}
					throw new ArgumentException(SR.ExceptionLegendCellNotFound((string)parameter));
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgumentType);
			}
			set
			{
				int num = -1;
				if (value.Name.Length != 0)
				{
					num = base.List.IndexOf(value);
				}
				else
				{
					this.AssignUniqueName(value);
				}
				if (parameter is int)
				{
					if (num != -1 && num != (int)parameter)
					{
						throw new ArgumentException(SR.ExceptionLegendCellNameAlreadyExistsInCollection(value.Name));
					}
					base.List[(int)parameter] = value;
					goto IL_00ee;
				}
				if (parameter is string)
				{
					int num2 = 0;
					foreach (LegendCell item in base.List)
					{
						if (item.Name == (string)parameter)
						{
							if (num != -1 && num != num2)
							{
								throw new ArgumentException(SR.ExceptionLegendCellNameAlreadyExistsInCollection(value.Name));
							}
							base.List[num2] = value;
							break;
						}
						num2++;
					}
					goto IL_00ee;
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgumentType);
				IL_00ee:
				this.Invalidate();
			}
		}

		public LegendCellCollection()
		{
		}

		internal LegendCellCollection(LegendItem legendItem)
		{
			this.legendItem = legendItem;
		}

		public void Remove(string name)
		{
			this.Remove(this.FindByName(name));
		}

		public void Remove(LegendCell cell)
		{
			if (cell != null)
			{
				base.List.Remove(cell);
			}
		}

		public int Add(LegendCell cell)
		{
			return base.List.Add(cell);
		}

		public int Add(LegendCellType cellType, string text, ContentAlignment alignment)
		{
			return base.List.Add(new LegendCell(cellType, text, alignment));
		}

		public void Insert(int index, LegendCell cell)
		{
			base.List.Insert(index, cell);
		}

		public void Insert(int index, LegendCellType cellType, string text, ContentAlignment alignment)
		{
			base.List.Insert(index, new LegendCell(cellType, text, alignment));
		}

		public bool Contains(LegendCell value)
		{
			return base.List.Contains(value);
		}

		protected override void OnInsert(int index, object value)
		{
			if (((LegendCell)value).Name.Length == 0)
			{
				this.AssignUniqueName((LegendCell)value);
			}
		}

		protected override void OnInsertComplete(int index, object value)
		{
			if (this.legendItem != null)
			{
				((LegendCell)value).SetContainingLegend(this.legendItem.Legend, this.legendItem);
			}
			this.Invalidate();
		}

		protected override void OnRemoveComplete(int index, object value)
		{
			this.Invalidate();
		}

		protected override void OnClearComplete()
		{
			this.Invalidate();
		}

		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			if (this.legendItem != null)
			{
				((LegendCell)newValue).SetContainingLegend(this.legendItem.Legend, this.legendItem);
			}
			this.Invalidate();
		}

		public int IndexOf(LegendCell cell)
		{
			return base.List.IndexOf(cell);
		}

		private void Invalidate()
		{
			if (this.legendItem != null && this.legendItem.Legend != null)
			{
				this.legendItem.Legend.Invalidate(false);
			}
		}

		private void AssignUniqueName(LegendCell cell)
		{
			string empty = string.Empty;
			int num = 1;
			do
			{
				empty = "Cell" + num.ToString(CultureInfo.InvariantCulture);
				num++;
			}
			while (this.FindByName(empty) != null && num < 10000);
			cell.Name = empty;
		}

		public LegendCell FindByName(string name)
		{
			LegendCell result = null;
			int num = 0;
			while (num < base.List.Count)
			{
				if (string.Compare(this[num].Name, name, false, CultureInfo.CurrentCulture) != 0)
				{
					num++;
					continue;
				}
				result = this[num];
				break;
			}
			return result;
		}
	}
}
