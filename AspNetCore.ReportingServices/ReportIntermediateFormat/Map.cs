using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.RdlExpressions;
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
	internal sealed class Map : ReportItem, IActionOwner, IPersistable, IPageBreakOwner
	{
		private List<MapDataRegion> m_mapDataRegions;

		private MapViewport m_mapViewport;

		private List<MapLayer> m_mapLayers;

		private List<MapLegend> m_mapLegends;

		private List<MapTitle> m_mapTitles;

		private MapDistanceScale m_mapDistanceScale;

		private MapColorScale m_mapColorScale;

		private MapBorderSkin m_mapBorderSkin;

		private ExpressionInfo m_antiAliasing;

		private ExpressionInfo m_textAntiAliasingQuality;

		private ExpressionInfo m_shadowIntensity;

		private Action m_action;

		private int m_maximumSpatialElementCount = 20000;

		private int m_maximumTotalPointCount = 1000000;

		private ExpressionInfo m_tileLanguage;

		private PageBreak m_pageBreak;

		private ExpressionInfo m_pageName;

		[NonSerialized]
		private static readonly Declaration m_Declaration = Map.GetDeclaration();

		[NonSerialized]
		private MapExprHost m_exprHost;

		[NonSerialized]
		private int m_actionOwnerCounter;

		[NonSerialized]
		private Formatter m_formatter;

		internal override AspNetCore.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map;
			}
		}

		internal MapViewport MapViewport
		{
			get
			{
				return this.m_mapViewport;
			}
			set
			{
				this.m_mapViewport = value;
			}
		}

		internal List<MapLayer> MapLayers
		{
			get
			{
				return this.m_mapLayers;
			}
			set
			{
				this.m_mapLayers = value;
			}
		}

		internal List<MapLegend> MapLegends
		{
			get
			{
				return this.m_mapLegends;
			}
			set
			{
				this.m_mapLegends = value;
			}
		}

		internal List<MapTitle> MapTitles
		{
			get
			{
				return this.m_mapTitles;
			}
			set
			{
				this.m_mapTitles = value;
			}
		}

		internal MapDistanceScale MapDistanceScale
		{
			get
			{
				return this.m_mapDistanceScale;
			}
			set
			{
				this.m_mapDistanceScale = value;
			}
		}

		internal MapColorScale MapColorScale
		{
			get
			{
				return this.m_mapColorScale;
			}
			set
			{
				this.m_mapColorScale = value;
			}
		}

		internal MapBorderSkin MapBorderSkin
		{
			get
			{
				return this.m_mapBorderSkin;
			}
			set
			{
				this.m_mapBorderSkin = value;
			}
		}

		internal ExpressionInfo AntiAliasing
		{
			get
			{
				return this.m_antiAliasing;
			}
			set
			{
				this.m_antiAliasing = value;
			}
		}

		internal ExpressionInfo TextAntiAliasingQuality
		{
			get
			{
				return this.m_textAntiAliasingQuality;
			}
			set
			{
				this.m_textAntiAliasingQuality = value;
			}
		}

		internal ExpressionInfo ShadowIntensity
		{
			get
			{
				return this.m_shadowIntensity;
			}
			set
			{
				this.m_shadowIntensity = value;
			}
		}

		internal ExpressionInfo TileLanguage
		{
			get
			{
				return this.m_tileLanguage;
			}
			set
			{
				this.m_tileLanguage = value;
			}
		}

		internal int MaximumSpatialElementCount
		{
			get
			{
				return this.m_maximumSpatialElementCount;
			}
			set
			{
				this.m_maximumSpatialElementCount = value;
			}
		}

		internal int MaximumTotalPointCount
		{
			get
			{
				return this.m_maximumTotalPointCount;
			}
			set
			{
				this.m_maximumTotalPointCount = value;
			}
		}

		internal List<MapDataRegion> MapDataRegions
		{
			get
			{
				return this.m_mapDataRegions;
			}
			set
			{
				this.m_mapDataRegions = value;
			}
		}

		internal Action Action
		{
			get
			{
				return this.m_action;
			}
			set
			{
				this.m_action = value;
			}
		}

		Action IActionOwner.Action
		{
			get
			{
				return this.m_action;
			}
		}

		internal ExpressionInfo PageName
		{
			get
			{
				return this.m_pageName;
			}
			set
			{
				this.m_pageName = value;
			}
		}

		internal PageBreak PageBreak
		{
			get
			{
				return this.m_pageBreak;
			}
			set
			{
				this.m_pageBreak = value;
			}
		}

		PageBreak IPageBreakOwner.PageBreak
		{
			get
			{
				return this.m_pageBreak;
			}
			set
			{
				this.m_pageBreak = value;
			}
		}

		AspNetCore.ReportingServices.ReportProcessing.ObjectType IPageBreakOwner.ObjectType
		{
			get
			{
				return AspNetCore.ReportingServices.ReportProcessing.ObjectType.Map;
			}
		}

		string IPageBreakOwner.ObjectName
		{
			get
			{
				return base.m_name;
			}
		}

		IInstancePath IPageBreakOwner.InstancePath
		{
			get
			{
				return this;
			}
		}

		List<string> IActionOwner.FieldsUsedInValueExpression
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		internal MapExprHost MapExprHost
		{
			get
			{
				return this.m_exprHost;
			}
		}

		internal Map(ReportItem parent)
			: base(parent)
		{
		}

		internal Map(int id, ReportItem parent)
			: base(id, parent)
		{
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = this.ObjectType;
			context.ObjectName = base.m_name;
			context.ExprHostBuilder.MapStart(base.m_name);
			base.Initialize(context);
			if (base.m_visibility != null)
			{
				base.m_visibility.Initialize(context);
			}
			context.IsTopLevelCellContents = false;
			if (this.m_mapViewport != null)
			{
				this.m_mapViewport.Initialize(context);
			}
			if (this.m_mapLayers != null)
			{
				for (int i = 0; i < this.m_mapLayers.Count; i++)
				{
					this.m_mapLayers[i].Initialize(context);
				}
			}
			if (this.m_mapLegends != null)
			{
				for (int j = 0; j < this.m_mapLegends.Count; j++)
				{
					this.m_mapLegends[j].Initialize(context);
				}
			}
			if (this.m_mapTitles != null)
			{
				for (int k = 0; k < this.m_mapTitles.Count; k++)
				{
					this.m_mapTitles[k].Initialize(context);
				}
			}
			if (this.m_mapDistanceScale != null)
			{
				this.m_mapDistanceScale.Initialize(context);
			}
			if (this.m_mapColorScale != null)
			{
				this.m_mapColorScale.Initialize(context);
			}
			if (this.m_mapBorderSkin != null)
			{
				this.m_mapBorderSkin.Initialize(context);
			}
			if (this.m_antiAliasing != null)
			{
				this.m_antiAliasing.Initialize("AntiAliasing", context);
				context.ExprHostBuilder.MapAntiAliasing(this.m_antiAliasing);
			}
			if (this.m_textAntiAliasingQuality != null)
			{
				this.m_textAntiAliasingQuality.Initialize("TextAntiAliasingQuality", context);
				context.ExprHostBuilder.MapTextAntiAliasingQuality(this.m_textAntiAliasingQuality);
			}
			if (this.m_shadowIntensity != null)
			{
				this.m_shadowIntensity.Initialize("ShadowIntensity", context);
				context.ExprHostBuilder.MapShadowIntensity(this.m_shadowIntensity);
			}
			if (this.m_tileLanguage != null)
			{
				this.m_tileLanguage.Initialize("TileLanguage", context);
				context.ExprHostBuilder.MapTileLanguage(this.m_tileLanguage);
			}
			if (this.m_mapDataRegions != null)
			{
				for (int l = 0; l < this.m_mapDataRegions.Count; l++)
				{
					this.m_mapDataRegions[l].Initialize(context);
				}
			}
			if (this.m_action != null)
			{
				this.m_action.Initialize(context);
			}
			if (this.m_pageBreak != null)
			{
				this.m_pageBreak.Initialize(context);
			}
			if (this.m_pageName != null)
			{
				this.m_pageName.Initialize("PageName", context);
				context.ExprHostBuilder.PageName(this.m_pageName);
			}
			base.ExprHostID = context.ExprHostBuilder.MapEnd();
			return false;
		}

		internal override void TraverseScopes(IRIFScopeVisitor visitor)
		{
			if (this.m_mapDataRegions != null)
			{
				for (int i = 0; i < this.m_mapDataRegions.Count; i++)
				{
					this.m_mapDataRegions[i].TraverseScopes(visitor);
				}
			}
		}

		internal bool ContainsMapDataRegion()
		{
			if (this.m_mapDataRegions != null)
			{
				return this.m_mapDataRegions.Count != 0;
			}
			return false;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			Map map2 = context.CurrentMapClone = (Map)base.PublishClone(context);
			if (this.m_mapDataRegions != null)
			{
				map2.m_mapDataRegions = new List<MapDataRegion>(this.m_mapDataRegions.Count);
				foreach (MapDataRegion mapDataRegion in this.m_mapDataRegions)
				{
					map2.m_mapDataRegions.Add((MapDataRegion)mapDataRegion.PublishClone(context));
				}
			}
			if (this.m_mapLayers != null)
			{
				map2.m_mapLayers = new List<MapLayer>(this.m_mapLayers.Count);
				foreach (MapLayer mapLayer in this.m_mapLayers)
				{
					map2.m_mapLayers.Add((MapLayer)mapLayer.PublishClone(context));
				}
			}
			if (this.m_mapViewport != null)
			{
				map2.m_mapViewport = (MapViewport)this.m_mapViewport.PublishClone(context);
			}
			if (this.m_mapLegends != null)
			{
				map2.m_mapLegends = new List<MapLegend>(this.m_mapLegends.Count);
				foreach (MapLegend mapLegend in this.m_mapLegends)
				{
					map2.m_mapLegends.Add((MapLegend)mapLegend.PublishClone(context));
				}
			}
			if (this.m_mapTitles != null)
			{
				map2.m_mapTitles = new List<MapTitle>(this.m_mapTitles.Count);
				foreach (MapTitle mapTitle in this.m_mapTitles)
				{
					map2.m_mapTitles.Add((MapTitle)mapTitle.PublishClone(context));
				}
			}
			if (this.m_mapDistanceScale != null)
			{
				map2.m_mapDistanceScale = (MapDistanceScale)this.m_mapDistanceScale.PublishClone(context);
			}
			if (this.m_mapColorScale != null)
			{
				map2.m_mapColorScale = (MapColorScale)this.m_mapColorScale.PublishClone(context);
			}
			if (this.m_mapBorderSkin != null)
			{
				map2.m_mapBorderSkin = (MapBorderSkin)this.m_mapBorderSkin.PublishClone(context);
			}
			if (this.m_antiAliasing != null)
			{
				map2.m_antiAliasing = (ExpressionInfo)this.m_antiAliasing.PublishClone(context);
			}
			if (this.m_textAntiAliasingQuality != null)
			{
				map2.m_textAntiAliasingQuality = (ExpressionInfo)this.m_textAntiAliasingQuality.PublishClone(context);
			}
			if (this.m_shadowIntensity != null)
			{
				map2.m_shadowIntensity = (ExpressionInfo)this.m_shadowIntensity.PublishClone(context);
			}
			if (this.m_tileLanguage != null)
			{
				map2.m_tileLanguage = (ExpressionInfo)this.m_tileLanguage.PublishClone(context);
			}
			if (this.m_action != null)
			{
				map2.m_action = (Action)this.m_action.PublishClone(context);
			}
			if (this.m_pageBreak != null)
			{
				map2.m_pageBreak = (PageBreak)this.m_pageBreak.PublishClone(context);
			}
			if (this.m_pageName != null)
			{
				map2.m_pageName = (ExpressionInfo)this.m_pageName.PublishClone(context);
			}
			return map2;
		}

		internal MapAntiAliasing EvaluateAntiAliasing(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return EnumTranslator.TranslateMapAntiAliasing(context.ReportRuntime.EvaluateMapAntiAliasingExpression(this, base.Name), context.ReportRuntime);
		}

		internal MapTextAntiAliasingQuality EvaluateTextAntiAliasingQuality(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return EnumTranslator.TranslateMapTextAntiAliasingQuality(context.ReportRuntime.EvaluateMapTextAntiAliasingQualityExpression(this, base.Name), context.ReportRuntime);
		}

		internal double EvaluateShadowIntensity(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapShadowIntensityExpression(this, base.Name);
		}

		internal string EvaluateTileLanguage(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapTileLanguageExpression(this, base.Name);
		}

		internal string EvaluatePageName(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapPageNameExpression(this, this.m_pageName, base.m_name);
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapDataRegions, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapDataRegion));
			list.Add(new MemberInfo(MemberName.MapViewport, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapViewport));
			list.Add(new MemberInfo(MemberName.MapLayers, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLayer));
			list.Add(new MemberInfo(MemberName.MapLegends, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLegend));
			list.Add(new MemberInfo(MemberName.MapTitles, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapTitle));
			list.Add(new MemberInfo(MemberName.MapDistanceScale, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapDistanceScale));
			list.Add(new MemberInfo(MemberName.MapColorScale, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorScale));
			list.Add(new MemberInfo(MemberName.MapBorderSkin, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapBorderSkin));
			list.Add(new ReadOnlyMemberInfo(MemberName.PageBreakLocation, Token.Enum));
			list.Add(new MemberInfo(MemberName.AntiAliasing, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TextAntiAliasingQuality, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ShadowIntensity, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MaximumSpatialElementCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.MaximumTotalPointCount, Token.Int32));
			list.Add(new MemberInfo(MemberName.Action, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.TileLanguage, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.PageBreak, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PageBreak));
			list.Add(new MemberInfo(MemberName.PageName, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem, list);
		}

		internal int GenerateActionOwnerID()
		{
			return ++this.m_actionOwnerCounter;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(Map.m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.MapDataRegions:
					writer.Write(this.m_mapDataRegions);
					break;
				case MemberName.MapViewport:
					writer.Write(this.m_mapViewport);
					break;
				case MemberName.MapLayers:
					writer.Write(this.m_mapLayers);
					break;
				case MemberName.MapLegends:
					writer.Write(this.m_mapLegends);
					break;
				case MemberName.MapTitles:
					writer.Write(this.m_mapTitles);
					break;
				case MemberName.MapDistanceScale:
					writer.Write(this.m_mapDistanceScale);
					break;
				case MemberName.MapColorScale:
					writer.Write(this.m_mapColorScale);
					break;
				case MemberName.MapBorderSkin:
					writer.Write(this.m_mapBorderSkin);
					break;
				case MemberName.AntiAliasing:
					writer.Write(this.m_antiAliasing);
					break;
				case MemberName.TextAntiAliasingQuality:
					writer.Write(this.m_textAntiAliasingQuality);
					break;
				case MemberName.ShadowIntensity:
					writer.Write(this.m_shadowIntensity);
					break;
				case MemberName.MaximumSpatialElementCount:
					writer.Write(this.m_maximumSpatialElementCount);
					break;
				case MemberName.MaximumTotalPointCount:
					writer.Write(this.m_maximumTotalPointCount);
					break;
				case MemberName.Action:
					writer.Write(this.m_action);
					break;
				case MemberName.TileLanguage:
					writer.Write(this.m_tileLanguage);
					break;
				case MemberName.PageBreak:
					writer.Write(this.m_pageBreak);
					break;
				case MemberName.PageName:
					writer.Write(this.m_pageName);
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
			reader.RegisterDeclaration(Map.m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.MapDataRegions:
					this.m_mapDataRegions = reader.ReadGenericListOfRIFObjects<MapDataRegion>();
					break;
				case MemberName.MapViewport:
					this.m_mapViewport = (MapViewport)reader.ReadRIFObject();
					break;
				case MemberName.MapLayers:
					this.m_mapLayers = reader.ReadGenericListOfRIFObjects<MapLayer>();
					break;
				case MemberName.MapLegends:
					this.m_mapLegends = reader.ReadGenericListOfRIFObjects<MapLegend>();
					break;
				case MemberName.MapTitles:
					this.m_mapTitles = reader.ReadGenericListOfRIFObjects<MapTitle>();
					break;
				case MemberName.MapDistanceScale:
					this.m_mapDistanceScale = (MapDistanceScale)reader.ReadRIFObject();
					break;
				case MemberName.MapColorScale:
					this.m_mapColorScale = (MapColorScale)reader.ReadRIFObject();
					break;
				case MemberName.MapBorderSkin:
					this.m_mapBorderSkin = (MapBorderSkin)reader.ReadRIFObject();
					break;
				case MemberName.PageBreakLocation:
					this.m_pageBreak = new PageBreak();
					this.m_pageBreak.BreakLocation = (PageBreakLocation)reader.ReadEnum();
					break;
				case MemberName.AntiAliasing:
					this.m_antiAliasing = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TextAntiAliasingQuality:
					this.m_textAntiAliasingQuality = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ShadowIntensity:
					this.m_shadowIntensity = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MaximumSpatialElementCount:
					this.m_maximumSpatialElementCount = reader.ReadInt32();
					break;
				case MemberName.MaximumTotalPointCount:
					this.m_maximumTotalPointCount = reader.ReadInt32();
					break;
				case MemberName.Action:
					this.m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.TileLanguage:
					this.m_tileLanguage = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.PageBreak:
					this.m_pageBreak = (PageBreak)reader.ReadRIFObject();
					break;
				case MemberName.PageName:
					this.m_pageName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
		}

		public override AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID >= 0)
			{
				Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
				this.m_exprHost = reportExprHost.MapHostsRemotable[base.ExprHostID];
				base.ReportItemSetExprHost(this.m_exprHost, reportObjectModel);
				if (this.m_mapViewport != null && this.m_exprHost.MapViewportHost != null)
				{
					this.m_mapViewport.SetExprHost(this.m_exprHost.MapViewportHost, reportObjectModel);
				}
				IList<MapPolygonLayerExprHost> mapPolygonLayersHostsRemotable = this.m_exprHost.MapPolygonLayersHostsRemotable;
				IList<MapPointLayerExprHost> mapPointLayersHostsRemotable = this.m_exprHost.MapPointLayersHostsRemotable;
				IList<MapLineLayerExprHost> mapLineLayersHostsRemotable = this.m_exprHost.MapLineLayersHostsRemotable;
				IList<MapTileLayerExprHost> mapTileLayersHostsRemotable = this.m_exprHost.MapTileLayersHostsRemotable;
				if (this.m_mapLayers != null)
				{
					for (int i = 0; i < this.m_mapLayers.Count; i++)
					{
						MapLayer mapLayer = this.m_mapLayers[i];
						if (mapLayer != null && mapLayer.ExpressionHostID > -1)
						{
							if (mapLayer is MapPolygonLayer)
							{
								if (mapPolygonLayersHostsRemotable != null)
								{
									mapLayer.SetExprHost(mapPolygonLayersHostsRemotable[mapLayer.ExpressionHostID], reportObjectModel);
								}
							}
							else if (mapLayer is MapPointLayer)
							{
								if (mapPointLayersHostsRemotable != null)
								{
									mapLayer.SetExprHost(mapPointLayersHostsRemotable[mapLayer.ExpressionHostID], reportObjectModel);
								}
							}
							else if (mapLayer is MapLineLayer)
							{
								if (mapLineLayersHostsRemotable != null)
								{
									mapLayer.SetExprHost(mapLineLayersHostsRemotable[mapLayer.ExpressionHostID], reportObjectModel);
								}
							}
							else if (mapLayer is MapTileLayer && mapTileLayersHostsRemotable != null)
							{
								mapLayer.SetExprHost(mapTileLayersHostsRemotable[mapLayer.ExpressionHostID], reportObjectModel);
							}
						}
					}
				}
				IList<MapLegendExprHost> mapLegendsHostsRemotable = this.m_exprHost.MapLegendsHostsRemotable;
				if (this.m_mapLegends != null && mapLegendsHostsRemotable != null)
				{
					for (int j = 0; j < this.m_mapLegends.Count; j++)
					{
						MapLegend mapLegend = this.m_mapLegends[j];
						if (mapLegend != null && mapLegend.ExpressionHostID > -1)
						{
							mapLegend.SetExprHost(mapLegendsHostsRemotable[mapLegend.ExpressionHostID], reportObjectModel);
						}
					}
				}
				IList<MapTitleExprHost> mapTitlesHostsRemotable = this.m_exprHost.MapTitlesHostsRemotable;
				if (this.m_mapTitles != null && mapTitlesHostsRemotable != null)
				{
					for (int k = 0; k < this.m_mapTitles.Count; k++)
					{
						MapTitle mapTitle = this.m_mapTitles[k];
						if (mapTitle != null && mapTitle.ExpressionHostID > -1)
						{
							mapTitle.SetExprHost(mapTitlesHostsRemotable[mapTitle.ExpressionHostID], reportObjectModel);
						}
					}
				}
				if (this.m_mapDistanceScale != null && this.m_exprHost.MapDistanceScaleHost != null)
				{
					this.m_mapDistanceScale.SetExprHost(this.m_exprHost.MapDistanceScaleHost, reportObjectModel);
				}
				if (this.m_mapColorScale != null && this.m_exprHost.MapColorScaleHost != null)
				{
					this.m_mapColorScale.SetExprHost(this.m_exprHost.MapColorScaleHost, reportObjectModel);
				}
				if (this.m_mapBorderSkin != null && this.m_exprHost.MapBorderSkinHost != null)
				{
					this.m_mapBorderSkin.SetExprHost(this.m_exprHost.MapBorderSkinHost, reportObjectModel);
				}
				if (this.m_action != null && this.m_exprHost.ActionInfoHost != null)
				{
					this.m_action.SetExprHost(this.m_exprHost.ActionInfoHost, reportObjectModel);
				}
				if (this.m_pageBreak != null && this.m_exprHost.PageBreakExprHost != null)
				{
					this.m_pageBreak.SetExprHost(this.m_exprHost.PageBreakExprHost, reportObjectModel);
				}
			}
		}

		internal string GetFormattedStringFromValue(ref AspNetCore.ReportingServices.RdlExpressions.VariantResult result, OnDemandProcessingContext context)
		{
			string result2 = null;
			if (result.ErrorOccurred)
			{
				result2 = RPRes.rsExpressionErrorValue;
			}
			else if (result.Value != null)
			{
				result2 = Formatter.Format(result.Value, ref this.m_formatter, base.StyleClass, (Style)null, context, this.ObjectType, base.Name);
			}
			return result2;
		}
	}
}
