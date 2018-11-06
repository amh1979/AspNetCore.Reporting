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
	internal sealed class ChartLegendColumnHeader : ChartStyleContainer, IPersistable
	{
		private ExpressionInfo m_value;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartLegendColumnHeader.GetDeclaration();

		[NonSerialized]
		private ChartLegendColumnHeaderExprHost m_exprHost;

		internal ChartLegendColumnHeaderExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal ExpressionInfo Value
		{
			get
			{
				return this.m_value;
			}
			set
			{
				this.m_value = value;
			}
		}

		internal ChartLegendColumnHeader()
		{
		}

		internal ChartLegendColumnHeader(Chart chart)
			: base(chart)
		{
		}

		internal void SetExprHost(ChartLegendColumnHeaderExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			this.m_exprHost = exprHost;
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartLegendColumnHeaderStart();
			base.Initialize(context);
			if (this.m_value != null)
			{
				this.m_value.Initialize("Value", context);
				context.ExprHostBuilder.ChartLegendColumnHeaderValue(this.m_value);
			}
			context.ExprHostBuilder.ChartLegendColumnHeaderEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartLegendColumnHeader chartLegendColumnHeader = (ChartLegendColumnHeader)base.PublishClone(context);
			if (this.m_value != null)
			{
				chartLegendColumnHeader.m_value = (ExpressionInfo)this.m_value.PublishClone(context);
			}
			return chartLegendColumnHeader;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Value, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendColumnHeader, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		internal string EvaluateValue(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return context.ReportRuntime.EvaluateChartLegendColumnHeaderValueExpression(this, base.m_chart.Name);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ChartLegendColumnHeader.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.Value)
				{
					writer.Write(this.m_value);
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
			reader.RegisterDeclaration(ChartLegendColumnHeader.m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.Value)
				{
					this.m_value = (ExpressionInfo)reader.ReadRIFObject();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartLegendColumnHeader;
		}
	}
}
