using AspNetCore.ReportingServices.RdlObjectModel;
using AspNetCore.ReportingServices.RdlObjectModel.Serialization;
using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.RdlObjectModel2005
{
	[XmlElementClass("Field")]
	internal class FieldEx : Field
	{
		private string m_typeName;

		[XmlElement(Namespace = "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner")]
		public string TypeName
		{
			get
			{
				return this.m_typeName;
			}
			set
			{
				this.m_typeName = value;
			}
		}
	}
}
