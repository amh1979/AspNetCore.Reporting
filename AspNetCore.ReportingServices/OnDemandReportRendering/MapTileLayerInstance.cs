using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.IO;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapTileLayerInstance : MapLayerInstance
	{
		private MapTileLayer m_defObject;

		private string m_serviceUrl;

		private MapTileStyle? m_tileStyle;

		private bool? m_useSecureConnection;

		public string ServiceUrl
		{
			get
			{
				if (this.m_serviceUrl == null)
				{
					this.m_serviceUrl = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapTileLayer)this.m_defObject.MapLayerDef).EvaluateServiceUrl(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_serviceUrl;
			}
		}

		public MapTileStyle TileStyle
		{
			get
			{
				if (!this.m_tileStyle.HasValue)
				{
					this.m_tileStyle = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapTileLayer)this.m_defObject.MapLayerDef).EvaluateTileStyle(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_tileStyle.Value;
			}
		}

		public bool UseSecureConnection
		{
			get
			{
				if (!this.m_useSecureConnection.HasValue)
				{
					this.m_useSecureConnection = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapTileLayer)this.m_defObject.MapLayerDef).EvaluateUseSecureConnection(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_useSecureConnection.Value;
			}
		}

		internal MapTileLayerInstance(MapTileLayer defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		public Stream GetTileData(string url, out string mimeType)
		{
			return this.m_defObject.MapTileLayerDef.GetTileData(url, out mimeType, this.m_defObject.MapDef.RenderingContext);
		}

		public void SetTileData(string url, byte[] data, string mimeType)
		{
			this.m_defObject.MapTileLayerDef.SetTileData(url, data, mimeType, this.m_defObject.MapDef.RenderingContext);
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_serviceUrl = null;
			this.m_tileStyle = null;
			this.m_useSecureConnection = null;
		}
	}
}
