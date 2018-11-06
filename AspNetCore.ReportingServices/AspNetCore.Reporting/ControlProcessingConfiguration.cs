using AspNetCore.ReportingServices.Diagnostics;

namespace AspNetCore.Reporting
{
	internal sealed class ControlProcessingConfiguration : BaseLocalProcessingConfiguration
	{
		private IMapTileServerConfiguration m_mapTileServerConfiguration;

		public override IMapTileServerConfiguration MapTileServerConfiguration
		{
			get
			{
				return this.m_mapTileServerConfiguration;
			}
		}

		public void SetMapTileServerConfiguration(IMapTileServerConfiguration serverConfig)
		{
			this.m_mapTileServerConfiguration = serverConfig;
		}
	}
}
