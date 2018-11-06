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
	internal class MapColorRule : MapAppearanceRule, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapColorRule.GetDeclaration();

		private ExpressionInfo m_showInColorScale;

		internal ExpressionInfo ShowInColorScale
		{
			get
			{
				return this.m_showInColorScale;
			}
			set
			{
				this.m_showInColorScale = value;
			}
		}

		internal new MapColorRuleExprHost ExprHost
		{
			get
			{
				return (MapColorRuleExprHost)base.m_exprHost;
			}
		}

		internal MapColorRule()
		{
		}

		internal MapColorRule(MapVectorLayer mapVectorLayer, Map map)
			: base(mapVectorLayer, map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			base.Initialize(context);
			if (this.m_showInColorScale != null)
			{
				this.m_showInColorScale.Initialize("ShowInColorScale", context);
				context.ExprHostBuilder.MapColorRuleShowInColorScale(this.m_showInColorScale);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapColorRule mapColorRule = (MapColorRule)base.PublishClone(context);
			if (this.m_showInColorScale != null)
			{
				mapColorRule.m_showInColorScale = (ExpressionInfo)this.m_showInColorScale.PublishClone(context);
			}
			return mapColorRule;
		}

		internal override void SetExprHost(MapAppearanceRuleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.ShowInColorScale, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorRule, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapAppearanceRule, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapColorRule.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.ShowInColorScale)
				{
					writer.Write(this.m_showInColorScale);
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
			reader.RegisterDeclaration(MapColorRule.m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.ShowInColorScale)
				{
					this.m_showInColorScale = (ExpressionInfo)reader.ReadRIFObject();
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorRule;
		}

		internal bool EvaluateShowInColorScale(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapColorRuleShowInColorScaleExpression(this, base.m_map.Name);
		}
	}
}
