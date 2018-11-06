using System;
using System.Drawing.Printing;

namespace AspNetCore.Reporting
{
	[Serializable]
	internal sealed class ReportInfo : IDisposable
	{
		[NonSerialized]
		public int CurrentPage;

		[NonSerialized]
		public string ScrollPosition;

		[NonSerialized]
		public PageSettings PageSettings;

		private static int PAGESETTINGS_COUNT = 3;

		private LocalModeSession m_localSession;

		public LocalModeSession LocalSession
		{
			get
			{
				return this.m_localSession;
			}
		}



		public InternalLocalReport LocalReport
		{
			get
			{
				return (InternalLocalReport)this.m_localSession.Report;
			}
		}



		public ReportInfo(LocalModeSession localSession)
		{
			this.m_localSession = localSession;
		}

		public void Dispose()
		{
			this.m_localSession.Dispose();
			
		}

		public void DisposeNonSessionResources()
		{
			ReportInfo.DisposeNonSessionResources(this.m_localSession);
		}

		public static void DisposeNonSessionResources(LocalModeSession localSession)
		{
			if (localSession != null)
			{
				((InternalLocalReport)localSession.Report).ReleaseSandboxAppDomain();
			}
		}

		public void LoadViewState(object viewStateObj)
		{
			object[] array = (object[])viewStateObj;
			this.CurrentPage = (int)array[0];
			if (array[1] != null)
			{
				//this.ServerReport.LoadViewState(array[1]);
			}
			this.DeserializePageSettings(array[2]);
			this.ScrollPosition = (string)array[3];
		}

		public object SaveViewState(bool includeReport)
		{
			object[] array = new object[4]
			{
				this.CurrentPage,
				null,
				null,
				null
			};
			if (includeReport)
			{
				//array[1] = this.ServerReport.SaveViewState();
			}
			array[2] = this.SerializePageSettings();
			array[3] = this.ScrollPosition;
			return array;
		}

		public void ConnectChangeEvent(EventHandler<ReportChangedEventArgs> changeHandler, InitializeDataSourcesEventHandler dataInitializationHandler)
		{
			//this.ServerReport.Change += changeHandler;
			this.LocalReport.Change += changeHandler;
			this.LocalReport.InitializeDataSources += dataInitializationHandler;
		}

		public void DisconnectChangeEvent(EventHandler<ReportChangedEventArgs> changeHandler, InitializeDataSourcesEventHandler dataInitializationHandler, bool disconnectUserEvents)
		{
			//this.ServerReport.Change -= changeHandler;
			this.LocalReport.Change -= changeHandler;
			this.LocalReport.InitializeDataSources -= dataInitializationHandler;
			if (disconnectUserEvents)
			{
				this.LocalReport.TransferEvents(null);
			}
		}

		private object SerializePageSettings()
		{
			if (this.PageSettings == null)
			{
				return null;
			}
			object[] array = new object[ReportInfo.PAGESETTINGS_COUNT];
			array[0] = this.PageSettings.Margins;
			array[1] = this.PageSettings.PaperSize;
			array[2] = this.PageSettings.Landscape;
			return array;
		}

		private void DeserializePageSettings(object pageSettings)
		{
			object[] array = pageSettings as object[];
			if (array == null || array.Length != ReportInfo.PAGESETTINGS_COUNT)
			{
				this.PageSettings = null;
			}
			else
			{
				this.PageSettings = new PageSettings();
				this.PageSettings.Margins = (array[0] as Margins);
				this.PageSettings.PaperSize = (array[1] as PaperSize);
				this.PageSettings.Landscape = (bool)array[2];
			}
		}
	}
}
