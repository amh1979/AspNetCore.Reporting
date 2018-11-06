using AspNetCore.ReportingServices.RdlObjectModel.Serialization;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.IO;
using System.Xml;

namespace AspNetCore.ReportingServices.RdlObjectModel.RdlUpgrade
{
	internal abstract class UpgraderBase
	{
		protected RDLUpgradeResult m_upgradeResults;

		internal RDLUpgradeResult UpgradeResults
		{
			get
			{
				return this.m_upgradeResults;
			}
		}

		internal UpgraderBase()
		{
		}

		internal abstract Type GetReportType();

		public void Upgrade(XmlReader xmlReader, Stream outStream)
		{
			this.InitUpgrade();
			RdlSerializerSettings settings = this.CreateReaderSettings();
			this.SetupReaderSettings(settings);
			RdlSerializer rdlSerializer = new RdlSerializer(settings);
			Report report = (Report)rdlSerializer.Deserialize(xmlReader, this.GetReportType());
			this.Upgrade(report);
			RdlSerializerSettings settings2 = this.CreateWriterSettings();
			RdlSerializer rdlSerializer2 = new RdlSerializer(settings2);
			rdlSerializer2.Serialize(outStream, report);
		}

		protected virtual void InitUpgrade()
		{
			this.m_upgradeResults = new RDLUpgradeResult();
		}

		protected virtual void Upgrade(Report report)
		{
		}

		protected abstract RdlSerializerSettings CreateReaderSettings();

		protected virtual void SetupReaderSettings(RdlSerializerSettings settings)
		{
		}

		protected abstract RdlSerializerSettings CreateWriterSettings();
	}
}
