namespace AspNetCore.ReportingServices.RdlObjectModel.Serialization
{
	internal class NameKey
	{
		internal string ns;

		internal string name;

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Namespace
		{
			get
			{
				return this.ns;
			}
		}

		internal NameKey(string name, string ns)
		{
			this.name = name;
			this.ns = ns;
		}

		public override bool Equals(object other)
		{
			if (!(other is NameKey))
			{
				return false;
			}
			NameKey nameKey = (NameKey)other;
			if (this.name == nameKey.name)
			{
				return this.ns == nameKey.ns;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((this.ns != null) ? this.ns.GetHashCode() : 0) ^ ((this.name != null) ? this.name.GetHashCode() : 0);
		}

		public static bool operator ==(NameKey a, NameKey b)
		{
			if (a.name == b.name)
			{
				return a.ns == b.ns;
			}
			return false;
		}

		public static bool operator !=(NameKey a, NameKey b)
		{
			return !(a == b);
		}
	}
}
