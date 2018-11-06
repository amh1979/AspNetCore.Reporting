using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapFieldName : MapObjectCollectionItem
	{
		private Map m_map;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapFieldName m_defObject;

		private ReportStringProperty m_name;

		public ReportStringProperty Name
		{
			get
			{
				if (this.m_name == null && this.m_defObject.Name != null)
				{
					this.m_name = new ReportStringProperty(this.m_defObject.Name);
				}
				return this.m_name;
			}
		}

		internal Map MapDef
		{
			get
			{
				return this.m_map;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapFieldName MapFieldNameDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public MapFieldNameInstance Instance
		{
			get
			{
				if (this.m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					base.m_instance = new MapFieldNameInstance(this);
				}
				return (MapFieldNameInstance)base.m_instance;
			}
		}

		internal MapFieldName(AspNetCore.ReportingServices.ReportIntermediateFormat.MapFieldName defObject, Map map)
		{
			this.m_defObject = defObject;
			this.m_map = map;
		}

		internal override void SetNewContext()
		{
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
		}
	}
}
