using AspNetCore.Reporting.Map.WebForms;
using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class RuleMapper
	{
		protected RuleBase m_coreRule;

		protected MapAppearanceRule m_mapRule;

		protected MapControl m_coreMap;

		protected MapVectorLayer m_mapVectorLayer;

		protected MapMapper m_mapMapper;

		private string m_ruleFieldName;

		private bool? m_fieldNameBased = null;

		private CoreSpatialElementManager m_coreSpatialElementManager;

		private object m_startValue;

		private bool m_startValueEvaluated;

		private object m_endValue;

		private bool m_endValueEvaluated;

		private static string m_distinctBucketFieldName = "(Name)";

		private static int m_defaultBucketCount = 5;

		private bool IsRuleFieldDefined
		{
			get
			{
				return this.m_mapRule.DataValue != null;
			}
		}

		private bool IsRuleFieldScalar
		{
			get
			{
				if (this.m_coreRule.Field != "")
				{
					return this.m_coreSpatialElementManager.FieldDefinitions[this.m_coreRule.Field].Type != typeof(string);
				}
				return true;
			}
		}

		internal RuleMapper(MapAppearanceRule mapRule, VectorLayerMapper vectorLayerMapper, CoreSpatialElementManager coreSpatialElementManager)
		{
			this.m_mapRule = mapRule;
			this.m_mapVectorLayer = vectorLayerMapper.m_mapVectorLayer;
			this.m_coreMap = vectorLayerMapper.m_coreMap;
			this.m_coreSpatialElementManager = coreSpatialElementManager;
			this.m_mapMapper = vectorLayerMapper.m_mapMapper;
		}

		internal bool HasDataValue(ISpatialElement element)
		{
			if (this.m_mapRule.DataValue != null)
			{
				if (element[this.m_coreRule.Field] != null)
				{
					return this.IsValueInRange(this.m_coreRule.Field, element);
				}
				return false;
			}
			return true;
		}

		private bool IsValueInRange(string fieldName, ISpatialElement element)
		{
			Type type = ((AspNetCore.Reporting.Map.WebForms.Field)this.m_coreSpatialElementManager.FieldDefinitions.GetByName(fieldName)).Type;
			object startValue = this.GetStartValue(type);
			object endValue = this.GetEndValue(type);
			if (type == typeof(int))
			{
				if (startValue != null && (int)startValue > (int)element[fieldName])
				{
					return false;
				}
				if (endValue != null && (int)endValue < (int)element[fieldName])
				{
					return false;
				}
			}
			else if (type == typeof(double))
			{
				if (startValue != null && (double)startValue > (double)element[fieldName])
				{
					return false;
				}
				if (endValue != null && (double)endValue < (double)element[fieldName])
				{
					return false;
				}
			}
			else if (type == typeof(decimal))
			{
				if (startValue != null && (decimal)startValue > (decimal)element[fieldName])
				{
					return false;
				}
				if (endValue != null && (decimal)endValue < (decimal)element[fieldName])
				{
					return false;
				}
			}
			return true;
		}

		private object GetStartValue(Type fieldType)
		{
			if (!this.m_startValueEvaluated)
			{
				if (this.GetDistributionType() == MapRuleDistributionType.Custom)
				{
					MapBucketCollection mapBuckets = this.m_mapRule.MapBuckets;
					if (mapBuckets != null && mapBuckets.Count > 0)
					{
						ReportVariantProperty startValue = ((ReportElementCollectionBase<MapBucket>)mapBuckets)[0].StartValue;
						if (startValue != null)
						{
							if (!startValue.IsExpression)
							{
								this.m_startValue = startValue.Value;
							}
							this.m_startValue = ((ReportElementCollectionBase<MapBucket>)mapBuckets)[0].Instance.StartValue;
						}
					}
				}
				if (this.m_startValue == null)
				{
					ReportVariantProperty startValue2 = this.m_mapRule.StartValue;
					if (startValue2 != null)
					{
						if (!startValue2.IsExpression)
						{
							this.m_startValue = startValue2.Value;
						}
						this.m_startValue = this.m_mapRule.Instance.StartValue;
					}
				}
				if (this.m_startValue != null)
				{
					try
					{
						this.m_startValue = Convert.ChangeType(this.m_startValue, fieldType, CultureInfo.InvariantCulture);
					}
					catch (Exception e)
					{
						if (AsynchronousExceptionDetection.IsStoppingException(e))
						{
							throw;
						}
						this.m_startValue = null;
					}
				}
				this.m_startValueEvaluated = true;
			}
			return this.m_startValue;
		}

		private object GetEndValue(Type fieldType)
		{
			if (!this.m_endValueEvaluated)
			{
				if (this.GetDistributionType() == MapRuleDistributionType.Custom)
				{
					MapBucketCollection mapBuckets = this.m_mapRule.MapBuckets;
					if (mapBuckets != null && mapBuckets.Count > 0)
					{
						ReportVariantProperty endValue = ((ReportElementCollectionBase<MapBucket>)mapBuckets)[mapBuckets.Count - 1].EndValue;
						if (endValue != null)
						{
							if (!endValue.IsExpression)
							{
								this.m_endValue = endValue.Value;
							}
							this.m_endValue = ((ReportElementCollectionBase<MapBucket>)mapBuckets)[mapBuckets.Count - 1].Instance.EndValue;
						}
					}
				}
				if (this.m_endValue == null)
				{
					ReportVariantProperty endValue2 = this.m_mapRule.EndValue;
					if (endValue2 != null)
					{
						if (!endValue2.IsExpression)
						{
							this.m_endValue = endValue2.Value;
						}
						this.m_endValue = this.m_mapRule.Instance.EndValue;
					}
				}
				if (this.m_endValue != null)
				{
					try
					{
						this.m_endValue = Convert.ChangeType(this.m_endValue, fieldType, CultureInfo.InvariantCulture);
					}
					catch (Exception e)
					{
						if (AsynchronousExceptionDetection.IsStoppingException(e))
						{
							throw;
						}
						this.m_endValue = null;
					}
				}
				this.m_endValueEvaluated = true;
			}
			return this.m_endValue;
		}

		internal void SetRuleFieldValue(ISpatialElement spatialElement)
		{
			if (this.m_fieldNameBased.HasValue && this.m_fieldNameBased.Value)
			{
				return;
			}
			object obj = this.EvaluateRuleDataValue();
			if (obj != null)
			{
				if (!this.m_fieldNameBased.HasValue)
				{
					TypeCode typeCode = Type.GetTypeCode(obj.GetType());
					if (typeCode == TypeCode.String && ((string)obj).StartsWith("#", StringComparison.Ordinal))
					{
						this.m_ruleFieldName = SpatialDataMapper.GetUniqueFieldName(this.m_mapVectorLayer.Name, ((string)obj).Remove(0, 1));
						this.m_coreRule.Field = this.m_ruleFieldName;
						this.m_fieldNameBased = true;
						return;
					}
				}
				if (this.m_ruleFieldName == null)
				{
					this.m_ruleFieldName = this.m_coreSpatialElementManager.AddRuleField(obj);
					this.m_fieldNameBased = false;
					this.m_coreRule.Field = this.m_ruleFieldName;
				}
				this.m_coreSpatialElementManager.AddFieldValue(spatialElement, this.m_ruleFieldName, obj);
			}
		}

		protected void SetRuleFieldName()
		{
			ReportVariantProperty dataValue = this.m_mapRule.DataValue;
			if (dataValue == null)
			{
				this.m_ruleFieldName = RuleMapper.m_distinctBucketFieldName;
				this.m_coreRule.Field = this.m_ruleFieldName;
				this.m_fieldNameBased = true;
			}
			else if (this.m_mapVectorLayer.MapDataRegion == null)
			{
				object obj = this.EvaluateRuleDataValue();
				if (obj is string)
				{
					string text = (string)obj;
					if (text.StartsWith("#", StringComparison.Ordinal))
					{
						this.m_ruleFieldName = SpatialDataMapper.GetUniqueFieldName(this.m_mapVectorLayer.Name, text.Remove(0, 1));
						this.m_coreRule.Field = this.m_ruleFieldName;
						this.m_fieldNameBased = true;
					}
				}
			}
		}

		protected void SetRuleLegendProperties(RuleBase coreRule)
		{
			this.SetLegendText(coreRule);
			if (this.m_mapRule.LegendName != null)
			{
				coreRule.ShowInLegend = this.m_mapRule.LegendName;
			}
		}

		protected void SetLegendText(RuleBase coreRule)
		{
			ReportStringProperty legendText = this.m_mapRule.LegendText;
			if (legendText != null)
			{
				if (!legendText.IsExpression)
				{
					coreRule.LegendText = legendText.Value;
				}
				else
				{
					coreRule.LegendText = this.m_mapRule.Instance.LegendText;
				}
			}
			else
			{
				coreRule.LegendText = "";
			}
		}

		protected void SetRuleDistribution(RuleBase coreRule)
		{
			MapRuleDistributionType distributionType = this.GetDistributionType();
			if (distributionType != MapRuleDistributionType.Custom)
			{
				coreRule.DataGrouping = this.GetDataGrouping(distributionType);
				coreRule.FromValue = this.GetFromValue();
				coreRule.ToValue = this.GetToValue();
			}
			else
			{
				coreRule.DataGrouping = DataGrouping.EqualInterval;
			}
		}

		protected MapRuleDistributionType GetDistributionType()
		{
			ReportEnumProperty<MapRuleDistributionType> distributionType = this.m_mapRule.DistributionType;
			if (distributionType != null)
			{
				if (!distributionType.IsExpression)
				{
					return distributionType.Value;
				}
				return this.m_mapRule.Instance.DistributionType;
			}
			return MapRuleDistributionType.Optimal;
		}

		protected DataGrouping GetDataGrouping(MapRuleDistributionType distributionType)
		{
			switch (distributionType)
			{
			case MapRuleDistributionType.EqualDistribution:
				return DataGrouping.EqualDistribution;
			case MapRuleDistributionType.EqualInterval:
				return DataGrouping.EqualInterval;
			default:
				return DataGrouping.Optimal;
			}
		}

		protected string GetFromValue()
		{
			ReportVariantProperty startValue = this.m_mapRule.StartValue;
			if (startValue != null)
			{
				if (!startValue.IsExpression)
				{
					return this.ConvertBucketValueToString(startValue.Value);
				}
				return this.ConvertBucketValueToString(this.m_mapRule.Instance.StartValue);
			}
			return "";
		}

		protected string GetToValue()
		{
			ReportVariantProperty endValue = this.m_mapRule.EndValue;
			if (endValue != null)
			{
				if (!endValue.IsExpression)
				{
					return this.ConvertBucketValueToString(endValue.Value);
				}
				return this.ConvertBucketValueToString(this.m_mapRule.Instance.EndValue);
			}
			return "";
		}

		protected string GetFromValue(MapBucket bucket)
		{
			ReportVariantProperty startValue = bucket.StartValue;
			if (startValue != null)
			{
				if (!startValue.IsExpression)
				{
					return this.ConvertBucketValueToString(startValue.Value);
				}
				return this.ConvertBucketValueToString(bucket.Instance.StartValue);
			}
			return "";
		}

		protected string GetToValue(MapBucket bucket)
		{
			ReportVariantProperty endValue = bucket.EndValue;
			if (endValue != null)
			{
				if (!endValue.IsExpression)
				{
					return this.ConvertBucketValueToString(endValue.Value);
				}
				return this.ConvertBucketValueToString(bucket.Instance.EndValue);
			}
			return "";
		}

		internal ShapeRule CreatePolygonRule()
		{
			ShapeRule shapeRule = (ShapeRule)(this.m_coreRule = new ShapeRule());
			shapeRule.BorderColor = Color.Empty;
			shapeRule.Text = "";
			shapeRule.Category = this.m_mapVectorLayer.Name;
			shapeRule.Field = "";
			this.m_coreMap.ShapeRules.Add(shapeRule);
			this.SetRuleFieldName();
			return shapeRule;
		}

		internal virtual SymbolRule CreateSymbolRule()
		{
			SymbolRule symbolRule = (SymbolRule)(this.m_coreRule = new SymbolRule());
			symbolRule.Category = this.m_mapVectorLayer.Name;
			symbolRule.Field = "";
			this.m_coreMap.SymbolRules.Add(symbolRule);
			this.SetRuleFieldName();
			return symbolRule;
		}

		protected void InitializePredefinedSymbols(PredefinedSymbol predefinedSymbol, PointTemplateMapper symbolTemplateMapper)
		{
			predefinedSymbol.BorderColor = symbolTemplateMapper.GetBorderColor(false);
			predefinedSymbol.BorderStyle = symbolTemplateMapper.GetBorderStyle(false);
			predefinedSymbol.BorderWidth = symbolTemplateMapper.GetBorderWidth(false);
			predefinedSymbol.Font = symbolTemplateMapper.GetFont(false);
			predefinedSymbol.GradientType = symbolTemplateMapper.GetGradientType(false);
			predefinedSymbol.HatchStyle = symbolTemplateMapper.GetHatchStyle(false);
			predefinedSymbol.SecondaryColor = symbolTemplateMapper.GetBackGradientEndColor(false);
			predefinedSymbol.ShadowOffset = symbolTemplateMapper.GetShadowOffset(false);
			predefinedSymbol.TextColor = symbolTemplateMapper.GetTextColor(false);
			predefinedSymbol.LegendText = "";
			predefinedSymbol.Text = "";
		}

		protected int GetBucketCount()
		{
			if (!this.IsRuleFieldDefined)
			{
				return this.m_coreSpatialElementManager.GetSpatialElementCount();
			}
			MapRuleDistributionType distributionType = this.GetDistributionType();
			ReportIntProperty bucketCount = this.m_mapRule.BucketCount;
			int num = RuleMapper.m_defaultBucketCount;
			if (bucketCount != null)
			{
				num = (bucketCount.IsExpression ? this.m_mapRule.Instance.BucketCount : bucketCount.Value);
			}
			if (!this.IsRuleFieldScalar)
			{
				return this.m_coreSpatialElementManager.GetDistinctValuesCount(this.m_coreRule.Field);
			}
			switch (distributionType)
			{
			case MapRuleDistributionType.Optimal:
			case MapRuleDistributionType.EqualDistribution:
				return Math.Min(num, this.m_coreSpatialElementManager.GetDistinctValuesCount(this.m_coreRule.Field));
			case MapRuleDistributionType.Custom:
			{
				MapBucketCollection mapBuckets = this.m_mapRule.MapBuckets;
				if (mapBuckets == null)
				{
					throw new RenderingObjectModelException(RPRes.rsMapLayerMissingProperty(RPRes.rsObjectTypeMap, this.m_mapRule.MapDef.Name, this.m_mapVectorLayer.Name, "MapBuckets"));
				}
				return mapBuckets.Count;
			}
			default:
				return num;
			}
		}

		private string ConvertBucketValueToString(object value)
		{
			if (value == null)
			{
				return "";
			}
			if (value is IFormattable)
			{
				return ((IFormattable)value).ToString("", CultureInfo.CurrentCulture);
			}
			return value.ToString();
		}

		private object EvaluateRuleDataValue()
		{
			ReportVariantProperty dataValue = this.m_mapRule.DataValue;
			object result = null;
			if (dataValue != null)
			{
				result = (dataValue.IsExpression ? this.m_mapRule.Instance.DataValue : dataValue.Value);
			}
			return result;
		}
	}
}
