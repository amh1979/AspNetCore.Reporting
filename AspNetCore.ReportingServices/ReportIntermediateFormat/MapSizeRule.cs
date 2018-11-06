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
	internal sealed class MapSizeRule : MapAppearanceRule, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapSizeRule.GetDeclaration();

		private ExpressionInfo m_startSize;

		private ExpressionInfo m_endSize;

		internal ExpressionInfo StartSize
		{
			get
			{
				return this.m_startSize;
			}
			set
			{
				this.m_startSize = value;
			}
		}

		internal ExpressionInfo EndSize
		{
			get
			{
				return this.m_endSize;
			}
			set
			{
				this.m_endSize = value;
			}
		}

		internal new MapSizeRuleExprHost ExprHost
		{
			get
			{
				return (MapSizeRuleExprHost)base.m_exprHost;
			}
		}

		internal MapSizeRule()
		{
		}

		internal MapSizeRule(MapVectorLayer mapVectorLayer, Map map)
			: base(mapVectorLayer, map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapSizeRuleStart();
			base.Initialize(context);
			if (this.m_startSize != null)
			{
				this.m_startSize.Initialize("StartSize", context);
				context.ExprHostBuilder.MapSizeRuleStartSize(this.m_startSize);
			}
			if (this.m_endSize != null)
			{
				this.m_endSize.Initialize("EndSize", context);
				context.ExprHostBuilder.MapSizeRuleEndSize(this.m_endSize);
			}
			context.ExprHostBuilder.MapSizeRuleEnd();
		}

		internal override void InitializeMapMember(InitializationContext context)
		{
			context.ExprHostBuilder.MapSizeRuleStart();
			base.InitializeMapMember(context);
			context.ExprHostBuilder.MapSizeRuleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapSizeRule mapSizeRule = (MapSizeRule)base.PublishClone(context);
			if (this.m_startSize != null)
			{
				mapSizeRule.m_startSize = (ExpressionInfo)this.m_startSize.PublishClone(context);
			}
			if (this.m_endSize != null)
			{
				mapSizeRule.m_endSize = (ExpressionInfo)this.m_endSize.PublishClone(context);
			}
			return mapSizeRule;
		}

		internal override void SetExprHost(MapAppearanceRuleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.StartSize, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.EndSize, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSizeRule, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapAppearanceRule, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapSizeRule.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.StartSize:
					writer.Write(this.m_startSize);
					break;
				case MemberName.EndSize:
					writer.Write(this.m_endSize);
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
			reader.RegisterDeclaration(MapSizeRule.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.StartSize:
					this.m_startSize = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.EndSize:
					this.m_endSize = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSizeRule;
		}

		internal string EvaluateStartSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSizeRuleStartSizeExpression(this, base.m_map.Name);
		}

		internal string EvaluateEndSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSizeRuleEndSizeExpression(this, base.m_map.Name);
		}
	}
}
