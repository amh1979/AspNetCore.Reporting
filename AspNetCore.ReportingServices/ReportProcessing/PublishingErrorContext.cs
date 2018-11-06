namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class PublishingErrorContext : ErrorContext
	{
		private const int MaxNumberOfMessages = 100;

		internal override ProcessingMessage Register(ProcessingErrorCode code, Severity severity, ObjectType objectType, string objectName, string propertyName, params string[] arguments)
		{
			return this.Register(code, severity, objectType, objectName, propertyName, null, arguments);
		}

		internal override ProcessingMessage Register(ProcessingErrorCode code, Severity severity, ObjectType objectType, string objectName, string propertyName, ProcessingMessageList innerMessages, params string[] arguments)
		{
			if (base.m_suspendErrors)
			{
				return null;
			}
			if (base.m_messages == null)
			{
				base.m_messages = new ProcessingMessageList();
			}
			ProcessingMessage processingMessage = null;
			if (base.m_messages.Count < 100 || (severity == Severity.Error && !base.m_hasError))
			{
				processingMessage = ErrorContext.CreateProcessingMessage(code, severity, objectType, objectName, propertyName, innerMessages, arguments);
				base.m_messages.Add(processingMessage);
			}
			if (severity == Severity.Error)
			{
				base.m_hasError = true;
			}
			return processingMessage;
		}
	}
}
