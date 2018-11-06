using AspNetCore.ReportingServices.ReportProcessing.ExprHostObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartDataPoint : IActionOwner
	{
		internal enum MarkerTypes
		{
			None,
			Square,
			Circle,
			Diamond,
			Triangle,
			Cross,
			Auto
		}

		private ExpressionInfoList m_dataValues;

		private ChartDataLabel m_dataLabel;

		private Action m_action;

		private Style m_styleClass;

		private MarkerTypes m_markerType;

		private string m_markerSize;

		private Style m_markerStyleClass;

		private string m_dataElementName;

		private DataElementOutputTypes m_dataElementOutput;

		private int m_exprHostID = -1;

		private DataValueList m_customProperties;

		[NonSerialized]
		private ChartDataPointExprHost m_exprHost;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		internal ExpressionInfoList DataValues
		{
			get
			{
				return this.m_dataValues;
			}
			set
			{
				this.m_dataValues = value;
			}
		}

		internal ChartDataLabel DataLabel
		{
			get
			{
				return this.m_dataLabel;
			}
			set
			{
				this.m_dataLabel = value;
			}
		}

		internal Action Action
		{
			get
			{
				return this.m_action;
			}
			set
			{
				this.m_action = value;
			}
		}

		internal Style StyleClass
		{
			get
			{
				return this.m_styleClass;
			}
			set
			{
				this.m_styleClass = value;
			}
		}

		internal MarkerTypes MarkerType
		{
			get
			{
				return this.m_markerType;
			}
			set
			{
				this.m_markerType = value;
			}
		}

		internal string MarkerSize
		{
			get
			{
				return this.m_markerSize;
			}
			set
			{
				this.m_markerSize = value;
			}
		}

		internal Style MarkerStyleClass
		{
			get
			{
				return this.m_markerStyleClass;
			}
			set
			{
				this.m_markerStyleClass = value;
			}
		}

		internal string DataElementName
		{
			get
			{
				return this.m_dataElementName;
			}
			set
			{
				this.m_dataElementName = value;
			}
		}

		internal DataElementOutputTypes DataElementOutput
		{
			get
			{
				return this.m_dataElementOutput;
			}
			set
			{
				this.m_dataElementOutput = value;
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

		internal DataValueList CustomProperties
		{
			get
			{
				return this.m_customProperties;
			}
			set
			{
				this.m_customProperties = value;
			}
		}

		internal ChartDataPointExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		Action IActionOwner.Action
		{
			get
			{
				return this.m_action;
			}
		}

		List<string> IActionOwner.FieldsUsedInValueExpression
		{
			get
			{
				return this.m_fieldsUsedInValueExpression;
			}
			set
			{
				this.m_fieldsUsedInValueExpression = value;
			}
		}

		internal ChartDataPoint()
		{
			this.m_dataValues = new ExpressionInfoList();
		}

		internal void Initialize(InitializationContext context)
		{
			ExprHostBuilder exprHostBuilder = context.ExprHostBuilder;
			exprHostBuilder.ChartDataPointStart();
			for (int i = 0; i < this.m_dataValues.Count; i++)
			{
				this.m_dataValues[i].Initialize("DataPoint", context);
				exprHostBuilder.ChartDataPointDataValue(this.m_dataValues[i]);
			}
			if (this.m_dataLabel != null)
			{
				this.m_dataLabel.Initialize(context);
			}
			if (this.m_action != null)
			{
				this.m_action.Initialize(context);
			}
			if (this.m_styleClass != null)
			{
				exprHostBuilder.DataPointStyleStart();
				this.m_styleClass.Initialize(context);
				exprHostBuilder.DataPointStyleEnd();
			}
			if (this.m_markerStyleClass != null)
			{
				exprHostBuilder.DataPointMarkerStyleStart();
				this.m_markerStyleClass.Initialize(context);
				exprHostBuilder.DataPointMarkerStyleEnd();
			}
			if (this.m_markerSize != null)
			{
				double size = context.ValidateSize(this.m_markerSize, "MarkerSize");
				this.m_markerSize = Converter.ConvertSize(size);
			}
			if (this.m_customProperties != null)
			{
				this.m_customProperties.Initialize(null, true, context);
			}
			this.DataRendererInitialize(context);
			this.m_exprHostID = exprHostBuilder.ChartDataPointEnd();
		}

		internal void DataRendererInitialize(InitializationContext context)
		{
			CLSNameValidator.ValidateDataElementName(ref this.m_dataElementName, "Value", context.ObjectType, context.ObjectName, "DataElementName", context.ErrorContext);
		}

		internal void SetExprHost(ChartDataPointExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
			if (this.m_action != null)
			{
				if (this.m_exprHost.ActionInfoHost != null)
				{
					this.m_action.SetExprHost(this.m_exprHost.ActionInfoHost, reportObjectModel);
				}
				else if (this.m_exprHost.ActionHost != null)
				{
					this.m_action.SetExprHost(this.m_exprHost.ActionHost, reportObjectModel);
				}
			}
			if (this.m_styleClass != null && this.m_exprHost.StyleHost != null)
			{
				this.m_exprHost.StyleHost.SetReportObjectModel(reportObjectModel);
				this.m_styleClass.SetStyleExprHost(this.m_exprHost.StyleHost);
			}
			if (this.m_markerStyleClass != null && this.m_exprHost.MarkerStyleHost != null)
			{
				this.m_exprHost.MarkerStyleHost.SetReportObjectModel(reportObjectModel);
				this.m_markerStyleClass.SetStyleExprHost(this.m_exprHost.MarkerStyleHost);
			}
			if (this.m_dataLabel != null && this.m_dataLabel.StyleClass != null && this.m_exprHost.DataLabelStyleHost != null)
			{
				this.m_dataLabel.SetExprHost(this.m_exprHost.DataLabelStyleHost, reportObjectModel);
			}
			if (this.m_customProperties != null && this.m_exprHost.CustomPropertyHostsRemotable != null)
			{
				this.m_customProperties.SetExprHost(this.m_exprHost.CustomPropertyHostsRemotable, reportObjectModel);
			}
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.DataValues, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfoList));
			memberInfoList.Add(new MemberInfo(MemberName.DataLabel, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ChartDataLabel));
			memberInfoList.Add(new MemberInfo(MemberName.Action, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Action));
			memberInfoList.Add(new MemberInfo(MemberName.StyleClass, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Style));
			memberInfoList.Add(new MemberInfo(MemberName.MarkerType, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.MarkerSize, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.MarkerStyleClass, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Style));
			memberInfoList.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.CustomProperties, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.DataValueList));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
