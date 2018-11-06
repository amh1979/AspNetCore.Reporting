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
	internal class ChartTitleBase : ChartStyleContainer, IPersistable
	{
		private ExpressionInfo m_caption;

		[NonSerialized]
		private Formatter m_formatter;

		[NonSerialized]
		private ChartTitleBaseExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartTitleBase.GetDeclaration();

		internal ExpressionInfo Caption
		{
			get
			{
				return this.m_caption;
			}
			set
			{
				this.m_caption = value;
			}
		}

		internal ChartTitleBaseExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal ChartTitleBase()
		{
		}

		internal ChartTitleBase(Chart chart)
			: base(chart)
		{
			base.m_chart = chart;
		}

		internal override void SetExprHost(StyleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			this.m_exprHost = (ChartTitleBaseExprHost)exprHost;
		}

		internal override void Initialize(InitializationContext context)
		{
			base.Initialize(context);
			if (this.m_caption != null)
			{
				this.m_caption.Initialize("Caption", context);
				context.ExprHostBuilder.ChartCaption(this.m_caption);
			}
		}

		internal string EvaluateCaption(IReportScopeInstance instance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, instance);
			AspNetCore.ReportingServices.RdlExpressions.VariantResult variantResult = context.ReportRuntime.EvaluateChartTitleCaptionExpression(this, base.Name, "Caption");
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

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartTitleBase chartTitleBase = (ChartTitleBase)base.PublishClone(context);
			if (this.m_caption != null)
			{
				chartTitleBase.m_caption = (ExpressionInfo)this.m_caption.PublishClone(context);
			}
			return chartTitleBase;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Caption, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartTitleBase, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ChartTitleBase.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.Caption)
				{
					writer.Write(this.m_caption);
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
			reader.RegisterDeclaration(ChartTitleBase.m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.Caption)
				{
					this.m_caption = (ExpressionInfo)reader.ReadRIFObject();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartTitleBase;
		}
	}
}
