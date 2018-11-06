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
	internal sealed class MapColorRangeRule : MapColorRule, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapColorRangeRule.GetDeclaration();

		private ExpressionInfo m_startColor;

		private ExpressionInfo m_middleColor;

		private ExpressionInfo m_endColor;

		internal ExpressionInfo StartColor
		{
			get
			{
				return this.m_startColor;
			}
			set
			{
				this.m_startColor = value;
			}
		}

		internal ExpressionInfo MiddleColor
		{
			get
			{
				return this.m_middleColor;
			}
			set
			{
				this.m_middleColor = value;
			}
		}

		internal ExpressionInfo EndColor
		{
			get
			{
				return this.m_endColor;
			}
			set
			{
				this.m_endColor = value;
			}
		}

		internal new MapColorRangeRuleExprHost ExprHost
		{
			get
			{
				return (MapColorRangeRuleExprHost)base.m_exprHost;
			}
		}

		internal MapColorRangeRule()
		{
		}

		internal MapColorRangeRule(MapVectorLayer mapVectorLayer, Map map)
			: base(mapVectorLayer, map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapColorRangeRuleStart();
			base.Initialize(context);
			if (this.m_startColor != null)
			{
				this.m_startColor.Initialize("StartColor", context);
				context.ExprHostBuilder.MapColorRangeRuleStartColor(this.m_startColor);
			}
			if (this.m_middleColor != null)
			{
				this.m_middleColor.Initialize("MiddleColor", context);
				context.ExprHostBuilder.MapColorRangeRuleMiddleColor(this.m_middleColor);
			}
			if (this.m_endColor != null)
			{
				this.m_endColor.Initialize("EndColor", context);
				context.ExprHostBuilder.MapColorRangeRuleEndColor(this.m_endColor);
			}
			context.ExprHostBuilder.MapColorRangeRuleEnd();
		}

		internal override void InitializeMapMember(InitializationContext context)
		{
			context.ExprHostBuilder.MapColorRangeRuleStart();
			base.InitializeMapMember(context);
			context.ExprHostBuilder.MapColorRangeRuleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapColorRangeRule mapColorRangeRule = (MapColorRangeRule)base.PublishClone(context);
			if (this.m_startColor != null)
			{
				mapColorRangeRule.m_startColor = (ExpressionInfo)this.m_startColor.PublishClone(context);
			}
			if (this.m_middleColor != null)
			{
				mapColorRangeRule.m_middleColor = (ExpressionInfo)this.m_middleColor.PublishClone(context);
			}
			if (this.m_endColor != null)
			{
				mapColorRangeRule.m_endColor = (ExpressionInfo)this.m_endColor.PublishClone(context);
			}
			return mapColorRangeRule;
		}

		internal override void SetExprHost(MapAppearanceRuleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.StartColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MiddleColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.EndColor, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorRangeRule, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorRule, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapColorRangeRule.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.StartColor:
					writer.Write(this.m_startColor);
					break;
				case MemberName.MiddleColor:
					writer.Write(this.m_middleColor);
					break;
				case MemberName.EndColor:
					writer.Write(this.m_endColor);
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
			reader.RegisterDeclaration(MapColorRangeRule.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.StartColor:
					this.m_startColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MiddleColor:
					this.m_middleColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.EndColor:
					this.m_endColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorRangeRule;
		}

		internal string EvaluateStartColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapColorRangeRuleStartColorExpression(this, base.m_map.Name);
		}

		internal string EvaluateMiddleColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapColorRangeRuleMiddleColorExpression(this, base.m_map.Name);
		}

		internal string EvaluateEndColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapColorRangeRuleEndColorExpression(this, base.m_map.Name);
		}
	}
}
