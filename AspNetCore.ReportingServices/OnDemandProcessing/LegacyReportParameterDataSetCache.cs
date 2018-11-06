using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal class LegacyReportParameterDataSetCache : ReportParameterDataSetCache
	{
		internal LegacyReportParameterDataSetCache(ProcessReportParameters aParamProcessor, ParameterInfo aParameter, ParameterDef aParamDef, bool aProcessValidValues, bool aProcessDefaultValues)
			: base(aParamProcessor, aParameter, aParamDef, aProcessValidValues, aProcessDefaultValues)
		{
		}

		internal override object GetFieldValue(object aRow, int col)
		{
			FieldImpl[] array = (FieldImpl[])aRow;
			if (array[col].IsMissing)
			{
				return null;
			}
			return array[col].Value;
		}
	}
}
