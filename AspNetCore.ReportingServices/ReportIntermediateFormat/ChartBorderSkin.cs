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
	internal sealed class ChartBorderSkin : ChartStyleContainer, IPersistable
	{
		private ExpressionInfo m_borderSkinType;

		[NonSerialized]
		private static readonly Declaration m_Declaration = ChartBorderSkin.GetDeclaration();

		[NonSerialized]
		private ChartBorderSkinExprHost m_exprHost;

		internal ChartBorderSkinExprHost ExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal ExpressionInfo BorderSkinType
		{
			get
			{
				return this.m_borderSkinType;
			}
			set
			{
				this.m_borderSkinType = value;
			}
		}

		internal ChartBorderSkin()
		{
		}

		internal ChartBorderSkin(Chart chart)
			: base(chart)
		{
		}

		internal void SetExprHost(ChartBorderSkinExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			this.m_exprHost = exprHost;
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ChartBorderSkinStart();
			base.Initialize(context);
			if (this.m_borderSkinType != null)
			{
				this.m_borderSkinType.Initialize("ChartBorderSkinType", context);
				context.ExprHostBuilder.ChartBorderSkinBorderSkinType(this.m_borderSkinType);
			}
			context.ExprHostBuilder.ChartBorderSkinEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ChartBorderSkin chartBorderSkin = (ChartBorderSkin)base.PublishClone(context);
			if (this.m_borderSkinType != null)
			{
				chartBorderSkin.m_borderSkinType = (ExpressionInfo)this.m_borderSkinType.PublishClone(context);
			}
			return chartBorderSkin;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.BorderSkinType, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartBorderSkin, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartStyleContainer, list);
		}

		internal ChartBorderSkinType EvaluateBorderSkinType(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_chart, reportScopeInstance);
			return EnumTranslator.TranslateChartBorderSkinType(context.ReportRuntime.EvaluateChartBorderSkinBorderSkinTypeExpression(this, base.m_chart.Name), context.ReportRuntime);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(ChartBorderSkin.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.BorderSkinType)
				{
					writer.Write(this.m_borderSkinType);
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
			reader.RegisterDeclaration(ChartBorderSkin.m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.BorderSkinType)
				{
					this.m_borderSkinType = (ExpressionInfo)reader.ReadRIFObject();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ChartBorderSkin;
		}
	}
}
