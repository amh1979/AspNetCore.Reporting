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
	internal sealed class MapPointLayer : MapVectorLayer, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapPointLayer.GetDeclaration();

		private MapPointTemplate m_mapPointTemplate;

		private MapPointRules m_mapPointRules;

		private List<MapPoint> m_mapPoints;

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

		internal MapPointRules MapPointRules
		{
			get
			{
				return this.m_mapPointRules;
			}
			set
			{
				this.m_mapPointRules = value;
			}
		}

		internal List<MapPoint> MapPoints
		{
			get
			{
				return this.m_mapPoints;
			}
			set
			{
				this.m_mapPoints = value;
			}
		}

		protected override bool Embedded
		{
			get
			{
				return this.MapPoints != null;
			}
		}

		internal new MapPointLayerExprHost ExprHost
		{
			get
			{
				return (MapPointLayerExprHost)base.m_exprHost;
			}
		}

		internal new MapPointLayerExprHost ExprHostMapMember
		{
			get
			{
				return (MapPointLayerExprHost)base.m_exprHostMapMember;
			}
		}

		internal MapPointLayer()
		{
		}

		internal MapPointLayer(int ID, Map map)
			: base(ID, map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapPointLayerStart(base.Name);
			base.Initialize(context);
			if (this.m_mapPointRules != null)
			{
				this.m_mapPointRules.Initialize(context);
			}
			if (base.MapDataRegionName == null)
			{
				if (this.m_mapPointTemplate != null)
				{
					this.m_mapPointTemplate.Initialize(context);
				}
				if (this.m_mapPoints != null)
				{
					for (int i = 0; i < this.m_mapPoints.Count; i++)
					{
						this.m_mapPoints[i].Initialize(context, i);
					}
				}
			}
			base.m_exprHostID = context.ExprHostBuilder.MapPointLayerEnd();
		}

		internal override void InitializeMapMember(InitializationContext context)
		{
			context.ExprHostBuilder.MapPointLayerStart(base.Name);
			base.InitializeMapMember(context);
			if (this.m_mapPointRules != null)
			{
				this.m_mapPointRules.InitializeMapMember(context);
			}
			if (base.MapDataRegionName != null)
			{
				if (this.m_mapPointTemplate != null)
				{
					this.m_mapPointTemplate.Initialize(context);
				}
				if (this.m_mapPoints != null)
				{
					for (int i = 0; i < this.m_mapPoints.Count; i++)
					{
						this.m_mapPoints[i].Initialize(context, i);
					}
				}
			}
			base.m_exprHostMapMemberID = context.ExprHostBuilder.MapPointLayerEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapPointLayer mapPointLayer = (MapPointLayer)(context.CurrentMapVectorLayerClone = (MapPointLayer)base.PublishClone(context));
			if (this.m_mapPointTemplate != null)
			{
				mapPointLayer.m_mapPointTemplate = (MapPointTemplate)this.m_mapPointTemplate.PublishClone(context);
			}
			if (this.m_mapPointRules != null)
			{
				mapPointLayer.m_mapPointRules = (MapPointRules)this.m_mapPointRules.PublishClone(context);
			}
			if (this.m_mapPoints != null)
			{
				mapPointLayer.m_mapPoints = new List<MapPoint>(this.m_mapPoints.Count);
				{
					foreach (MapPoint mapPoint in this.m_mapPoints)
					{
						mapPointLayer.m_mapPoints.Add((MapPoint)mapPoint.PublishClone(context));
					}
					return mapPointLayer;
				}
			}
			return mapPointLayer;
		}

		internal override void SetExprHost(MapLayerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			if (this.m_mapPointRules != null && this.ExprHost.MapPointRulesHost != null)
			{
				this.m_mapPointRules.SetExprHost(this.ExprHost.MapPointRulesHost, reportObjectModel);
			}
			if (base.MapDataRegionName == null)
			{
				if (this.m_mapPointTemplate != null && this.ExprHost.MapPointTemplateHost != null)
				{
					this.m_mapPointTemplate.SetExprHost(this.ExprHost.MapPointTemplateHost, reportObjectModel);
				}
				IList<MapPointExprHost> mapPointsHostsRemotable = this.ExprHost.MapPointsHostsRemotable;
				if (this.m_mapPoints != null && mapPointsHostsRemotable != null)
				{
					for (int i = 0; i < this.m_mapPoints.Count; i++)
					{
						MapPoint mapPoint = this.m_mapPoints[i];
						if (mapPoint != null && mapPoint.ExpressionHostID > -1)
						{
							mapPoint.SetExprHost(mapPointsHostsRemotable[mapPoint.ExpressionHostID], reportObjectModel);
						}
					}
				}
			}
		}

		internal override void SetExprHostMapMember(MapVectorLayerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHostMapMember(exprHost, reportObjectModel);
			if (this.m_mapPointRules != null && this.ExprHostMapMember.MapPointRulesHost != null)
			{
				this.m_mapPointRules.SetExprHostMapMember(this.ExprHostMapMember.MapPointRulesHost, reportObjectModel);
			}
			if (base.MapDataRegionName != null)
			{
				if (this.m_mapPointTemplate != null && this.ExprHostMapMember.MapPointTemplateHost != null)
				{
					this.m_mapPointTemplate.SetExprHost(this.ExprHostMapMember.MapPointTemplateHost, reportObjectModel);
				}
				IList<MapPointExprHost> mapPointsHostsRemotable = this.ExprHostMapMember.MapPointsHostsRemotable;
				if (this.m_mapPoints != null && mapPointsHostsRemotable != null)
				{
					for (int i = 0; i < this.m_mapPoints.Count; i++)
					{
						MapPoint mapPoint = this.m_mapPoints[i];
						if (mapPoint != null && mapPoint.ExpressionHostID > -1)
						{
							mapPoint.SetExprHost(mapPointsHostsRemotable[mapPoint.ExpressionHostID], reportObjectModel);
						}
					}
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapPointTemplate, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPointTemplate));
			list.Add(new MemberInfo(MemberName.MapPointRules, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPointRules));
			list.Add(new MemberInfo(MemberName.MapPoints, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPoint));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPointLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapVectorLayer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapPointLayer.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.MapPointTemplate:
					writer.Write(this.m_mapPointTemplate);
					break;
				case MemberName.MapPointRules:
					writer.Write(this.m_mapPointRules);
					break;
				case MemberName.MapPoints:
					writer.Write(this.m_mapPoints);
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
			reader.RegisterDeclaration(MapPointLayer.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.MapPointTemplate:
					this.m_mapPointTemplate = (MapPointTemplate)reader.ReadRIFObject();
					break;
				case MemberName.MapPointRules:
					this.m_mapPointRules = (MapPointRules)reader.ReadRIFObject();
					break;
				case MemberName.MapPoints:
					this.m_mapPoints = reader.ReadGenericListOfRIFObjects<MapPoint>();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPointLayer;
		}
	}
}
