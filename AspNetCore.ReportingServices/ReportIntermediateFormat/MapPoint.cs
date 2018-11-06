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
	internal sealed class MapPoint : MapSpatialElement, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapPoint.GetDeclaration();

		private ExpressionInfo m_useCustomPointTemplate;

		private MapPointTemplate m_mapPointTemplate;

		internal ExpressionInfo UseCustomPointTemplate
		{
			get
			{
				return this.m_useCustomPointTemplate;
			}
			set
			{
				this.m_useCustomPointTemplate = value;
			}
		}

		internal MapPointTemplate MapPointTemplate
		{
			get
			{
				return this.m_mapPointTemplate;
			}
			set
			{
				this.m_mapPointTemplate = value;
			}
		}

		internal new MapPointExprHost ExprHost
		{
			get
			{
				return (MapPointExprHost)base.m_exprHost;
			}
		}

		internal MapPoint()
		{
		}

		internal MapPoint(MapPointLayer mapPointLayer, Map map)
			: base(mapPointLayer, map)
		{
		}

		internal override void Initialize(InitializationContext context, int index)
		{
			context.ExprHostBuilder.MapPointStart(index.ToString(CultureInfo.InvariantCulture.NumberFormat));
			base.Initialize(context, index);
			if (this.m_useCustomPointTemplate != null)
			{
				this.m_useCustomPointTemplate.Initialize("UseCustomPointTemplate", context);
				context.ExprHostBuilder.MapPointUseCustomPointTemplate(this.m_useCustomPointTemplate);
			}
			if (this.m_mapPointTemplate != null)
			{
				this.m_mapPointTemplate.Initialize(context);
			}
			base.m_exprHostID = context.ExprHostBuilder.MapPointEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapPoint mapPoint = (MapPoint)base.PublishClone(context);
			if (this.m_useCustomPointTemplate != null)
			{
				mapPoint.m_useCustomPointTemplate = (ExpressionInfo)this.m_useCustomPointTemplate.PublishClone(context);
			}
			if (this.m_mapPointTemplate != null)
			{
				mapPoint.m_mapPointTemplate = (MapPointTemplate)this.m_mapPointTemplate.PublishClone(context);
			}
			return mapPoint;
		}

		internal void SetExprHost(MapPointExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			if (this.m_mapPointTemplate != null && this.ExprHost.MapPointTemplateHost != null)
			{
				this.m_mapPointTemplate.SetExprHost(this.ExprHost.MapPointTemplateHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.UseCustomPointTemplate, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MapPointTemplate, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPointTemplate));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPoint, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialElement, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapPoint.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.UseCustomPointTemplate:
					writer.Write(this.m_useCustomPointTemplate);
					break;
				case MemberName.MapPointTemplate:
					writer.Write(this.m_mapPointTemplate);
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
			reader.RegisterDeclaration(MapPoint.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.UseCustomPointTemplate:
					this.m_useCustomPointTemplate = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MapPointTemplate:
					this.m_mapPointTemplate = (MapPointTemplate)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPoint;
		}

		internal bool EvaluateUseCustomPointTemplate(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(base.InstancePath, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapPointUseCustomPointTemplateExpression(this, base.m_map.Name);
		}
	}
}
