using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System.Collections;
using System.Globalization;
using System.Threading;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class ProcessingErrorContext : ErrorContext
	{
		private Hashtable m_itemsRegistered;

		internal override ProcessingMessage Register(ProcessingErrorCode code, Severity severity, ObjectType objectType, string objectName, string propertyName, params string[] arguments)
		{
			return this.Register(code, severity, objectType, objectName, propertyName, null, arguments);
		}

		internal override ProcessingMessage Register(ProcessingErrorCode code, Severity severity, ObjectType objectType, string objectName, string propertyName, ProcessingMessageList innerMessages, params string[] arguments)
		{
			try
			{
				Monitor.Enter(this);
				if (severity == Severity.Error)
				{
					base.m_hasError = true;
				}
				if (this.RegisterItem(severity, code, objectType, objectName))
				{
					if (base.m_messages == null)
					{
						base.m_messages = new ProcessingMessageList();
					}
					ProcessingMessage processingMessage = ErrorContext.CreateProcessingMessage(code, severity, objectType, objectName, propertyName, innerMessages, arguments);
					base.m_messages.Add(processingMessage);
					return processingMessage;
				}
				return null;
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		internal override void Register(RSException rsException, ObjectType objectType)
		{
			try
			{
				Monitor.Enter(this);
				base.Register(rsException, objectType);
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		private bool RegisterItem(Severity severity, ProcessingErrorCode code, ObjectType objectType, string objectName)
		{
			if (this.m_itemsRegistered == null)
			{
				this.m_itemsRegistered = new Hashtable();
			}
			if (ObjectType.DataSet == objectType && (ProcessingErrorCode.rsErrorReadingDataSetField == code || ProcessingErrorCode.rsDataSetFieldTypeNotSupported == code || ProcessingErrorCode.rsMissingFieldInDataSet == code || ProcessingErrorCode.rsErrorReadingFieldProperty == code))
			{
				return true;
			}
			bool result = false;
			int num = (int)code;
			string text = num.ToString(CultureInfo.InvariantCulture);
			if (objectType == ObjectType.Report || ObjectType.PageHeader == objectType || ObjectType.PageFooter == objectType)
			{
				string key = text + objectType;
				if (!this.m_itemsRegistered.ContainsKey(key))
				{
					result = true;
					this.m_itemsRegistered.Add(key, null);
				}
			}
			else
			{
				Hashtable hashtable = (Hashtable)this.m_itemsRegistered[objectType];
				if (hashtable == null)
				{
					hashtable = new Hashtable();
					this.m_itemsRegistered[objectType] = hashtable;
				}
				Global.Tracer.Assert(null != objectName, "(null != objectName)");
				string key2 = severity.ToString() + text + objectName;
				if (!hashtable.ContainsKey(key2))
				{
					result = true;
					hashtable.Add(key2, null);
				}
			}
			return result;
		}

		internal void Combine(ProcessingMessageList messages)
		{
			if (messages != null)
			{
				for (int i = 0; i < messages.Count; i++)
				{
					ProcessingMessage processingMessage = messages[i];
					if (processingMessage.Severity == Severity.Error)
					{
						base.m_hasError = true;
					}
					if (this.RegisterItem(processingMessage.Severity, processingMessage.Code, processingMessage.ObjectType, processingMessage.ObjectName))
					{
						if (base.m_messages == null)
						{
							base.m_messages = new ProcessingMessageList();
						}
						base.m_messages.Add(processingMessage);
					}
				}
			}
		}
	}
}
