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
	internal sealed class ChartEmptyPoints : ChartStyleContainer, IPersistable, IActionOwner, ICustomPropertiesHolder
	{
		private Action m_action;

		private ChartMarker m_marker;

		private ChartDataLabel m_dataLabel;

		private ExpressionInfo m_axisLabel;

		private DataValueList m_customProperties;

		private ExpressionInfo m_toolTip;

		[Reference]
		private ChartSeries m_chartSeries;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartEmptyPoints.GetDeclaration();

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		[NonSerialized]
		private ChartEmptyPointsExprHost m_exprHost;

		[NonSerialized]
		private Formatter m_formatter;

		internal ChartEmptyPointsExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
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

		public DataValueList CustomProperties
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

		public override IInstancePath InstancePath
		{
			get
			{
				if (this.m_chartSeries != null)
				{
					return this.m_chartSeries;
				}
				return base.InstancePath;
			}
		}

		internal ChartEmptyPoints()
		{
		}

		internal ChartEmptyPoints(Chart chart, ChartSeries chartSeries)
			: base(chart)
		{
			this.m_chartSeries = chartSeries;
		}

		internal void SetExprHost(ChartEmptyPointsExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			this.m_exprHost = exprHost;
			if (this.m_marker != null && this.m_exprHost.ChartMarkerHost != null)
			{
				this.m_marker.SetExprHost(this.m_exprHost.ChartMarkerHost, reportObjectModel);
			}
			if (this.m_dataLabel != null && this.m_exprHost.DataLabelHost != null)
			{
				this.m_dataLabel.SetExprHost(this.m_exprHost.DataLabelHost, reportObjectModel);
			}
			if (this.m_action != null && this.m_exprHost.ActionInfoHost != null)
			{
				this.m_action.SetExprHost(this.m_exprHost.ActionInfoHost, reportObjectModel);
			}
			if (this.m_customProperties != null && this.m_exprHost.CustomPropertyHostsRemotable != null)
			{
				this.m_customProperties.SetExprHost(this.m_exprHost.CustomPropertyHostsRemotable, reportObjectModel);
			}
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartEmptyPointsStart();
			base.Initialize(context);
			if (this.m_action != null)
			{
				this.m_action.Initialize(context);
			}
			if (this.m_marker != null)
			{
				this.m_marker.Initialize(context);
			}
			if (this.m_dataLabel != null)
			{
				this.m_dataLabel.Initialize(context);
			}
			if (this.m_axisLabel != null)
			{
				this.m_axisLabel.Initialize("AxisLabel", context);
				context.ExprHostBuilder.ChartEmptyPointsAxisLabel(this.m_axisLabel);
			}
			if (this.m_customProperties != null)
			{
				this.m_customProperties.Initialize(null, context);
			}
			if (this.m_toolTip != null)
			{
				this.m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.ChartEmptyPointsToolTip(this.m_toolTip);
			}
			context.ExprHostBuilder.ChartEmptyPointsEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartEmptyPoints chartEmptyPoints = (ChartEmptyPoints)base.PublishClone(context);
			if (this.m_action != null)
			{
				chartEmptyPoints.m_action = (Action)this.m_action.PublishClone(context);
			}
			if (this.m_marker != null)
			{
				chartEmptyPoints.m_marker = (ChartMarker)this.m_marker.PublishClone(context);
			}
			if (this.m_dataLabel != null)
			{
				chartEmptyPoints.m_dataLabel = (ChartDataLabel)this.m_dataLabel.PublishClone(context);
			}
			if (this.m_axisLabel != null)
			{
				chartEmptyPoints.m_axisLabel = (ExpressionInfo)this.m_axisLabel.PublishClone(context);
			}
			if (this.m_customProperties != null)
			{
				chartEmptyPoints.m_customProperties = new DataValueList(this.m_customProperties.Count);
				foreach (DataValue customProperty in this.m_customProperties)
				{
					chartEmptyPoints.m_customProperties.Add((DataValue)customProperty.PublishClone(context));
				}
			}
			if (this.m_toolTip != null)
			{
				chartEmptyPoints.m_toolTip = (ExpressionInfo)this.m_toolTip.PublishClone(context);
			}
			return chartEmptyPoints;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Action, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.Marker, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartMarker));
			list.Add(new MemberInfo(MemberName.DataLabel, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataLabel));
			list.Add(new MemberInfo(MemberName.AxisLabel, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.CustomProperties, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataValue));
			list.Add(new MemberInfo(MemberName.ChartSeries, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSeries, Token.Reference));
			list.Add(new MemberInfo(MemberName.ToolTip, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartEmptyPoints, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateAxisLabel(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartEmptyPointsAxisLabelExpression(this, base.m_chart.Name);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			AspNetCore.ReportingServices.RdlExpressions.VariantResult variantResult = context.ReportRuntime.EvaluateChartEmptyPointsToolTipExpression(this, base.m_chart.Name);
			string result = null;
			if (variantResult.ErrorOccurred)
			{
				result = RPRes.rsExpressionErrorValue;
			}
			else if (variantResult.Value != null)
			{
				result = Formatter.Format(variantResult.Value, ref this.m_formatter, base.m_chart.StyleClass, base.m_styleClass, context, base.ObjectType, base.Name);
			}
			return result;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ChartEmptyPoints.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Action:
					writer.Write(this.m_action);
					break;
				case MemberName.Marker:
					writer.Write(this.m_marker);
					break;
				case MemberName.DataLabel:
					writer.Write(this.m_dataLabel);
					break;
				case MemberName.AxisLabel:
					writer.Write(this.m_axisLabel);
					break;
				case MemberName.CustomProperties:
					writer.Write(this.m_customProperties);
					break;
				case MemberName.ChartSeries:
					writer.WriteReference(this.m_chartSeries);
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
			reader.RegisterDeclaration(ChartEmptyPoints.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Action:
					this.m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.Marker:
					this.m_marker = (ChartMarker)reader.ReadRIFObject();
					break;
				case MemberName.DataLabel:
					this.m_dataLabel = (ChartDataLabel)reader.ReadRIFObject();
					break;
				case MemberName.AxisLabel:
					this.m_axisLabel = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CustomProperties:
					this.m_customProperties = reader.ReadListOfRIFObjects<DataValueList>();
					break;
				case MemberName.ChartSeries:
					this.m_chartSeries = reader.ReadReference<ChartSeries>(this);
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
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(ChartEmptyPoints.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					MemberName memberName = item.MemberName;
					if (memberName == MemberName.ChartSeries)
					{
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_chartSeries = (ChartSeries)referenceableItems[item.RefID];
					}
					else
					{
						Global.Tracer.Assert(false);
					}
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartEmptyPoints;
		}
	}
}
