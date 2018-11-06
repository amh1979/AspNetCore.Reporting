using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapSpatialElement : MapObjectCollectionItem
	{
		protected Map m_map;

		protected MapVectorLayer m_mapVectorLayer;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialElement m_defObject;

		private MapFieldCollection m_mapFields;

		public string VectorData
		{
			get
			{
				return this.m_defObject.VectorData;
			}
		}

		public MapFieldCollection MapFields
		{
			get
			{
				if (this.m_mapFields == null && this.m_defObject.MapFields != null)
				{
					this.m_mapFields = new MapFieldCollection(this, this.m_map);
				}
				return this.m_mapFields;
			}
		}

		internal IReportScope ReportScope
		{
			get
			{
				return this.m_mapVectorLayer.ReportScope;
			}
		}

		internal Map MapDef
		{
			get
			{
				return this.m_map;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialElement MapSpatialElementDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		internal MapSpatialElementInstance Instance
		{
			get
			{
				return this.GetInstance();
			}
		}

		internal MapSpatialElement(AspNetCore.ReportingServices.ReportIntermediateFormat.MapSpatialElement defObject, MapVectorLayer mapVectorLayer, Map map)
		{
			this.m_defObject = defObject;
			this.m_mapVectorLayer = mapVectorLayer;
			this.m_map = map;
		}

		internal abstract MapSpatialElementInstance GetInstance();

		internal override void SetNewContext()
		{
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
			if (this.m_mapFields != null)
			{
				this.m_mapFields.SetNewContext();
			}
		}
	}
}
