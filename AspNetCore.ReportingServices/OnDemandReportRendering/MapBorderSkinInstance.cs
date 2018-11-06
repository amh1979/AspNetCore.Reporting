namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapBorderSkinInstance : BaseInstance
	{
		private MapBorderSkin m_defObject;

		private StyleInstance m_style;

		private MapBorderSkinType? m_mapBorderSkinType;

		public StyleInstance Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new StyleInstance(this.m_defObject, this.m_defObject.MapDef.ReportScope, this.m_defObject.MapDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public MapBorderSkinType MapBorderSkinType
		{
			get
			{
				if (!this.m_mapBorderSkinType.HasValue)
				{
					this.m_mapBorderSkinType = this.m_defObject.MapBorderSkinDef.EvaluateMapBorderSkinType(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_mapBorderSkinType.Value;
			}
		}

		internal MapBorderSkinInstance(MapBorderSkin defObject)
			: base(defObject.MapDef.ReportScope)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_mapBorderSkinType = null;
		}
	}
}
