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
	internal sealed class ChartAxisScaleBreak : ChartStyleContainer, IPersistable
	{
		private ExpressionInfo m_enabled;

		private ExpressionInfo m_breakLineType;

		private ExpressionInfo m_collapsibleSpaceThreshold;

		private ExpressionInfo m_maxNumberOfBreaks;

		private ExpressionInfo m_spacing;

		private ExpressionInfo m_includeZero;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartAxisScaleBreak.GetDeclaration();

		[NonSerialized]
		private ChartAxisScaleBreakExprHost m_exprHost;

		internal ChartAxisScaleBreakExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal ExpressionInfo Enabled
		{
			get
			{
				return this.m_enabled;
			}
			set
			{
				this.m_enabled = value;
			}
		}

		internal ExpressionInfo BreakLineType
		{
			get
			{
				return this.m_breakLineType;
			}
			set
			{
				this.m_breakLineType = value;
			}
		}

		internal ExpressionInfo CollapsibleSpaceThreshold
		{
			get
			{
				return this.m_collapsibleSpaceThreshold;
			}
			set
			{
				this.m_collapsibleSpaceThreshold = value;
			}
		}

		internal ExpressionInfo MaxNumberOfBreaks
		{
			get
			{
				return this.m_maxNumberOfBreaks;
			}
			set
			{
				this.m_maxNumberOfBreaks = value;
			}
		}

		internal ExpressionInfo Spacing
		{
			get
			{
				return this.m_spacing;
			}
			set
			{
				this.m_spacing = value;
			}
		}

		internal ExpressionInfo IncludeZero
		{
			get
			{
				return this.m_includeZero;
			}
			set
			{
				this.m_includeZero = value;
			}
		}

		internal ChartAxisScaleBreak()
		{
		}

		internal ChartAxisScaleBreak(Chart chart)
			: base(chart)
		{
		}

		internal void SetExprHost(ChartAxisScaleBreakExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			this.m_exprHost = exprHost;
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartAxisScaleBreakStart();
			base.Initialize(context);
			if (this.m_enabled != null)
			{
				this.m_enabled.Initialize("Enabled", context);
				context.ExprHostBuilder.ChartAxisScaleBreakEnabled(this.m_enabled);
			}
			if (this.m_breakLineType != null)
			{
				this.m_breakLineType.Initialize("BreakLineType", context);
				context.ExprHostBuilder.ChartAxisScaleBreakBreakLineType(this.m_breakLineType);
			}
			if (this.m_collapsibleSpaceThreshold != null)
			{
				this.m_collapsibleSpaceThreshold.Initialize("CollapsibleSpaceThreshold", context);
				context.ExprHostBuilder.ChartAxisScaleBreakCollapsibleSpaceThreshold(this.m_collapsibleSpaceThreshold);
			}
			if (this.m_maxNumberOfBreaks != null)
			{
				this.m_maxNumberOfBreaks.Initialize("MaxNumberOfBreaks", context);
				context.ExprHostBuilder.ChartAxisScaleBreakMaxNumberOfBreaks(this.m_maxNumberOfBreaks);
			}
			if (this.m_spacing != null)
			{
				this.m_spacing.Initialize("Spacing", context);
				context.ExprHostBuilder.ChartAxisScaleBreakSpacing(this.m_spacing);
			}
			if (this.m_includeZero != null)
			{
				this.m_includeZero.Initialize("IncludeZero", context);
				context.ExprHostBuilder.ChartAxisScaleBreakIncludeZero(this.m_includeZero);
			}
			context.ExprHostBuilder.ChartAxisScaleBreakEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartAxisScaleBreak chartAxisScaleBreak = (ChartAxisScaleBreak)base.PublishClone(context);
			if (this.m_enabled != null)
			{
				chartAxisScaleBreak.m_enabled = (ExpressionInfo)this.m_enabled.PublishClone(context);
			}
			if (this.m_breakLineType != null)
			{
				chartAxisScaleBreak.m_breakLineType = (ExpressionInfo)this.m_breakLineType.PublishClone(context);
			}
			if (this.m_collapsibleSpaceThreshold != null)
			{
				chartAxisScaleBreak.m_collapsibleSpaceThreshold = (ExpressionInfo)this.m_collapsibleSpaceThreshold.PublishClone(context);
			}
			if (this.m_maxNumberOfBreaks != null)
			{
				chartAxisScaleBreak.m_maxNumberOfBreaks = (ExpressionInfo)this.m_maxNumberOfBreaks.PublishClone(context);
			}
			if (this.m_spacing != null)
			{
				chartAxisScaleBreak.m_spacing = (ExpressionInfo)this.m_spacing.PublishClone(context);
			}
			if (this.m_includeZero != null)
			{
				chartAxisScaleBreak.m_includeZero = (ExpressionInfo)this.m_includeZero.PublishClone(context);
			}
			return chartAxisScaleBreak;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Enabled, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.BreakLineType, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.CollapsibleSpaceThreshold, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MaxNumberOfBreaks, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Spacing, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IncludeZero, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartAxisScaleBreak, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		internal bool EvaluateEnabled(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisScaleBreakEnabledExpression(this, base.m_chart.Name);
		}

		internal ChartBreakLineType EvaluateBreakLineType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartBreakLineType(context.ReportRuntime.EvaluateChartAxisScaleBreakBreakLineTypeExpression(this, base.m_chart.Name), context.ReportRuntime);
		}

		internal int EvaluateCollapsibleSpaceThreshold(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisScaleBreakCollapsibleSpaceThresholdExpression(this, base.m_chart.Name);
		}

		internal int EvaluateMaxNumberOfBreaks(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisScaleBreakMaxNumberOfBreaksExpression(this, base.m_chart.Name);
		}

		internal double EvaluateSpacing(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisScaleBreakSpacingExpression(this, base.m_chart.Name);
		}

		internal string EvaluateIncludeZero(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartAxisScaleBreakIncludeZeroExpression(this, base.m_chart.Name);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ChartAxisScaleBreak.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Enabled:
					writer.Write(this.m_enabled);
					break;
				case MemberName.BreakLineType:
					writer.Write(this.m_breakLineType);
					break;
				case MemberName.CollapsibleSpaceThreshold:
					writer.Write(this.m_collapsibleSpaceThreshold);
					break;
				case MemberName.MaxNumberOfBreaks:
					writer.Write(this.m_maxNumberOfBreaks);
					break;
				case MemberName.Spacing:
					writer.Write(this.m_spacing);
					break;
				case MemberName.IncludeZero:
					writer.Write(this.m_includeZero);
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
			reader.RegisterDeclaration(ChartAxisScaleBreak.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Enabled:
					this.m_enabled = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.BreakLineType:
					this.m_breakLineType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.CollapsibleSpaceThreshold:
					this.m_collapsibleSpaceThreshold = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MaxNumberOfBreaks:
					this.m_maxNumberOfBreaks = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Spacing:
					this.m_spacing = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IncludeZero:
					this.m_includeZero = (ExpressionInfo)reader.ReadRIFObject();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartAxisScaleBreak;
		}
	}
}
