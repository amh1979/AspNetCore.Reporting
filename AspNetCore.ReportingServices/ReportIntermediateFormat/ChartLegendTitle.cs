using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class ChartLegendTitle : ChartTitleBase, IPersistable
	{
		private ExpressionInfo m_titleSeparator;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartLegendTitle.GetDeclaration();

		internal ExpressionInfo TitleSeparator
		{
			get
			{
				return this.m_titleSeparator;
			}
			set
			{
				this.m_titleSeparator = value;
			}
		}

		internal ChartLegendTitle()
		{
		}

		internal ChartLegendTitle(Chart chart)
			: base(chart)
		{
			base.m_chart = chart;
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartLegendTitleStart();
			base.Initialize(context);
			if (this.m_titleSeparator != null)
			{
				this.m_titleSeparator.Initialize("TitleSeparator", context);
				context.ExprHostBuilder.ChartLegendTitleSeparator(this.m_titleSeparator);
			}
			context.ExprHostBuilder.ChartLegendTitleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartLegendTitle chartLegendTitle = (ChartLegendTitle)base.PublishClone(context);
			if (this.m_titleSeparator != null)
			{
				chartLegendTitle.m_titleSeparator = (ExpressionInfo)this.m_titleSeparator.PublishClone(context);
			}
			return chartLegendTitle;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.TitleSeparator, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendTitle, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartTitleBase, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ChartLegendTitle.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.TitleSeparator)
				{
					writer.Write(this.m_titleSeparator);
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(ChartLegendTitle.m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.TitleSeparator)
				{
					this.m_titleSeparator = (ExpressionInfo)reader.ReadRIFObject();
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendTitle;
		}

		internal ChartSeparators EvaluateTitleSeparator(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartSeparator(context.ReportRuntime.EvaluateChartLegendTitleTitleSeparatorExpression(this, base.m_chart.Name), context.ReportRuntime);
		}
	}
}
