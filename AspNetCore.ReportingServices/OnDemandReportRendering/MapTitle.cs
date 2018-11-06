using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapTitle : MapDockableSubItem
	{
		private ReportStringProperty m_text;

		private ReportDoubleProperty m_angle;

		private ReportSizeProperty m_textShadowOffset;

		public string Name
		{
			get
			{
				return this.MapTitleDef.Name;
			}
		}

		public ReportStringProperty Text
		{
			get
			{
				if (this.m_text == null && this.MapTitleDef.Text != null)
				{
					this.m_text = new ReportStringProperty(this.MapTitleDef.Text);
				}
				return this.m_text;
			}
		}

		public ReportDoubleProperty Angle
		{
			get
			{
				if (this.m_angle == null && this.MapTitleDef.Angle != null)
				{
					this.m_angle = new ReportDoubleProperty(this.MapTitleDef.Angle);
				}
				return this.m_angle;
			}
		}

		public ReportSizeProperty TextShadowOffset
		{
			get
			{
				if (this.m_textShadowOffset == null && this.MapTitleDef.TextShadowOffset != null)
				{
					this.m_textShadowOffset = new ReportSizeProperty(this.MapTitleDef.TextShadowOffset);
				}
				return this.m_textShadowOffset;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapTitle MapTitleDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapTitle)base.m_defObject;
			}
		}

		public new MapTitleInstance Instance
		{
			get
			{
				return (MapTitleInstance)this.GetInstance();
			}
		}

		internal MapTitle(AspNetCore.ReportingServices.ReportIntermediateFormat.MapTitle defObject, Map map)
			: base(defObject, map)
		{
		}

		internal override MapSubItemInstance GetInstance()
		{
			if (base.m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (base.m_instance == null)
			{
				base.m_instance = new MapTitleInstance(this);
			}
			return (MapSubItemInstance)base.m_instance;
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
