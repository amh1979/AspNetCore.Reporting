namespace AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel
{
	public sealed class CalculatedFieldWrapperImpl : CalculatedFieldWrapper
	{
		private AspNetCore.ReportingServices.ReportProcessing.Field m_fieldDef;

		private object m_value;

		private bool m_isValueReady;

		private bool m_isVisited;

		private ReportRuntime m_reportRT;

		private IErrorContext m_iErrorContext;

		public override object Value
		{
			get
			{
				if (!this.m_isValueReady)
				{
					if (this.m_isVisited)
					{
						this.m_iErrorContext.Register(ProcessingErrorCode.rsCyclicExpression, Severity.Warning, ObjectType.Field, this.m_fieldDef.Name, "Value");
						throw new ReportProcessingException_InvalidOperationException();
					}
					this.m_isVisited = true;
					this.m_value = this.m_reportRT.EvaluateFieldValueExpression(this.m_fieldDef);
					this.m_isVisited = false;
					this.m_isValueReady = true;
				}
				return this.m_value;
			}
		}

		internal CalculatedFieldWrapperImpl(AspNetCore.ReportingServices.ReportProcessing.Field fieldDef, ReportRuntime reportRT)
		{
			this.m_fieldDef = fieldDef;
			this.m_reportRT = reportRT;
			this.m_iErrorContext = reportRT;
		}
	}
}
