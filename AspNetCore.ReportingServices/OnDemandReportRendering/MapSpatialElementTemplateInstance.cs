namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapSpatialElementTemplateInstance : BaseInstance
	{
		private MapSpatialElementTemplate m_defObject;

		private StyleInstance m_style;

		private bool? m_hidden;

		private double? m_offsetX;

		private double? m_offsetY;

		private string m_label;

		private bool m_labelEvaluated;

		private string m_toolTip;

		private bool m_toolTipEvaluated;

		private string m_dataElementLabel;

		private bool m_dataElementLabelEvaluated;

		public StyleInstance Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new StyleInstance(this.m_defObject, this.m_defObject.ReportScope, this.m_defObject.MapDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!this.m_hidden.HasValue)
				{
					this.m_hidden = this.m_defObject.MapSpatialElementTemplateDef.EvaluateHidden(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_hidden.Value;
			}
		}

		public double OffsetX
		{
			get
			{
				if (!this.m_offsetX.HasValue)
				{
					this.m_offsetX = this.m_defObject.MapSpatialElementTemplateDef.EvaluateOffsetX(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_offsetX.Value;
			}
		}

		public double OffsetY
		{
			get
			{
				if (!this.m_offsetY.HasValue)
				{
					this.m_offsetY = this.m_defObject.MapSpatialElementTemplateDef.EvaluateOffsetY(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_offsetY.Value;
			}
		}

		public string Label
		{
			get
			{
				if (!this.m_labelEvaluated)
				{
					this.m_label = this.m_defObject.MapSpatialElementTemplateDef.EvaluateLabel(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
					this.m_labelEvaluated = true;
				}
				return this.m_label;
			}
		}

		public string ToolTip
		{
			get
			{
				if (!this.m_toolTipEvaluated)
				{
					this.m_toolTip = this.m_defObject.MapSpatialElementTemplateDef.EvaluateToolTip(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
					this.m_toolTipEvaluated = true;
				}
				return this.m_toolTip;
			}
		}

		public string DataElementLabel
		{
			get
			{
				if (!this.m_dataElementLabelEvaluated)
				{
					this.m_dataElementLabel = this.m_defObject.MapSpatialElementTemplateDef.EvaluateDataElementLabel(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
					this.m_dataElementLabelEvaluated = true;
				}
				if (this.m_dataElementLabel == null)
				{
					return this.Label;
				}
				return this.m_dataElementLabel;
			}
		}

		internal MapSpatialElementTemplateInstance(MapSpatialElementTemplate defObject)
			: base(defObject.ReportScope)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_hidden = null;
			this.m_offsetX = null;
			this.m_offsetY = null;
			this.m_label = null;
			this.m_labelEvaluated = false;
			this.m_toolTip = null;
			this.m_toolTipEvaluated = false;
			this.m_dataElementLabel = null;
			this.m_dataElementLabelEvaluated = false;
		}
	}
}
