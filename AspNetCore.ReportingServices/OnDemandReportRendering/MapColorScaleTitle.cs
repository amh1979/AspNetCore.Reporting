using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapColorScaleTitle : IROMStyleDefinitionContainer
	{
		private Map m_map;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScaleTitle m_defObject;

		private MapColorScaleTitleInstance m_instance;

		private Style m_style;

		private ReportStringProperty m_caption;

		public Style Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new Style(this.m_map, this.m_map.ReportScope, this.m_defObject, this.m_map.RenderingContext);
				}
				return this.m_style;
			}
		}

		public ReportStringProperty Caption
		{
			get
			{
				if (this.m_caption == null && this.m_defObject.Caption != null)
				{
					this.m_caption = new ReportStringProperty(this.m_defObject.Caption);
				}
				return this.m_caption;
			}
		}

		internal Map MapDef
		{
			get
			{
				return this.m_map;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScaleTitle MapColorScaleTitleDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public MapColorScaleTitleInstance Instance
		{
			get
			{
				if (this.m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new MapColorScaleTitleInstance(this);
				}
				return this.m_instance;
			}
		}

		internal MapColorScaleTitle(AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorScaleTitle defObject, Map map)
		{
			this.m_defObject = defObject;
			this.m_map = map;
		}

		internal void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
		}
	}
}
