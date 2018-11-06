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
	internal sealed class MapLineLayer : MapVectorLayer, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = MapLineLayer.GetDeclaration();

		private MapLineTemplate m_mapLineTemplate;

		private MapLineRules m_mapLineRules;

		private List<MapLine> m_mapLines;

		internal MapLineTemplate MapLineTemplate
		{
			get
			{
				return this.m_mapLineTemplate;
			}
			set
			{
				this.m_mapLineTemplate = value;
			}
		}

		internal MapLineRules MapLineRules
		{
			get
			{
				return this.m_mapLineRules;
			}
			set
			{
				this.m_mapLineRules = value;
			}
		}

		internal List<MapLine> MapLines
		{
			get
			{
				return this.m_mapLines;
			}
			set
			{
				this.m_mapLines = value;
			}
		}

		protected override bool Embedded
		{
			get
			{
				return this.MapLines != null;
			}
		}

		internal new MapLineLayerExprHost ExprHost
		{
			get
			{
				return (MapLineLayerExprHost)base.m_exprHost;
			}
		}

		internal new MapLineLayerExprHost ExprHostMapMember
		{
			get
			{
				return (MapLineLayerExprHost)base.m_exprHostMapMember;
			}
		}

		internal MapLineLayer()
		{
		}

		internal MapLineLayer(int ID, Map map)
			: base(ID, map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapLineLayerStart(base.Name);
			base.Initialize(context);
			if (this.m_mapLineRules != null)
			{
				this.m_mapLineRules.Initialize(context);
			}
			if (base.MapDataRegionName == null)
			{
				if (this.m_mapLineTemplate != null)
				{
					this.m_mapLineTemplate.Initialize(context);
				}
				if (this.m_mapLines != null)
				{
					for (int i = 0; i < this.m_mapLines.Count; i++)
					{
						this.m_mapLines[i].Initialize(context, i);
					}
				}
			}
			base.m_exprHostID = context.ExprHostBuilder.MapLineLayerEnd();
		}

		internal override void InitializeMapMember(InitializationContext context)
		{
			context.ExprHostBuilder.MapLineLayerStart(base.Name);
			base.InitializeMapMember(context);
			if (this.m_mapLineRules != null)
			{
				this.m_mapLineRules.InitializeMapMember(context);
			}
			if (base.MapDataRegionName != null)
			{
				if (this.m_mapLineTemplate != null)
				{
					this.m_mapLineTemplate.Initialize(context);
				}
				if (this.m_mapLines != null)
				{
					for (int i = 0; i < this.m_mapLines.Count; i++)
					{
						this.m_mapLines[i].Initialize(context, i);
					}
				}
			}
			base.m_exprHostMapMemberID = context.ExprHostBuilder.MapLineLayerEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapLineLayer mapLineLayer = (MapLineLayer)(context.CurrentMapVectorLayerClone = (MapLineLayer)base.PublishClone(context));
			if (this.m_mapLineTemplate != null)
			{
				mapLineLayer.m_mapLineTemplate = (MapLineTemplate)this.m_mapLineTemplate.PublishClone(context);
			}
			if (this.m_mapLineRules != null)
			{
				mapLineLayer.m_mapLineRules = (MapLineRules)this.m_mapLineRules.PublishClone(context);
			}
			if (this.m_mapLines != null)
			{
				mapLineLayer.m_mapLines = new List<MapLine>(this.m_mapLines.Count);
				{
					foreach (MapLine mapLine in this.m_mapLines)
					{
						mapLineLayer.m_mapLines.Add((MapLine)mapLine.PublishClone(context));
					}
					return mapLineLayer;
				}
			}
			return mapLineLayer;
		}

		internal override void SetExprHost(MapLayerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			if (this.m_mapLineRules != null && this.ExprHost.MapLineRulesHost != null)
			{
				this.m_mapLineRules.SetExprHost(this.ExprHost.MapLineRulesHost, reportObjectModel);
			}
			if (base.MapDataRegionName == null)
			{
				if (this.m_mapLineTemplate != null && this.ExprHost.MapLineTemplateHost != null)
				{
					this.m_mapLineTemplate.SetExprHost(this.ExprHost.MapLineTemplateHost, reportObjectModel);
				}
				IList<MapLineExprHost> mapLinesHostsRemotable = this.ExprHost.MapLinesHostsRemotable;
				if (this.m_mapLines != null && mapLinesHostsRemotable != null)
				{
					for (int i = 0; i < this.m_mapLines.Count; i++)
					{
						MapLine mapLine = this.m_mapLines[i];
						if (mapLine != null && mapLine.ExpressionHostID > -1)
						{
							mapLine.SetExprHost(mapLinesHostsRemotable[mapLine.ExpressionHostID], reportObjectModel);
						}
					}
				}
			}
		}

		internal override void SetExprHostMapMember(MapVectorLayerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHostMapMember(exprHost, reportObjectModel);
			if (this.m_mapLineRules != null && this.ExprHostMapMember.MapLineRulesHost != null)
			{
				this.m_mapLineRules.SetExprHostMapMember(this.ExprHostMapMember.MapLineRulesHost, reportObjectModel);
			}
			if (base.MapDataRegionName != null)
			{
				if (this.m_mapLineTemplate != null && this.ExprHostMapMember.MapLineTemplateHost != null)
				{
					this.m_mapLineTemplate.SetExprHost(this.ExprHostMapMember.MapLineTemplateHost, reportObjectModel);
				}
				IList<MapLineExprHost> mapLinesHostsRemotable = this.ExprHostMapMember.MapLinesHostsRemotable;
				if (this.m_mapLines != null && mapLinesHostsRemotable != null)
				{
					for (int i = 0; i < this.m_mapLines.Count; i++)
					{
						MapLine mapLine = this.m_mapLines[i];
						if (mapLine != null && mapLine.ExpressionHostID > -1)
						{
							mapLine.SetExprHost(mapLinesHostsRemotable[mapLine.ExpressionHostID], reportObjectModel);
						}
					}
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapLineTemplate, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLineTemplate));
			list.Add(new MemberInfo(MemberName.MapLineRules, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLineRules));
			list.Add(new MemberInfo(MemberName.MapLines, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLine));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLineLayer, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapVectorLayer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(MapLineLayer.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.MapLineTemplate:
					writer.Write(this.m_mapLineTemplate);
					break;
				case MemberName.MapLineRules:
					writer.Write(this.m_mapLineRules);
					break;
				case MemberName.MapLines:
					writer.Write(this.m_mapLines);
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
			reader.RegisterDeclaration(MapLineLayer.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.MapLineTemplate:
					this.m_mapLineTemplate = (MapLineTemplate)reader.ReadRIFObject();
					break;
				case MemberName.MapLineRules:
					this.m_mapLineRules = (MapLineRules)reader.ReadRIFObject();
					break;
				case MemberName.MapLines:
					this.m_mapLines = reader.ReadGenericListOfRIFObjects<MapLine>();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLineLayer;
		}
	}
}
