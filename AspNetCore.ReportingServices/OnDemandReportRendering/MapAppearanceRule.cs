using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapAppearanceRule
	{
		protected Map m_map;

		protected MapVectorLayer m_mapVectorLayer;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapAppearanceRule m_defObject;

		protected MapAppearanceRuleInstance m_instance;

		private ReportVariantProperty m_dataValue;

		private ReportEnumProperty<MapRuleDistributionType> m_distributionType;

		private ReportIntProperty m_bucketCount;

		private ReportVariantProperty m_startValue;

		private ReportVariantProperty m_endValue;

		private MapBucketCollection m_mapBuckets;

		private ReportStringProperty m_legendText;

		public string DataElementName
		{
			get
			{
				return this.m_defObject.DataElementName;
			}
		}

		public DataElementOutputTypes DataElementOutput
		{
			get
			{
				return this.m_defObject.DataElementOutput;
			}
		}

		public ReportVariantProperty DataValue
		{
			get
			{
				if (this.m_dataValue == null && this.m_defObject.DataValue != null)
				{
					this.m_dataValue = new ReportVariantProperty(this.m_defObject.DataValue);
				}
				return this.m_dataValue;
			}
		}

		public ReportEnumProperty<MapRuleDistributionType> DistributionType
		{
			get
			{
				if (this.m_distributionType == null && this.m_defObject.DistributionType != null)
				{
					this.m_distributionType = new ReportEnumProperty<MapRuleDistributionType>(this.m_defObject.DistributionType.IsExpression, this.m_defObject.DistributionType.OriginalText, EnumTranslator.TranslateMapRuleDistributionType(this.m_defObject.DistributionType.StringValue, null));
				}
				return this.m_distributionType;
			}
		}

		public ReportIntProperty BucketCount
		{
			get
			{
				if (this.m_bucketCount == null && this.m_defObject.BucketCount != null)
				{
					this.m_bucketCount = new ReportIntProperty(this.m_defObject.BucketCount.IsExpression, this.m_defObject.BucketCount.OriginalText, this.m_defObject.BucketCount.IntValue, 0);
				}
				return this.m_bucketCount;
			}
		}

		public ReportVariantProperty StartValue
		{
			get
			{
				if (this.m_startValue == null && this.m_defObject.StartValue != null)
				{
					this.m_startValue = new ReportVariantProperty(this.m_defObject.StartValue);
				}
				return this.m_startValue;
			}
		}

		public ReportVariantProperty EndValue
		{
			get
			{
				if (this.m_endValue == null && this.m_defObject.EndValue != null)
				{
					this.m_endValue = new ReportVariantProperty(this.m_defObject.EndValue);
				}
				return this.m_endValue;
			}
		}

		public MapBucketCollection MapBuckets
		{
			get
			{
				if (this.m_mapBuckets == null && this.m_defObject.MapBuckets != null)
				{
					this.m_mapBuckets = new MapBucketCollection(this, this.m_map);
				}
				return this.m_mapBuckets;
			}
		}

		public string LegendName
		{
			get
			{
				return this.m_defObject.LegendName;
			}
		}

		public ReportStringProperty LegendText
		{
			get
			{
				if (this.m_legendText == null && this.m_defObject.LegendText != null)
				{
					this.m_legendText = new ReportStringProperty(this.m_defObject.LegendText);
				}
				return this.m_legendText;
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

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapAppearanceRule MapAppearanceRuleDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public MapAppearanceRuleInstance Instance
		{
			get
			{
				return this.GetInstance();
			}
		}

		internal MapAppearanceRule(AspNetCore.ReportingServices.ReportIntermediateFormat.MapAppearanceRule defObject, MapVectorLayer mapVectorLayer, Map map)
		{
			this.m_defObject = defObject;
			this.m_mapVectorLayer = mapVectorLayer;
			this.m_map = map;
		}

		internal abstract MapAppearanceRuleInstance GetInstance();

		internal virtual void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
			if (this.m_mapBuckets != null)
			{
				this.m_mapBuckets.SetNewContext();
			}
		}
	}
}
