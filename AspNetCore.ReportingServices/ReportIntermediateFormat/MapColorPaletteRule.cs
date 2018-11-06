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
	internal sealed class MapColorPaletteRule : MapColorRule, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapColorPaletteRule.GetDeclaration();

		private ExpressionInfo m_palette;

		internal ExpressionInfo Palette
		{
			get
			{
				return this.m_palette;
			}
			set
			{
				this.m_palette = value;
			}
		}

		internal new MapColorPaletteRuleExprHost ExprHost
		{
			get
			{
				return (MapColorPaletteRuleExprHost)base.m_exprHost;
			}
		}

		internal MapColorPaletteRule()
		{
		}

		internal MapColorPaletteRule(MapVectorLayer mapVectorLayer, Map map)
			: base(mapVectorLayer, map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapColorPaletteRuleStart();
			base.Initialize(context);
			if (this.m_palette != null)
			{
				this.m_palette.Initialize("Palette", context);
				context.ExprHostBuilder.MapColorPaletteRulePalette(this.m_palette);
			}
			context.ExprHostBuilder.MapColorPaletteRuleEnd();
		}

		internal override void InitializeMapMember(InitializationContext context)
		{
			context.ExprHostBuilder.MapColorPaletteRuleStart();
			base.InitializeMapMember(context);
			context.ExprHostBuilder.MapColorPaletteRuleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapColorPaletteRule mapColorPaletteRule = (MapColorPaletteRule)base.PublishClone(context);
			if (this.m_palette != null)
			{
				mapColorPaletteRule.m_palette = (ExpressionInfo)this.m_palette.PublishClone(context);
			}
			return mapColorPaletteRule;
		}

		internal override void SetExprHost(MapAppearanceRuleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Palette, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorPaletteRule, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorRule, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapColorPaletteRule.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.Palette)
				{
					writer.Write(this.m_palette);
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
			reader.RegisterDeclaration(MapColorPaletteRule.m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.Palette)
				{
					this.m_palette = (ExpressionInfo)reader.ReadRIFObject();
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorPaletteRule;
		}

		internal MapPalette EvaluatePalette(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.m_map, reportScopeInstance);
			return EnumTranslator.TranslateMapPalette(context.ReportRuntime.EvaluateMapColorPaletteRulePaletteExpression(this, base.m_map.Name), context.ReportRuntime);
		}
	}
}
