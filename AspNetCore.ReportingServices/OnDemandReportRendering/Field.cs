using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class Field
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.Field m_fieldDef;

		public string Name
		{
			get
			{
				return this.m_fieldDef.Name;
			}
		}

		public string DataField
		{
			get
			{
				return this.m_fieldDef.DataField;
			}
		}

		internal Field(AspNetCore.ReportingServices.ReportIntermediateFormat.Field fieldDef)
		{
			this.m_fieldDef = fieldDef;
		}
	}
}
