namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLegendTitleInstance : BaseInstance
	{
		private MapLegendTitle m_defObject;

		private StyleInstance m_style;

		private string m_caption;

		private bool m_captionEvaluated;

		private MapLegendTitleSeparator? m_titleSeparator;

		private ReportColor m_titleSeparatorColor;

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

		public string Caption
		{
			get
			{
				if (!this.m_captionEvaluated)
				{
					this.m_caption = this.m_defObject.MapLegendTitleDef.EvaluateCaption(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
					this.m_captionEvaluated = true;
				}
				return this.m_caption;
			}
		}

		public MapLegendTitleSeparator TitleSeparator
		{
			get
			{
				if (!this.m_titleSeparator.HasValue)
				{
					this.m_titleSeparator = this.m_defObject.MapLegendTitleDef.EvaluateTitleSeparator(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_titleSeparator.Value;
			}
		}

		public ReportColor TitleSeparatorColor
		{
			get
			{
				if (this.m_titleSeparatorColor == null)
				{
					this.m_titleSeparatorColor = new ReportColor(this.m_defObject.MapLegendTitleDef.EvaluateTitleSeparatorColor(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return this.m_titleSeparatorColor;
			}
		}

		internal MapLegendTitleInstance(MapLegendTitle defObject)
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
			this.m_caption = null;
			this.m_captionEvaluated = false;
			this.m_titleSeparator = null;
			this.m_titleSeparatorColor = null;
		}
	}
}
