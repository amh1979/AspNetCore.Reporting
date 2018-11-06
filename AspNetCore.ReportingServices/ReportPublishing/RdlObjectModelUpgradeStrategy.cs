using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;
using System.IO;

namespace AspNetCore.ReportingServices.ReportPublishing
{
	internal sealed class RdlObjectModelUpgradeStrategy : ReportUpgradeStrategy
	{
		private readonly bool m_renameInvalidDataSources;

		private readonly bool m_throwUpgradeException;

		internal RdlObjectModelUpgradeStrategy(bool renameInvalidDataSources, bool throwUpgradeException)
		{
			this.m_renameInvalidDataSources = renameInvalidDataSources;
			this.m_throwUpgradeException = throwUpgradeException;
		}

		internal override Stream Upgrade(Stream definitionStream)
		{
			Stream stream = RDLUpgrader.UpgradeToCurrent(definitionStream, this.m_throwUpgradeException, this.m_renameInvalidDataSources);
			if (definitionStream != stream)
			{
				definitionStream.Close();
				definitionStream = null;
				if (Global.Tracer.TraceVerbose)
				{
					try
					{
						StreamReader streamReader = new StreamReader(stream);
						Global.Tracer.Trace(TraceLevel.Verbose, "Upgraded Report Definition\r\n");
						Global.Tracer.Trace(TraceLevel.Verbose, streamReader.ReadToEnd());
						Global.Tracer.Trace(TraceLevel.Verbose, "\r\n");
						stream.Seek(0L, SeekOrigin.Begin);
						return stream;
					}
					catch (Exception e)
					{
						if (AsynchronousExceptionDetection.IsStoppingException(e))
						{
							throw;
						}
						return stream;
					}
				}
			}
			return stream;
		}
	}
}
