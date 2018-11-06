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
	internal sealed class ChartItemInLegend : IPersistable, IActionOwner
	{
		[Reference]
		private Chart m_chart;

		[Reference]
		private ChartSeries m_chartSeries;

		[Reference]
		private ChartDataPoint m_chartDataPoint;

		private Action m_action;

		private ExpressionInfo m_legendText;

		private ExpressionInfo m_toolTip;

		private ExpressionInfo m_hidden;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartItemInLegend.GetDeclaration();

		[NonSerialized]
		private ChartDataPointInLegendExprHost m_exprHost;

		[NonSerialized]
		private Formatter m_formatter;

		internal ChartDataPointInLegendExprHost ExprHost
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

		internal ExpressionInfo LegendText
		{
			get
			{
				return this.m_legendText;
			}
			set
			{
				this.m_legendText = value;
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

		internal ExpressionInfo Hidden
		{
			get
			{
				return this.m_hidden;
			}
			set
			{
				this.m_hidden = value;
			}
		}

		private IInstancePath InstancePath
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
				return this.m_chart;
			}
		}

		internal ChartItemInLegend()
		{
		}

		internal ChartItemInLegend(Chart chart, ChartDataPoint chartDataPoint)
		{
			this.m_chart = chart;
			this.m_chartDataPoint = chartDataPoint;
		}

		internal ChartItemInLegend(Chart chart, ChartSeries chartSeries)
		{
			this.m_chart = chart;
			this.m_chartSeries = chartSeries;
		}

		internal void SetExprHost(ChartDataPointInLegendExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			this.m_exprHost = exprHost;
			this.m_exprHost.SetReportObjectModel(reportObjectModel);
			if (this.m_action != null && this.m_exprHost.ActionInfoHost != null)
			{
				this.m_action.SetExprHost(this.m_exprHost.ActionInfoHost, reportObjectModel);
			}
		}

		internal void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartItemInLegendStart();
			if (this.m_action != null)
			{
				this.m_action.Initialize(context);
			}
			if (this.m_legendText != null)
			{
				this.m_legendText.Initialize("LegendText", context);
				context.ExprHostBuilder.ChartItemInLegendLegendText(this.m_legendText);
			}
			if (this.m_toolTip != null)
			{
				this.m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.ChartItemInLegendToolTip(this.m_toolTip);
			}
			if (this.m_hidden != null)
			{
				this.m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.ChartItemInLegendHidden(this.m_hidden);
			}
			context.ExprHostBuilder.ChartItemInLegendEnd();
		}

		internal object PublishClone(AutomaticSubtotalContext context)
		{
			ChartItemInLegend chartItemInLegend = (ChartItemInLegend)base.MemberwiseClone();
			chartItemInLegend.m_chart = (Chart)context.CurrentDataRegionClone;
			if (this.m_action != null)
			{
				chartItemInLegend.m_action = (Action)this.m_action.PublishClone(context);
			}
			if (this.m_legendText != null)
			{
				chartItemInLegend.m_legendText = (ExpressionInfo)this.m_legendText.PublishClone(context);
			}
			if (this.m_toolTip != null)
			{
				chartItemInLegend.m_toolTip = (ExpressionInfo)this.m_toolTip.PublishClone(context);
			}
			if (this.m_hidden != null)
			{
				chartItemInLegend.m_hidden = (ExpressionInfo)this.m_hidden.PublishClone(context);
			}
			return chartItemInLegend;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Action, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.LegendText, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Chart, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Chart, Token.Reference));
			list.Add(new MemberInfo(MemberName.ChartSeries, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartSeries, Token.Reference));
			list.Add(new MemberInfo(MemberName.ChartDataPoint, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartDataPoint, Token.Reference));
			list.Add(new MemberInfo(MemberName.ToolTip, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Hidden, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartItemInLegend, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		internal string EvaluateLegendText(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartItemInLegendLegendTextExpression(this, this.m_chart.Name);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			AspNetCore.ReportingServices.RdlExpressions.VariantResult variantResult = context.ReportRuntime.EvaluateChartItemInLegendToolTipExpression(this, this.m_chart.Name);
			string result = null;
			if (variantResult.ErrorOccurred)
			{
				result = RPRes.rsExpressionErrorValue;
			}
			else if (variantResult.Value != null)
			{
				result = Formatter.Format(variantResult.Value, ref this.m_formatter, this.m_chart.StyleClass, (Style)null, context, AspNetCore.ReportingServices.ReportProcessing.ObjectType.Chart, this.m_chart.Name);
			}
			return result;
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartItemInLegendHiddenExpression(this, this.m_chart.Name);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(ChartItemInLegend.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Chart:
					writer.WriteReference(this.m_chart);
					break;
				case MemberName.ChartSeries:
					writer.WriteReference(this.m_chartSeries);
					break;
				case MemberName.ChartDataPoint:
					writer.WriteReference(this.m_chartDataPoint);
					break;
				case MemberName.Action:
					writer.Write(this.m_action);
					break;
				case MemberName.LegendText:
					writer.Write(this.m_legendText);
					break;
				case MemberName.ToolTip:
					writer.Write(this.m_toolTip);
					break;
				case MemberName.Hidden:
					writer.Write(this.m_hidden);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(ChartItemInLegend.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Chart:
					this.m_chart = reader.ReadReference<Chart>(this);
					break;
				case MemberName.ChartSeries:
					this.m_chartSeries = reader.ReadReference<ChartSeries>(this);
					break;
				case MemberName.ChartDataPoint:
					this.m_chartDataPoint = reader.ReadReference<ChartDataPoint>(this);
					break;
				case MemberName.Action:
					this.m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.LegendText:
					this.m_legendText = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					this.m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Hidden:
					this.m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			List<MemberReference> list = default(List<MemberReference>);
			if (memberReferencesCollection.TryGetValue(ChartItemInLegend.m_Declaration.ObjectType, out list))
			{
				foreach (MemberReference item in list)
				{
					switch (item.MemberName)
					{
					case MemberName.Chart:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_chart = (Chart)referenceableItems[item.RefID];
						break;
					case MemberName.ChartSeries:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_chartSeries = (ChartSeries)referenceableItems[item.RefID];
						break;
					case MemberName.ChartDataPoint:
						Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
						this.m_chartDataPoint = (ChartDataPoint)referenceableItems[item.RefID];
						break;
					default:
						Global.Tracer.Assert(false);
						break;
					}
				}
			}
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartItemInLegend;
		}
	}
}
