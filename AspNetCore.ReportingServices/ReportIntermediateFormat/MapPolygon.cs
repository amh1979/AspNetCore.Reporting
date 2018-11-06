using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class MapPolygon : MapSpatialElement, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapPolygon.GetDeclaration();

		private ExpressionInfo m_useCustomPolygonTemplate;

		private MapPolygonTemplate m_mapPolygonTemplate;

		private ExpressionInfo m_useCustomCenterPointTemplate;

		private MapPointTemplate m_mapCenterPointTemplate;

		internal ExpressionInfo UseCustomPolygonTemplate
		{
			get
			{
				return this.m_useCustomPolygonTemplate;
			}
			set
			{
				this.m_useCustomPolygonTemplate = value;
			}
		}

		internal MapPolygonTemplate MapPolygonTemplate
		{
			get
			{
				return this.m_mapPolygonTemplate;
			}
			set
			{
				this.m_mapPolygonTemplate = value;
			}
		}

		internal ExpressionInfo UseCustomCenterPointTemplate
		{
			get
			{
				return this.m_useCustomCenterPointTemplate;
			}
			set
			{
				this.m_useCustomCenterPointTemplate = value;
			}
		}

		internal MapPointTemplate MapCenterPointTemplate
		{
			get
			{
				return this.m_mapCenterPointTemplate;
			}
			set
			{
				this.m_mapCenterPointTemplate = value;
			}
		}

		internal new MapPolygonExprHost ExprHost
		{
			get
			{
				return (MapPolygonExprHost)base.m_exprHost;
			}
		}

		internal MapPolygon()
		{
		}

		internal MapPolygon(MapPolygonLayer mapPolygonLayer, Map map)
			: base(mapPolygonLayer, map)
		{
		}

		internal override void Initialize(InitializationContext context, int index)
		{
			context.ExprHostBuilder.MapPolygonStart(index.ToString(CultureInfo.InvariantCulture.NumberFormat));
			base.Initialize(context, index);
			if (this.m_useCustomPolygonTemplate != null)
			{
				this.m_useCustomPolygonTemplate.Initialize("UseCustomPolygonTemplate", context);
				context.ExprHostBuilder.MapPolygonUseCustomPolygonTemplate(this.m_useCustomPolygonTemplate);
			}
			if (this.m_mapPolygonTemplate != null)
			{
				this.m_mapPolygonTemplate.Initialize(context);
			}
			if (this.m_useCustomCenterPointTemplate != null)
			{
				this.m_useCustomCenterPointTemplate.Initialize("UseCustomPointTemplate", context);
				context.ExprHostBuilder.MapPolygonUseCustomCenterPointTemplate(this.m_useCustomCenterPointTemplate);
			}
			if (this.m_mapCenterPointTemplate != null)
			{
				this.m_mapCenterPointTemplate.Initialize(context);
			}
			base.m_exprHostID = context.ExprHostBuilder.MapPolygonEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapPolygon mapPolygon = (MapPolygon)base.PublishClone(context);
			if (this.m_useCustomPolygonTemplate != null)
			{
				mapPolygon.m_useCustomPolygonTemplate = (ExpressionInfo)this.m_useCustomPolygonTemplate.PublishClone(context);
			}
			if (this.m_mapPolygonTemplate != null)
			{
				mapPolygon.m_mapPolygonTemplate = (MapPolygonTemplate)this.m_mapPolygonTemplate.PublishClone(context);
			}
			if (this.m_useCustomCenterPointTemplate != null)
			{
				mapPolygon.m_useCustomCenterPointTemplate = (ExpressionInfo)this.m_useCustomCenterPointTemplate.PublishClone(context);
			}
			if (this.m_mapCenterPointTemplate != null)
			{
				mapPolygon.m_mapCenterPointTemplate = (MapPointTemplate)this.m_mapCenterPointTemplate.PublishClone(context);
			}
			return mapPolygon;
		}

		internal void SetExprHost(MapPolygonExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			if (this.m_mapPolygonTemplate != null && this.ExprHost.MapPolygonTemplateHost != null)
			{
				this.m_mapPolygonTemplate.SetExprHost(this.ExprHost.MapPolygonTemplateHost, reportObjectModel);
			}
			if (this.m_mapCenterPointTemplate != null && this.ExprHost.MapPointTemplateHost != null)
			{
				this.m_mapCenterPointTemplate.SetExprHost(this.ExprHost.MapPointTemplateHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.UseCustomPolygonTemplate, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MapPolygonTemplate, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPolygonTemplate));
			list.Add(new MemberInfo(MemberName.UseCustomPointTemplate, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MapPointTemplate, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPointTemplate));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPolygon, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialElement, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapPolygon.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.UseCustomPolygonTemplate:
					writer.Write(this.m_useCustomPolygonTemplate);
					break;
				case MemberName.MapPolygonTemplate:
					writer.Write(this.m_mapPolygonTemplate);
					break;
				case MemberName.UseCustomPointTemplate:
					writer.Write(this.m_useCustomCenterPointTemplate);
					break;
				case MemberName.MapPointTemplate:
					writer.Write(this.m_mapCenterPointTemplate);
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
			reader.RegisterDeclaration(MapPolygon.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.UseCustomPolygonTemplate:
					this.m_useCustomPolygonTemplate = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MapPolygonTemplate:
					this.m_mapPolygonTemplate = (MapPolygonTemplate)reader.ReadRIFObject();
					break;
				case MemberName.UseCustomPointTemplate:
					this.m_useCustomCenterPointTemplate = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MapPointTemplate:
					this.m_mapCenterPointTemplate = (MapPointTemplate)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPolygon;
		}

		internal bool EvaluateUseCustomPolygonTemplate(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapPolygonUseCustomPolygonTemplateExpression(this, base.m_map.Name);
		}

		internal bool EvaluateUseCustomCenterPointTemplate(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapPolygonUseCustomPointTemplateExpression(this, base.m_map.Name);
		}
	}
}
