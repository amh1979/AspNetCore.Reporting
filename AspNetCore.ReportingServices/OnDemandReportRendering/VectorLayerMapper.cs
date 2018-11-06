using AspNetCore.Reporting.Map.WebForms;
using AspNetCore.ReportingServices.ReportProcessing;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class VectorLayerMapper
	{
		internal MapVectorLayer m_mapVectorLayer;

		internal MapControl m_coreMap;

		internal MapMapper m_mapMapper;

		protected SpatialDataMapper m_spatialDataMapper;

		protected PointTemplateMapper m_pointTemplateMapper;

		private ColorRuleMapper m_pointColorRuleMapper;

		private SizeRuleMapper m_pointlSizeRuleMapper;

		private MarkerRuleMapper m_pointMarkerRuleMapper;

		private CoreSymbolManager m_symbolManager;

		private Dictionary<SpatialElementKey, SpatialElementInfoGroup> m_spatialElementsDictionary = new Dictionary<SpatialElementKey, SpatialElementInfoGroup>();

		protected bool IsEmbeddedLayer
		{
			get
			{
				return this.SpatialElementCollection != null;
			}
		}

		protected abstract ISpatialElementCollection SpatialElementCollection
		{
			get;
		}

		internal VectorLayerMapper(MapVectorLayer mapVectorLayer, MapControl coreMap, MapMapper mapMapper)
		{
			this.m_mapVectorLayer = mapVectorLayer;
			this.m_coreMap = coreMap;
			this.m_mapMapper = mapMapper;
		}

		internal void Render()
		{
			this.PopulateSpatialElements();
			this.CreateRules();
			this.RenderSpatialElements();
			this.RenderRules();
			this.UpdateView();
		}

		private void PopulateSpatialElements()
		{
			this.CreateSpatialDataMapper();
			if (this.m_spatialDataMapper != null)
			{
				this.m_spatialDataMapper.Populate();
			}
		}

		private void CreateSpatialDataMapper()
		{
			MapSpatialData mapSpatialDatum = this.m_mapVectorLayer.MapSpatialData;
			if (this.IsEmbeddedLayer)
			{
				this.m_spatialDataMapper = new EmbeddedSpatialDataMapper(this, this.m_spatialElementsDictionary, this.SpatialElementCollection, this.GetSpatialElementManager(), this.m_coreMap, this.m_mapMapper);
			}
			else if (this.m_mapVectorLayer.MapSpatialData is MapSpatialDataSet)
			{
				this.m_spatialDataMapper = new SpatialDataSetMapper(this, this.m_spatialElementsDictionary, this.GetSpatialElementManager(), this.m_coreMap, this.m_mapMapper);
			}
			else if (this.m_mapVectorLayer.MapSpatialData is MapShapefile)
			{
				this.m_spatialDataMapper = new ShapefileMapper(this, this.m_spatialElementsDictionary, this.m_coreMap, this.m_mapMapper);
			}
			else
			{
				this.m_spatialDataMapper = null;
			}
		}

		private void RenderGrouping(MapMember mapMember)
		{
			if (!mapMember.IsStatic)
			{
				MapDynamicMemberInstance mapDynamicMemberInstance = (MapDynamicMemberInstance)mapMember.Instance;
				mapDynamicMemberInstance.ResetContext();
				while (mapDynamicMemberInstance.MoveNext())
				{
					if (mapMember.ChildMapMember != null)
					{
						this.RenderGrouping(mapMember.ChildMapMember);
					}
					else
					{
						this.RenderInnerMostMember();
					}
				}
			}
			else if (mapMember.ChildMapMember != null)
			{
				this.RenderGrouping(mapMember.ChildMapMember);
			}
			else
			{
				this.RenderInnerMostMember();
			}
		}

		private void RenderSpatialElements()
		{
			MapDataRegion mapDataRegion = this.m_mapVectorLayer.MapDataRegion;
			if (mapDataRegion != null)
			{
				this.RenderGrouping(mapDataRegion.MapMember);
			}
			this.RenderNonBoundSpatialElements();
		}

		private void RenderInnerMostMember()
		{
			SpatialElementInfoGroup spatialElementInfoGroup = (this.m_spatialDataMapper == null) ? this.CreateSpatialElementFromDataRegion() : this.GetSpatialElementsFromDataRegionKey();
			if (spatialElementInfoGroup != null)
			{
				if (spatialElementInfoGroup.BoundToData)
				{
					throw new RenderingObjectModelException(RPRes.rsMapSpatialElementHasMoreThanOnMatchingGroupInstance(RPRes.rsObjectTypeMap, this.m_mapVectorLayer.MapDef.Name, this.m_mapVectorLayer.Name));
				}
				this.RenderSpatialElementGroup(spatialElementInfoGroup, true);
				spatialElementInfoGroup.BoundToData = true;
			}
		}

		private void RenderSpatialElementGroup(SpatialElementInfoGroup group, bool hasScope)
		{
			foreach (SpatialElementInfo element in group.Elements)
			{
				this.RenderSpatialElement(element, hasScope);
			}
		}

		protected void InitializeSpatialElement(ISpatialElement spatialElement)
		{
			spatialElement.Text = "";
		}

		private void RenderNonBoundSpatialElements()
		{
			if (this.m_spatialDataMapper != null)
			{
				MapDataRegion mapDataRegion = this.m_mapVectorLayer.MapDataRegion;
				foreach (KeyValuePair<SpatialElementKey, SpatialElementInfoGroup> item in this.m_spatialElementsDictionary)
				{
					if (!item.Value.BoundToData)
					{
						this.RenderSpatialElementGroup(item.Value, mapDataRegion == null);
					}
				}
			}
		}

		private SpatialElementInfoGroup GetSpatialElementsFromDataRegionKey()
		{
			MapBindingFieldPairCollection mapBindingFieldPairs = this.m_mapVectorLayer.MapBindingFieldPairs;
			if (mapBindingFieldPairs != null)
			{
				SpatialElementKey spatialElementKey = VectorLayerMapper.CreateDataRegionSpatialElementKey(mapBindingFieldPairs);
				this.ValidateKey(spatialElementKey, mapBindingFieldPairs);
				SpatialElementInfoGroup result = default(SpatialElementInfoGroup);
				if (this.m_spatialElementsDictionary.TryGetValue(spatialElementKey, out result))
				{
					return result;
				}
			}
			return null;
		}

		internal static SpatialElementKey CreateDataRegionSpatialElementKey(MapBindingFieldPairCollection mapBindingFieldPairs)
		{
			List<object> list = new List<object>();
			for (int i = 0; i < mapBindingFieldPairs.Count; i++)
			{
				list.Add(VectorLayerMapper.EvaluateBindingExpression(((ReportElementCollectionBase<MapBindingFieldPair>)mapBindingFieldPairs)[i]));
			}
			return new SpatialElementKey(list);
		}

		internal void ValidateKey(SpatialElementKey spatialElementKey, MapBindingFieldPairCollection mapBindingFieldPairs)
		{
			if (this.m_spatialDataMapper.KeyTypes != null)
			{
				int num = 0;
				while (true)
				{
					if (num < spatialElementKey.KeyValues.Count)
					{
						object obj = spatialElementKey.KeyValues[num];
						if (obj != null)
						{
							Type type = obj.GetType();
							Type type2 = this.m_spatialDataMapper.KeyTypes[num];
							if (type2 != null && type != type2)
							{
								object obj2 = VectorLayerMapper.Convert(obj, type, type2);
								if (obj2 == null)
								{
									break;
								}
								spatialElementKey.KeyValues[num] = obj2;
							}
						}
						num++;
						continue;
					}
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsMapFieldBindingExpressionTypeMismatch(RPRes.rsObjectTypeMap, this.m_mapVectorLayer.MapDef.Name, this.m_mapVectorLayer.Name, SpatialDataMapper.GetBindingFieldName(((ReportElementCollectionBase<MapBindingFieldPair>)mapBindingFieldPairs)[num])));
			}
		}

		private static object Convert(object value, Type type, Type conversionType)
		{
			TypeCode typeCode = Type.GetTypeCode(conversionType);
			TypeCode typeCode2 = Type.GetTypeCode(type);
			switch (typeCode)
			{
			case TypeCode.Int32:
			{
				int num = default(int);
				switch (typeCode2)
				{
				case TypeCode.Decimal:
					if (!VectorLayerMapper.TryConvertDecimalToInt((decimal)value, out num))
					{
						break;
					}
					return num;
				case TypeCode.Double:
					if (!VectorLayerMapper.TryConvertDoubleToInt((double)value, out num))
					{
						break;
					}
					return num;
				}
				break;
			}
			case TypeCode.Decimal:
				if (typeCode2 != TypeCode.Int32)
				{
					break;
				}
				return (decimal)(int)value;
			case TypeCode.Double:
				switch (typeCode2)
				{
				case TypeCode.Int32:
					return (double)(int)value;
				case TypeCode.Single:
					return (double)(float)value;
				}
				break;
			}
			return null;
		}

		private static bool TryConvertDecimalToInt(decimal value, out int convertedValue)
		{
			if (!(value > 2147483647m) && !(value < -2147483648m))
			{
				convertedValue = (int)value;
				return value.Equals(convertedValue);
			}
			convertedValue = 0;
			return false;
		}

		private static bool TryConvertDoubleToInt(double value, out int convertedValue)
		{
			if (!(value > 2147483647.0) && !(value < -2147483648.0))
			{
				convertedValue = (int)value;
				return value.Equals((double)convertedValue);
			}
			convertedValue = 0;
			return false;
		}

		internal static object EvaluateBindingExpression(MapBindingFieldPair mapBindingFieldPair)
		{
			ReportVariantProperty bindingExpression = mapBindingFieldPair.BindingExpression;
			if (!bindingExpression.IsExpression)
			{
				return bindingExpression.Value;
			}
			return mapBindingFieldPair.Instance.BindingExpression;
		}

		private SpatialElementInfoGroup CreateSpatialElementFromDataRegion()
		{
            return null;
            /*
            if (!this.m_mapMapper.CanAddSpatialElement)
            {
                return null;
            }
            MapSpatialDataRegion mapSpatialDataRegion = (MapSpatialDataRegion)this.m_mapVectorLayer.MapSpatialData;
            if (mapSpatialDataRegion == null)
            {
                return null;
            }
            object vectorData = mapSpatialDataRegion.Instance.VectorData;
            if (vectorData == null)
            {
                return null;
            }
            ISpatialElement spatialElement;
            if (vectorData is SqlGeography)
            {
                spatialElement = this.GetSpatialElementManager().AddGeography((SqlGeography)vectorData, this.m_mapVectorLayer.Name);
            }
            else
            {
                if (!(vectorData is SqlGeometry))
                {
                    throw new RenderingObjectModelException(RPRes.rsMapInvalidSpatialFieldType(RPRes.rsObjectTypeMap, this.m_mapVectorLayer.MapDef.Name, this.m_mapVectorLayer.Name));
                }
                spatialElement = this.GetSpatialElementManager().AddGeometry((SqlGeometry)vectorData, this.m_mapVectorLayer.Name);
            }
            if (spatialElement == null)
            {
                return null;
            }
            SpatialElementInfo spatialElementInfo = new SpatialElementInfo();
            spatialElementInfo.CoreSpatialElement = spatialElement;
            spatialElementInfo.MapSpatialElement = null;
            SpatialElementInfoGroup spatialElementInfoGroup = new SpatialElementInfoGroup();
            spatialElementInfoGroup.Elements.Add(spatialElementInfo);
            this.m_spatialElementsDictionary.Add(new SpatialElementKey(null), spatialElementInfoGroup);
            this.OnSpatialElementAdded(spatialElement);
            this.m_mapMapper.OnSpatialElementAdded(spatialElementInfo);
            return spatialElementInfoGroup;
            */
        }

		protected void RenderSymbolRuleFields(Symbol symbol)
		{
			if (this.m_pointColorRuleMapper != null)
			{
				this.m_pointColorRuleMapper.SetRuleFieldValue(symbol);
			}
			if (this.m_pointlSizeRuleMapper != null)
			{
				this.m_pointlSizeRuleMapper.SetRuleFieldValue(symbol);
			}
			if (this.m_pointMarkerRuleMapper != null)
			{
				this.m_pointMarkerRuleMapper.SetRuleFieldValue(symbol);
			}
		}

		protected void RenderPoint(MapSpatialElement mapSpatialElement, Symbol symbol, bool hasScope)
		{
			if (hasScope)
			{
				this.RenderSymbolRuleFields(symbol);
			}
			this.RenderSymbolTemplate(mapSpatialElement, symbol, hasScope);
		}

		protected void CreatePointRules(MapPointRules mapPointRules)
		{
			if (mapPointRules.MapColorRule != null)
			{
				this.m_pointColorRuleMapper = new ColorRuleMapper(mapPointRules.MapColorRule, this, this.GetSymbolManager());
				this.m_pointColorRuleMapper.CreateSymbolRule();
			}
			if (mapPointRules.MapSizeRule != null)
			{
				this.m_pointlSizeRuleMapper = new SizeRuleMapper(mapPointRules.MapSizeRule, this, this.GetSymbolManager());
				this.m_pointlSizeRuleMapper.CreateSymbolRule();
			}
			if (mapPointRules.MapMarkerRule != null)
			{
				this.m_pointMarkerRuleMapper = new MarkerRuleMapper(mapPointRules.MapMarkerRule, this, this.GetSymbolManager());
				this.m_pointMarkerRuleMapper.CreateSymbolRule();
			}
		}

		protected void RenderPointRules(MapPointRules mapPointRules)
		{
			int? legendSymbolSize = this.GetLegendSymbolSize();
			Color? legendSymbolColor = this.GetLegendSymbolColor();
			MarkerStyle? legendSymbolMarker = this.GetLegendSymbolMarker();
			if (mapPointRules.MapColorRule != null)
			{
				this.m_pointColorRuleMapper.RenderSymbolRule(this.m_pointTemplateMapper, legendSymbolSize, legendSymbolMarker);
			}
			if (mapPointRules.MapSizeRule != null)
			{
				this.m_pointlSizeRuleMapper.RenderSymbolRule(this.m_pointTemplateMapper, legendSymbolColor, legendSymbolMarker);
			}
			if (mapPointRules.MapMarkerRule != null)
			{
				this.m_pointMarkerRuleMapper.RenderPointRule(this.m_pointTemplateMapper, legendSymbolColor, legendSymbolSize);
			}
		}

		private Color? GetLegendSymbolColor()
		{
			if (this.m_pointTemplateMapper == null)
			{
				return Color.Empty;
			}
			return this.m_pointTemplateMapper.GetBackgroundColor(false);
		}

		private int? GetLegendSymbolSize()
		{
			if (this.m_pointTemplateMapper == null)
			{
				return PointTemplateMapper.GetDefaultSymbolSize(this.m_mapMapper.DpiX);
			}
			return this.m_pointTemplateMapper.GetSize(this.GetMapPointTemplate(), false);
		}

		private MarkerStyle? GetLegendSymbolMarker()
		{
			if (!(this.m_pointTemplateMapper is SymbolMarkerTemplateMapper))
			{
				return MarkerStyle.None;
			}
			MapMarkerStyle markerStyle = MapMapper.GetMarkerStyle(((MapMarkerTemplate)this.GetMapPointTemplate()).MapMarker, false);
			return MapMapper.GetMarkerStyle(markerStyle);
		}

		protected virtual void RenderSymbolTemplate(MapSpatialElement mapSpatialElement, Symbol coreSymbol, bool hasScope)
		{
		}

		internal virtual MapPointRules GetMapPointRules()
		{
			return null;
		}

		internal virtual MapPointTemplate GetMapPointTemplate()
		{
			return null;
		}

		internal bool HasPointColorRule(Symbol symbol)
		{
			if (!this.HasPointColorRule())
			{
				return false;
			}
			return this.m_pointColorRuleMapper.HasDataValue(symbol);
		}

		protected bool HasPointColorRule()
		{
			MapPointRules mapPointRules = this.GetMapPointRules();
			if (mapPointRules == null)
			{
				return false;
			}
			if (mapPointRules.MapColorRule != null)
			{
				return true;
			}
			return false;
		}

		internal bool HasPointSizeRule(Symbol symbol)
		{
			if (!this.HasPointSizeRule())
			{
				return false;
			}
			return this.m_pointlSizeRuleMapper.HasDataValue(symbol);
		}

		protected bool HasPointSizeRule()
		{
			MapPointRules mapPointRules = this.GetMapPointRules();
			if (mapPointRules == null)
			{
				return false;
			}
			if (mapPointRules.MapSizeRule != null)
			{
				return true;
			}
			return false;
		}

		internal bool HasMarkerRule(Symbol symbol)
		{
			if (!this.HasMarkerRule())
			{
				return false;
			}
			return this.m_pointMarkerRuleMapper.HasDataValue(symbol);
		}

		protected bool HasMarkerRule()
		{
			MapPointRules mapPointRules = this.GetMapPointRules();
			if (mapPointRules == null)
			{
				return false;
			}
			if (mapPointRules.MapMarkerRule != null)
			{
				return true;
			}
			return false;
		}

		protected PointTemplateMapper CreatePointTemplateMapper()
		{
			return new SymbolMarkerTemplateMapper(this.m_mapMapper, this, this.m_mapVectorLayer);
		}

		internal static string AddPrefixToFieldNames(string layerName, string expression)
		{
			if (expression == null)
			{
				return null;
			}
			string[] array = expression.Split('#');
			string text = "";
			if (array.Length == 1)
			{
				return expression;
			}
			for (int i = 0; i < array.Length; i++)
			{
				text = ((!(array[i] == "")) ? string.Format(CultureInfo.InvariantCulture, "{0}{1}_{2}", text, layerName, array[i]) : string.Format(CultureInfo.InvariantCulture, "{0}{1}", text, "#"));
			}
			return text;
		}

		protected CoreSymbolManager GetSymbolManager()
		{
			if (this.m_symbolManager == null)
			{
				this.m_symbolManager = new CoreSymbolManager(this.m_coreMap, this.m_mapVectorLayer);
			}
			return this.m_symbolManager;
		}

		private void UpdateView()
		{
			this.m_coreMap.mapCore.UpdateCachedBounds();
			MapView mapView = this.m_mapVectorLayer.MapDef.MapViewport.MapView;
			if (mapView is MapDataBoundView)
			{
				this.AddBoundSpatialElementsToView();
			}
			else if (mapView is MapElementView)
			{
				this.AddSpatialElementToView((MapElementView)mapView);
			}
		}

		private void AddBoundSpatialElementsToView()
		{
			foreach (KeyValuePair<SpatialElementKey, SpatialElementInfoGroup> item in this.m_spatialElementsDictionary)
			{
				if (item.Value.BoundToData)
				{
					this.AddSpatialElementGroupToView(item.Value);
				}
			}
		}

		private void AddSpatialElementGroupToView(SpatialElementInfoGroup group)
		{
			foreach (SpatialElementInfo element in group.Elements)
			{
				this.m_mapMapper.AddSpatialElementToView(element.CoreSpatialElement);
			}
		}

		private void AddSpatialElementToView(MapElementView mapView)
		{
			if (!(this.GetElementViewLayerName(mapView) != this.m_mapVectorLayer.Name))
			{
				List<ISpatialElement> elementViewSpatialElements = this.GetElementViewSpatialElements(mapView);
				if (elementViewSpatialElements != null)
				{
					foreach (ISpatialElement item in elementViewSpatialElements)
					{
						this.m_mapMapper.AddSpatialElementToView(item);
					}
				}
				else
				{
					foreach (KeyValuePair<SpatialElementKey, SpatialElementInfoGroup> item2 in this.m_spatialElementsDictionary)
					{
						this.AddSpatialElementGroupToView(item2.Value);
					}
				}
			}
		}

		private List<ISpatialElement> GetElementViewSpatialElements(MapElementView mapView)
		{
			MapBindingFieldPairCollection mapBindingFieldPairs = mapView.MapBindingFieldPairs;
			if (mapBindingFieldPairs == null)
			{
				return null;
			}
			SpatialElementKey obj = VectorLayerMapper.CreateDataRegionSpatialElementKey(mapBindingFieldPairs);
			List<ISpatialElement> list = null;
			foreach (KeyValuePair<SpatialElementKey, SpatialElementInfoGroup> item in this.m_spatialElementsDictionary)
			{
				foreach (SpatialElementInfo element in item.Value.Elements)
				{
					if (SpatialDataMapper.CreateCoreSpatialElementKey(element.CoreSpatialElement, mapView.MapBindingFieldPairs, this.m_mapVectorLayer.MapDef.Name, this.m_mapVectorLayer.Name).Equals(obj))
					{
						if (list == null)
						{
							list = new List<ISpatialElement>();
						}
						list.Add(element.CoreSpatialElement);
					}
				}
			}
			return list;
		}

		private string GetElementViewLayerName(MapElementView mapView)
		{
			ReportStringProperty layerName = mapView.LayerName;
			if (!layerName.IsExpression)
			{
				return layerName.Value;
			}
			return mapView.Instance.LayerName;
		}

		protected abstract CoreSpatialElementManager GetSpatialElementManager();

		protected abstract void CreateRules();

		protected abstract void RenderRules();

		protected abstract void RenderSpatialElement(SpatialElementInfo spatialElementInfo, bool hasScope);

		internal abstract bool IsValidSpatialElement(ISpatialElement spatialElement);

		internal abstract void OnSpatialElementAdded(ISpatialElement spatialElement);
	}
}
