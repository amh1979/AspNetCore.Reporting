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
	internal sealed class ChartDataLabel : ChartStyleContainer, IPersistable
	{
		private ExpressionInfo m_visible;

		private ExpressionInfo m_label;

		private ExpressionInfo m_position;

		private ExpressionInfo m_rotation;

		private ExpressionInfo m_useValueAsLabel;

		private Action m_action;

		private ExpressionInfo m_toolTip;

		[Reference]
		private ChartDataPoint m_chartDataPoint;

		[Reference]
		private ChartSeries m_chartSeries;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartDataLabel.GetDeclaration();

		[NonSerialized]
		private Formatter m_formatter;

		[NonSerialized]
		private ChartDataLabelExprHost m_exprHost;

		internal ExpressionInfo Visible
		{
			get
			{
				return this.m_visible;
			}
			set
			{
				this.m_visible = value;
			}
		}

		internal ExpressionInfo Label
		{
			get
			{
				return this.m_label;
			}
			set
			{
				this.m_label = value;
			}
		}

		internal ExpressionInfo UseValueAsLabel
		{
			get
			{
				return this.m_useValueAsLabel;
			}
			set
			{
				this.m_useValueAsLabel = value;
			}
		}

		internal ExpressionInfo Position
		{
			get
			{
				return this.m_position;
			}
			set
			{
				this.m_position = value;
			}
		}

		internal ExpressionInfo Rotation
		{
			get
			{
				return this.m_rotation;
			}
			set
			{
				this.m_rotation = value;
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

		internal ChartDataLabelExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		public override IInstancePath InstancePath
		{
			get
			{
				if (this.m_chartDataPoint != null)
				{
					return this.m_chartDataPoint;
				}
				if (this.m_chartSeries != null)
				{
					return this.m_chartSeries;
				}
				return base.InstancePath;
			}
		}

		internal ChartDataLabel()
		{
		}

		internal ChartDataLabel(Chart chart, ChartDataPoint chartDataPoint)
			: base(chart)
		{
			this.m_chartDataPoint = chartDataPoint;
		}

		internal ChartDataLabel(Chart chart, ChartSeries chartSeries)
			: base(chart)
		{
			this.m_chartSeries = chartSeries;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartDataLabel chartDataLabel = (ChartDataLabel)base.PublishClone(context);
			if (this.m_label != null)
			{
				chartDataLabel.m_label = (ExpressionInfo)this.m_label.PublishClone(context);
			}
			if (this.m_visible != null)
			{
				chartDataLabel.m_visible = (ExpressionInfo)this.m_visible.PublishClone(context);
			}
			if (this.m_position != null)
			{
				chartDataLabel.m_position = (ExpressionInfo)this.m_position.PublishClone(context);
			}
			if (this.m_rotation != null)
			{
				chartDataLabel.m_rotation = (ExpressionInfo)this.m_rotation.PublishClone(context);
			}
			if (this.m_useValueAsLabel != null)
			{
				chartDataLabel.m_useValueAsLabel = (ExpressionInfo)this.m_useValueAsLabel.PublishClone(context);
			}
			if (this.m_action != null)
			{
				chartDataLabel.m_action = (Action)this.m_action.PublishClone(context);
			}
			if (this.m_toolTip != null)
			{
				chartDataLabel.m_toolTip = (ExpressionInfo)this.m_toolTip.PublishClone(context);
			}
			return chartDataLabel;
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.DataLabelStart();
			base.Initialize(context);
			if (this.m_label != null)
			{
				this.m_label.Initialize("Label", context);
				context.ExprHostBuilder.DataLabelLabel(this.m_label);
			}
			if (this.m_visible != null)
			{
				this.m_visible.Initialize("Visible", context);
				context.ExprHostBuilder.DataLabelVisible(this.m_visible);
			}
			if (this.m_position != null)
			{
				this.m_position.Initialize("Position", context);
				context.ExprHostBuilder.DataLabelPosition(this.m_position);
			}
			if (this.m_rotation != null)
			{
				this.m_rotation.Initialize("Rotation", context);
				context.ExprHostBuilder.DataLabelRotation(this.m_rotation);
			}
			if (this.m_useValueAsLabel != null)
			{
				this.m_useValueAsLabel.Initialize("UseValueAsLabel", context);
				context.ExprHostBuilder.DataLabelUseValueAsLabel(this.m_useValueAsLabel);
			}
			if (this.m_action != null)
			{
				this.m_action.Initialize(context);
			}
			if (this.m_toolTip != null)
			{
				this.m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.ChartDataLabelToolTip(this.m_toolTip);
			}
			context.ExprHostBuilder.DataLabelEnd();
		}

		internal void SetExprHost(ChartDataLabelExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && null != reportObjectModel, "(null != exprHost && null != reportObjectModel)");
			base.SetExprHost(exprHost, reportObjectModel);
			this.m_exprHost = exprHost;
			if (this.m_action != null && this.m_exprHost.ActionInfoHost != null)
			{
				this.m_action.SetExprHost(this.m_exprHost.ActionInfoHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Visible, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Label, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.UseValueAsLabel, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Position, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Rotation, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Action, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.ChartDataPoint, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataPoint, Token.Reference));
			list.Add(new MemberInfo(MemberName.ChartSeries, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSeries, Token.Reference));
			list.Add(new MemberInfo(MemberName.ToolTip, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataLabel, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ChartDataLabel.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Visible:
					writer.Write(this.m_visible);
					break;
				case MemberName.Label:
					writer.Write(this.m_label);
					break;
				case MemberName.Position:
					writer.Write(this.m_position);
					break;
				case MemberName.Rotation:
					writer.Write(this.m_rotation);
					break;
				case MemberName.UseValueAsLabel:
					writer.Write(this.m_useValueAsLabel);
					break;
				case MemberName.Action:
					writer.Write(this.m_action);
					break;
				case MemberName.ChartDataPoint:
					writer.WriteReference(this.m_chartDataPoint);
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
			reader.RegisterDeclaration(ChartDataLabel.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Visible:
					this.m_visible = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Label:
					this.m_label = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Position:
					this.m_position = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Rotation:
					this.m_rotation = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.UseValueAsLabel:
					this.m_useValueAsLabel = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Action:
					this.m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.ChartDataPoint:
					this.m_chartDataPoint = reader.ReadReference<ChartDataPoint>(this);
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
			if (memberReferencesCollection.TryGetValue(ChartDataLabel.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					switch (item.MemberName)
					{
					case MemberName.ChartDataPoint:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_chartDataPoint = (ChartDataPoint)referenceableItems[item.RefID];
						break;
					case MemberName.ChartSeries:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_chartSeries = (ChartSeries)referenceableItems[item.RefID];
						break;
					default:
						Global.Tracer.Assert(false);
						break;
					}
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataLabel;
		}

		internal AspNetCore.ReportingServices.RdlExpressions.VariantResult EvaluateLabel(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataLabelLabelExpression(this, base.Name);
		}

		internal ChartDataLabelPositions EvaluatePosition(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return EnumTranslator.TranslateChartDataLabelPosition(context.ReportRuntime.EvaluateChartDataLabePositionExpression(this, base.Name), context.ReportRuntime);
		}

		internal int EvaluateRotation(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataLabelRotationExpression(this, base.Name);
		}

		internal bool EvaluateUseValueAsLabel(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataLabelUseValueAsLabelExpression(this, base.Name);
		}

		internal bool EvaluateVisible(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartDataLabelVisibleExpression(this, base.Name);
		}

		internal string GetFormattedValue(AspNetCore.ReportingServices.RdlExpressions.VariantResult originalValue, IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			if (originalValue.ErrorOccurred)
			{
				return RPRes.rsExpressionErrorValue;
			}
			if (originalValue.Value != null)
			{
				return Formatter.Format(originalValue.Value, ref this.m_formatter, base.m_chart.StyleClass, base.m_styleClass, context, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, base.m_chart.Name);
			}
			return null;
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			AspNetCore.ReportingServices.RdlExpressions.VariantResult variantResult = context.ReportRuntime.EvaluateChartDataLabelToolTipExpression(this, base.m_chart.Name);
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
	}
}
