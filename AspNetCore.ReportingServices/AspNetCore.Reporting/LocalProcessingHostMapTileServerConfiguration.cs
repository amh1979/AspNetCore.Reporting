using AspNetCore.ReportingServices.Diagnostics;
using System;

namespace AspNetCore.Reporting
{
	[Serializable]
	internal sealed class LocalProcessingHostMapTileServerConfiguration : IMapTileServerConfiguration
	{
		private int m_maxConnections;

		private int m_timeout;

		private string m_appID;

		public int MaxConnections
		{
			get
			{
				return this.m_maxConnections;
			}
			set
			{
				if (!LocalProcessingHostMapTileServerConfiguration.IsInRange(value, 1, 2147483647))
				{
					throw new ArgumentOutOfRangeException("value", ProcessingStrings.MapTileServerConfiguration_MaxConnectionsOutOfRange(1, 2147483647));
				}
				this.m_maxConnections = value;
			}
		}

		public int Timeout
		{
			get
			{
				return this.m_timeout;
			}
			set
			{
				if (!LocalProcessingHostMapTileServerConfiguration.IsInRange(value, 1, 2147483647))
				{
					throw new ArgumentOutOfRangeException("value", ProcessingStrings.MapTileServerConfiguration_TimeoutOutOfRange(1, 2147483647));
				}
				this.m_timeout = value;
			}
		}

		public string AppID
		{
			get
			{
				return this.m_appID;
			}
			set
			{
				this.m_appID = value;
			}
		}

		public MapTileCacheLevel CacheLevel
		{
			get
			{
				return MapTileCacheLevel.Default;
			}
		}

		internal LocalProcessingHostMapTileServerConfiguration()
		{
			this.MaxConnections = 2;
			this.Timeout = 10;
			this.AppID = "(Default)";
		}

		private static bool IsInRange(int value, int min, int max)
		{
			if (min <= value)
			{
				return value <= max;
			}
			return false;
		}
	}
}
