using System;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class CircularRangeCollection : NamedCollection
	{
		private CircularRange this[int index]
		{
			get
			{
				if (index == 0 && base.List.Count == 0)
				{
					this.Add(new CircularRange());
				}
				return (CircularRange)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private CircularRange this[string name]
		{
			get
			{
				return (CircularRange)base.GetByNameCheck(name);
			}
			set
			{
				base.SetByNameCheck(name, value);
			}
		}

		public CircularRange this[object obj]
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
				throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidIndexer_error"));
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
				throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidIndexer_error"));
			}
		}

		internal CircularRangeCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			base.elementType = typeof(CircularRange);
		}

		public CircularRange Add(string name)
		{
			CircularRange circularRange = new CircularRange();
			circularRange.Name = name;
			this.Add(circularRange);
			return circularRange;
		}

		public int Add(CircularRange value)
		{
			return base.List.Add(value);
		}

		public void Remove(CircularRange value)
		{
			base.List.Remove(value);
		}

		public bool Contains(CircularRange value)
		{
			return base.List.Contains(value);
		}

		public void Insert(int index, CircularRange value)
		{
			base.List.Insert(index, value);
		}

		public int IndexOf(CircularRange value)
		{
			return base.List.IndexOf(value);
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "Range{0}";
		}
	}
}
