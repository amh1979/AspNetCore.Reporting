using AspNetCore.Reporting.Map.WebForms.BingMaps;
using System;
using System.Text;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal static class VirtualEarthTileSystem
	{
		internal const int TileSize = 256;

		internal const double EarthRadius = 6378137.0;

		private const double MinLatitude = -85.05112878;

		private const double MaxLatitude = 85.05112878;

		private const double MinLongitude = -180.0;

		private const double MaxLongitude = 180.0;

		private static double Clip(double n, double minValue, double maxValue)
		{
			return Math.Min(Math.Max(n, minValue), maxValue);
		}

		public static uint MapSize(int levelOfDetail)
		{
			return (uint)(256 << levelOfDetail);
		}

		public static uint MapSize(double levelOfDetail)
		{
			return (uint)(256.0 * Math.Pow(2.0, levelOfDetail));
		}

		public static double GroundResolution(double latitude, int levelOfDetail)
		{
			latitude = VirtualEarthTileSystem.Clip(latitude, -85.05112878, 85.05112878);
			return Math.Cos(latitude * 3.1415926535897931 / 180.0) * 2.0 * 3.1415926535897931 * 6378137.0 / (double)VirtualEarthTileSystem.MapSize(levelOfDetail);
		}

		public static double MapScale(double latitude, int levelOfDetail, int screenDpi)
		{
			return VirtualEarthTileSystem.GroundResolution(latitude, levelOfDetail) * (double)screenDpi / 0.0254;
		}

		public static void LongLatToPixelXY(double longitude, double latitude, int levelOfDetail, out int pixelX, out int pixelY)
		{
			latitude = VirtualEarthTileSystem.Clip(latitude, -85.05112878, 85.05112878);
			longitude = VirtualEarthTileSystem.Clip(longitude, -180.0, 180.0);
			double num = (longitude + 180.0) / 360.0;
			double num2 = Math.Sin(latitude * 3.1415926535897931 / 180.0);
			double num3 = 0.5 - Math.Log((1.0 + num2) / (1.0 - num2)) / 12.566370614359172;
			uint num4 = VirtualEarthTileSystem.MapSize(levelOfDetail);
			pixelX = (int)VirtualEarthTileSystem.Clip(num * (double)num4 + 0.5, 0.0, (double)(num4 - 1));
			pixelY = (int)VirtualEarthTileSystem.Clip(num3 * (double)num4 + 0.5, 0.0, (double)(num4 - 1));
		}

		public static void LongLatToPixelXY(double longitude, double latitude, double levelOfDetail, out double pixelX, out double pixelY)
		{
			latitude = VirtualEarthTileSystem.Clip(latitude, -85.05112878, 85.05112878);
			longitude = VirtualEarthTileSystem.Clip(longitude, -180.0, 180.0);
			double num = (longitude + 180.0) / 360.0;
			double num2 = Math.Sin(latitude * 3.1415926535897931 / 180.0);
			double num3 = 0.5 - Math.Log((1.0 + num2) / (1.0 - num2)) / 12.566370614359172;
			uint num4 = VirtualEarthTileSystem.MapSize(levelOfDetail);
			pixelX = VirtualEarthTileSystem.Clip(num * (double)num4 + 0.5, 0.0, (double)(num4 - 1));
			pixelY = VirtualEarthTileSystem.Clip(num3 * (double)num4 + 0.5, 0.0, (double)(num4 - 1));
		}

		public static void PixelXYToTileXY(int pixelX, int pixelY, out int tileX, out int tileY)
		{
			tileX = pixelX / 256;
			tileY = pixelY / 256;
		}

		public static string TileXYToQuadKey(int tileX, int tileY, int levelOfDetail)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int num = levelOfDetail; num > 0; num--)
			{
				char c = '0';
				int num2 = 1 << num - 1;
				if ((tileX & num2) != 0)
				{
					c = (char)(c + 1);
				}
				if ((tileY & num2) != 0)
				{
					c = (char)(c + 1);
					c = (char)(c + 1);
				}
				stringBuilder.Append(c);
			}
			return stringBuilder.ToString();
		}

		public static double LevelOfDetail(double groundResolution)
		{
			return Math.Log(156543.03392804097 / groundResolution, 2.0);
		}

		internal static ImageryMetadataRequest.ImageryType TileSystemToMapStyle(TileSystem tyleSystem)
		{
			switch (tyleSystem)
			{
			case TileSystem.VirtualEarthAerial:
				return ImageryMetadataRequest.ImageryType.Aerial;
			case TileSystem.VirtualEarthHybrid:
				return ImageryMetadataRequest.ImageryType.AerialWithLabels;
			case TileSystem.VirtualEarthRoad:
				return ImageryMetadataRequest.ImageryType.Road;
			default:
				throw new NotSupportedException();
			}
		}
	}
}
