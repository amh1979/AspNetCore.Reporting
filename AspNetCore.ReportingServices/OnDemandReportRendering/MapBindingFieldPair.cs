using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapBindingFieldPair : MapObjectCollectionItem
	{
		private Map m_map;

		private MapVectorLayer m_mapVectorLayer;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair m_defObject;

		private ReportStringProperty m_fieldName;

		private ReportVariantProperty m_bindingExpression;

		public ReportStringProperty FieldName
		{
			get
			{
				if (this.m_fieldName == null && this.m_defObject.FieldName != null)
				{
					this.m_fieldName = new ReportStringProperty(this.m_defObject.FieldName);
				}
				return this.m_fieldName;
			}
		}

		public ReportVariantProperty BindingExpression
		{
			get
			{
				if (this.m_bindingExpression == null && this.m_defObject.BindingExpression != null)
				{
					this.m_bindingExpression = new ReportVariantProperty(this.m_defObject.BindingExpression);
				}
				return this.m_bindingExpression;
			}
		}

		internal Map MapDef
		{
			get
			{
				return this.m_map;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair MapBindingFieldPairDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public MapBindingFieldPairInstance Instance
		{
			get
			{
				if (this.m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					base.m_instance = new MapBindingFieldPairInstance(this);
				}
				return (MapBindingFieldPairInstance)base.m_instance;
			}
		}

		internal IReportScope ReportScope
		{
			get
			{
				if (this.m_mapVectorLayer != null)
				{
					return this.m_mapVectorLayer.ReportScope;
				}
				return this.MapDef.ReportScope;
			}
		}

		internal MapBindingFieldPair(AspNetCore.ReportingServices.ReportIntermediateFormat.MapBindingFieldPair defObject, MapVectorLayer mapVectorLayer, Map map)
		{
			this.m_defObject = defObject;
			this.m_mapVectorLayer = mapVectorLayer;
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
