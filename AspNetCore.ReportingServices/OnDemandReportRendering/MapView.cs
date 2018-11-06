using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapView
	{
		protected Map m_map;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapView m_defObject;

		protected MapViewInstance m_instance;

		private ReportDoubleProperty m_zoom;

		public ReportDoubleProperty Zoom
		{
			get
			{
				if (this.m_zoom == null && this.m_defObject.Zoom != null)
				{
					this.m_zoom = new ReportDoubleProperty(this.m_defObject.Zoom);
				}
				return this.m_zoom;
			}
		}

		internal Map MapDef
		{
			get
			{
				return this.m_map;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapView MapViewDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		internal MapViewInstance Instance
		{
			get
			{
				return this.GetInstance();
			}
		}

		internal MapView(AspNetCore.ReportingServices.ReportIntermediateFormat.MapView defObject, Map map)
		{
			this.m_defObject = defObject;
			this.m_map = map;
		}

		internal abstract MapViewInstance GetInstance();

		internal virtual void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
		}
	}
}
