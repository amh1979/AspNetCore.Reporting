using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class DataValue : IPersistable
	{
		private ExpressionInfo m_name;

		private ExpressionInfo m_value;

		private int m_exprHostID = -1;

		[NonSerialized]
		private DataValueExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = DataValue.GetDeclaration();

		internal ExpressionInfo Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}

		internal ExpressionInfo Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = value;
			}
		}

		internal int ExprHostID
		{
			get
			{
				return this.m_exprHostID;
			}
			set
			{
				this.m_exprHostID = value;
			}
		}

		internal DataValueExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		public object PublishClone(AutomaticSubtotalContext context)
		{
			DataValue dataValue = (DataValue)base.MemberwiseClone();
			if (this.m_name == null)
			{
				dataValue.m_name = (ExpressionInfo)this.m_name.PublishClone(context);
			}
			if (this.m_value == null)
			{
				dataValue.m_value = (ExpressionInfo)this.m_value.PublishClone(context);
			}
			return dataValue;
		}

		internal void Initialize(string propertyName, bool isCustomProperty, DynamicImageOrCustomUniqueNameValidator validator, InitializationContext context)
		{
			context.ExprHostBuilder.DataValueStart();
			if (this.m_name != null)
			{
				this.m_name.Initialize(propertyName + ".Name", context);
				if (isCustomProperty && ExpressionInfo.Types.Constant == this.m_name.Type)
				{
					validator.Validate(Severity.Error, propertyName + ".Name", context.ObjectType, context.ObjectName, this.m_name.StringValue, context.ErrorContext);
				}
				context.ExprHostBuilder.DataValueName(this.m_name);
			}
			if (this.m_value != null)
			{
				this.m_value.Initialize(propertyName + ".Value", context);
				context.ExprHostBuilder.DataValueValue(this.m_value);
			}
			this.m_exprHostID = context.ExprHostBuilder.DataValueEnd(isCustomProperty);
		}

		internal void SetExprHost(IList<DataValueExprHost> dataValueHosts, ObjectModelImpl reportObjectModel)
		{
			if (this.m_exprHostID >= 0)
			{
				Global.Tracer.Assert(dataValueHosts != null && dataValueHosts.Count > this.m_exprHostID && reportObjectModel != null);
				this.m_exprHost = dataValueHosts[this.m_exprHostID];
				this.m_exprHost.SetReportObjectModel(reportObjectModel);
			}
		}

		internal void EvaluateNameAndValue(ReportElement reportElementOwner, IReportScopeInstance romInstance, IInstancePath instancePath, OnDemandProcessingContext context, AspNetCore.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, out string name, out object value, out TypeCode valueTypeCode)
		{
			context.SetupContext(instancePath, romInstance);
			name = null;
			value = null;
			valueTypeCode = TypeCode.Empty;
			if (this.m_name != null)
			{
				if (!this.m_name.IsExpression)
				{
					name = this.m_name.StringValue;
				}
				else if (reportElementOwner == null || (reportElementOwner != null && reportElementOwner.CriOwner == null))
				{
					name = context.ReportRuntime.EvaluateDataValueNameExpression(this, objectType, objectName, "Name");
				}
			}
			if (this.m_value != null)
			{
				if (!this.m_value.IsExpression)
				{
					value = this.m_value.Value;
				}
				else
				{
					if (reportElementOwner != null)
					{
						if (reportElementOwner == null)
						{
							return;
						}
						if (reportElementOwner.CriOwner != null)
						{
							return;
						}
					}
					value = context.ReportRuntime.EvaluateDataValueValueExpression(this, objectType, objectName, "Value", out valueTypeCode);
				}
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Value, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataValue, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(DataValue.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(this.m_name);
					break;
				case MemberName.Value:
					writer.Write(this.m_value);
					break;
				case MemberName.ExprHostID:
					writer.Write(this.m_exprHostID);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(DataValue.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					this.m_name = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Value:
					this.m_value = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					this.m_exprHostID = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataValue;
		}
	}
}
