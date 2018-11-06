using System;

namespace AspNetCore.ReportingServices.Interfaces
{
	[AttributeUsage(AttributeTargets.All)]
	internal class LocalizedNameAttribute : Attribute
	{
		private string m_name;

		private bool m_localized;

		public string Name
		{
			get
			{
				if (!this.m_localized)
				{
					this.m_localized = true;
					string localizedString = this.GetLocalizedString(this.m_name);
					if (localizedString != null)
					{
						this.m_name = localizedString;
					}
				}
				return this.m_name;
			}
		}

		public LocalizedNameAttribute()
		{
		}

		public LocalizedNameAttribute(string name)
		{
			this.m_name = name;
			this.m_localized = false;
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			if (obj is LocalizedNameAttribute)
			{
				return this.Name.Equals(((LocalizedNameAttribute)obj).Name);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		protected virtual string GetLocalizedString(string value)
		{
			return value;
		}
	}
}
