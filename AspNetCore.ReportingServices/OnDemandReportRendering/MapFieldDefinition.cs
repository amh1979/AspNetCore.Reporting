using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapFieldDefinition : MapObjectCollectionItem
	{
		private Map m_map;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapFieldDefinition m_defObject;

		public string Name
		{
			get
			{
				return this.m_defObject.Name;
			}
		}

		public MapDataType DataType
		{
			get
			{
				return this.m_defObject.DataType;
			}
		}

		internal Map MapDef
		{
			get
			{
				return this.m_map;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapFieldDefinition MapFieldDefinitionDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public MapFieldDefinitionInstance Instance
		{
			get
			{
				if (this.m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					base.m_instance = new MapFieldDefinitionInstance(this);
				}
				return (MapFieldDefinitionInstance)base.m_instance;
			}
		}

		internal MapFieldDefinition(AspNetCore.ReportingServices.ReportIntermediateFormat.MapFieldDefinition defObject, Map map)
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
