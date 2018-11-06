namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser
{
	internal struct OoxmlBool
	{
		public static readonly OoxmlBool OoxmlTrue = new OoxmlBool(true);

		public static readonly OoxmlBool OoxmlFalse = new OoxmlBool(false);

		private bool _value;

		private OoxmlBool(bool b)
		{
			this._value = b;
		}

		public static implicit operator OoxmlBool(bool b)
		{
			if (!b)
			{
				return OoxmlBool.OoxmlFalse;
			}
			return OoxmlBool.OoxmlTrue;
		}

		public static implicit operator bool(OoxmlBool b)
		{
			return b._value;
		}

		public static OoxmlBool operator ==(OoxmlBool b1, OoxmlBool b2)
		{
			if (b1._value != b2._value)
			{
				return OoxmlBool.OoxmlFalse;
			}
			return OoxmlBool.OoxmlTrue;
		}

		public static OoxmlBool operator !=(OoxmlBool b1, OoxmlBool b2)
		{
			if (b1._value == b2._value)
			{
				return OoxmlBool.OoxmlFalse;
			}
			return OoxmlBool.OoxmlTrue;
		}

		public static OoxmlBool operator !(OoxmlBool b)
		{
			if (!(bool)b)
			{
				return OoxmlBool.OoxmlTrue;
			}
			return OoxmlBool.OoxmlFalse;
		}

		public override string ToString()
		{
			if (!this._value)
			{
				return "0";
			}
			return "1";
		}

		public bool Equals(OoxmlBool other)
		{
			return other._value.Equals(this._value);
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(null, obj))
			{
				return false;
			}
			if (obj.GetType() != typeof(OoxmlBool))
			{
				return false;
			}
			return this.Equals((OoxmlBool)obj);
		}

		public override int GetHashCode()
		{
			return this._value.GetHashCode();
		}
	}
}
