using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DataValue
	{
		private ExpressionInfo m_name;

		private ExpressionInfo m_value;

		private int m_exprHostID = -1;

		[NonSerialized]
		private DataValueExprHost m_exprHost;

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

		internal DataValue DeepClone(InitializationContext context)
		{
			DataValue dataValue = new DataValue();
			Global.Tracer.Assert(-1 == this.m_exprHostID);
			dataValue.m_name = ((this.m_name == null) ? null : this.m_name.DeepClone(context));
			dataValue.m_value = ((this.m_value == null) ? null : this.m_value.DeepClone(context));
			return dataValue;
		}

		internal void Initialize(string propertyName, bool isCustomProperty, CustomPropertyUniqueNameValidator validator, InitializationContext context)
		{
			context.ExprHostBuilder.DataValueStart();
			if (this.m_name != null)
			{
				this.m_name.Initialize(propertyName + ".Name", context);
				if (isCustomProperty && ExpressionInfo.Types.Constant == this.m_name.Type)
				{
					validator.Validate(Severity.Error, context.ObjectType, context.ObjectName, this.m_name.Value, context.ErrorContext);
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

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Name, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.Value, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
