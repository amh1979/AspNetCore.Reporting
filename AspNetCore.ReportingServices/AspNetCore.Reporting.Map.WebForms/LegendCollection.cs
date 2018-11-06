using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Map.WebForms
{
	[Description("Legend collection.")]
	internal class LegendCollection : NamedCollection
	{
		private Legend this[int index]
		{
			get
			{
				return (Legend)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private Legend this[string name]
		{
			get
			{
				return (Legend)base.GetByNameCheck(name);
			}
			set
			{
				base.SetByNameCheck(name, value);
			}
		}

		public Legend this[object obj]
		{
			get
			{
				if (obj is string)
				{
					return this[(string)obj];
				}
				if (obj is int)
				{
					return this[(int)obj];
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgument);
			}
			set
			{
				if (obj is string)
				{
					this[(string)obj] = value;
					return;
				}
				if (obj is int)
				{
					this[(int)obj] = value;
					return;
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgument);
			}
		}

		internal LegendCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			base.elementType = typeof(Legend);
		}

		protected override void OnClearComplete()
		{
			base.OnClearComplete();
			this.Invalidate();
		}

		internal override void Invalidate()
		{
			if (base.Common != null)
			{
				base.Common.MapCore.InvalidateAndLayout();
			}
		}

		public Legend Add(string name)
		{
			Legend legend = new Legend();
			legend.Name = name;
			this.Add(legend);
			return legend;
		}

		public int Add(Legend value)
		{
			return base.List.Add(value);
		}

		public void Remove(Legend value)
		{
			base.List.Remove(value);
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			return "Legend1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "Legend{0}";
		}
	}
}
