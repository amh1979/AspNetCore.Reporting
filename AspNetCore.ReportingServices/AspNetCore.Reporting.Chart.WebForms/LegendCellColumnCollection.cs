using System;
using System.Collections;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeLegendCellColumnCollection_LegendCellColumnCollection")]
	internal class LegendCellColumnCollection : CollectionBase
	{
		private Legend legend;

		[SRDescription("DescriptionAttributeLegendCellColumnCollection_Item")]
		public LegendCellColumn this[object parameter]
		{
			get
			{
				if (parameter is int)
				{
					return (LegendCellColumn)base.List[(int)parameter];
				}
				if (parameter is string)
				{
					foreach (LegendCellColumn item in base.List)
					{
						if (item.Name == (string)parameter)
						{
							return item;
						}
					}
					throw new ArgumentException(SR.ExceptionLegendCellColumnNotFound((string)parameter));
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
						throw new ArgumentException(SR.ExceptionLegendCellColumnAlreadyExistsInCollection(value.Name));
					}
					base.List[(int)parameter] = value;
					goto IL_00ee;
				}
				if (parameter is string)
				{
					int num2 = 0;
					foreach (LegendCellColumn item in base.List)
					{
						if (item.Name == (string)parameter)
						{
							if (num != -1 && num != num2)
							{
								throw new ArgumentException(SR.ExceptionLegendCellColumnAlreadyExistsInCollection(value.Name));
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

		public LegendCellColumnCollection()
		{
		}

		internal LegendCellColumnCollection(Legend legend)
		{
			this.legend = legend;
		}

		public void Remove(string name)
		{
			this.Remove(this.FindByName(name));
		}

		public void Remove(LegendCellColumn column)
		{
			if (column != null)
			{
				base.List.Remove(column);
			}
		}

		public int Add(LegendCellColumn column)
		{
			return base.List.Add(column);
		}

		public int Add(string headerText, LegendCellColumnType columnType, string text, ContentAlignment alignment)
		{
			return base.List.Add(new LegendCellColumn(headerText, columnType, text, alignment));
		}

		public void Insert(int index, LegendCellColumn column)
		{
			base.List.Insert(index, column);
		}

		public void Insert(int index, string headerText, LegendCellColumnType columnType, string text, ContentAlignment alignment)
		{
			base.List.Insert(index, new LegendCellColumn(headerText, columnType, text, alignment));
		}

		public bool Contains(LegendCellColumn value)
		{
			return base.List.Contains(value);
		}

		public int IndexOf(LegendCellColumn value)
		{
			return base.List.IndexOf(value);
		}

		protected override void OnInsert(int index, object value)
		{
			if (((LegendCellColumn)value).Name.Length == 0)
			{
				this.AssignUniqueName((LegendCellColumn)value);
			}
		}

		protected override void OnInsertComplete(int index, object value)
		{
			((LegendCellColumn)value).SetContainingLegend(this.legend);
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
			((LegendCellColumn)newValue).SetContainingLegend(this.legend);
			this.Invalidate();
		}

		private void Invalidate()
		{
			if (this.legend != null)
			{
				this.legend.Invalidate(false);
			}
		}

		private void AssignUniqueName(LegendCellColumn column)
		{
			if (column.Name.Length == 0)
			{
				string empty = string.Empty;
				int num = 1;
				do
				{
					empty = "Column" + num.ToString(CultureInfo.InvariantCulture);
					num++;
				}
				while (this.FindByName(empty) != null && num < 10000);
				column.Name = empty;
			}
		}

		public LegendCellColumn FindByName(string name)
		{
			LegendCellColumn result = null;
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
