namespace AspNetCore.Reporting
{
	internal sealed class WebConfigReader
	{
		private ConfigFilePropertyInterface<IReportServerConnection> m_serverConnection = new ConfigFilePropertyInterface<IReportServerConnection>("ReportViewerServerConnection", "IReportServerConnection");

		private ConfigFilePropertyInterface<ITemporaryStorage> m_tempStorage = new ConfigFilePropertyInterface<ITemporaryStorage>("ReportViewerTemporaryStorage", "ITemporaryStorage");

		private ConfigFilePropertyInterface<IReportViewerMessages> m_viewerMessages = new ConfigFilePropertyInterface<IReportViewerMessages>("ReportViewerMessages", "IReportViewerMessages");

		private static WebConfigReader m_theInstance;

		private static object m_lockObject = new object();

		public static WebConfigReader Current
		{
			get
			{
				lock (WebConfigReader.m_lockObject)
				{
					if (WebConfigReader.m_theInstance == null)
					{
						WebConfigReader.m_theInstance = new WebConfigReader();
					}
					return WebConfigReader.m_theInstance;
				}
			}
		}

		public IReportServerConnection ServerConnection
		{
			get
			{
				return this.m_serverConnection.GetInstance();
			}
		}

		public ITemporaryStorage TempStorage
		{
			get
			{
				return this.m_tempStorage.GetInstance();
			}
		}

		public IReportViewerMessages ViewerMessages
		{
			get
			{
				return this.m_viewerMessages.GetInstance();
			}
		}

		private WebConfigReader()
		{
		}
	}
}
