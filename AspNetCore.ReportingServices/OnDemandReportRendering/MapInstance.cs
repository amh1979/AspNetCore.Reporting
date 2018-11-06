using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System;
using System.IO;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapInstance : ReportItemInstance, IDynamicImageInstance
	{
		private float m_dpiX = 96f;

		private float m_dpiY = 96f;

		private double? m_widthOverride = null;

		private double? m_heightOverride = null;

		private MapAntiAliasing? m_antiAliasing;

		private MapTextAntiAliasingQuality? m_textAntiAliasingQuality;

		private double? m_shadowIntensity;

		private string m_tileLanguage;

		private bool m_tileLanguageEvaluated;

		private bool m_pageNameEvaluated;

		private string m_pageName;

		public MapAntiAliasing AntiAliasing
		{
			get
			{
				if (!this.m_antiAliasing.HasValue)
				{
					this.m_antiAliasing = this.MapDef.MapDef.EvaluateAntiAliasing(this.ReportScopeInstance, this.MapDef.RenderingContext.OdpContext);
				}
				return this.m_antiAliasing.Value;
			}
		}

		public MapTextAntiAliasingQuality TextAntiAliasingQuality
		{
			get
			{
				if (!this.m_textAntiAliasingQuality.HasValue)
				{
					this.m_textAntiAliasingQuality = this.MapDef.MapDef.EvaluateTextAntiAliasingQuality(this.ReportScopeInstance, this.MapDef.RenderingContext.OdpContext);
				}
				return this.m_textAntiAliasingQuality.Value;
			}
		}

		public double ShadowIntensity
		{
			get
			{
				if (!this.m_shadowIntensity.HasValue)
				{
					this.m_shadowIntensity = this.MapDef.MapDef.EvaluateShadowIntensity(this.ReportScopeInstance, this.MapDef.RenderingContext.OdpContext);
				}
				return this.m_shadowIntensity.Value;
			}
		}

		public string TileLanguage
		{
			get
			{
				if (!this.m_tileLanguageEvaluated)
				{
					this.m_tileLanguage = this.MapDef.MapDef.EvaluateTileLanguage(this.ReportScopeInstance, this.MapDef.RenderingContext.OdpContext);
					this.m_tileLanguageEvaluated = true;
				}
				return this.m_tileLanguage;
			}
		}

		public string PageName
		{
			get
			{
				if (!this.m_pageNameEvaluated)
				{
					if (base.m_reportElementDef.IsOldSnapshot)
					{
						this.m_pageName = null;
					}
					else
					{
						this.m_pageNameEvaluated = true;
						AspNetCore.ReportingServices.ReportIntermediateFormat.Map mapDef = this.MapDef.MapDef;
						ExpressionInfo pageName = mapDef.PageName;
						if (pageName != null)
						{
							if (pageName.IsExpression)
							{
								this.m_pageName = mapDef.EvaluatePageName(this.ReportScopeInstance, base.m_reportElementDef.RenderingContext.OdpContext);
							}
							else
							{
								this.m_pageName = pageName.StringValue;
							}
						}
					}
				}
				return this.m_pageName;
			}
		}

		private Map MapDef
		{
			get
			{
				return (Map)base.m_reportElementDef;
			}
		}

		private int WidthInPixels
		{
			get
			{
				return MappingHelper.ToIntPixels(((ReportItem)base.m_reportElementDef).Width, this.m_dpiX);
			}
		}

		private int HeightInPixels
		{
			get
			{
				return MappingHelper.ToIntPixels(((ReportItem)base.m_reportElementDef).Height, this.m_dpiX);
			}
		}

		internal MapInstance(Map reportItemDef)
			: base(reportItemDef)
		{
		}

		public void SetDpi(int xDpi, int yDpi)
		{
			this.m_dpiX = (float)xDpi;
			this.m_dpiY = (float)yDpi;
		}

		public void SetSize(double width, double height)
		{
			this.m_widthOverride = width;
			this.m_heightOverride = height;
		}

		public Stream GetImage()
		{
			bool flag = default(bool);
			return this.GetImage(DynamicImageInstance.ImageType.PNG, out flag);
		}

		public Stream GetImage(DynamicImageInstance.ImageType type)
		{
			bool flag = default(bool);
			return this.GetImage(type, out flag);
		}

		public Stream GetImage(out ActionInfoWithDynamicImageMapCollection actionImageMaps)
		{
			return this.GetImage(DynamicImageInstance.ImageType.PNG, out actionImageMaps);
		}

		public Stream GetImage(DynamicImageInstance.ImageType type, out ActionInfoWithDynamicImageMapCollection actionImageMaps)
		{
			try
			{
				Stream result = default(Stream);
				this.GetImage(type, out actionImageMaps, out result);
				return result;
			}
			catch (Exception exception)
			{
				actionImageMaps = null;
				return DynamicImageInstance.CreateExceptionImage(exception, this.WidthInPixels, this.HeightInPixels, this.m_dpiX, this.m_dpiY);
			}
		}

		private Stream GetImage(DynamicImageInstance.ImageType type, out bool hasImageMap)
		{
			ActionInfoWithDynamicImageMapCollection actionInfoWithDynamicImageMapCollection = default(ActionInfoWithDynamicImageMapCollection);
			Stream image = this.GetImage(type, out actionInfoWithDynamicImageMapCollection);
			hasImageMap = (actionInfoWithDynamicImageMapCollection != null);
			return image;
		}

		private void GetImage(DynamicImageInstance.ImageType type, out ActionInfoWithDynamicImageMapCollection actionImageMaps, out Stream image)
		{
			using (IMapMapper mapMapper = MapMapperFactory.CreateMapMapperInstance((Map)base.m_reportElementDef, base.GetDefaultFontFamily()))
			{
				mapMapper.DpiX = this.m_dpiX;
				mapMapper.DpiY = this.m_dpiY;
				mapMapper.WidthOverride = this.m_widthOverride;
				mapMapper.HeightOverride = this.m_heightOverride;
				mapMapper.RenderMap();
				image = mapMapper.GetImage(type);
				actionImageMaps = mapMapper.GetImageMaps();
			}
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_antiAliasing = null;
			this.m_textAntiAliasingQuality = null;
			this.m_shadowIntensity = null;
			this.m_tileLanguage = null;
			this.m_tileLanguageEvaluated = false;
			this.m_pageNameEvaluated = false;
			this.m_pageName = null;
		}

		public Stream GetCoreXml()
		{
			using (IMapMapper mapMapper = MapMapperFactory.CreateMapMapperInstance((Map)base.m_reportElementDef, base.GetDefaultFontFamily()))
			{
				mapMapper.DpiX = this.m_dpiX;
				mapMapper.DpiY = this.m_dpiY;
				mapMapper.WidthOverride = this.m_widthOverride;
				mapMapper.HeightOverride = this.m_heightOverride;
				mapMapper.RenderMap();
				return mapMapper.GetCoreXml();
			}
		}
	}
}
