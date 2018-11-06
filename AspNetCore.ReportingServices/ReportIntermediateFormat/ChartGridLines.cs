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
	internal sealed class ChartGridLines : ChartStyleContainer, IPersistable
	{
		private ExpressionInfo m_enabled;

		private ExpressionInfo m_interval;

		private ExpressionInfo m_intervalType;

		private ExpressionInfo m_intervalOffset;

		private ExpressionInfo m_intervalOffsetType;

		[NonSerialized]
		private ChartGridLinesExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartGridLines.GetDeclaration();

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

		internal ExpressionInfo Interval
		{
			get
			{
				return this.m_interval;
			}
			set
			{
				this.m_interval = value;
			}
		}

		internal ExpressionInfo IntervalType
		{
			get
			{
				return this.m_intervalType;
			}
			set
			{
				this.m_intervalType = value;
			}
		}

		internal ExpressionInfo IntervalOffset
		{
			get
			{
				return this.m_intervalOffset;
			}
			set
			{
				this.m_intervalOffset = value;
			}
		}

		internal ExpressionInfo IntervalOffsetType
		{
			get
			{
				return this.m_intervalOffsetType;
			}
			set
			{
				this.m_intervalOffsetType = value;
			}
		}

		internal ChartGridLinesExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal ChartGridLines()
		{
		}

		internal ChartGridLines(Chart chart)
			: base(chart)
		{
		}

		internal void Initialize(InitializationContext context, bool isMajor)
		{
			context.ExprHostBuilder.ChartGridLinesStart(isMajor);
			base.Initialize(context);
			if (this.m_enabled != null)
			{
				this.m_enabled.Initialize("Enabled", context);
				context.ExprHostBuilder.ChartGridLinesEnabled(this.m_enabled);
			}
			if (this.m_interval != null)
			{
				this.m_interval.Initialize("Interval", context);
				context.ExprHostBuilder.ChartGridLinesInterval(this.m_interval);
			}
			if (this.m_intervalType != null)
			{
				this.m_intervalType.Initialize("IntervalType", context);
				context.ExprHostBuilder.ChartGridLinesEnabledIntervalType(this.m_intervalType);
			}
			if (this.m_intervalOffset != null)
			{
				this.m_intervalOffset.Initialize("IntervalOffset", context);
				context.ExprHostBuilder.ChartGridLinesIntervalOffset(this.m_intervalOffset);
			}
			if (this.m_intervalOffsetType != null)
			{
				this.m_intervalOffsetType.Initialize("IntervalOffsetType", context);
				context.ExprHostBuilder.ChartGridLinesIntervalOffsetType(this.m_intervalOffsetType);
			}
			context.ExprHostBuilder.ChartGridLinesEnd(isMajor);
		}

		internal void SetExprHost(ChartGridLinesExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			base.SetExprHost(exprHost, reportObjectModel);
			this.m_exprHost = exprHost;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartGridLines chartGridLines = (ChartGridLines)base.PublishClone(context);
			if (this.m_enabled != null)
			{
				chartGridLines.m_enabled = (ExpressionInfo)this.m_enabled.PublishClone(context);
			}
			if (this.m_interval != null)
			{
				chartGridLines.m_interval = (ExpressionInfo)this.m_interval.PublishClone(context);
			}
			if (this.m_intervalType != null)
			{
				chartGridLines.m_intervalType = (ExpressionInfo)this.m_intervalType.PublishClone(context);
			}
			if (this.m_intervalOffset != null)
			{
				chartGridLines.m_intervalOffset = (ExpressionInfo)this.m_intervalOffset.PublishClone(context);
			}
			if (this.m_intervalOffsetType != null)
			{
				chartGridLines.m_intervalOffsetType = (ExpressionInfo)this.m_intervalOffsetType.PublishClone(context);
			}
			return chartGridLines;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Enabled, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Interval, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalType, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalOffset, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalOffsetType, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GridLines, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ChartGridLines.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Enabled:
					writer.Write(this.m_enabled);
					break;
				case MemberName.Interval:
					writer.Write(this.m_interval);
					break;
				case MemberName.IntervalType:
					writer.Write(this.m_intervalType);
					break;
				case MemberName.IntervalOffset:
					writer.Write(this.m_intervalOffset);
					break;
				case MemberName.IntervalOffsetType:
					writer.Write(this.m_intervalOffsetType);
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
			reader.RegisterDeclaration(ChartGridLines.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Enabled:
					this.m_enabled = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Interval:
					this.m_interval = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IntervalType:
					this.m_intervalType = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IntervalOffset:
					this.m_intervalOffset = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IntervalOffsetType:
					this.m_intervalOffsetType = (ExpressionInfo)reader.ReadRIFObject();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GridLines;
		}

		internal ChartAutoBool EvaluateEnabled(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartAutoBool(context.ReportRuntime.EvaluateChartGridLinesEnabledExpression(this, base.m_chart.Name), context.ReportRuntime);
		}

		internal double EvaluateInterval(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, instance);
			return context.ReportRuntime.EvaluateChartGridLinesIntervalExpression(this, base.m_chart.Name, "Interval");
		}

		internal ChartIntervalType EvaluateIntervalType(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, instance);
			return EnumTranslator.TranslateChartIntervalType(context.ReportRuntime.EvaluateChartGridLinesIntervalTypeExpression(this, base.m_chart.Name, "IntervalType"), context.ReportRuntime);
		}

		internal double EvaluateIntervalOffset(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, instance);
			return context.ReportRuntime.EvaluateChartGridLinesIntervalOffsetExpression(this, base.m_chart.Name, "IntervalOffset");
		}

		internal ChartIntervalType EvaluateIntervalOffsetType(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, instance);
			return EnumTranslator.TranslateChartIntervalType(context.ReportRuntime.EvaluateChartGridLinesIntervalOffsetTypeExpression(this, base.m_chart.Name, "IntervalOffsetType"), context.ReportRuntime);
		}
	}
}
