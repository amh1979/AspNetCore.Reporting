using System;
using System.Runtime.Serialization;

namespace AspNetCore.ReportingServices.Diagnostics.Utilities
{
	[Serializable]
	internal sealed class MissingElementException : ReportCatalogException
	{
		private string m_elementName;

		public string MissingElementName
		{
			get
			{
				return this.m_elementName;
			}
		}

		public MissingElementException(string elementName)
			: base(ErrorCode.rsMissingElement, ErrorStrings.rsMissingElement(elementName), null, null)
		{
			this.m_elementName = elementName;
		}

		private MissingElementException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
