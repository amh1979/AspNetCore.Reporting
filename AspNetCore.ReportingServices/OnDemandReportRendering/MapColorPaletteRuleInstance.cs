using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapColorPaletteRuleInstance : MapColorRuleInstance
	{
		private MapColorPaletteRule m_defObject;

		private MapPalette? m_palette;

		public MapPalette Palette
		{
			get
			{
				if (!this.m_palette.HasValue)
				{
					this.m_palette = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorPaletteRule)this.m_defObject.MapColorRuleDef).EvaluatePalette(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_palette.Value;
			}
		}

		internal MapColorPaletteRuleInstance(MapColorPaletteRule defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_palette = null;
		}
	}
}
