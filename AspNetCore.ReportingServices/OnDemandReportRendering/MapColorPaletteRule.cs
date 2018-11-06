using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapColorPaletteRule : MapColorRule
	{
		private ReportEnumProperty<MapPalette> m_palette;

		public ReportEnumProperty<MapPalette> Palette
		{
			get
			{
				if (this.m_palette == null && this.MapColorPaletteRuleDef.Palette != null)
				{
					this.m_palette = new ReportEnumProperty<MapPalette>(this.MapColorPaletteRuleDef.Palette.IsExpression, this.MapColorPaletteRuleDef.Palette.OriginalText, EnumTranslator.TranslateMapPalette(this.MapColorPaletteRuleDef.Palette.StringValue, null));
				}
				return this.m_palette;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule MapColorPaletteRuleDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule)base.MapAppearanceRuleDef;
			}
		}

		public new MapColorPaletteRuleInstance Instance
		{
			get
			{
				return (MapColorPaletteRuleInstance)this.GetInstance();
			}
		}

		internal MapColorPaletteRule(AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule defObject, MapVectorLayer mapVectorLayer, Map map)
			: base(defObject, mapVectorLayer, map)
		{
		}

		internal override MapAppearanceRuleInstance GetInstance()
		{
			if (base.m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (base.m_instance == null)
			{
				base.m_instance = new MapColorPaletteRuleInstance(this);
			}
			return base.m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
		}
	}
}
