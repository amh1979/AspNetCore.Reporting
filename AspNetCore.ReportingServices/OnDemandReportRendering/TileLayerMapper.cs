using AspNetCore.Reporting.Map.WebForms;
using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class TileLayerMapper
	{
		private bool m_success;

		private Map m_map;

		private MapControl m_coreMap;

		private Dictionary<string, MapTileLayer> m_mapTileLayers;

		internal TileLayerMapper(Map map, MapControl coreMap)
		{
			this.m_map = map;
			this.m_coreMap = coreMap;
			this.m_mapTileLayers = new Dictionary<string, MapTileLayer>();
			this.m_coreMap.mapCore.LoadTilesHandler = this.LoadTilesHandler;
			this.m_coreMap.mapCore.SaveTilesHandler = this.SaveTilesHandler;
		}

		internal void AddLayer(MapTileLayer mapTileLayer)
		{
			this.m_mapTileLayers.Add(mapTileLayer.Name, mapTileLayer);
			this.m_coreMap.Layers[mapTileLayer.Name].TileSystem = this.GetTileSystem(mapTileLayer);
			this.m_coreMap.Layers[mapTileLayer.Name].UseSecureConnectionForTiles = this.GetUseSecureConnection(mapTileLayer);
		}

		private bool GetUseSecureConnection(MapTileLayer mapTileLayer)
		{
			ReportBoolProperty useSecureConnection = mapTileLayer.UseSecureConnection;
			if (useSecureConnection == null)
			{
				return false;
			}
			if (!useSecureConnection.IsExpression)
			{
				return useSecureConnection.Value;
			}
			return mapTileLayer.Instance.UseSecureConnection;
		}

		private TileSystem GetTileSystem(MapTileLayer mapTileLayer)
		{
			ReportEnumProperty<MapTileStyle> tileStyle = mapTileLayer.TileStyle;
			MapTileStyle mapTileStyle = MapTileStyle.Road;
			if (tileStyle != null)
			{
				mapTileStyle = (tileStyle.IsExpression ? mapTileLayer.Instance.TileStyle : tileStyle.Value);
			}
			switch (mapTileStyle)
			{
			case MapTileStyle.Aerial:
				return TileSystem.VirtualEarthAerial;
			case MapTileStyle.Hybrid:
				return TileSystem.VirtualEarthHybrid;
			default:
				return TileSystem.VirtualEarthRoad;
			}
		}

		private bool Embedded(MapTileLayer mapTileLayer)
		{
			return mapTileLayer.MapTiles != null;
		}

		private MapTileLayer GetLayer(string layerName)
		{
			MapTileLayer mapTileLayer = default(MapTileLayer);
			this.m_mapTileLayers.TryGetValue(layerName, out mapTileLayer);
			Global.Tracer.Assert(null != mapTileLayer, "null != tileLayer");
			return mapTileLayer;
		}

		private System.Drawing.Image[,] LoadTilesHandler(Layer layer, string[,] tileUrls)
		{
			System.Drawing.Image[,] array = null;
			int num = tileUrls.GetUpperBound(0) + 1;
			int num2 = tileUrls.GetUpperBound(1) + 1;
			MapTileLayer layer2 = this.GetLayer(layer.Name);
			try
			{
				for (int i = 0; i < num; i++)
				{
					for (int j = 0; j < num2; j++)
					{
						System.Drawing.Image image = (!this.Embedded(layer2)) ? this.GetSnapshotTile(layer2, tileUrls[i, j]) : this.GetEmbeddedTile(layer2, tileUrls[i, j]);
						if (image == null)
						{
							this.DisposeTiles(array, num, num2);
							return null;
						}
						if (array == null)
						{
							array = new System.Drawing.Image[num, num2];
						}
						array[i, j] = image;
					}
				}
			}
			catch (Exception e)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(e))
				{
					throw;
				}
				this.DisposeTiles(array, num, num2);
				return null;
			}
			this.m_success = (array != null);
			return array;
		}

		private void DisposeTiles(System.Drawing.Image[,] tiles, int row, int col)
		{
			if (tiles != null)
			{
				for (int i = 0; i < row; i++)
				{
					for (int j = 0; j < col; j++)
					{
						if (tiles[i, j] != null)
						{
							tiles[i, j].Dispose();
						}
					}
				}
			}
		}

		private System.Drawing.Image GetSnapshotTile(MapTileLayer mapTileLayer, string url)
		{
			string text = default(string);
			Stream tileData = mapTileLayer.Instance.GetTileData(url, out text);
			if (tileData == null)
			{
				return null;
			}
			return System.Drawing.Image.FromStream(tileData);
		}

		private System.Drawing.Image GetEmbeddedTile(MapTileLayer mapTileLayer, string url)
		{
			foreach (MapTile mapTile in mapTileLayer.MapTiles)
			{
				if (mapTile.Name == url)
				{
					using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(mapTile.TileData)))
					{
						return System.Drawing.Image.FromStream(stream);
					}
				}
			}
			return null;
		}

		private void SaveTilesHandler(Layer layer, string[,] tileUrls, System.Drawing.Image[,] tileImages)
		{
			MapTileLayer layer2 = this.GetLayer(layer.Name);
			if (!this.Embedded(layer2) && !this.m_success)
			{
				int num = tileUrls.GetUpperBound(0) + 1;
				int num2 = tileUrls.GetUpperBound(1) + 1;
				for (int i = 0; i < num; i++)
				{
					for (int j = 0; j < num2; j++)
					{
						string url = tileUrls[i, j];
						System.Drawing.Image image = tileImages[i, j];
						using (MemoryStream memoryStream = new MemoryStream())
						{
							image.Save(memoryStream, ImageFormat.Png);
							string text = default(string);
							if (layer2.Instance.GetTileData(url, out text) == null)
							{
								layer2.Instance.SetTileData(url, memoryStream.ToArray(), null);
							}
						}
					}
				}
			}
		}
	}
}
