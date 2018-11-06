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
	internal sealed class GaugeTickMarks : TickMarkStyle, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GaugeTickMarks.GetDeclaration();

		private ExpressionInfo m_interval;

		private ExpressionInfo m_intervalOffset;

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

		internal GaugeTickMarks()
		{
		}

		internal GaugeTickMarks(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal void Initialize(InitializationContext context, bool isMajor)
		{
			context.ExprHostBuilder.GaugeTickMarksStart(isMajor);
			base.InitializeInternal(context);
			if (this.m_interval != null)
			{
				this.m_interval.Initialize("Interval", context);
				context.ExprHostBuilder.GaugeTickMarksInterval(this.m_interval);
			}
			if (this.m_intervalOffset != null)
			{
				this.m_intervalOffset.Initialize("IntervalOffset", context);
				context.ExprHostBuilder.GaugeTickMarksIntervalOffset(this.m_intervalOffset);
			}
			context.ExprHostBuilder.GaugeTickMarksEnd(isMajor);
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			GaugeTickMarks gaugeTickMarks = (GaugeTickMarks)base.PublishClone(context);
			if (this.m_interval != null)
			{
				gaugeTickMarks.m_interval = (ExpressionInfo)this.m_interval.PublishClone(context);
			}
			if (this.m_intervalOffset != null)
			{
				gaugeTickMarks.m_intervalOffset = (ExpressionInfo)this.m_intervalOffset.PublishClone(context);
			}
			return gaugeTickMarks;
		}

		internal void SetExprHost(GaugeTickMarksExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			base.m_exprHost = exprHost;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Interval, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalOffset, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeTickMarks, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.TickMarkStyle, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(GaugeTickMarks.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Interval:
					writer.Write(this.m_interval);
					break;
				case MemberName.IntervalOffset:
					writer.Write(this.m_intervalOffset);
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
			reader.RegisterDeclaration(GaugeTickMarks.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Interval:
					this.m_interval = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IntervalOffset:
					this.m_intervalOffset = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeTickMarks;
		}

		internal double EvaluateInterval(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeTickMarksIntervalExpression(this, base.m_gaugePanel.Name);
		}

		internal double EvaluateIntervalOffset(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeTickMarksIntervalOffsetExpression(this, base.m_gaugePanel.Name);
		}
	}
}
