
using AspNetCore.ReportingServices.Interfaces;
using System;

namespace AspNetCore.Reporting
{
	[Serializable]
	internal sealed class RenderingExtension
	{
		private string m_name;

		private string m_localizedName;

		private bool m_isVisible;

		public string Name
		{
			get
			{
				return this.m_name;
			}
		}

		public string LocalizedName
		{
			get
			{
				return this.m_localizedName;
			}
		}

		public bool Visible
		{
			get
			{
				return this.m_isVisible;
			}
		}

		internal RenderingExtension(string name, string localizedName, bool isVisible)
		{
			this.m_name = name;
			this.m_localizedName = localizedName;
			this.m_isVisible = isVisible;
		}

		internal static RenderingExtension[] FromSoapExtensions(Extension[] soapExtensions)
		{
			if (soapExtensions == null)
			{
				return null;
			}
			RenderingExtension[] array = new RenderingExtension[soapExtensions.Length];
			for (int i = 0; i < soapExtensions.Length; i++)
			{
				array[i] = new RenderingExtension(soapExtensions[i].Name, soapExtensions[i].LocalizedName, soapExtensions[i].Visible);
			}
			return array;
		}
	}
}
