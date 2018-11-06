using System;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	internal class CircularGaugeCollection : NamedCollection
	{
		private CircularGauge this[int index]
		{
			get
			{
				if (index == 0 && base.List.Count == 0)
				{
					this.Add(new CircularGauge());
				}
				return (CircularGauge)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private CircularGauge this[string name]
		{
			get
			{
				return (CircularGauge)base.GetByNameCheck(name);
			}
			set
			{
				base.SetByNameCheck(name, value);
			}
		}

		public CircularGauge this[object obj]
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
				throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidIndexer_error"), "obj");
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
				throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidIndexer_error"), "obj");
			}
		}

		internal CircularGaugeCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			base.elementType = typeof(CircularGauge);
		}

		public CircularGauge Add(string name)
		{
			CircularGauge circularGauge = new CircularGauge();
			circularGauge.Name = name;
			circularGauge.Scales.Add(new CircularScale());
			circularGauge.Pointers.Add(new CircularPointer());
			circularGauge.Ranges.Add(new CircularRange());
			this.Add(circularGauge);
			return circularGauge;
		}

		public int Add(CircularGauge value)
		{
			return base.List.Add(value);
		}

		public void Remove(CircularGauge value)
		{
			base.List.Remove(value);
		}

		public bool Contains(CircularGauge value)
		{
			return base.List.Contains(value);
		}

		public void Insert(int index, CircularGauge value)
		{
			base.List.Insert(index, value);
		}

		public int IndexOf(CircularGauge value)
		{
			return base.List.IndexOf(value);
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "Gauge{0}";
		}
	}
}
