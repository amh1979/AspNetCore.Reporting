using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ProcessingMessage : IPersistable
	{
		private ProcessingErrorCode m_code;

		private Severity m_severity;

		private ObjectType m_objectType;

		private string m_objectName;

		private string m_propertyName;

		private string m_message;

		private ProcessingMessageList m_processingMessages;

		private ErrorCode m_commonCode;

		[NonSerialized]
		private static readonly AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Declaration m_Declaration = ProcessingMessage.GetNewDeclaration();

		public ProcessingErrorCode Code
		{
			get
			{
				return this.m_code;
			}
			set
			{
				this.m_code = value;
			}
		}

		public ErrorCode CommonCode
		{
			get
			{
				return this.m_commonCode;
			}
			set
			{
				this.m_commonCode = value;
			}
		}

		public Severity Severity
		{
			get
			{
				return this.m_severity;
			}
			set
			{
				this.m_severity = value;
			}
		}

		public ObjectType ObjectType
		{
			get
			{
				return this.m_objectType;
			}
			set
			{
				this.m_objectType = value;
			}
		}

		public string ObjectName
		{
			get
			{
				return this.m_objectName;
			}
			set
			{
				this.m_objectName = value;
			}
		}

		public string PropertyName
		{
			get
			{
				return this.m_propertyName;
			}
			set
			{
				this.m_propertyName = value;
			}
		}

		public string Message
		{
			get
			{
				return this.m_message;
			}
			set
			{
				this.m_message = value;
			}
		}

		public ProcessingMessageList ProcessingMessages
		{
			get
			{
				return this.m_processingMessages;
			}
			set
			{
				this.m_processingMessages = value;
			}
		}

		internal ProcessingMessage(ProcessingErrorCode code, Severity severity, ObjectType objectType, string objectName, string propertyName, string message, ProcessingMessageList innerMessages)
		{
			this.m_code = code;
			this.m_commonCode = ErrorCode.rsProcessingError;
			this.m_severity = severity;
			this.m_objectType = objectType;
			this.m_objectName = objectName;
			this.m_propertyName = propertyName;
			this.m_message = message;
			this.m_processingMessages = innerMessages;
		}

		internal ProcessingMessage(ErrorCode code, Severity severity, ObjectType objectType, string objectName, string propertyName, string message, ProcessingMessageList innerMessages)
		{
			this.m_code = ProcessingErrorCode.rsNone;
			this.m_commonCode = code;
			this.m_severity = severity;
			this.m_objectType = objectType;
			this.m_objectName = objectName;
			this.m_propertyName = propertyName;
			this.m_message = message;
			this.m_processingMessages = innerMessages;
		}

		internal ProcessingMessage()
		{
		}

		public string FormatMessage()
		{
			return string.Format(CultureInfo.CurrentCulture, "{0} ({1}.{2}) : {3} [{4}]", (this.m_severity == Severity.Warning) ? "Warning" : "Error", this.m_objectName, this.m_propertyName, this.m_message, this.m_code);
		}

		internal static AspNetCore.ReportingServices.ReportProcessing.Persistence.Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberName.Code, AspNetCore.ReportingServices.ReportProcessing.Persistence.Token.Enum));
			memberInfoList.Add(new AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberName.Severity, AspNetCore.ReportingServices.ReportProcessing.Persistence.Token.Enum));
			memberInfoList.Add(new AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberName.ObjectType, AspNetCore.ReportingServices.ReportProcessing.Persistence.Token.Enum));
			memberInfoList.Add(new AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberName.ObjectName, AspNetCore.ReportingServices.ReportProcessing.Persistence.Token.String));
			memberInfoList.Add(new AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberName.PropertyName, AspNetCore.ReportingServices.ReportProcessing.Persistence.Token.String));
			memberInfoList.Add(new AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberName.Message, AspNetCore.ReportingServices.ReportProcessing.Persistence.Token.String));
			memberInfoList.Add(new AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberName.ProcessingMessages, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ProcessingMessageList));
			memberInfoList.Add(new AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportProcessing.Persistence.MemberName.CommonCode, AspNetCore.ReportingServices.ReportProcessing.Persistence.Token.Enum));
			return new AspNetCore.ReportingServices.ReportProcessing.Persistence.Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}

		internal static AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Declaration GetNewDeclaration()
		{
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo> list = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo>();
			list.Add(new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Code, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Token.Enum));
			list.Add(new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Severity, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Token.Enum));
			list.Add(new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ObjectType, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Token.Enum));
			list.Add(new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ObjectName, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Token.String));
			list.Add(new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.PropertyName, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Token.String));
			list.Add(new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Message, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Token.String));
			list.Add(new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ProcessingMessages, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ProcessingMessage));
			list.Add(new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberInfo(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.CommonCode, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Token.Enum));
			return new AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ProcessingMessage, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		void IPersistable.Serialize(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ProcessingMessage.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Code:
					writer.WriteEnum((int)this.m_code);
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Severity:
					writer.WriteEnum((int)this.m_severity);
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ObjectType:
					writer.WriteEnum((int)this.m_objectType);
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ObjectName:
					writer.Write(this.m_objectName);
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.PropertyName:
					writer.Write(this.m_propertyName);
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Message:
					writer.Write(this.m_message);
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ProcessingMessages:
					writer.Write(this.m_processingMessages);
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.CommonCode:
					writer.WriteEnum((int)this.m_commonCode);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ProcessingMessage.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Code:
					this.m_code = (ProcessingErrorCode)reader.ReadEnum();
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Severity:
					this.m_severity = (Severity)reader.ReadEnum();
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ObjectType:
					this.m_objectType = (ObjectType)reader.ReadEnum();
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ObjectName:
					this.m_objectName = reader.ReadString();
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.PropertyName:
					this.m_propertyName = reader.ReadString();
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.Message:
					this.m_message = reader.ReadString();
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.ProcessingMessages:
					this.m_processingMessages = reader.ReadListOfRIFObjects<ProcessingMessageList>();
					break;
				case AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.MemberName.CommonCode:
					this.m_commonCode = (ErrorCode)reader.ReadEnum();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		void IPersistable.ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ProcessingMessage;
		}
	}
}
