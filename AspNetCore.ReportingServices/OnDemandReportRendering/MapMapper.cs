using AspNetCore.Reporting.Map.WebForms;
using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.Interfaces;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class MapMapper : MapperBase, IMapMapper, IDVMappingLayer, IDisposable
	{
		private class BoundsRectCalculator
		{
			private bool hasValue;

			internal AspNetCore.Reporting.Map.WebForms.MapPoint Min;

			internal AspNetCore.Reporting.Map.WebForms.MapPoint Max;

			internal AspNetCore.Reporting.Map.WebForms.MapPoint Center
			{
				get
				{
					return new AspNetCore.Reporting.Map.WebForms.MapPoint((this.Min.X + this.Max.X) / 2.0, (this.Min.Y + this.Max.Y) / 2.0);
				}
			}

			internal void AddSpatialElement(ISpatialElement spatialElement)
			{
				if (!this.hasValue)
				{
					this.Min = new AspNetCore.Reporting.Map.WebForms.MapPoint(spatialElement.MinimumExtent.X + spatialElement.Offset.X, spatialElement.MinimumExtent.Y + spatialElement.Offset.Y);
					this.Max = new AspNetCore.Reporting.Map.WebForms.MapPoint(spatialElement.MaximumExtent.X + spatialElement.Offset.X, spatialElement.MaximumExtent.Y + spatialElement.Offset.Y);
					this.hasValue = true;
				}
				else
				{
					this.Min.X = Math.Min(this.Min.X, spatialElement.MinimumExtent.X + spatialElement.Offset.X);
					this.Min.Y = Math.Min(this.Min.Y, spatialElement.MinimumExtent.Y + spatialElement.Offset.Y);
					this.Max.X = Math.Max(this.Max.X, spatialElement.MaximumExtent.X + spatialElement.Offset.X);
					this.Max.Y = Math.Max(this.Max.Y, spatialElement.MaximumExtent.Y + spatialElement.Offset.Y);
				}
			}
		}

		private Map m_map;

		private int m_remainingSpatialElementCount = 20000;

		private int m_remainingTotalPointCount = 1000000;

		private MapControl m_coreMap;

		private ActionInfoWithDynamicImageMapCollection m_actions = new ActionInfoWithDynamicImageMapCollection();

		private static string m_defaultContentMarginString = "10pt";

		private static ReportSize m_defaultContentMargin = new ReportSize(MapMapper.m_defaultContentMarginString);

		private static string m_defaultTickMarkLengthString = "2.25pt";

		private static ReportSize m_defaultTickMarkLength = new ReportSize(MapMapper.m_defaultTickMarkLengthString);

		private BoundsRectCalculator m_boundRectCalculator;

		private MapSimplifier m_mapSimplifier;

		private double? m_simpificationResolution = null;

		private TileLayerMapper m_tileLayerMapper;

		private Formatter m_formatter;

		internal int RemainingSpatialElementCount
		{
			get
			{
				return this.m_remainingSpatialElementCount;
			}
		}

		internal int RemainingTotalPointCount
		{
			get
			{
				return this.m_remainingTotalPointCount;
			}
		}

		internal bool CanAddSpatialElement
		{
			get
			{
				if (this.m_remainingSpatialElementCount > 0 && this.m_remainingTotalPointCount > 0)
				{
					return true;
				}
				if (this.m_remainingSpatialElementCount < 1)
				{
					this.m_coreMap.Viewport.ErrorMessage = RPRes.rsMapMaximumSpatialElementCountReached(RPRes.rsObjectTypeMap, this.m_map.Name);
				}
				else if (this.m_remainingTotalPointCount < 1)
				{
					this.m_coreMap.Viewport.ErrorMessage = RPRes.rsMapMaximumTotalPointCountReached(RPRes.rsObjectTypeMap, this.m_map.Name);
				}
				return false;
			}
		}

		private MapSimplifier Simplifier
		{
			get
			{
				if (!this.m_simpificationResolution.HasValue)
				{
					this.m_simpificationResolution = this.EvaluateSimplificationResolution();
					if (this.m_simpificationResolution.Value != 0.0)
					{
						this.m_mapSimplifier = new MapSimplifier();
					}
				}
				return this.m_mapSimplifier;
			}
		}

		public MapMapper(Map map, string defaultFontFamily)
			: base(defaultFontFamily)
		{
			this.m_map = map;
		}

		public void RenderMap()
		{
			try
			{
				if (this.m_map != null)
				{
					this.InitializeMap();
					this.SetMapProperties();
					this.RenderLayers();
					this.RenderViewport();
					this.RenderLegends();
					this.RenderTitles();
					this.RenderDistanceScale();
					this.RenderColorScale();
					this.RenderBorderSkin();
					this.RenderMapStyle();
				}
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex2))
				{
					throw;
				}
				throw new RenderingObjectModelException(ex2);
			}
		}

		public Stream GetCoreXml()
		{
			try
			{
				this.m_coreMap.Serializer.Content = SerializationContent.All;
				this.m_coreMap.Serializer.NonSerializableContent = "";
				MemoryStream memoryStream = new MemoryStream();
				this.m_coreMap.Serializer.Save(memoryStream);
				memoryStream.Position = 0L;
				return memoryStream;
			}
			catch (Exception ex)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex))
				{
					throw;
				}
				Global.Tracer.Trace(TraceLevel.Verbose, ex.Message);
				return null;
			}
		}

		public Stream GetImage(DynamicImageInstance.ImageType imageType)
		{
			try
			{
				if (this.m_coreMap == null)
				{
					return null;
				}
				int width = 300;
				if (base.WidthOverrideInPixels.HasValue)
				{
					width = base.WidthOverrideInPixels.Value;
				}
				else if (this.m_map.Width != null)
				{
					width = MappingHelper.ToIntPixels(this.m_map.Width, base.DpiX);
				}
				this.m_coreMap.Width = width;
				int height = 300;
				if (base.HeightOverrideInPixels.HasValue)
				{
					height = base.HeightOverrideInPixels.Value;
				}
				else if (this.m_map.Height != null)
				{
					height = MappingHelper.ToIntPixels(this.m_map.Height, base.DpiY);
				}
				this.m_coreMap.Height = height;
				Stream stream = null;
				switch (imageType)
				{
				case DynamicImageInstance.ImageType.EMF:
					this.GetEmfImage(out stream, width, height);
					break;
				case DynamicImageInstance.ImageType.PNG:
					this.GetPngImage(out stream, width, height);
					break;
				}
				stream.Position = 0L;
				return stream;
			}
			catch (RSException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex2))
				{
					throw;
				}
				throw new RenderingObjectModelException(ex2);
			}
		}

		public ActionInfoWithDynamicImageMapCollection GetImageMaps()
		{
			ActionInfoWithDynamicImageMapCollection actionInfoWithDynamicImageMapCollection = MappingHelper.GetImageMaps(this.GetMapAreaInfoList(), this.m_actions, this.m_map);
			ActionInfoWithDynamicImageMap mapImageMap = this.GetMapImageMap();
			if (mapImageMap != null)
			{
				if (actionInfoWithDynamicImageMapCollection == null)
				{
					actionInfoWithDynamicImageMapCollection = new ActionInfoWithDynamicImageMapCollection();
				}
				actionInfoWithDynamicImageMapCollection.InternalList.Add(mapImageMap);
			}
			return actionInfoWithDynamicImageMapCollection;
		}

		private ActionInfoWithDynamicImageMap GetMapImageMap()
		{
			string text = default(string);
			ActionInfoWithDynamicImageMap actionInfoWithDynamicImageMap = MappingHelper.CreateActionInfoDynamic((ReportItem)this.m_map, this.m_map.ActionInfo, string.Empty, out text, true);
			if (actionInfoWithDynamicImageMap != null)
			{
				actionInfoWithDynamicImageMap.CreateImageMapAreaInstance(ImageMapArea.ImageMapAreaShape.Rectangle, new float[4]
				{
					0f,
					0f,
					100f,
					100f
				}, string.Empty);
			}
			return actionInfoWithDynamicImageMap;
		}

		internal IEnumerable<MappingHelper.MapAreaInfo> GetMapAreaInfoList()
		{
			this.m_coreMap.mapCore.PopulateImageMaps();
			float width = (float)this.m_coreMap.Width;
			float height = (float)this.m_coreMap.Height;
			foreach (MapArea mapArea in this.m_coreMap.MapAreas)
			{
				yield return new MappingHelper.MapAreaInfo(mapArea.ToolTip, ((IMapAreaAttributes)mapArea).Tag, this.GetMapAreaShape(mapArea.Shape), MappingHelper.ConvertCoordinatesToRelative(mapArea.Coordinates, width, height));
			}
		}

		private void InitializeMap()
		{
			this.m_coreMap = new MapControl();
			this.m_coreMap.mapCore.UppercaseFieldKeywords = false;
			this.m_coreMap.mapCore.SetUserLocales(new string[3]
			{
				Localization.ClientBrowserCultureName,
				Localization.ClientCurrentCultureName,
				Localization.ClientPrimaryCulture.Name
			});
			this.m_coreMap.ShapeFields.Clear();
			this.m_coreMap.ShapeRules.Clear();
			this.m_coreMap.Shapes.Clear();
			this.m_coreMap.SymbolFields.Clear();
			this.m_coreMap.SymbolRules.Clear();
			this.m_coreMap.Symbols.Clear();
			this.m_coreMap.PathFields.Clear();
			this.m_coreMap.PathRules.Clear();
			this.m_coreMap.Paths.Clear();
			MapControl coreMap = this.m_coreMap;
			coreMap.FormatNumberHandler = (FormatNumberHandler)Delegate.Combine(coreMap.FormatNumberHandler, new FormatNumberHandler(this.FormatNumber));
			bool traceVerbose = RSTrace.ProcessingTracer.TraceVerbose;
		}

		private void RenderViewport()
		{
			this.RenderSubItem(this.m_map.MapViewport, this.m_coreMap.Viewport);
			this.SetViewportProperties();
			this.RenderMapLimits();
			this.RenderMapView();
			this.RenderGridLines(this.m_map.MapViewport.MapMeridians, this.m_coreMap.Meridians);
			this.RenderGridLines(this.m_map.MapViewport.MapParallels, this.m_coreMap.Parallels);
		}

		private void RenderGridLines(MapGridLines mapGridLines, GridAttributes coreGridLines)
		{
			if (mapGridLines != null)
			{
				this.SetGridLinesProperties(mapGridLines, coreGridLines);
				this.RenderGridLinesStyle(mapGridLines, coreGridLines);
			}
		}

		private void RenderSubItem(MapSubItem mapSubItem, Panel coreSubItem)
		{
			this.SetSubItemProperties(mapSubItem, coreSubItem);
			this.RenderSubItemStyle(mapSubItem, coreSubItem);
			if (mapSubItem != null)
			{
				this.RenderLocation(mapSubItem.MapLocation, coreSubItem);
			}
			if (mapSubItem != null)
			{
				this.RenderSize(mapSubItem.MapSize, coreSubItem);
			}
		}

		private void RenderDockableSubItem(MapDockableSubItem mapDockableSubItem, DockablePanel coreSubItem)
		{
			this.RenderSubItem(mapDockableSubItem, coreSubItem);
			this.SetDockableSubItemProperties(mapDockableSubItem, coreSubItem);
			this.RenderActionInfo(mapDockableSubItem.ActionInfo, coreSubItem.ToolTip, coreSubItem, null, true);
		}

		private void RenderLegends()
		{
			if (this.m_map.MapLegends != null)
			{
				foreach (MapLegend mapLegend in this.m_map.MapLegends)
				{
					this.RenderLegend(mapLegend);
				}
			}
		}

		private void RenderLayers()
		{
			if (this.m_map.MapLayers != null)
			{
				foreach (MapLayer mapLayer in this.m_map.MapLayers)
				{
					this.RenderLayer(mapLayer);
				}
			}
		}

		private void RenderTitles()
		{
			if (this.m_map.MapTitles != null)
			{
				foreach (MapTitle mapTitle in this.m_map.MapTitles)
				{
					this.RenderTitle(mapTitle);
				}
			}
		}

		private void RenderLegend(MapLegend mapLegend)
		{
			AspNetCore.Reporting.Map.WebForms.Legend legend = new AspNetCore.Reporting.Map.WebForms.Legend();
			this.RenderDockableSubItem(mapLegend, legend);
			this.SetLegendProperties(mapLegend, legend);
			this.RenderLegendTitle(mapLegend.MapLegendTitle, legend);
			this.m_coreMap.Legends.Add(legend);
		}

		private void RenderLegendTitle(MapLegendTitle mapLegendTitle, AspNetCore.Reporting.Map.WebForms.Legend coreLegend)
		{
			if (mapLegendTitle != null)
			{
				this.SetLegendTitleProperties(mapLegendTitle, coreLegend);
				this.RenderLegendTitleStyle(mapLegendTitle, coreLegend);
			}
		}

		private void RenderTitle(MapTitle mapTitle)
		{
			MapLabel mapLabel = new MapLabel();
			this.RenderDockableSubItem(mapTitle, mapLabel);
			this.SetTitleProperties(mapTitle, mapLabel);
			this.m_coreMap.Labels.Add(mapLabel);
		}

		private void RenderLayer(MapLayer mapLayer)
		{
			Layer layer = new Layer();
			this.SetLayerProperties(mapLayer, layer);
			this.m_coreMap.Layers.Add(layer);
			if (mapLayer is MapTileLayer)
			{
				this.RenderTileLayer((MapTileLayer)mapLayer);
			}
			else if (mapLayer is MapVectorLayer)
			{
				this.RenderVectorLayer((MapVectorLayer)mapLayer);
			}
		}

		private void SetLayerProperties(MapLayer mapLayer, Layer coreLayer)
		{
			coreLayer.Name = mapLayer.Name;
			ReportDoubleProperty transparency = mapLayer.Transparency;
			if (transparency != null)
			{
				if (!transparency.IsExpression)
				{
					coreLayer.Transparency = (float)transparency.Value;
				}
				else
				{
					coreLayer.Transparency = (float)mapLayer.Instance.Transparency;
				}
			}
			else
			{
				coreLayer.Transparency = 0f;
			}
			ReportDoubleProperty maximumZoom = mapLayer.MaximumZoom;
			if (maximumZoom != null)
			{
				if (!maximumZoom.IsExpression)
				{
					coreLayer.VisibleToZoom = (float)maximumZoom.Value;
				}
				else
				{
					coreLayer.VisibleToZoom = (float)mapLayer.Instance.MaximumZoom;
				}
			}
			else
			{
				coreLayer.VisibleToZoom = 200f;
			}
			ReportDoubleProperty minimumZoom = mapLayer.MinimumZoom;
			if (minimumZoom != null)
			{
				if (!minimumZoom.IsExpression)
				{
					coreLayer.VisibleFromZoom = (float)minimumZoom.Value;
				}
				else
				{
					coreLayer.VisibleFromZoom = (float)mapLayer.Instance.MinimumZoom;
				}
			}
			else
			{
				coreLayer.VisibleFromZoom = 50f;
			}
			ReportEnumProperty<MapVisibilityMode> visibilityMode = mapLayer.VisibilityMode;
			if (visibilityMode != null)
			{
				if (!visibilityMode.IsExpression)
				{
					coreLayer.Visibility = MapMapper.GetLayerVisibility(visibilityMode.Value);
				}
				else
				{
					coreLayer.Visibility = MapMapper.GetLayerVisibility(mapLayer.Instance.VisibilityMode);
				}
			}
			else
			{
				coreLayer.Visibility = LayerVisibility.Shown;
			}
		}

		private void RenderVectorLayer(MapVectorLayer mapVectorLayer)
		{
			if (mapVectorLayer is MapPolygonLayer)
			{
				new PolygonLayerMapper((MapPolygonLayer)mapVectorLayer, this.m_coreMap, this).Render();
			}
			else if (mapVectorLayer is MapPointLayer)
			{
				new PointLayerMapper((MapPointLayer)mapVectorLayer, this.m_coreMap, this).Render();
			}
			else if (mapVectorLayer is MapLineLayer)
			{
				new LineLayerMapper((MapLineLayer)mapVectorLayer, this.m_coreMap, this).Render();
			}
		}

		private void RenderTileLayer(MapTileLayer mapTileLayer)
		{
			if (this.m_tileLayerMapper == null)
			{
				this.m_tileLayerMapper = new TileLayerMapper(this.m_map, this.m_coreMap);
			}
			this.m_tileLayerMapper.AddLayer(mapTileLayer);
		}

		private void RenderDistanceScale()
		{
			if (this.m_map.MapDistanceScale != null)
			{
				this.RenderDockableSubItem(this.m_map.MapDistanceScale, this.m_coreMap.DistanceScalePanel);
				this.SetDistanceScaleProperties();
			}
		}

		private void RenderColorScale()
		{
			if (this.m_map.MapColorScale != null)
			{
				this.RenderDockableSubItem(this.m_map.MapColorScale, this.m_coreMap.ColorSwatchPanel);
				this.SetColorScaleProperties();
				this.RenderColorScaleTitle();
			}
		}

		private void RenderColorScaleTitle()
		{
			this.SetColorScaleTitleProperties();
			this.RenderColorScaleTitleStyle();
		}

		private void RenderBorderSkin()
		{
			if (this.m_map.MapBorderSkin != null)
			{
				this.SetBorderSkinProperties();
				this.RenderBorderSkinStyle();
			}
		}

		private void SetSubItemProperties(MapSubItem mapSubItem, Panel coreSubItem)
		{
			ReportSizeProperty leftMargin = mapSubItem.LeftMargin;
			if (leftMargin != null)
			{
				if (!leftMargin.IsExpression)
				{
					coreSubItem.Margins.Left = MappingHelper.ToIntPixels(leftMargin.Value, base.DpiX);
				}
				else
				{
					coreSubItem.Margins.Left = MappingHelper.ToIntPixels(mapSubItem.Instance.LeftMargin, base.DpiX);
				}
			}
			leftMargin = mapSubItem.TopMargin;
			if (leftMargin != null)
			{
				if (!leftMargin.IsExpression)
				{
					coreSubItem.Margins.Top = MappingHelper.ToIntPixels(leftMargin.Value, base.DpiY);
				}
				else
				{
					coreSubItem.Margins.Top = MappingHelper.ToIntPixels(mapSubItem.Instance.TopMargin, base.DpiY);
				}
			}
			leftMargin = mapSubItem.RightMargin;
			if (leftMargin != null)
			{
				if (!leftMargin.IsExpression)
				{
					coreSubItem.Margins.Right = MappingHelper.ToIntPixels(leftMargin.Value, base.DpiX);
				}
				else
				{
					coreSubItem.Margins.Right = MappingHelper.ToIntPixels(mapSubItem.Instance.RightMargin, base.DpiX);
				}
			}
			leftMargin = mapSubItem.BottomMargin;
			if (leftMargin != null)
			{
				if (!leftMargin.IsExpression)
				{
					coreSubItem.Margins.Bottom = MappingHelper.ToIntPixels(leftMargin.Value, base.DpiX);
				}
				else
				{
					coreSubItem.Margins.Bottom = MappingHelper.ToIntPixels(mapSubItem.Instance.BottomMargin, base.DpiX);
				}
			}
			ReportIntProperty zIndex = mapSubItem.ZIndex;
			if (zIndex != null)
			{
				if (!zIndex.IsExpression)
				{
					coreSubItem.ZOrder = zIndex.Value;
				}
				else
				{
					coreSubItem.ZOrder = mapSubItem.Instance.ZIndex;
				}
			}
		}

		private void RenderLocation(MapLocation mapLocation, Panel coreSubItem)
		{
			if (mapLocation != null)
			{
				ReportEnumProperty<Unit> unit = mapLocation.Unit;
				Unit unit2 = Unit.Percentage;
				if (unit != null)
				{
					unit2 = (unit.IsExpression ? mapLocation.Instance.Unit : unit.Value);
				}
				coreSubItem.LocationUnit = (CoordinateUnit)((unit2 == Unit.Percentage) ? 1 : 0);
				ReportDoubleProperty left = mapLocation.Left;
				if (left != null)
				{
					double num = left.IsExpression ? mapLocation.Instance.Left : left.Value;
					if (unit2 != 0)
					{
						num = MappingHelper.ToPixels(num, unit2, base.DpiX);
					}
					coreSubItem.Location.X = (float)num;
				}
				left = mapLocation.Top;
				if (left != null)
				{
					double num = left.IsExpression ? mapLocation.Instance.Top : left.Value;
					if (unit2 != 0)
					{
						num = MappingHelper.ToPixels(num, unit2, base.DpiY);
					}
					coreSubItem.Location.Y = (float)num;
				}
			}
		}

		private void RenderSize(MapSize mapSize, Panel coreSubItem)
		{
			if (mapSize != null)
			{
				ReportEnumProperty<Unit> unit = mapSize.Unit;
				Unit unit2 = Unit.Percentage;
				if (unit != null)
				{
					unit2 = (unit.IsExpression ? mapSize.Instance.Unit : unit.Value);
				}
				coreSubItem.SizeUnit = (CoordinateUnit)((unit2 == Unit.Percentage) ? 1 : 0);
				ReportDoubleProperty width = mapSize.Width;
				if (width != null)
				{
					double num = width.IsExpression ? mapSize.Instance.Width : width.Value;
					if (unit2 != 0)
					{
						num = MappingHelper.ToPixels(num, unit2, base.DpiX);
					}
					coreSubItem.Size.Width = (float)num;
				}
				width = mapSize.Height;
				if (width != null)
				{
					double num = width.IsExpression ? mapSize.Instance.Height : width.Value;
					if (unit2 != 0)
					{
						num = MappingHelper.ToPixels(num, unit2, base.DpiY);
					}
					coreSubItem.Size.Height = (float)num;
				}
			}
		}

		private void RenderMapLimits()
		{
			MapLimits mapLimits = this.m_map.MapViewport.MapLimits;
			if (mapLimits != null)
			{
				ReportDoubleProperty minimumX = mapLimits.MinimumX;
				if (minimumX != null)
				{
					if (!minimumX.IsExpression)
					{
						this.m_coreMap.MapLimits.MinimumX = minimumX.Value;
					}
					else
					{
						this.m_coreMap.MapLimits.MinimumX = mapLimits.Instance.MinimumX;
					}
				}
				minimumX = mapLimits.MinimumY;
				if (minimumX != null)
				{
					if (!minimumX.IsExpression)
					{
						this.m_coreMap.MapLimits.MinimumY = minimumX.Value;
					}
					else
					{
						this.m_coreMap.MapLimits.MinimumY = mapLimits.Instance.MinimumY;
					}
				}
				minimumX = mapLimits.MaximumX;
				if (minimumX != null)
				{
					if (!minimumX.IsExpression)
					{
						this.m_coreMap.MapLimits.MaximumX = minimumX.Value;
					}
					else
					{
						this.m_coreMap.MapLimits.MaximumX = mapLimits.Instance.MaximumX;
					}
				}
				minimumX = mapLimits.MaximumY;
				if (minimumX != null)
				{
					if (!minimumX.IsExpression)
					{
						this.m_coreMap.MapLimits.MaximumY = minimumX.Value;
					}
					else
					{
						this.m_coreMap.MapLimits.MaximumY = mapLimits.Instance.MaximumY;
					}
				}
			}
		}

		private void RenderMapView()
		{
			MapView mapView = this.m_map.MapViewport.MapView;
			if (mapView != null)
			{
				ReportDoubleProperty zoom = mapView.Zoom;
				double num = 0.0;
				if (zoom != null)
				{
					num = (zoom.IsExpression ? ((double)(float)mapView.Instance.Zoom) : ((double)(float)zoom.Value));
				}
				if (num != 0.0)
				{
					this.m_coreMap.Viewport.Zoom = (float)num;
				}
				if (mapView is MapCustomView)
				{
					this.RenderCustomView((MapCustomView)mapView);
				}
				else if (this.m_boundRectCalculator != null)
				{
					this.CenterView(num == 0.0);
				}
			}
		}

		private void CenterView(bool zoomToFit)
		{
			if (zoomToFit)
			{
				this.m_coreMap.MapLimits.MinimumX = this.m_boundRectCalculator.Min.X;
				this.m_coreMap.MapLimits.MinimumY = this.m_boundRectCalculator.Min.Y;
				this.m_coreMap.MapLimits.MaximumX = this.m_boundRectCalculator.Max.X;
				this.m_coreMap.MapLimits.MaximumY = this.m_boundRectCalculator.Max.Y;
				this.m_coreMap.Viewport.Zoom = 100f;
			}
			else
			{
				this.m_coreMap.CenterView(this.m_boundRectCalculator.Center);
			}
		}

		internal void AddSpatialElementToView(ISpatialElement spatialElement)
		{
			if (this.m_boundRectCalculator == null)
			{
				this.m_boundRectCalculator = new BoundsRectCalculator();
			}
			this.m_boundRectCalculator.AddSpatialElement(spatialElement);
		}

		private void RenderCustomView(MapCustomView mapView)
		{
			ReportDoubleProperty centerX = mapView.CenterX;
			if (centerX != null)
			{
				if (!centerX.IsExpression)
				{
					this.m_coreMap.Viewport.ViewCenter.X = (float)centerX.Value;
				}
				else
				{
					this.m_coreMap.Viewport.ViewCenter.X = (float)mapView.Instance.CenterX;
				}
			}
			centerX = mapView.CenterY;
			if (centerX != null)
			{
				if (!centerX.IsExpression)
				{
					this.m_coreMap.Viewport.ViewCenter.Y = (float)centerX.Value;
				}
				else
				{
					this.m_coreMap.Viewport.ViewCenter.Y = (float)mapView.Instance.CenterY;
				}
			}
		}

		private void SetDockableSubItemProperties(MapDockableSubItem mapDockableSubItem, DockablePanel coreDockableSubItem)
		{
			ReportEnumProperty<MapPosition> position2 = mapDockableSubItem.Position;
			MapPosition position = MapPosition.TopCenter;
			if (mapDockableSubItem.Position != null)
			{
				position = (mapDockableSubItem.Position.IsExpression ? mapDockableSubItem.Instance.Position : mapDockableSubItem.Position.Value);
			}
			coreDockableSubItem.DockAlignment = this.GetDockablePanelAlignment(position);
			coreDockableSubItem.Dock = ((mapDockableSubItem.MapLocation == null) ? this.GetDockablePanelDocking(position) : PanelDockStyle.None);
			ReportBoolProperty dockOutsideViewport = mapDockableSubItem.DockOutsideViewport;
			if (dockOutsideViewport != null)
			{
				if (!dockOutsideViewport.IsExpression)
				{
					coreDockableSubItem.DockedInsideViewport = !dockOutsideViewport.Value;
				}
				else
				{
					coreDockableSubItem.DockedInsideViewport = !mapDockableSubItem.Instance.DockOutsideViewport;
				}
			}
			else
			{
				coreDockableSubItem.DockedInsideViewport = true;
			}
			ReportBoolProperty hidden = mapDockableSubItem.Hidden;
			if (hidden != null)
			{
				if (!hidden.IsExpression)
				{
					coreDockableSubItem.Visible = !hidden.Value;
				}
				else
				{
					coreDockableSubItem.Visible = !mapDockableSubItem.Instance.Hidden;
				}
			}
			else
			{
				coreDockableSubItem.Visible = true;
			}
			ReportStringProperty toolTip = mapDockableSubItem.ToolTip;
			if (toolTip != null)
			{
				if (!toolTip.IsExpression)
				{
					coreDockableSubItem.ToolTip = toolTip.Value;
				}
				else
				{
					coreDockableSubItem.ToolTip = mapDockableSubItem.Instance.ToolTip;
				}
			}
		}

		private DockAlignment GetDockablePanelAlignment(MapPosition position)
		{
			switch (position)
			{
			case MapPosition.TopCenter:
			case MapPosition.LeftCenter:
			case MapPosition.RightCenter:
			case MapPosition.BottomCenter:
				return DockAlignment.Center;
			case MapPosition.TopRight:
			case MapPosition.LeftBottom:
			case MapPosition.RightBottom:
			case MapPosition.BottomRight:
				return DockAlignment.Far;
			default:
				return DockAlignment.Near;
			}
		}

		private PanelDockStyle GetDockablePanelDocking(MapPosition position)
		{
			switch (position)
			{
			case MapPosition.BottomRight:
			case MapPosition.BottomCenter:
			case MapPosition.BottomLeft:
				return PanelDockStyle.Bottom;
			case MapPosition.TopCenter:
			case MapPosition.TopLeft:
			case MapPosition.TopRight:
				return PanelDockStyle.Top;
			case MapPosition.LeftTop:
			case MapPosition.LeftCenter:
			case MapPosition.LeftBottom:
				return PanelDockStyle.Left;
			default:
				return PanelDockStyle.Right;
			}
		}

		private void SetMapProperties()
		{
			if (this.m_map.AntiAliasing != null)
			{
				if (!this.m_map.AntiAliasing.IsExpression)
				{
					this.m_coreMap.AntiAliasing = this.GetAntiAliasing(this.m_map.AntiAliasing.Value);
				}
				else
				{
					this.m_coreMap.AntiAliasing = this.GetAntiAliasing(this.m_map.Instance.AntiAliasing);
				}
			}
			if (this.m_map.ShadowIntensity != null)
			{
				if (!this.m_map.ShadowIntensity.IsExpression)
				{
					this.m_coreMap.ShadowIntensity = (float)this.m_map.ShadowIntensity.Value;
				}
				else
				{
					this.m_coreMap.ShadowIntensity = (float)this.m_map.Instance.ShadowIntensity;
				}
			}
			if (this.m_map.TextAntiAliasingQuality != null)
			{
				if (!this.m_map.TextAntiAliasingQuality.IsExpression)
				{
					this.m_coreMap.TextAntiAliasingQuality = this.GetTextAntiAliasingQuality(this.m_map.TextAntiAliasingQuality.Value);
				}
				else
				{
					this.m_coreMap.TextAntiAliasingQuality = this.GetTextAntiAliasingQuality(this.m_map.Instance.TextAntiAliasingQuality);
				}
			}
			this.m_remainingSpatialElementCount = this.m_map.MaximumSpatialElementCount;
			this.m_remainingTotalPointCount = this.m_map.MaximumTotalPointCount;
			this.SetTileServerConfiguration();
		}

		private void SetTileServerConfiguration()
		{
			IConfiguration configuration = this.m_map.RenderingContext.OdpContext.Configuration;
			IMapTileServerConfiguration mapTileServerConfiguration = null;
			if (configuration != null)
			{
				mapTileServerConfiguration = configuration.MapTileServerConfiguration;
			}
			if (mapTileServerConfiguration != null)
			{
				this.m_coreMap.TileServerMaxConnections = mapTileServerConfiguration.MaxConnections;
				this.m_coreMap.TileServerTimeout = mapTileServerConfiguration.Timeout * 1000;
				this.m_coreMap.TileServerAppId = mapTileServerConfiguration.AppID;
				this.m_coreMap.TileCacheLevel = MapTileServerConsts.ConvertFromMapTileCacheLevel(mapTileServerConfiguration.CacheLevel);
				this.m_coreMap.TileCulture = this.GetTileLanguage();
			}
		}

		private CultureInfo GetTileLanguage()
		{
			Formatter formatter = new Formatter(this.m_map.MapDef.StyleClass, this.m_map.RenderingContext.OdpContext, this.m_map.MapDef.ObjectType, this.m_map.Name);
			return formatter.GetCulture(this.EvaluateLanguage());
		}

		private string EvaluateLanguage()
		{
			ReportStringProperty tileLanguage = this.m_map.TileLanguage;
			if (tileLanguage != null)
			{
				if (!tileLanguage.IsExpression)
				{
					return tileLanguage.Value;
				}
				return this.m_map.Instance.TileLanguage;
			}
			if (this.m_map.Style != null)
			{
				tileLanguage = this.m_map.Style.Language;
				if (tileLanguage != null)
				{
					if (!tileLanguage.IsExpression)
					{
						return tileLanguage.Value;
					}
					return this.m_map.Instance.Style.Language;
				}
			}
			return null;
		}

		private AntiAliasing GetAntiAliasing(MapAntiAliasing mapAntiAliasing)
		{
			switch (mapAntiAliasing)
			{
			case MapAntiAliasing.Graphics:
				return AntiAliasing.Graphics;
			case MapAntiAliasing.None:
				return AntiAliasing.None;
			case MapAntiAliasing.Text:
				return AntiAliasing.Text;
			default:
				return AntiAliasing.All;
			}
		}

		private TextAntiAliasingQuality GetTextAntiAliasingQuality(MapTextAntiAliasingQuality textAntiAliasingQuality)
		{
			switch (textAntiAliasingQuality)
			{
			case MapTextAntiAliasingQuality.Normal:
				return TextAntiAliasingQuality.Normal;
			case MapTextAntiAliasingQuality.SystemDefault:
				return TextAntiAliasingQuality.SystemDefault;
			default:
				return TextAntiAliasingQuality.High;
			}
		}

		private void SetViewportProperties()
		{
			MapViewport mapViewport = this.m_map.MapViewport;
			this.m_coreMap.Viewport.AutoSize = (mapViewport.MapSize == null && mapViewport.MapLocation == null);
			ReportEnumProperty<MapCoordinateSystem> mapCoordinateSystem = mapViewport.MapCoordinateSystem;
			MapCoordinateSystem mapCoordinateSystem2 = (mapCoordinateSystem != null) ? (mapCoordinateSystem.IsExpression ? mapViewport.Instance.MapCoordinateSystem : mapCoordinateSystem.Value) : MapCoordinateSystem.Planar;
			this.m_coreMap.GeographyMode = (mapCoordinateSystem2 == MapCoordinateSystem.Geographic);
			ReportEnumProperty<MapProjection> mapProjection = mapViewport.MapProjection;
			MapProjection projection = (mapProjection != null) ? (mapProjection.IsExpression ? mapViewport.Instance.MapProjection : mapProjection.Value) : MapProjection.Equirectangular;
			this.m_coreMap.Projection = this.GetProjection(projection);
			ReportDoubleProperty projectionCenterX = mapViewport.ProjectionCenterX;
			if (projectionCenterX != null)
			{
				if (!projectionCenterX.IsExpression)
				{
					this.m_coreMap.ProjectionCenter.X = projectionCenterX.Value;
				}
				else
				{
					this.m_coreMap.ProjectionCenter.X = mapViewport.Instance.ProjectionCenterX;
				}
			}
			projectionCenterX = mapViewport.ProjectionCenterY;
			if (projectionCenterX != null)
			{
				if (!projectionCenterX.IsExpression)
				{
					this.m_coreMap.ProjectionCenter.Y = projectionCenterX.Value;
				}
				else
				{
					this.m_coreMap.ProjectionCenter.Y = mapViewport.Instance.ProjectionCenterY;
				}
			}
			ReportDoubleProperty maximumZoom = mapViewport.MaximumZoom;
			if (maximumZoom != null)
			{
				if (!maximumZoom.IsExpression)
				{
					this.m_coreMap.Viewport.MaximumZoom = (int)Math.Round(maximumZoom.Value);
				}
				else
				{
					this.m_coreMap.Viewport.MaximumZoom = (int)Math.Round(mapViewport.Instance.MaximumZoom);
				}
			}
			maximumZoom = mapViewport.MinimumZoom;
			if (maximumZoom != null)
			{
				if (!maximumZoom.IsExpression)
				{
					this.m_coreMap.Viewport.MinimumZoom = (int)Math.Round(maximumZoom.Value);
				}
				else
				{
					this.m_coreMap.Viewport.MinimumZoom = (int)Math.Round(mapViewport.Instance.MinimumZoom);
				}
			}
			ReportSizeProperty contentMargin = mapViewport.ContentMargin;
			ReportSize size = (contentMargin == null) ? MapMapper.m_defaultContentMargin : (contentMargin.IsExpression ? mapViewport.Instance.ContentMargin : mapViewport.ContentMargin.Value);
			this.m_coreMap.Viewport.ContentAutoFitMargin = MappingHelper.ToIntPixels(size, base.DpiX);
			ReportBoolProperty gridUnderContent = mapViewport.GridUnderContent;
			if (gridUnderContent != null)
			{
				if (!gridUnderContent.IsExpression)
				{
					this.m_coreMap.GridUnderContent = gridUnderContent.Value;
				}
				else
				{
					this.m_coreMap.GridUnderContent = gridUnderContent.Value;
				}
			}
			else
			{
				this.m_coreMap.GridUnderContent = false;
			}
		}

		private Projection GetProjection(MapProjection projection)
		{
			switch (projection)
			{
			case MapProjection.Bonne:
				return Projection.Bonne;
			case MapProjection.Eckert1:
				return Projection.Eckert1;
			case MapProjection.Eckert3:
				return Projection.Eckert3;
			case MapProjection.Fahey:
				return Projection.Fahey;
			case MapProjection.HammerAitoff:
				return Projection.HammerAitoff;
			case MapProjection.Mercator:
				return Projection.Mercator;
			case MapProjection.Robinson:
				return Projection.Robinson;
			case MapProjection.Wagner3:
				return Projection.Wagner3;
			default:
				return Projection.Equirectangular;
			}
		}

		private void SetGridLinesProperties(MapGridLines mapGridLines, GridAttributes coreGridLines)
		{
			if (mapGridLines != null)
			{
				ReportBoolProperty hidden = mapGridLines.Hidden;
				if (hidden != null)
				{
					if (!hidden.IsExpression)
					{
						coreGridLines.Visible = !hidden.Value;
					}
					else
					{
						coreGridLines.Visible = !mapGridLines.Instance.Hidden;
					}
				}
				ReportDoubleProperty interval = mapGridLines.Interval;
				if (interval != null)
				{
					if (!interval.IsExpression)
					{
						coreGridLines.Interval = interval.Value;
					}
					else
					{
						coreGridLines.Interval = mapGridLines.Instance.Interval;
					}
				}
				ReportBoolProperty showLabels = mapGridLines.ShowLabels;
				if (showLabels != null)
				{
					if (!showLabels.IsExpression)
					{
						coreGridLines.ShowLabels = showLabels.Value;
					}
					else
					{
						coreGridLines.ShowLabels = mapGridLines.Instance.ShowLabels;
					}
				}
				ReportEnumProperty<MapLabelPosition> labelPosition = mapGridLines.LabelPosition;
				MapLabelPosition labelPosition2 = MapLabelPosition.Near;
				if (labelPosition != null)
				{
					labelPosition2 = (labelPosition.IsExpression ? mapGridLines.Instance.LabelPosition : labelPosition.Value);
				}
				coreGridLines.LabelPosition = this.GetLabelPosition(labelPosition2);
			}
		}

		private LabelPosition GetLabelPosition(MapLabelPosition labelPosition)
		{
			switch (labelPosition)
			{
			case MapLabelPosition.Center:
				return LabelPosition.Center;
			case MapLabelPosition.Far:
				return LabelPosition.Far;
			case MapLabelPosition.OneQuarter:
				return LabelPosition.OneQuarter;
			case MapLabelPosition.ThreeQuarters:
				return LabelPosition.ThreeQuarters;
			default:
				return LabelPosition.Near;
			}
		}

		private void SetLegendProperties(MapLegend mapLegend, AspNetCore.Reporting.Map.WebForms.Legend legend)
		{
			legend.MaxAutoSize = 50f;
			Style style = mapLegend.Style;
			if (style == null)
			{
				legend.Font = base.GetDefaultFontFromCache(0);
			}
			else
			{
				StyleInstance style2 = mapLegend.Instance.Style;
				legend.Font = base.GetFontFromCache(0, style, style2);
				legend.TextColor = MappingHelper.GetStyleColor(style, style2);
			}
			legend.AutoSize = (mapLegend.MapSize == null);
			if (mapLegend.Hidden != null)
			{
				if (!mapLegend.Hidden.IsExpression)
				{
					legend.Visible = !mapLegend.Hidden.Value;
				}
				else
				{
					legend.Visible = !mapLegend.Instance.Hidden;
				}
			}
			if (mapLegend.Layout != null)
			{
				if (!mapLegend.Layout.IsExpression)
				{
					this.SetLegendLayout(mapLegend.Layout.Value, legend);
				}
				else
				{
					this.SetLegendLayout(mapLegend.Instance.Layout, legend);
				}
			}
			if (mapLegend.AutoFitTextDisabled != null)
			{
				if (!mapLegend.AutoFitTextDisabled.IsExpression)
				{
					legend.AutoFitText = !mapLegend.AutoFitTextDisabled.Value;
				}
				else
				{
					legend.AutoFitText = !mapLegend.Instance.AutoFitTextDisabled;
				}
			}
			else
			{
				legend.AutoFitText = true;
			}
			if (mapLegend.EquallySpacedItems != null)
			{
				if (!mapLegend.EquallySpacedItems.IsExpression)
				{
					legend.EquallySpacedItems = mapLegend.EquallySpacedItems.Value;
				}
				else
				{
					legend.EquallySpacedItems = mapLegend.Instance.EquallySpacedItems;
				}
			}
			if (mapLegend.InterlacedRows != null)
			{
				if (!mapLegend.InterlacedRows.IsExpression)
				{
					legend.InterlacedRows = mapLegend.InterlacedRows.Value;
				}
				else
				{
					legend.InterlacedRows = mapLegend.Instance.InterlacedRows;
				}
			}
			if (mapLegend.InterlacedRowsColor != null)
			{
				Color empty = Color.Empty;
				if (MappingHelper.GetColorFromReportColorProperty(mapLegend.InterlacedRowsColor, ref empty))
				{
					legend.InterlacedRowsColor = empty;
				}
				else if (mapLegend.Instance.InterlacedRowsColor != null)
				{
					legend.InterlacedRowsColor = mapLegend.Instance.InterlacedRowsColor.ToColor();
				}
			}
			if (mapLegend.MinFontSize != null)
			{
				if (!mapLegend.MinFontSize.IsExpression)
				{
					legend.AutoFitMinFontSize = (int)Math.Round(mapLegend.MinFontSize.Value.ToPoints());
				}
				else
				{
					legend.AutoFitMinFontSize = (int)Math.Round(mapLegend.Instance.MinFontSize.ToPoints());
				}
			}
			if (mapLegend.TextWrapThreshold != null)
			{
				if (!mapLegend.TextWrapThreshold.IsExpression)
				{
					legend.TextWrapThreshold = mapLegend.TextWrapThreshold.Value;
				}
				else
				{
					legend.TextWrapThreshold = mapLegend.Instance.TextWrapThreshold;
				}
			}
		}

		private void SetLegendLayout(MapLegendLayout layout, AspNetCore.Reporting.Map.WebForms.Legend legend)
		{
			switch (layout)
			{
			case MapLegendLayout.Row:
				legend.LegendStyle = LegendStyle.Row;
				break;
			case MapLegendLayout.Column:
				legend.LegendStyle = LegendStyle.Column;
				break;
			case MapLegendLayout.AutoTable:
				legend.LegendStyle = LegendStyle.Table;
				legend.TableStyle = LegendTableStyle.Auto;
				break;
			case MapLegendLayout.TallTable:
				legend.LegendStyle = LegendStyle.Table;
				legend.TableStyle = LegendTableStyle.Tall;
				break;
			case MapLegendLayout.WideTable:
				legend.LegendStyle = LegendStyle.Table;
				legend.TableStyle = LegendTableStyle.Wide;
				break;
			}
		}

		private void SetLegendTitleProperties(MapLegendTitle mapLegendTitle, AspNetCore.Reporting.Map.WebForms.Legend legend)
		{
			if (mapLegendTitle.Caption != null)
			{
				if (!mapLegendTitle.Caption.IsExpression)
				{
					if (mapLegendTitle.Caption.Value != null)
					{
						legend.Title = mapLegendTitle.Caption.Value;
					}
				}
				else if (mapLegendTitle.Instance.Caption != null)
				{
					legend.Title = mapLegendTitle.Instance.Caption;
				}
			}
			if (mapLegendTitle.TitleSeparator != null)
			{
				if (!mapLegendTitle.TitleSeparator.IsExpression)
				{
					legend.TitleSeparator = this.GetLegendSeparatorStyle(mapLegendTitle.TitleSeparator.Value);
				}
				else
				{
					legend.TitleSeparator = this.GetLegendSeparatorStyle(mapLegendTitle.Instance.TitleSeparator);
				}
			}
		}

		private LegendSeparatorType GetLegendSeparatorStyle(MapLegendTitleSeparator legendTitleSeparator)
		{
			switch (legendTitleSeparator)
			{
			case MapLegendTitleSeparator.DashLine:
				return LegendSeparatorType.DashLine;
			case MapLegendTitleSeparator.DotLine:
				return LegendSeparatorType.DotLine;
			case MapLegendTitleSeparator.DoubleLine:
				return LegendSeparatorType.DoubleLine;
			case MapLegendTitleSeparator.GradientLine:
				return LegendSeparatorType.GradientLine;
			case MapLegendTitleSeparator.Line:
				return LegendSeparatorType.Line;
			case MapLegendTitleSeparator.ThickGradientLine:
				return LegendSeparatorType.ThickGradientLine;
			case MapLegendTitleSeparator.ThickLine:
				return LegendSeparatorType.ThickLine;
			default:
				return LegendSeparatorType.None;
			}
		}

		private void SetTitleProperties(MapTitle mapTitle, MapLabel coreTitle)
		{
			Style style = mapTitle.Style;
			if (style == null)
			{
				coreTitle.Font = base.GetDefaultFontFromCache(0);
			}
			else
			{
				StyleInstance style2 = mapTitle.Instance.Style;
				coreTitle.Font = base.GetFontFromCache(0, style, style2);
				coreTitle.TextColor = MappingHelper.GetStyleColor(style, style2);
				coreTitle.TextAlignment = MappingHelper.GetStyleContentAlignment(style, style2);
			}
			coreTitle.AutoSize = (mapTitle.MapSize == null);
			coreTitle.Name = mapTitle.Name;
			ReportDoubleProperty angle = mapTitle.Angle;
			if (angle != null)
			{
				if (!angle.IsExpression)
				{
					coreTitle.Angle = (float)angle.Value;
				}
				else
				{
					coreTitle.Angle = (float)mapTitle.Instance.Angle;
				}
			}
			ReportStringProperty text = mapTitle.Text;
			if (text != null)
			{
				if (!text.IsExpression)
				{
					if (text.Value != null)
					{
						coreTitle.Text = text.Value;
					}
				}
				else
				{
					string text2 = mapTitle.Instance.Text;
					if (text2 != null)
					{
						coreTitle.Text = text2;
					}
				}
			}
			ReportSizeProperty textShadowOffset = mapTitle.TextShadowOffset;
			if (textShadowOffset != null)
			{
				int shadowOffset = textShadowOffset.IsExpression ? MappingHelper.ToIntPixels(mapTitle.Instance.TextShadowOffset, base.DpiX) : MappingHelper.ToIntPixels(textShadowOffset.Value, base.DpiX);
				coreTitle.TextShadowOffset = MapMapper.GetValidShadowOffset(shadowOffset);
			}
		}

		private void SetDistanceScaleProperties()
		{
			MapDistanceScale mapDistanceScale = this.m_map.MapDistanceScale;
			Style style = mapDistanceScale.Style;
			if (style == null)
			{
				this.m_coreMap.DistanceScalePanel.Font = base.GetDefaultFontFromCache(0);
			}
			else
			{
				StyleInstance style2 = mapDistanceScale.Instance.Style;
				this.m_coreMap.DistanceScalePanel.Font = base.GetFontFromCache(0, style, style2);
				this.m_coreMap.DistanceScalePanel.LabelColor = MappingHelper.GetStyleColor(style, style2);
			}
			ReportColorProperty scaleColor = mapDistanceScale.ScaleColor;
			Color empty = Color.Empty;
			if (scaleColor != null)
			{
				if (MappingHelper.GetColorFromReportColorProperty(scaleColor, ref empty))
				{
					this.m_coreMap.DistanceScalePanel.ScaleForeColor = empty;
				}
				else
				{
					ReportColor scaleColor2 = mapDistanceScale.Instance.ScaleColor;
					if (scaleColor2 != null)
					{
						this.m_coreMap.DistanceScalePanel.ScaleForeColor = scaleColor2.ToColor();
					}
				}
			}
			else
			{
				this.m_coreMap.DistanceScalePanel.ScaleForeColor = Color.White;
			}
			scaleColor = mapDistanceScale.ScaleBorderColor;
			if (scaleColor != null)
			{
				if (MappingHelper.GetColorFromReportColorProperty(scaleColor, ref empty))
				{
					this.m_coreMap.DistanceScalePanel.ScaleBorderColor = empty;
				}
				else
				{
					ReportColor scaleBorderColor = mapDistanceScale.Instance.ScaleBorderColor;
					if (scaleBorderColor != null)
					{
						this.m_coreMap.DistanceScalePanel.ScaleBorderColor = scaleBorderColor.ToColor();
					}
				}
			}
			else
			{
				this.m_coreMap.DistanceScalePanel.ScaleBorderColor = Color.DarkGray;
			}
			if (!this.m_coreMap.GeographyMode)
			{
				this.m_coreMap.DistanceScalePanel.Visible = false;
			}
		}

		private void SetColorScaleProperties()
		{
			MapColorScale mapColorScale = this.m_map.MapColorScale;
			Style style = mapColorScale.Style;
			if (style == null)
			{
				this.m_coreMap.ColorSwatchPanel.Font = base.GetDefaultFontFromCache(0);
			}
			else
			{
				StyleInstance style2 = mapColorScale.Instance.Style;
				this.m_coreMap.ColorSwatchPanel.Font = base.GetFontFromCache(0, style, style2);
				this.m_coreMap.ColorSwatchPanel.LabelColor = MappingHelper.GetStyleColor(style, style2);
			}
			this.m_coreMap.ColorSwatchPanel.AutoSize = (mapColorScale.MapSize == null);
			ReportSizeProperty tickMarkLength = mapColorScale.TickMarkLength;
			ReportSize size = (tickMarkLength == null) ? MapMapper.m_defaultTickMarkLength : (tickMarkLength.IsExpression ? mapColorScale.Instance.TickMarkLength : tickMarkLength.Value);
			this.m_coreMap.ColorSwatchPanel.TickMarkLength = MappingHelper.ToIntPixels(size, base.DpiX);
			ReportColorProperty colorBarBorderColor = mapColorScale.ColorBarBorderColor;
			Color empty = Color.Empty;
			if (colorBarBorderColor != null)
			{
				if (MappingHelper.GetColorFromReportColorProperty(colorBarBorderColor, ref empty))
				{
					this.m_coreMap.ColorSwatchPanel.OutlineColor = empty;
				}
				else if (mapColorScale.Instance.ColorBarBorderColor != null)
				{
					this.m_coreMap.ColorSwatchPanel.OutlineColor = mapColorScale.Instance.ColorBarBorderColor.ToColor();
				}
			}
			else
			{
				this.m_coreMap.ColorSwatchPanel.OutlineColor = Color.Black;
			}
			colorBarBorderColor = mapColorScale.RangeGapColor;
			if (colorBarBorderColor != null)
			{
				if (MappingHelper.GetColorFromReportColorProperty(colorBarBorderColor, ref empty))
				{
					this.m_coreMap.ColorSwatchPanel.RangeGapColor = empty;
				}
				else if (mapColorScale.Instance.RangeGapColor != null)
				{
					this.m_coreMap.ColorSwatchPanel.RangeGapColor = mapColorScale.Instance.RangeGapColor.ToColor();
				}
			}
			else
			{
				this.m_coreMap.ColorSwatchPanel.RangeGapColor = Color.White;
			}
			ReportIntProperty labelInterval = mapColorScale.LabelInterval;
			if (labelInterval != null)
			{
				if (!labelInterval.IsExpression)
				{
					this.m_coreMap.ColorSwatchPanel.LabelInterval = labelInterval.Value;
				}
				else
				{
					this.m_coreMap.ColorSwatchPanel.LabelInterval = mapColorScale.Instance.LabelInterval;
				}
			}
			ReportStringProperty labelFormat = mapColorScale.LabelFormat;
			string text = null;
			if (labelFormat != null)
			{
				text = (labelFormat.IsExpression ? mapColorScale.Instance.LabelFormat : labelFormat.Value);
			}
			if (text != null)
			{
				this.m_coreMap.ColorSwatchPanel.NumericLabelFormat = text;
			}
			ReportEnumProperty<MapLabelPlacement> labelPlacement = mapColorScale.LabelPlacement;
			if (labelPlacement != null)
			{
				if (!labelPlacement.IsExpression)
				{
					this.m_coreMap.ColorSwatchPanel.LabelAlignment = this.GetLabelAlignment(labelPlacement.Value);
				}
				else
				{
					this.m_coreMap.ColorSwatchPanel.LabelAlignment = this.GetLabelAlignment(mapColorScale.Instance.LabelPlacement);
				}
			}
			ReportEnumProperty<MapLabelBehavior> labelBehavior = mapColorScale.LabelBehavior;
			if (labelBehavior != null)
			{
				if (!labelBehavior.IsExpression)
				{
					this.m_coreMap.ColorSwatchPanel.LabelType = this.GetSwatchLabelType(labelBehavior.Value);
				}
				else
				{
					this.m_coreMap.ColorSwatchPanel.LabelType = this.GetSwatchLabelType(mapColorScale.Instance.LabelBehavior);
				}
			}
			ReportBoolProperty hideEndLabels = mapColorScale.HideEndLabels;
			if (hideEndLabels != null)
			{
				if (!hideEndLabels.IsExpression)
				{
					this.m_coreMap.ColorSwatchPanel.ShowEndLabels = !hideEndLabels.Value;
				}
				else
				{
					this.m_coreMap.ColorSwatchPanel.ShowEndLabels = !mapColorScale.Instance.HideEndLabels;
				}
			}
			ReportStringProperty noDataText = mapColorScale.NoDataText;
			string text2 = null;
			if (noDataText != null)
			{
				text2 = (noDataText.IsExpression ? mapColorScale.Instance.NoDataText : noDataText.Value);
			}
			this.m_coreMap.ColorSwatchPanel.NoDataText = ((text2 != null) ? text2 : "");
		}

		private void SetColorScaleTitleProperties()
		{
			MapColorScaleTitle mapColorScaleTitle = this.m_map.MapColorScale.MapColorScaleTitle;
			ReportStringProperty caption = mapColorScaleTitle.Caption;
			string text = null;
			if (caption != null)
			{
				text = (caption.IsExpression ? mapColorScaleTitle.Instance.Caption : caption.Value);
			}
			this.m_coreMap.ColorSwatchPanel.Title = ((text != null) ? text : "");
		}

		private LabelAlignment GetLabelAlignment(MapLabelPlacement labelPlacement)
		{
			switch (labelPlacement)
			{
			case MapLabelPlacement.Bottom:
				return LabelAlignment.Bottom;
			case MapLabelPlacement.Top:
				return LabelAlignment.Top;
			default:
				return LabelAlignment.Alternate;
			}
		}

		private SwatchLabelType GetSwatchLabelType(MapLabelBehavior labelBehavior)
		{
			switch (labelBehavior)
			{
			case MapLabelBehavior.ShowBorderValue:
				return SwatchLabelType.ShowBorderValue;
			case MapLabelBehavior.ShowMiddleValue:
				return SwatchLabelType.ShowMiddleValue;
			default:
				return SwatchLabelType.Auto;
			}
		}

		private void SetBorderSkinProperties()
		{
			ReportEnumProperty<MapBorderSkinType> mapBorderSkinType = this.m_map.MapBorderSkin.MapBorderSkinType;
			if (mapBorderSkinType != null)
			{
				MapBorderSkinType mapBorderSkinType2 = MapBorderSkinType.None;
				mapBorderSkinType2 = (mapBorderSkinType.IsExpression ? this.m_map.MapBorderSkin.Instance.MapBorderSkinType : mapBorderSkinType.Value);
				this.m_coreMap.Frame.FrameStyle = MapMapper.GetFrameStyle(mapBorderSkinType2);
			}
		}

		private static FrameStyle GetFrameStyle(MapBorderSkinType type)
		{
			FrameStyle result = FrameStyle.None;
			switch (type)
			{
			case MapBorderSkinType.Emboss:
				result = FrameStyle.Emboss;
				break;
			case MapBorderSkinType.FrameThin1:
				result = FrameStyle.FrameThin1;
				break;
			case MapBorderSkinType.FrameThin2:
				result = FrameStyle.FrameThin2;
				break;
			case MapBorderSkinType.FrameThin3:
				result = FrameStyle.FrameThin3;
				break;
			case MapBorderSkinType.FrameThin4:
				result = FrameStyle.FrameThin4;
				break;
			case MapBorderSkinType.FrameThin5:
				result = FrameStyle.FrameThin5;
				break;
			case MapBorderSkinType.FrameThin6:
				result = FrameStyle.FrameThin6;
				break;
			case MapBorderSkinType.FrameTitle1:
				result = FrameStyle.FrameTitle1;
				break;
			case MapBorderSkinType.FrameTitle2:
				result = FrameStyle.FrameTitle2;
				break;
			case MapBorderSkinType.FrameTitle3:
				result = FrameStyle.FrameTitle3;
				break;
			case MapBorderSkinType.FrameTitle4:
				result = FrameStyle.FrameTitle4;
				break;
			case MapBorderSkinType.FrameTitle5:
				result = FrameStyle.FrameTitle5;
				break;
			case MapBorderSkinType.FrameTitle6:
				result = FrameStyle.FrameTitle6;
				break;
			case MapBorderSkinType.FrameTitle7:
				result = FrameStyle.FrameTitle7;
				break;
			case MapBorderSkinType.FrameTitle8:
				result = FrameStyle.FrameTitle8;
				break;
			case MapBorderSkinType.None:
				result = FrameStyle.None;
				break;
			case MapBorderSkinType.Raised:
				result = FrameStyle.Raised;
				break;
			case MapBorderSkinType.Sunken:
				result = FrameStyle.Sunken;
				break;
			}
			return result;
		}

		private void RenderMapStyle()
		{
			Border border = null;
			this.m_coreMap.BackColor = Color.Empty;
			Style style = this.m_map.Style;
			if (style != null)
			{
				StyleInstance style2 = this.m_map.Instance.Style;
				this.m_coreMap.BackColor = MappingHelper.GetStyleBackgroundColor(style, style2);
				this.m_coreMap.BackSecondaryColor = MappingHelper.GetStyleBackGradientEndColor(style, style2);
				this.m_coreMap.BackGradientType = MapMapper.GetGradientType(style, style2);
				this.m_coreMap.BackHatchStyle = MapMapper.GetHatchStyle(style, style2);
				border = this.m_map.Style.Border;
			}
			if (this.m_coreMap.BackColor.A != 255)
			{
				this.m_coreMap.AntiAliasing = AntiAliasing.None;
			}
			if (this.m_map.SpecialBorderHandling)
			{
				this.RenderMapBorder(border);
			}
		}

		private void RenderMapBorder(Border border)
		{
			if (border != null)
			{
				this.m_coreMap.BorderLineColor = MappingHelper.GetStyleBorderColor(border);
				this.m_coreMap.BorderLineStyle = MapMapper.GetDashStyle(MappingHelper.GetStyleBorderStyle(border), false);
				this.m_coreMap.BorderLineWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void RenderSubItemStyle(MapSubItem mapSubItem, Panel coreSubItem)
		{
			Style style = mapSubItem.Style;
			if (style == null)
			{
				coreSubItem.BackColor = Color.Empty;
			}
			else
			{
				StyleInstance style2 = mapSubItem.Instance.Style;
				coreSubItem.BackColor = MappingHelper.GetStyleBackgroundColor(style, style2);
				coreSubItem.BackSecondaryColor = MappingHelper.GetStyleBackGradientEndColor(style, style2);
				coreSubItem.BackGradientType = MapMapper.GetGradientType(style, style2);
				coreSubItem.BackHatchStyle = MapMapper.GetHatchStyle(style, style2);
				coreSubItem.BackShadowOffset = MapMapper.GetValidShadowOffset(MappingHelper.GetStyleShadowOffset(style, style2, base.DpiX));
				Border border = style.Border;
				if (border != null)
				{
					coreSubItem.BorderColor = MappingHelper.GetStyleBorderColor(border);
					coreSubItem.BorderStyle = MapMapper.GetDashStyle(MappingHelper.GetStyleBorderStyle(border), false);
					coreSubItem.BorderWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
				}
			}
		}

		private void RenderGridLinesStyle(MapGridLines mapGridLines, GridAttributes coreGridLines)
		{
			Style style = mapGridLines.Style;
			Border border = null;
			if (style != null)
			{
				border = style.Border;
			}
			if (style == null)
			{
				coreGridLines.Font = base.GetDefaultFontFromCache(0);
				coreGridLines.LabelColor = Color.Black;
				coreGridLines.LabelFormatString = "";
			}
			else
			{
				StyleInstance style2 = mapGridLines.Instance.Style;
				coreGridLines.Font = base.GetFontFromCache(0, style, style2);
				coreGridLines.LabelColor = MappingHelper.GetStyleColor(style, style2);
				coreGridLines.LabelFormatString = MappingHelper.GetStyleFormat(style, style2);
			}
			if (border == null)
			{
				coreGridLines.LineWidth = MappingHelper.GetDefaultBorderWidth(base.DpiX);
				coreGridLines.LineColor = Color.Black;
				coreGridLines.LineStyle = MapDashStyle.Solid;
			}
			else
			{
				coreGridLines.LineWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
				coreGridLines.LineColor = MappingHelper.GetStyleBorderColor(border);
				coreGridLines.LineStyle = MapMapper.GetDashStyle(MappingHelper.GetStyleBorderStyle(border), true);
			}
		}

		private void RenderLegendTitleStyle(MapLegendTitle mapLegendTitle, AspNetCore.Reporting.Map.WebForms.Legend legend)
		{
			Style style = mapLegendTitle.Style;
			if (style != null)
			{
				StyleInstance style2 = mapLegendTitle.Instance.Style;
				if (MappingHelper.IsStylePropertyDefined(style.Color))
				{
					legend.TitleColor = MappingHelper.GetStyleColor(style, style2);
				}
				if (MappingHelper.IsStylePropertyDefined(style.BackgroundColor))
				{
					legend.TitleBackColor = MappingHelper.GetStyleBackgroundColor(style, style2);
				}
				this.RenderLegendTitleBorder(style.Border, legend);
				legend.TitleAlignment = this.GetLegendTitleAlign(MappingHelper.GetStyleTextAlign(style, style2));
			}
			this.RenderLegendTitleFont(mapLegendTitle, legend);
		}

		private void RenderLegendTitleFont(MapLegendTitle mapLegendTitle, AspNetCore.Reporting.Map.WebForms.Legend legend)
		{
			Style style = mapLegendTitle.Style;
			if (style == null)
			{
				legend.TitleFont = base.GetDefaultFont();
			}
			else
			{
				legend.TitleFont = base.GetFont(style, mapLegendTitle.Instance.Style);
			}
		}

		private StringAlignment GetLegendTitleAlign(TextAlignments textAlignment)
		{
			switch (textAlignment)
			{
			case TextAlignments.Left:
				return StringAlignment.Near;
			case TextAlignments.Right:
				return StringAlignment.Far;
			default:
				return StringAlignment.Center;
			}
		}

		private void RenderLegendTitleBorder(Border border, AspNetCore.Reporting.Map.WebForms.Legend legend)
		{
			if (border != null && MappingHelper.IsStylePropertyDefined(border.Color))
			{
				legend.TitleSeparatorColor = MappingHelper.GetStyleBorderColor(border);
			}
		}

		private void RenderColorScaleTitleStyle()
		{
			Style style = this.m_map.MapColorScale.MapColorScaleTitle.Style;
			if (style == null)
			{
				this.m_coreMap.ColorSwatchPanel.TitleFont = base.GetDefaultFontFromCache(0);
			}
			else
			{
				StyleInstance style2 = this.m_map.MapColorScale.MapColorScaleTitle.Instance.Style;
				this.m_coreMap.ColorSwatchPanel.TitleColor = MappingHelper.GetStyleColor(style, style2);
				this.m_coreMap.ColorSwatchPanel.TitleFont = base.GetFontFromCache(0, style, style2);
			}
		}

		private void RenderBorderSkinStyle()
		{
			Style style = this.m_map.MapBorderSkin.Style;
			if (style != null)
			{
				StyleInstance style2 = this.m_map.MapBorderSkin.Instance.Style;
				Frame frame = this.m_coreMap.Frame;
				frame.PageColor = MappingHelper.GetStyleColor(style, style2);
				frame.BackColor = MappingHelper.GetStyleBackgroundColor(style, style2);
				frame.BackSecondaryColor = MappingHelper.GetStyleBackGradientEndColor(style, style2);
				frame.BackGradientType = MapMapper.GetGradientType(style, style2);
				frame.BackHatchStyle = MapMapper.GetHatchStyle(style, style2);
				this.RenderBorderSkinBorder(style.Border, frame);
			}
		}

		private void RenderBorderSkinBorder(Border border, Frame borderSkin)
		{
			if (border != null)
			{
				if (MappingHelper.IsStylePropertyDefined(border.Color))
				{
					borderSkin.BorderColor = MappingHelper.GetStyleBorderColor(border);
				}
				if (MappingHelper.IsStylePropertyDefined(border.Style))
				{
					borderSkin.BorderStyle = MapMapper.GetDashStyle(MappingHelper.GetStyleBorderStyle(border), false);
				}
				borderSkin.BorderWidth = MappingHelper.GetStyleBorderWidth(border, base.DpiX);
			}
		}

		private void GetPngImage(out Stream imageStream, int width, int height)
		{
			using (Bitmap bitmap = new Bitmap(width, height))
			{
				bitmap.SetResolution(base.DpiX, base.DpiY);
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					this.GetImage(graphics);
					imageStream = new MemoryStream();
					bitmap.Save(imageStream, ImageFormat.Png);
				}
			}
		}

		private void GetEmfImage(out Stream imageStream, int width, int height)
		{
			using (Bitmap image = new Bitmap(width, height))
			{
				using (Graphics graphics = Graphics.FromImage(image))
				{
					IntPtr hdc = graphics.GetHdc();
					imageStream = this.m_map.RenderingContext.OdpContext.CreateStreamCallback(this.m_map.Name, "emf", null, "image/emf", true, StreamOper.CreateOnly);
					using (Metafile image2 = new Metafile(imageStream, hdc, new System.Drawing.Rectangle(0, 0, width, height), MetafileFrameUnit.Pixel, EmfType.EmfPlusOnly))
					{
						using (Graphics graphics2 = Graphics.FromImage(image2))
						{
							this.GetImage(graphics2);
						}
					}
					graphics.ReleaseHdc(hdc);
				}
			}
		}

		private void GetImage(Graphics graphics)
		{
			this.m_coreMap.mapCore.Paint(graphics);
		}

		private static LayerVisibility GetLayerVisibility(MapVisibilityMode visibility)
		{
			switch (visibility)
			{
			case MapVisibilityMode.Hidden:
				return LayerVisibility.Hidden;
			case MapVisibilityMode.ZoomBased:
				return LayerVisibility.ZoomBased;
			default:
				return LayerVisibility.Shown;
			}
		}

		internal static MarkerStyle GetMarkerStyle(MapMarkerStyle mapMarkerStyle)
		{
			switch (mapMarkerStyle)
			{
			case MapMarkerStyle.Circle:
				return MarkerStyle.Circle;
			case MapMarkerStyle.Diamond:
				return MarkerStyle.Diamond;
			case MapMarkerStyle.Pentagon:
				return MarkerStyle.Pentagon;
			case MapMarkerStyle.PushPin:
				return MarkerStyle.PushPin;
			case MapMarkerStyle.Rectangle:
				return MarkerStyle.Rectangle;
			case MapMarkerStyle.Star:
				return MarkerStyle.Star;
			case MapMarkerStyle.Trapezoid:
				return MarkerStyle.Trapezoid;
			case MapMarkerStyle.Triangle:
				return MarkerStyle.Triangle;
			case MapMarkerStyle.Wedge:
				return MarkerStyle.Wedge;
			default:
				return MarkerStyle.None;
			}
		}

		internal static MapMarkerStyle GetMarkerStyle(MapMarker mapMarker, bool hasScope)
		{
			if (mapMarker != null)
			{
				ReportEnumProperty<MapMarkerStyle> mapMarkerStyle = mapMarker.MapMarkerStyle;
				if (mapMarkerStyle != null)
				{
					if (!mapMarkerStyle.IsExpression)
					{
						return mapMarkerStyle.Value;
					}
					if (hasScope)
					{
						return mapMarker.Instance.MapMarkerStyle;
					}
				}
			}
			return MapMarkerStyle.None;
		}

		internal static GradientType GetGradientType(Style style, StyleInstance styleInstance)
		{
			switch (MappingHelper.GetStyleBackGradientType(style, styleInstance))
			{
			case BackgroundGradients.Center:
				return GradientType.Center;
			case BackgroundGradients.DiagonalLeft:
				return GradientType.DiagonalLeft;
			case BackgroundGradients.DiagonalRight:
				return GradientType.DiagonalRight;
			case BackgroundGradients.HorizontalCenter:
				return GradientType.HorizontalCenter;
			case BackgroundGradients.LeftRight:
				return GradientType.LeftRight;
			case BackgroundGradients.TopBottom:
				return GradientType.TopBottom;
			case BackgroundGradients.VerticalCenter:
				return GradientType.VerticalCenter;
			default:
				return GradientType.None;
			}
		}

		internal static MapHatchStyle GetHatchStyle(Style style, StyleInstance styleInstance)
		{
			switch (MappingHelper.GetStyleBackgroundHatchType(style, styleInstance))
			{
			case BackgroundHatchTypes.BackwardDiagonal:
				return MapHatchStyle.BackwardDiagonal;
			case BackgroundHatchTypes.Cross:
				return MapHatchStyle.Cross;
			case BackgroundHatchTypes.DarkDownwardDiagonal:
				return MapHatchStyle.DarkDownwardDiagonal;
			case BackgroundHatchTypes.DarkHorizontal:
				return MapHatchStyle.DarkHorizontal;
			case BackgroundHatchTypes.DarkUpwardDiagonal:
				return MapHatchStyle.DarkUpwardDiagonal;
			case BackgroundHatchTypes.DarkVertical:
				return MapHatchStyle.DarkVertical;
			case BackgroundHatchTypes.DashedDownwardDiagonal:
				return MapHatchStyle.DashedDownwardDiagonal;
			case BackgroundHatchTypes.DashedHorizontal:
				return MapHatchStyle.DashedHorizontal;
			case BackgroundHatchTypes.DashedUpwardDiagonal:
				return MapHatchStyle.DashedUpwardDiagonal;
			case BackgroundHatchTypes.DashedVertical:
				return MapHatchStyle.DashedVertical;
			case BackgroundHatchTypes.DiagonalBrick:
				return MapHatchStyle.DiagonalBrick;
			case BackgroundHatchTypes.DiagonalCross:
				return MapHatchStyle.DiagonalCross;
			case BackgroundHatchTypes.Divot:
				return MapHatchStyle.Divot;
			case BackgroundHatchTypes.DottedDiamond:
				return MapHatchStyle.DottedDiamond;
			case BackgroundHatchTypes.DottedGrid:
				return MapHatchStyle.DottedGrid;
			case BackgroundHatchTypes.ForwardDiagonal:
				return MapHatchStyle.ForwardDiagonal;
			case BackgroundHatchTypes.Horizontal:
				return MapHatchStyle.Horizontal;
			case BackgroundHatchTypes.HorizontalBrick:
				return MapHatchStyle.HorizontalBrick;
			case BackgroundHatchTypes.LargeCheckerBoard:
				return MapHatchStyle.LargeCheckerBoard;
			case BackgroundHatchTypes.LargeConfetti:
				return MapHatchStyle.LargeConfetti;
			case BackgroundHatchTypes.LargeGrid:
				return MapHatchStyle.LargeGrid;
			case BackgroundHatchTypes.LightDownwardDiagonal:
				return MapHatchStyle.LightDownwardDiagonal;
			case BackgroundHatchTypes.LightHorizontal:
				return MapHatchStyle.LightHorizontal;
			case BackgroundHatchTypes.LightUpwardDiagonal:
				return MapHatchStyle.LightUpwardDiagonal;
			case BackgroundHatchTypes.LightVertical:
				return MapHatchStyle.LightVertical;
			case BackgroundHatchTypes.NarrowHorizontal:
				return MapHatchStyle.NarrowHorizontal;
			case BackgroundHatchTypes.NarrowVertical:
				return MapHatchStyle.NarrowVertical;
			case BackgroundHatchTypes.OutlinedDiamond:
				return MapHatchStyle.OutlinedDiamond;
			case BackgroundHatchTypes.Percent05:
				return MapHatchStyle.Percent05;
			case BackgroundHatchTypes.Percent10:
				return MapHatchStyle.Percent10;
			case BackgroundHatchTypes.Percent20:
				return MapHatchStyle.Percent20;
			case BackgroundHatchTypes.Percent25:
				return MapHatchStyle.Percent25;
			case BackgroundHatchTypes.Percent30:
				return MapHatchStyle.Percent30;
			case BackgroundHatchTypes.Percent40:
				return MapHatchStyle.Percent40;
			case BackgroundHatchTypes.Percent50:
				return MapHatchStyle.Percent50;
			case BackgroundHatchTypes.Percent60:
				return MapHatchStyle.Percent60;
			case BackgroundHatchTypes.Percent70:
				return MapHatchStyle.Percent70;
			case BackgroundHatchTypes.Percent75:
				return MapHatchStyle.Percent75;
			case BackgroundHatchTypes.Percent80:
				return MapHatchStyle.Percent80;
			case BackgroundHatchTypes.Percent90:
				return MapHatchStyle.Percent90;
			case BackgroundHatchTypes.Plaid:
				return MapHatchStyle.Plaid;
			case BackgroundHatchTypes.Shingle:
				return MapHatchStyle.Shingle;
			case BackgroundHatchTypes.SmallCheckerBoard:
				return MapHatchStyle.SmallCheckerBoard;
			case BackgroundHatchTypes.SmallConfetti:
				return MapHatchStyle.SmallConfetti;
			case BackgroundHatchTypes.SmallGrid:
				return MapHatchStyle.SmallGrid;
			case BackgroundHatchTypes.SolidDiamond:
				return MapHatchStyle.SolidDiamond;
			case BackgroundHatchTypes.Sphere:
				return MapHatchStyle.Sphere;
			case BackgroundHatchTypes.Trellis:
				return MapHatchStyle.Trellis;
			case BackgroundHatchTypes.Vertical:
				return MapHatchStyle.Vertical;
			case BackgroundHatchTypes.Wave:
				return MapHatchStyle.Wave;
			case BackgroundHatchTypes.Weave:
				return MapHatchStyle.Weave;
			case BackgroundHatchTypes.WideDownwardDiagonal:
				return MapHatchStyle.WideDownwardDiagonal;
			case BackgroundHatchTypes.WideUpwardDiagonal:
				return MapHatchStyle.WideUpwardDiagonal;
			case BackgroundHatchTypes.ZigZag:
				return MapHatchStyle.ZigZag;
			default:
				return MapHatchStyle.None;
			}
		}

		internal static int GetValidShadowOffset(int shadowOffset)
		{
			return Math.Min(shadowOffset, 100);
		}

		internal static MapDashStyle GetDashStyle(Border border, bool hasScope, bool isLine)
		{
			BorderStyles borderStyle = (!MappingHelper.IsPropertyExpression(border.Style) || hasScope) ? MappingHelper.GetStyleBorderStyle(border) : BorderStyles.Default;
			return MapMapper.GetDashStyle(borderStyle, isLine);
		}

		private static MapDashStyle GetDashStyle(BorderStyles borderStyle, bool isLine)
		{
			switch (borderStyle)
			{
			case BorderStyles.DashDot:
				return MapDashStyle.DashDot;
			case BorderStyles.DashDotDot:
				return MapDashStyle.DashDotDot;
			case BorderStyles.Dashed:
				return MapDashStyle.Dash;
			case BorderStyles.Dotted:
				return MapDashStyle.Dot;
			case BorderStyles.Solid:
			case BorderStyles.Double:
				return MapDashStyle.Solid;
			case BorderStyles.None:
				return MapDashStyle.None;
			default:
				if (isLine)
				{
					return MapDashStyle.Solid;
				}
				return MapDashStyle.None;
			}
		}

		internal string AddImage(MapMarkerImage mapMarkerImage)
		{
			byte[] imageData = mapMarkerImage.Instance.ImageData;
			if (imageData == null)
			{
				return "";
			}
			MemoryStream stream = new MemoryStream(imageData, false);
			System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
			if (image == null)
			{
				return "";
			}
			string text = this.m_coreMap.NamedImages.Count.ToString(CultureInfo.InvariantCulture);
			this.m_coreMap.NamedImages.Add(new NamedImage(text, image));
			return text;
		}

		internal ResizeMode GetImageResizeMode(MapMarkerImage mapMarkerImage)
		{
			ReportEnumProperty<MapResizeMode> resizeMode = mapMarkerImage.ResizeMode;
			if (resizeMode != null)
			{
				if (!resizeMode.IsExpression)
				{
					return this.GetResizeMode(resizeMode.Value);
				}
				return this.GetResizeMode(mapMarkerImage.Instance.ResizeMode);
			}
			return ResizeMode.AutoFit;
		}

		private ResizeMode GetResizeMode(MapResizeMode resizeMode)
		{
			if (resizeMode == MapResizeMode.None)
			{
				return ResizeMode.None;
			}
			return ResizeMode.AutoFit;
		}

		internal Color GetImageTransColor(MapMarkerImage image)
		{
			ReportColorProperty transparentColor = image.TransparentColor;
			Color result = Color.Empty;
			if (transparentColor != null && !MappingHelper.GetColorFromReportColorProperty(transparentColor, ref result))
			{
				ReportColor transparentColor2 = image.Instance.TransparentColor;
				if (transparentColor2 != null)
				{
					result = transparentColor2.ToColor();
				}
			}
			return result;
		}

		internal void RenderActionInfo(ActionInfo actionInfo, string toolTip, IImageMapProvider imageMapProvider, string layerName, bool hasScope)
		{
			if (actionInfo == null && string.IsNullOrEmpty(toolTip))
			{
				return;
			}
			string text = default(string);
			ActionInfoWithDynamicImageMap actionInfoWithDynamicImageMap = MappingHelper.CreateActionInfoDynamic((ReportItem)this.m_map, actionInfo, toolTip, out text, hasScope);
			if (actionInfoWithDynamicImageMap != null)
			{
				if (text != null)
				{
					if (layerName != null)
					{
						text = VectorLayerMapper.AddPrefixToFieldNames(layerName, text);
					}
					imageMapProvider.Href = text;
				}
				int count = this.m_actions.Count;
				this.m_actions.InternalList.Add(actionInfoWithDynamicImageMap);
				imageMapProvider.Tag = count;
			}
		}

		internal void OnSpatialElementAdded(SpatialElementInfo spatialElementInfo)
		{
			this.DecrementRemainingSpatialElementCount();
			if (spatialElementInfo.CoreSpatialElement.Points != null)
			{
				this.DecrementRemainingTotalCount(spatialElementInfo.CoreSpatialElement.Points.Length);
			}
		}

		private void DecrementRemainingSpatialElementCount()
		{
			this.m_remainingSpatialElementCount--;
		}

		private void DecrementRemainingTotalCount(int count)
		{
			this.m_remainingTotalPointCount -= count;
		}

		private ImageMapArea.ImageMapAreaShape GetMapAreaShape(MapAreaShape shape)
		{
			if (shape == MapAreaShape.Rectangle)
			{
				return ImageMapArea.ImageMapAreaShape.Rectangle;
			}
			if (MapAreaShape.Circle == shape)
			{
				return ImageMapArea.ImageMapAreaShape.Circle;
			}
			return ImageMapArea.ImageMapAreaShape.Polygon;
		}

		private string FormatNumber(object sender, object value, string format)
		{
			if (this.m_formatter == null)
			{
				this.m_formatter = new Formatter(this.m_map.MapDef.StyleClass, this.m_map.RenderingContext.OdpContext, this.m_map.MapDef.ObjectType, this.m_map.Name);
			}
			return this.m_formatter.FormatValue(value, format, Type.GetTypeCode(value.GetType()));
		}

		private double EvaluateSimplificationResolution()
		{
			ReportDoubleProperty simplificationResolution = this.m_map.MapViewport.SimplificationResolution;
			if (simplificationResolution != null)
			{
				if (!simplificationResolution.IsExpression)
				{
					return simplificationResolution.Value;
				}
				return this.m_map.MapViewport.Instance.SimplificationResolution;
			}
			return 0.0;
		}

		internal void Simplify(Shape shape)
		{
			if (this.Simplifier != null)
			{
				this.Simplifier.Simplify(shape, this.m_simpificationResolution.Value);
			}
		}

		internal void Simplify(AspNetCore.Reporting.Map.WebForms.Path path)
		{
			if (this.Simplifier != null)
			{
				this.Simplifier.Simplify(path, this.m_simpificationResolution.Value);
			}
		}

		public override void Dispose()
		{
			if (this.m_coreMap != null)
			{
				this.m_coreMap.Dispose();
			}
			this.m_coreMap = null;
			base.Dispose();
		}
	}
}
