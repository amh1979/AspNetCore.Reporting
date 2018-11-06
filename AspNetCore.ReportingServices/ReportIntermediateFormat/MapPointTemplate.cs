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
	internal class MapPointTemplate : MapSpatialElementTemplate, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapPointTemplate.GetDeclaration();

		private ExpressionInfo m_size;

		private ExpressionInfo m_labelPlacement;

		internal ExpressionInfo Size
		{
			get
			{
				return this.m_size;
			}
			set
			{
				this.m_size = value;
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

		internal new MapPointTemplateExprHost ExprHost
		{
			get
			{
				return (MapPointTemplateExprHost)base.m_exprHost;
			}
		}

		internal MapPointTemplate()
		{
		}

		internal MapPointTemplate(MapVectorLayer mapVectorLayer, Map map, int id)
			: base(mapVectorLayer, map, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			base.Initialize(context);
			if (this.m_size != null)
			{
				this.m_size.Initialize("Size", context);
				context.ExprHostBuilder.MapPointTemplateSize(this.m_size);
			}
			if (this.m_labelPlacement != null)
			{
				this.m_labelPlacement.Initialize("LabelPlacement", context);
				context.ExprHostBuilder.MapPointTemplateLabelPlacement(this.m_labelPlacement);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapPointTemplate mapPointTemplate = (MapPointTemplate)base.PublishClone(context);
			if (this.m_size != null)
			{
				mapPointTemplate.m_size = (ExpressionInfo)this.m_size.PublishClone(context);
			}
			if (this.m_labelPlacement != null)
			{
				mapPointTemplate.m_labelPlacement = (ExpressionInfo)this.m_labelPlacement.PublishClone(context);
			}
			return mapPointTemplate;
		}

		internal virtual void SetExprHost(MapPointTemplateExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Size, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LabelPlacement, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPointTemplate, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialElementTemplate, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapPointTemplate.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Size:
					writer.Write(this.m_size);
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
			reader.RegisterDeclaration(MapPointTemplate.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Size:
					this.m_size = (ExpressionInfo)reader.ReadRIFObject();
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
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPointTemplate;
		}

		internal string EvaluateSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapPointTemplateSizeExpression(this, base.m_map.Name);
		}

		internal MapPointLabelPlacement EvaluateLabelPlacement(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.InstancePath, reportScopeInstance);
			return EnumTranslator.TranslateMapPointLabelPlacement(context.ReportRuntime.EvaluateMapPointTemplateLabelPlacementExpression(this, base.m_map.Name), context.ReportRuntime);
		}
	}
}
