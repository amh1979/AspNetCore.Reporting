namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class GaugePanelItemInstance : BaseInstance
	{
		private GaugePanelItem m_defObject;

		private StyleInstance m_style;

		private double? m_top;

		private double? m_left;

		private double? m_height;

		private double? m_width;

		private int? m_zIndex;

		private bool? m_hidden;

		private string m_toolTip;

		public StyleInstance Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new StyleInstance(this.m_defObject, this.m_defObject.GaugePanelDef, this.m_defObject.GaugePanelDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public double Top
		{
			get
			{
				if (!this.m_top.HasValue)
				{
					this.m_top = this.m_defObject.GaugePanelItemDef.EvaluateTop(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_top.Value;
			}
		}

		public double Left
		{
			get
			{
				if (!this.m_left.HasValue)
				{
					this.m_left = this.m_defObject.GaugePanelItemDef.EvaluateLeft(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_left.Value;
			}
		}

		public double Height
		{
			get
			{
				if (!this.m_height.HasValue)
				{
					this.m_height = this.m_defObject.GaugePanelItemDef.EvaluateHeight(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_height.Value;
			}
		}

		public double Width
		{
			get
			{
				if (!this.m_width.HasValue)
				{
					this.m_width = this.m_defObject.GaugePanelItemDef.EvaluateWidth(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_width.Value;
			}
		}

		public int ZIndex
		{
			get
			{
				if (!this.m_zIndex.HasValue)
				{
					this.m_zIndex = this.m_defObject.GaugePanelItemDef.EvaluateZIndex(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_zIndex.Value;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!this.m_hidden.HasValue)
				{
					this.m_hidden = this.m_defObject.GaugePanelItemDef.EvaluateHidden(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_hidden.Value;
			}
		}

		public string ToolTip
		{
			get
			{
				if (this.m_toolTip == null)
				{
					this.m_toolTip = this.m_defObject.GaugePanelItemDef.EvaluateToolTip(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_toolTip;
			}
		}

		internal GaugePanelItemInstance(GaugePanelItem defObject)
			: base(defObject.GaugePanelDef)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_top = null;
			this.m_left = null;
			this.m_height = null;
			this.m_width = null;
			this.m_zIndex = null;
			this.m_hidden = null;
			this.m_toolTip = null;
		}
	}
}
