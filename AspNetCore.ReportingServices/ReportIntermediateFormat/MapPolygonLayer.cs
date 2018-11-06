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
	internal sealed class MapPolygonLayer : MapVectorLayer, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapPolygonLayer.GetDeclaration();

		private MapPolygonTemplate m_mapPolygonTemplate;

		private MapPolygonRules m_mapPolygonRules;

		private MapPointTemplate m_mapCenterPointTemplate;

		private MapPointRules m_mapCenterPointRules;

		private List<MapPolygon> m_mapPolygons;

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

		internal MapPolygonRules MapPolygonRules
		{
			get
			{
				return this.m_mapPolygonRules;
			}
			set
			{
				this.m_mapPolygonRules = value;
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

		internal MapPointRules MapCenterPointRules
		{
			get
			{
				return this.m_mapCenterPointRules;
			}
			set
			{
				this.m_mapCenterPointRules = value;
			}
		}

		internal List<MapPolygon> MapPolygons
		{
			get
			{
				return this.m_mapPolygons;
			}
			set
			{
				this.m_mapPolygons = value;
			}
		}

		protected override bool Embedded
		{
			get
			{
				return this.MapPolygons != null;
			}
		}

		internal new MapPolygonLayerExprHost ExprHost
		{
			get
			{
				return (MapPolygonLayerExprHost)base.m_exprHost;
			}
		}

		internal new MapPolygonLayerExprHost ExprHostMapMember
		{
			get
			{
				return (MapPolygonLayerExprHost)base.m_exprHostMapMember;
			}
		}

		internal MapPolygonLayer()
		{
		}

		internal MapPolygonLayer(int ID, Map map)
			: base(ID, map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapPolygonLayerStart(base.Name);
			base.Initialize(context);
			if (this.m_mapPolygonRules != null)
			{
				this.m_mapPolygonRules.Initialize(context);
			}
			if (this.m_mapCenterPointRules != null)
			{
				this.m_mapCenterPointRules.Initialize(context);
			}
			if (base.MapDataRegionName == null)
			{
				if (this.m_mapPolygonTemplate != null)
				{
					this.m_mapPolygonTemplate.Initialize(context);
				}
				if (this.m_mapCenterPointTemplate != null)
				{
					this.m_mapCenterPointTemplate.Initialize(context);
				}
				if (this.m_mapPolygons != null)
				{
					for (int i = 0; i < this.m_mapPolygons.Count; i++)
					{
						this.m_mapPolygons[i].Initialize(context, i);
					}
				}
			}
			base.m_exprHostID = context.ExprHostBuilder.MapPolygonLayerEnd();
		}

		internal override void InitializeMapMember(InitializationContext context)
		{
			context.ExprHostBuilder.MapPolygonLayerStart(base.Name);
			base.InitializeMapMember(context);
			if (this.m_mapPolygonRules != null)
			{
				this.m_mapPolygonRules.InitializeMapMember(context);
			}
			if (this.m_mapCenterPointRules != null)
			{
				this.m_mapCenterPointRules.InitializeMapMember(context);
			}
			if (base.MapDataRegionName != null)
			{
				if (this.m_mapPolygonTemplate != null)
				{
					this.m_mapPolygonTemplate.Initialize(context);
				}
				if (this.m_mapCenterPointTemplate != null)
				{
					this.m_mapCenterPointTemplate.Initialize(context);
				}
				if (this.m_mapPolygons != null)
				{
					for (int i = 0; i < this.m_mapPolygons.Count; i++)
					{
						this.m_mapPolygons[i].Initialize(context, i);
					}
				}
			}
			base.m_exprHostMapMemberID = context.ExprHostBuilder.MapPolygonLayerEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapPolygonLayer mapPolygonLayer = (MapPolygonLayer)(context.CurrentMapVectorLayerClone = (MapPolygonLayer)base.PublishClone(context));
			if (this.m_mapPolygonTemplate != null)
			{
				mapPolygonLayer.m_mapPolygonTemplate = (MapPolygonTemplate)this.m_mapPolygonTemplate.PublishClone(context);
			}
			if (this.m_mapPolygonRules != null)
			{
				mapPolygonLayer.m_mapPolygonRules = (MapPolygonRules)this.m_mapPolygonRules.PublishClone(context);
			}
			if (this.m_mapCenterPointTemplate != null)
			{
				mapPolygonLayer.m_mapCenterPointTemplate = (MapPointTemplate)this.m_mapCenterPointTemplate.PublishClone(context);
			}
			if (this.m_mapCenterPointRules != null)
			{
				mapPolygonLayer.m_mapCenterPointRules = (MapPointRules)this.m_mapCenterPointRules.PublishClone(context);
			}
			if (this.m_mapPolygons != null)
			{
				mapPolygonLayer.m_mapPolygons = new List<MapPolygon>(this.m_mapPolygons.Count);
				{
					foreach (MapPolygon mapPolygon in this.m_mapPolygons)
					{
						mapPolygonLayer.m_mapPolygons.Add((MapPolygon)mapPolygon.PublishClone(context));
					}
					return mapPolygonLayer;
				}
			}
			return mapPolygonLayer;
		}

		internal override void SetExprHost(MapLayerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			if (this.m_mapPolygonRules != null && this.ExprHost.MapPolygonRulesHost != null)
			{
				this.m_mapPolygonRules.SetExprHost(this.ExprHost.MapPolygonRulesHost, reportObjectModel);
			}
			if (this.m_mapCenterPointRules != null && this.ExprHost.MapPointRulesHost != null)
			{
				this.m_mapCenterPointRules.SetExprHost(this.ExprHost.MapPointRulesHost, reportObjectModel);
			}
			if (base.MapDataRegionName == null)
			{
				if (this.m_mapPolygonTemplate != null && this.ExprHost.MapPolygonTemplateHost != null)
				{
					this.m_mapPolygonTemplate.SetExprHost(this.ExprHost.MapPolygonTemplateHost, reportObjectModel);
				}
				if (this.m_mapCenterPointTemplate != null && this.ExprHost.MapPointTemplateHost != null)
				{
					this.m_mapCenterPointTemplate.SetExprHost(this.ExprHost.MapPointTemplateHost, reportObjectModel);
				}
				IList<MapPolygonExprHost> mapPolygonsHostsRemotable = this.ExprHost.MapPolygonsHostsRemotable;
				if (this.m_mapPolygons != null && mapPolygonsHostsRemotable != null)
				{
					for (int i = 0; i < this.m_mapPolygons.Count; i++)
					{
						MapPolygon mapPolygon = this.m_mapPolygons[i];
						if (mapPolygon != null && mapPolygon.ExpressionHostID > -1)
						{
							mapPolygon.SetExprHost(mapPolygonsHostsRemotable[mapPolygon.ExpressionHostID], reportObjectModel);
						}
					}
				}
			}
		}

		internal override void SetExprHostMapMember(MapVectorLayerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHostMapMember(exprHost, reportObjectModel);
			if (this.m_mapPolygonRules != null && this.ExprHostMapMember.MapPolygonRulesHost != null)
			{
				this.m_mapPolygonRules.SetExprHostMapMember(this.ExprHostMapMember.MapPolygonRulesHost, reportObjectModel);
			}
			if (this.m_mapCenterPointRules != null && this.ExprHostMapMember.MapPointRulesHost != null)
			{
				this.m_mapCenterPointRules.SetExprHostMapMember(this.ExprHostMapMember.MapPointRulesHost, reportObjectModel);
			}
			if (base.MapDataRegionName != null)
			{
				if (this.m_mapPolygonTemplate != null && this.ExprHostMapMember.MapPolygonTemplateHost != null)
				{
					this.m_mapPolygonTemplate.SetExprHost(this.ExprHostMapMember.MapPolygonTemplateHost, reportObjectModel);
				}
				if (this.m_mapCenterPointTemplate != null && this.ExprHostMapMember.MapPointTemplateHost != null)
				{
					this.m_mapCenterPointTemplate.SetExprHost(this.ExprHostMapMember.MapPointTemplateHost, reportObjectModel);
				}
				IList<MapPolygonExprHost> mapPolygonsHostsRemotable = this.ExprHostMapMember.MapPolygonsHostsRemotable;
				if (this.m_mapPolygons != null && mapPolygonsHostsRemotable != null)
				{
					for (int i = 0; i < this.m_mapPolygons.Count; i++)
					{
						MapPolygon mapPolygon = this.m_mapPolygons[i];
						if (mapPolygon != null && mapPolygon.ExpressionHostID > -1)
						{
							mapPolygon.SetExprHost(mapPolygonsHostsRemotable[mapPolygon.ExpressionHostID], reportObjectModel);
						}
					}
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapPolygonTemplate, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPolygonTemplate));
			list.Add(new MemberInfo(MemberName.MapPolygonRules, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPolygonRules));
			list.Add(new MemberInfo(MemberName.MapPointTemplate, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPointTemplate));
			list.Add(new MemberInfo(MemberName.MapPointRules, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPointRules));
			list.Add(new MemberInfo(MemberName.MapPolygons, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPolygon));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPolygonLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapVectorLayer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapPolygonLayer.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.MapPolygonTemplate:
					writer.Write(this.m_mapPolygonTemplate);
					break;
				case MemberName.MapPolygonRules:
					writer.Write(this.m_mapPolygonRules);
					break;
				case MemberName.MapPointTemplate:
					writer.Write(this.m_mapCenterPointTemplate);
					break;
				case MemberName.MapPointRules:
					writer.Write(this.m_mapCenterPointRules);
					break;
				case MemberName.MapPolygons:
					writer.Write(this.m_mapPolygons);
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
			reader.RegisterDeclaration(MapPolygonLayer.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.MapPolygonTemplate:
					this.m_mapPolygonTemplate = (MapPolygonTemplate)reader.ReadRIFObject();
					break;
				case MemberName.MapPolygonRules:
					this.m_mapPolygonRules = (MapPolygonRules)reader.ReadRIFObject();
					break;
				case MemberName.MapPointTemplate:
					this.m_mapCenterPointTemplate = (MapPointTemplate)reader.ReadRIFObject();
					break;
				case MemberName.MapPointRules:
					this.m_mapCenterPointRules = (MapPointRules)reader.ReadRIFObject();
					break;
				case MemberName.MapPolygons:
					this.m_mapPolygons = reader.ReadGenericListOfRIFObjects<MapPolygon>();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPolygonLayer;
		}
	}
}
