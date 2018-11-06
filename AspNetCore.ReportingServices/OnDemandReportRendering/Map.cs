using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class Map : ReportItem, IROMActionOwner, IPageBreakItem
	{
		private MapDataRegionCollection m_mapDataRegions;

		private PageBreak m_pageBreak;

		private ReportStringProperty m_pageName;

		private MapViewport m_mapViewport;

		private MapLayerCollection m_mapLayers;

		private MapLegendCollection m_mapLegends;

		private MapTitleCollection m_mapTitles;

		private MapDistanceScale m_mapDistanceScale;

		private MapColorScale m_mapColorScale;

		private MapBorderSkin m_mapBorderSkin;

		private ReportEnumProperty<MapAntiAliasing> m_antiAliasing;

		private ReportEnumProperty<MapTextAntiAliasingQuality> m_textAntiAliasingQuality;

		private ReportDoubleProperty m_shadowIntensity;

		private ReportStringProperty m_tileLanguage;

		private ActionInfo m_actionInfo;

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.Map MapDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.Map)base.m_reportItemDef;
			}
		}

		public MapDataRegionCollection MapDataRegions
		{
			get
			{
				if (this.m_mapDataRegions == null && this.MapDef.MapDataRegions != null)
				{
					this.m_mapDataRegions = new MapDataRegionCollection(this);
				}
				return this.m_mapDataRegions;
			}
		}

		public new MapInstance Instance
		{
			get
			{
				return (MapInstance)this.GetOrCreateInstance();
			}
		}

		public MapViewport MapViewport
		{
			get
			{
				if (this.m_mapViewport == null && this.MapDef.MapViewport != null)
				{
					this.m_mapViewport = new MapViewport(this.MapDef.MapViewport, this);
				}
				return this.m_mapViewport;
			}
		}

		public MapLayerCollection MapLayers
		{
			get
			{
				if (this.m_mapLayers == null && this.MapDef.MapLayers != null)
				{
					this.m_mapLayers = new MapLayerCollection(this);
				}
				return this.m_mapLayers;
			}
		}

		public MapLegendCollection MapLegends
		{
			get
			{
				if (this.m_mapLegends == null && this.MapDef.MapLegends != null)
				{
					this.m_mapLegends = new MapLegendCollection(this);
				}
				return this.m_mapLegends;
			}
		}

		public MapTitleCollection MapTitles
		{
			get
			{
				if (this.m_mapTitles == null && this.MapDef.MapTitles != null)
				{
					this.m_mapTitles = new MapTitleCollection(this);
				}
				return this.m_mapTitles;
			}
		}

		public MapDistanceScale MapDistanceScale
		{
			get
			{
				if (this.m_mapDistanceScale == null && this.MapDef.MapDistanceScale != null)
				{
					this.m_mapDistanceScale = new MapDistanceScale(this.MapDef.MapDistanceScale, this);
				}
				return this.m_mapDistanceScale;
			}
		}

		public MapColorScale MapColorScale
		{
			get
			{
				if (this.m_mapColorScale == null && this.MapDef.MapColorScale != null)
				{
					this.m_mapColorScale = new MapColorScale(this.MapDef.MapColorScale, this);
				}
				return this.m_mapColorScale;
			}
		}

		public MapBorderSkin MapBorderSkin
		{
			get
			{
				if (this.m_mapBorderSkin == null && this.MapDef.MapBorderSkin != null)
				{
					this.m_mapBorderSkin = new MapBorderSkin(this.MapDef.MapBorderSkin, this);
				}
				return this.m_mapBorderSkin;
			}
		}

		public ReportEnumProperty<MapAntiAliasing> AntiAliasing
		{
			get
			{
				if (this.m_antiAliasing == null && this.MapDef.AntiAliasing != null)
				{
					this.m_antiAliasing = new ReportEnumProperty<MapAntiAliasing>(this.MapDef.AntiAliasing.IsExpression, this.MapDef.AntiAliasing.OriginalText, EnumTranslator.TranslateMapAntiAliasing(this.MapDef.AntiAliasing.StringValue, null));
				}
				return this.m_antiAliasing;
			}
		}

		public ReportEnumProperty<MapTextAntiAliasingQuality> TextAntiAliasingQuality
		{
			get
			{
				if (this.m_textAntiAliasingQuality == null && this.MapDef.TextAntiAliasingQuality != null)
				{
					this.m_textAntiAliasingQuality = new ReportEnumProperty<MapTextAntiAliasingQuality>(this.MapDef.TextAntiAliasingQuality.IsExpression, this.MapDef.TextAntiAliasingQuality.OriginalText, EnumTranslator.TranslateMapTextAntiAliasingQuality(this.MapDef.TextAntiAliasingQuality.StringValue, null));
				}
				return this.m_textAntiAliasingQuality;
			}
		}

		public ReportDoubleProperty ShadowIntensity
		{
			get
			{
				if (this.m_shadowIntensity == null && this.MapDef.ShadowIntensity != null)
				{
					this.m_shadowIntensity = new ReportDoubleProperty(this.MapDef.ShadowIntensity);
				}
				return this.m_shadowIntensity;
			}
		}

		public ReportStringProperty TileLanguage
		{
			get
			{
				if (this.m_tileLanguage == null && this.MapDef.TileLanguage != null)
				{
					this.m_tileLanguage = new ReportStringProperty(this.MapDef.TileLanguage);
				}
				return this.m_tileLanguage;
			}
		}

		public int MaximumSpatialElementCount
		{
			get
			{
				return this.MapDef.MaximumSpatialElementCount;
			}
		}

		public int MaximumTotalPointCount
		{
			get
			{
				return this.MapDef.MaximumTotalPointCount;
			}
		}

		public PageBreak PageBreak
		{
			get
			{
				if (this.m_pageBreak == null)
				{
					IPageBreakOwner pageBreakOwner = (AspNetCore.ReportingServices.ReportIntermediateFormat.Map)base.m_reportItemDef;
					this.m_pageBreak = new PageBreak(base.m_renderingContext, this.ReportScope, pageBreakOwner);
				}
				return this.m_pageBreak;
			}
		}

		public ReportStringProperty PageName
		{
			get
			{
				if (this.m_pageName == null)
				{
					if (base.m_isOldSnapshot)
					{
						this.m_pageName = new ReportStringProperty();
					}
					else
					{
						this.m_pageName = new ReportStringProperty(((AspNetCore.ReportingServices.ReportIntermediateFormat.Map)base.m_reportItemDef).PageName);
					}
				}
				return this.m_pageName;
			}
		}

		[Obsolete("Use PageBreak.BreakLocation instead.")]
		PageBreakLocation IPageBreakItem.PageBreakLocation
		{
			get
			{
				IPageBreakOwner pageBreakOwner = (IPageBreakOwner)base.ReportItemDef;
				if (pageBreakOwner.PageBreak != null)
				{
					PageBreak pageBreak = this.PageBreak;
					if (pageBreak.HasEnabledInstance)
					{
						return pageBreak.BreakLocation;
					}
				}
				return PageBreakLocation.None;
			}
		}

		string IROMActionOwner.UniqueName
		{
			get
			{
				return this.MapDef.UniqueName;
			}
		}

		List<string> IROMActionOwner.FieldsUsedInValueExpression
		{
			get
			{
				return null;
			}
		}

		public bool SpecialBorderHandling
		{
			get
			{
				MapBorderSkin mapBorderSkin = this.MapBorderSkin;
				if (mapBorderSkin == null)
				{
					return false;
				}
				ReportEnumProperty<MapBorderSkinType> mapBorderSkinType = mapBorderSkin.MapBorderSkinType;
				if (mapBorderSkinType == null)
				{
					return false;
				}
				MapBorderSkinType mapBorderSkinType2 = mapBorderSkinType.IsExpression ? mapBorderSkin.Instance.MapBorderSkinType : mapBorderSkinType.Value;
				return mapBorderSkinType2 != MapBorderSkinType.None;
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				if (this.m_actionInfo == null)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.Action action = this.MapDef.Action;
					if (action != null)
					{
						this.m_actionInfo = new ActionInfo(base.RenderingContext, this.ReportScope, action, base.m_reportItemDef, this, base.m_reportItemDef.ObjectType, base.m_reportItemDef.Name, this);
					}
				}
				return this.m_actionInfo;
			}
		}

		internal Map(IReportScope reportScope, IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, AspNetCore.ReportingServices.ReportIntermediateFormat.Map reportItemDef, RenderingContext renderingContext)
			: base(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, reportItemDef, renderingContext)
		{
		}

		internal override ReportItemInstance GetOrCreateInstance()
		{
			if (base.m_instance == null)
			{
				base.m_instance = new MapInstance(this);
			}
			return base.m_instance;
		}

		internal override void SetNewContextChildren()
		{
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
			if (this.m_mapDataRegions != null)
			{
				this.m_mapDataRegions.SetNewContext();
			}
			if (this.m_mapViewport != null)
			{
				this.m_mapViewport.SetNewContext();
			}
			if (this.m_mapLayers != null)
			{
				this.m_mapLayers.SetNewContext();
			}
			if (this.m_mapLegends != null)
			{
				this.m_mapLegends.SetNewContext();
			}
			if (this.m_mapTitles != null)
			{
				this.m_mapTitles.SetNewContext();
			}
			if (this.m_mapDistanceScale != null)
			{
				this.m_mapDistanceScale.SetNewContext();
			}
			if (this.m_mapColorScale != null)
			{
				this.m_mapColorScale.SetNewContext();
			}
			if (this.m_mapBorderSkin != null)
			{
				this.m_mapBorderSkin.SetNewContext();
			}
			if (this.m_actionInfo != null)
			{
				this.m_actionInfo.SetNewContext();
			}
			if (this.m_pageBreak != null)
			{
				this.m_pageBreak.SetNewContext();
			}
		}
	}
}
