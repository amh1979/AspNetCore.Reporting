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
	internal sealed class MapMarkerTemplate : MapPointTemplate, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapMarkerTemplate.GetDeclaration();

		private MapMarker m_mapMarker;

		internal MapMarker MapMarker
		{
			get
			{
				return this.m_mapMarker;
			}
			set
			{
				this.m_mapMarker = value;
			}
		}

		internal new MapMarkerTemplateExprHost ExprHost
		{
			get
			{
				return (MapMarkerTemplateExprHost)base.m_exprHost;
			}
		}

		internal MapMarkerTemplate()
		{
		}

		internal MapMarkerTemplate(MapVectorLayer mapVectorLayer, Map map, int id)
			: base(mapVectorLayer, map, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapMarkerTemplateStart();
			base.Initialize(context);
			if (this.m_mapMarker != null)
			{
				this.m_mapMarker.Initialize(context);
			}
			context.ExprHostBuilder.MapMarkerTemplateEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapMarkerTemplate mapMarkerTemplate = (MapMarkerTemplate)base.PublishClone(context);
			if (this.m_mapMarker != null)
			{
				mapMarkerTemplate.m_mapMarker = (MapMarker)this.m_mapMarker.PublishClone(context);
			}
			return mapMarkerTemplate;
		}

		internal override void SetExprHost(MapPointTemplateExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			if (this.m_mapMarker != null && this.ExprHost.MapMarkerHost != null)
			{
				this.m_mapMarker.SetExprHost(this.ExprHost.MapMarkerHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapMarker, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMarker));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMarkerTemplate, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPointTemplate, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapMarkerTemplate.m_Declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				if (memberName == MemberName.MapMarker)
				{
					writer.Write(this.m_mapMarker);
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
			reader.RegisterDeclaration(MapMarkerTemplate.m_Declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				if (memberName == MemberName.MapMarker)
				{
					this.m_mapMarker = (MapMarker)reader.ReadRIFObject();
				}
				else
				{
					Global.Tracer.Assert(false);
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapMarkerTemplate;
		}
	}
}
