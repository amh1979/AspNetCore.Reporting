using System;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class MapLabelCollection : NamedCollection
	{
		private MapLabel this[int index]
		{
			get
			{
				return (MapLabel)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private MapLabel this[string name]
		{
			get
			{
				return (MapLabel)base.GetByNameCheck(name);
			}
			set
			{
				base.SetByNameCheck(name, value);
			}
		}

		public MapLabel this[object obj]
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

		internal MapLabelCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			base.elementType = typeof(MapLabel);
		}

		public MapLabel Add(string name)
		{
			MapLabel mapLabel = new MapLabel();
			mapLabel.Name = name;
			this.Add(mapLabel);
			return mapLabel;
		}

		public int Add(MapLabel value)
		{
			return base.List.Add(value);
		}

		public void Remove(MapLabel value)
		{
			base.List.Remove(value);
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			return "Label1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "Label{0}";
		}

		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete(index, value);
			MapLabel mapLabel = (MapLabel)value;
			if (mapLabel.Position.DefaultValues && index != 0)
			{
				MapLabel mapLabel2 = this[index - 1];
				mapLabel.Location.X = (float)(mapLabel2.Location.X + 3.0);
				mapLabel.Location.Y = (float)(mapLabel2.Location.Y + 3.0);
			}
		}

		internal override void Invalidate()
		{
			if (base.Common != null)
			{
				base.Common.MapCore.InvalidateAndLayout();
			}
		}
	}
}
