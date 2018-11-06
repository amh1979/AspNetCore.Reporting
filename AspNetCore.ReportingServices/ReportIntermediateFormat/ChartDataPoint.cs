using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions;
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
	internal sealed class ChartDataPoint : Cell, IPersistable, IActionOwner, IStyleContainer, ICustomPropertiesHolder
	{
		private ChartDataPointValues m_dataPointValues;

		private ChartDataLabel m_dataLabel;

		private Action m_action;

		private Style m_styleClass;

		private string m_dataElementName;

		private DataElementOutputTypes m_dataElementOutput = DataElementOutputTypes.ContentsOnly;

		private DataValueList m_customProperties;

		private ChartMarker m_marker;

		private ExpressionInfo m_axisLabel;

		private ChartItemInLegend m_itemInLegend;

		private ExpressionInfo m_toolTip;

		[NonSerialized]
		private ChartDataPointExprHost m_exprHost;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartDataPoint.GetDeclaration();

		[NonSerialized]
		private Formatter m_formatter;

		protected override bool IsDataRegionBodyCell
		{
			get
			{
				return true;
			}
		}

		internal ChartDataPointValues DataPointValues
		{
			get
			{
				return this.m_dataPointValues;
			}
			set
			{
				this.m_dataPointValues = value;
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

		public Style StyleClass
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

		IInstancePath IStyleContainer.InstancePath
		{
			get
			{
				return this;
			}
		}

		public AspNetCore.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart;
			}
		}

		public string Name
		{
			get
			{
				return base.m_dataRegionDef.Name;
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

		DataValueList ICustomPropertiesHolder.CustomProperties
		{
			get
			{
				return this.CustomProperties;
			}
		}

		IInstancePath ICustomPropertiesHolder.InstancePath
		{
			get
			{
				return this;
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

		internal ChartMarker Marker
		{
			get
			{
				return this.m_marker;
			}
			set
			{
				this.m_marker = value;
			}
		}

		internal ExpressionInfo AxisLabel
		{
			get
			{
				return this.m_axisLabel;
			}
			set
			{
				this.m_axisLabel = value;
			}
		}

		internal ChartItemInLegend ItemInLegend
		{
			get
			{
				return this.m_itemInLegend;
			}
			set
			{
				this.m_itemInLegend = value;
			}
		}

		internal ExpressionInfo ToolTip
		{
			get
			{
				return this.m_toolTip;
			}
			set
			{
				this.m_toolTip = value;
			}
		}

		public override AspNetCore.ReportingServices.ReportProcessing.ObjectType DataScopeObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.ChartDataPoint;
			}
		}

		protected override AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode ExprHostDataRegionMode
		{
			get
			{
				return AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder.DataRegionMode.Chart;
			}
		}

		internal ChartDataPoint()
		{
		}

		internal ChartDataPoint(int id, Chart chart)
			: base(id, chart)
		{
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartDataPoint chartDataPoint = (ChartDataPoint)base.PublishClone(context);
			if (this.m_action != null)
			{
				chartDataPoint.m_action = (Action)this.m_action.PublishClone(context);
			}
			if (this.m_styleClass != null)
			{
				chartDataPoint.m_styleClass = (Style)this.m_styleClass.PublishClone(context);
			}
			if (this.m_customProperties != null)
			{
				chartDataPoint.m_customProperties = new DataValueList(this.m_customProperties.Count);
				foreach (DataValue customProperty in this.m_customProperties)
				{
					chartDataPoint.m_customProperties.Add((DataValue)customProperty.PublishClone(context));
				}
			}
			if (this.m_marker != null)
			{
				chartDataPoint.m_marker = (ChartMarker)this.m_marker.PublishClone(context);
			}
			if (this.m_dataPointValues != null)
			{
				chartDataPoint.m_dataPointValues = (ChartDataPointValues)this.m_dataPointValues.PublishClone(context);
				chartDataPoint.m_dataPointValues.DataPoint = chartDataPoint;
			}
			if (this.m_dataLabel != null)
			{
				chartDataPoint.m_dataLabel = (ChartDataLabel)this.m_dataLabel.PublishClone(context);
			}
			if (this.m_axisLabel != null)
			{
				chartDataPoint.m_axisLabel = (ExpressionInfo)this.m_axisLabel.PublishClone(context);
			}
			if (this.m_itemInLegend != null)
			{
				chartDataPoint.m_itemInLegend = (ChartItemInLegend)this.m_itemInLegend.PublishClone(context);
			}
			if (this.m_toolTip != null)
			{
				chartDataPoint.m_toolTip = (ExpressionInfo)this.m_toolTip.PublishClone(context);
			}
			return chartDataPoint;
		}

		internal override void InternalInitialize(int parentRowID, int parentColumnID, int rowindex, int colIndex, InitializationContext context)
		{
			AspNetCore.ReportingServices.RdlExpressions.ExprHostBuilder exprHostBuilder = context.ExprHostBuilder;
			if (this.m_dataPointValues != null)
			{
				this.m_dataPointValues.Initialize(context);
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
			if (this.m_marker != null)
			{
				this.m_marker.Initialize(context);
			}
			if (this.m_customProperties != null)
			{
				this.m_customProperties.Initialize(null, context);
			}
			if (this.m_axisLabel != null)
			{
				this.m_axisLabel.Initialize("AxisLabel", context);
				context.ExprHostBuilder.ChartDataPointAxisLabel(this.m_axisLabel);
			}
			if (this.m_itemInLegend != null)
			{
				this.m_itemInLegend.Initialize(context);
			}
			if (this.m_toolTip != null)
			{
				this.m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.ChartDataPointToolTip(this.m_toolTip);
			}
			this.DataRendererInitialize(context);
		}

		internal void DataRendererInitialize(InitializationContext context)
		{
			if (this.m_dataElementOutput == DataElementOutputTypes.Auto)
			{
				this.m_dataElementOutput = DataElementOutputTypes.Output;
			}
			AspNetCore.ReportingServices.ReportPublishing.CLSNameValidator.ValidateDataElementName(ref this.m_dataElementName, "Value", context.ObjectType, context.ObjectName, "DataElementName", context.ErrorContext);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.DataLabel, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataLabel));
			list.Add(new MemberInfo(MemberName.Action, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.StyleClass, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Style));
			list.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			list.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			list.Add(new MemberInfo(MemberName.CustomProperties, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataValue));
			list.Add(new MemberInfo(MemberName.DataPointValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataPointValues));
			list.Add(new MemberInfo(MemberName.Marker, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartMarker));
			list.Add(new MemberInfo(MemberName.AxisLabel, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ChartItemInLegend, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartItemInLegend));
			list.Add(new MemberInfo(MemberName.ToolTip, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataPoint, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Cell, list);
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateAxisLabel(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataPointAxisLabelExpression(this, base.m_dataRegionDef.Name);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			AspNetCore.ReportingServices.RdlExpressions.VariantResult variantResult = context.ReportRuntime.EvaluateChartDataPointToolTipExpression(this, base.m_dataRegionDef.Name);
			string result = null;
			if (variantResult.ErrorOccurred)
			{
				result = RPRes.rsExpressionErrorValue;
			}
			else if (variantResult.Value != null)
			{
				result = Formatter.Format(variantResult.Value, ref this.m_formatter, base.m_dataRegionDef.StyleClass, this.m_styleClass, context, this.ObjectType, this.Name);
			}
			return result;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ChartDataPoint.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DataPointValues:
					writer.Write(this.m_dataPointValues);
					break;
				case MemberName.DataLabel:
					writer.Write(this.m_dataLabel);
					break;
				case MemberName.Action:
					writer.Write(this.m_action);
					break;
				case MemberName.StyleClass:
					writer.Write(this.m_styleClass);
					break;
				case MemberName.DataElementName:
					writer.Write(this.m_dataElementName);
					break;
				case MemberName.DataElementOutput:
					writer.WriteEnum((int)this.m_dataElementOutput);
					break;
				case MemberName.CustomProperties:
					writer.Write(this.m_customProperties);
					break;
				case MemberName.Marker:
					writer.Write(this.m_marker);
					break;
				case MemberName.AxisLabel:
					writer.Write(this.m_axisLabel);
					break;
				case MemberName.ChartItemInLegend:
					writer.Write(this.m_itemInLegend);
					break;
				case MemberName.ToolTip:
					writer.Write(this.m_toolTip);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(ChartDataPoint.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.DataPointValues:
					this.m_dataPointValues = (ChartDataPointValues)reader.ReadRIFObject();
					break;
				case MemberName.DataLabel:
					this.m_dataLabel = (ChartDataLabel)reader.ReadRIFObject();
					break;
				case MemberName.Action:
					this.m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.StyleClass:
					this.m_styleClass = (Style)reader.ReadRIFObject();
					break;
				case MemberName.DataElementName:
					this.m_dataElementName = reader.ReadString();
					break;
				case MemberName.DataElementOutput:
					this.m_dataElementOutput = (DataElementOutputTypes)reader.ReadEnum();
					break;
				case MemberName.CustomProperties:
					this.m_customProperties = reader.ReadListOfRIFObjects<DataValueList>();
					break;
				case MemberName.Marker:
					this.m_marker = (ChartMarker)reader.ReadRIFObject();
					break;
				case MemberName.AxisLabel:
					this.m_axisLabel = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ChartItemInLegend:
					this.m_itemInLegend = (ChartItemInLegend)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					this.m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataPoint;
		}

		internal void SetExprHost(ChartDataPointExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
			if (this.m_action != null && this.m_exprHost.ActionInfoHost != null)
			{
				this.m_action.SetExprHost(this.m_exprHost.ActionInfoHost, reportObjectModel);
			}
			if (this.m_styleClass != null && this.m_exprHost.StyleHost != null)
			{
				this.m_exprHost.StyleHost.SetReportObjectModel(reportObjectModel);
				this.m_styleClass.SetStyleExprHost(this.m_exprHost.StyleHost);
			}
			if (this.m_marker != null && this.m_exprHost.ChartMarkerHost != null)
			{
				this.m_marker.SetExprHost(this.m_exprHost.ChartMarkerHost, reportObjectModel);
			}
			if (this.m_dataLabel != null && this.m_exprHost.DataLabelHost != null)
			{
				this.m_dataLabel.SetExprHost(this.m_exprHost.DataLabelHost, reportObjectModel);
			}
			if (this.m_itemInLegend != null && this.m_exprHost.DataPointInLegendHost != null)
			{
				this.m_itemInLegend.SetExprHost(this.m_exprHost.DataPointInLegendHost, reportObjectModel);
			}
			if (this.m_customProperties != null && this.m_exprHost.CustomPropertyHostsRemotable != null)
			{
				this.m_customProperties.SetExprHost(this.m_exprHost.CustomPropertyHostsRemotable, reportObjectModel);
			}
			base.BaseSetExprHost(exprHost, reportObjectModel);
		}
	}
}
