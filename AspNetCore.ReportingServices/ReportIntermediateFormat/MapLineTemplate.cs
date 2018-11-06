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
	internal sealed class MapLineTemplate : MapSpatialElementTemplate, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapLineTemplate.GetDeclaration();

		private ExpressionInfo m_width;

		private ExpressionInfo m_labelPlacement;

		internal ExpressionInfo Width
		{
			get
			{
				return this.m_width;
			}
			set
			{
				this.m_width = value;
			}
		}

		internal ExpressionInfo LabelPlacement
		{
			get
			{
				return this.m_labelPlacement;
			}
			set
			{
				this.m_labelPlacement = value;
			}
		}

		internal new MapLineTemplateExprHost ExprHost
		{
			get
			{
				return (MapLineTemplateExprHost)base.m_exprHost;
			}
		}

		internal MapLineTemplate()
		{
		}

		internal MapLineTemplate(MapLineLayer mapLineLayer, Map map, int id)
			: base(mapLineLayer, map, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapLineTemplateStart();
			base.Initialize(context);
			if (this.m_width != null)
			{
				this.m_width.Initialize("Width", context);
				context.ExprHostBuilder.MapLineTemplateWidth(this.m_width);
			}
			if (this.m_labelPlacement != null)
			{
				this.m_labelPlacement.Initialize("LabelPlacement", context);
				context.ExprHostBuilder.MapLineTemplateLabelPlacement(this.m_labelPlacement);
			}
			context.ExprHostBuilder.MapLineTemplateEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapLineTemplate mapLineTemplate = (MapLineTemplate)base.PublishClone(context);
			if (this.m_width != null)
			{
				mapLineTemplate.m_width = (ExpressionInfo)this.m_width.PublishClone(context);
			}
			if (this.m_labelPlacement != null)
			{
				mapLineTemplate.m_labelPlacement = (ExpressionInfo)this.m_labelPlacement.PublishClone(context);
			}
			return mapLineTemplate;
		}

		internal void SetExprHost(MapLineTemplateExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Width, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LabelPlacement, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLineTemplate, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialElementTemplate, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapLineTemplate.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Width:
					writer.Write(this.m_width);
					break;
				case MemberName.LabelPlacement:
					writer.Write(this.m_labelPlacement);
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
			reader.RegisterDeclaration(MapLineTemplate.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Width:
					this.m_width = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LabelPlacement:
					this.m_labelPlacement = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLineTemplate;
		}

		internal string EvaluateWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLineTemplateWidthExpression(this, base.m_map.Name);
		}

		internal MapLineLabelPlacement EvaluateLabelPlacement(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.InstancePath, reportScopeInstance);
			return EnumTranslator.TranslateMapLineLabelPlacement(context.ReportRuntime.EvaluateMapLineTemplateLabelPlacementExpression(this, base.m_map.Name), context.ReportRuntime);
		}
	}
}
