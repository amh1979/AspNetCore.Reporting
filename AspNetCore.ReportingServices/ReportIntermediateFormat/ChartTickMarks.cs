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
	internal sealed class ChartTickMarks : ChartStyleContainer, IPersistable
	{
		private ExpressionInfo m_enabled;

		private ExpressionInfo m_type;

		private ExpressionInfo m_length;

		private ExpressionInfo m_interval;

		private ExpressionInfo m_intervalType;

		private ExpressionInfo m_intervalOffset;

		private ExpressionInfo m_intervalOffsetType;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartTickMarks.GetDeclaration();

		[NonSerialized]
		private ChartTickMarksExprHost m_exprHost;

		internal ChartTickMarksExprHost ExprHost
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

		internal ExpressionInfo Type
		{
			get
			{
				return this.m_type;
			}
			set
			{
				this.m_type = value;
			}
		}

		internal ExpressionInfo Length
		{
			get
			{
				return this.m_length;
			}
			set
			{
				this.m_length = value;
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

		internal ChartTickMarks()
		{
		}

		internal ChartTickMarks(Chart chart)
			: base(chart)
		{
		}

		internal void SetExprHost(ChartTickMarksExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			base.SetExprHost(exprHost, reportObjectModel);
			this.m_exprHost = exprHost;
		}

		internal void Initialize(InitializationContext context, bool isMajor)
		{
			context.ExprHostBuilder.ChartTickMarksStart(isMajor);
			base.Initialize(context);
			if (this.m_enabled != null)
			{
				this.m_enabled.Initialize("Enabled", context);
				context.ExprHostBuilder.ChartTickMarksEnabled(this.m_enabled);
			}
			if (this.m_type != null)
			{
				this.m_type.Initialize("Type", context);
				context.ExprHostBuilder.ChartTickMarksType(this.m_type);
			}
			if (this.m_length != null)
			{
				this.m_length.Initialize("Length", context);
				context.ExprHostBuilder.ChartTickMarksLength(this.m_length);
			}
			if (this.m_interval != null)
			{
				this.m_interval.Initialize("Interval", context);
				context.ExprHostBuilder.ChartTickMarksInterval(this.m_interval);
			}
			if (this.m_intervalType != null)
			{
				this.m_intervalType.Initialize("IntervalType", context);
				context.ExprHostBuilder.ChartTickMarksIntervalType(this.m_intervalType);
			}
			if (this.m_intervalOffset != null)
			{
				this.m_intervalOffset.Initialize("IntervalOffset", context);
				context.ExprHostBuilder.ChartTickMarksIntervalOffset(this.m_intervalOffset);
			}
			if (this.m_intervalOffsetType != null)
			{
				this.m_intervalOffsetType.Initialize("IntervalOffsetType", context);
				context.ExprHostBuilder.ChartTickMarksIntervalOffsetType(this.m_intervalOffsetType);
			}
			context.ExprHostBuilder.ChartTickMarksEnd(isMajor);
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartTickMarks chartTickMarks = (ChartTickMarks)base.PublishClone(context);
			if (this.m_enabled != null)
			{
				chartTickMarks.m_enabled = (ExpressionInfo)this.m_enabled.PublishClone(context);
			}
			if (this.m_type != null)
			{
				chartTickMarks.m_type = (ExpressionInfo)this.m_type.PublishClone(context);
			}
			if (this.m_length != null)
			{
				chartTickMarks.m_length = (ExpressionInfo)this.m_length.PublishClone(context);
			}
			if (this.m_interval != null)
			{
				chartTickMarks.m_interval = (ExpressionInfo)this.m_interval.PublishClone(context);
			}
			if (this.m_intervalType != null)
			{
				chartTickMarks.m_intervalType = (ExpressionInfo)this.m_intervalType.PublishClone(context);
			}
			if (this.m_intervalOffset != null)
			{
				chartTickMarks.m_intervalOffset = (ExpressionInfo)this.m_intervalOffset.PublishClone(context);
			}
			if (this.m_intervalOffsetType != null)
			{
				chartTickMarks.m_intervalOffsetType = (ExpressionInfo)this.m_intervalOffsetType.PublishClone(context);
			}
			return chartTickMarks;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Enabled, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Type, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Length, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Interval, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalType, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalOffset, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalOffsetType, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartTickMarks, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		internal string EvaluateEnabled(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartTickMarksEnabledExpression(this, base.m_chart.Name);
		}

		internal ChartTickMarksType EvaluateType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartTickMarksType(context.ReportRuntime.EvaluateChartTickMarksTypeExpression(this, base.m_chart.Name), context.ReportRuntime);
		}

		internal double EvaluateLength(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartTickMarksLengthExpression(this, base.m_chart.Name);
		}

		internal double EvaluateInterval(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartTickMarksIntervalExpression(this, base.m_chart.Name);
		}

		internal ChartIntervalType EvaluateIntervalType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartIntervalType(context.ReportRuntime.EvaluateChartTickMarksIntervalTypeExpression(this, base.m_chart.Name), context.ReportRuntime);
		}

		internal double EvaluateIntervalOffset(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartTickMarksIntervalOffsetExpression(this, base.m_chart.Name);
		}

		internal ChartIntervalType EvaluateIntervalOffsetType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartIntervalType(context.ReportRuntime.EvaluateChartTickMarksIntervalOffsetTypeExpression(this, base.m_chart.Name), context.ReportRuntime);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ChartTickMarks.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Enabled:
					writer.Write(this.m_enabled);
					break;
				case MemberName.Type:
					writer.Write(this.m_type);
					break;
				case MemberName.Length:
					writer.Write(this.m_length);
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
			reader.RegisterDeclaration(ChartTickMarks.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Enabled:
					this.m_enabled = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Type:
					this.m_type = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Length:
					this.m_length = (ExpressionInfo)reader.ReadRIFObject();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartTickMarks;
		}
	}
}
