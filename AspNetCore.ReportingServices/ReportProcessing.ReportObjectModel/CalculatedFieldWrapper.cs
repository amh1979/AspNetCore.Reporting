using System;

namespace AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel
{
	public abstract class CalculatedFieldWrapper : MarshalByRefObject
	{
		public abstract object Value
		{
			get;
		}
	}
}
