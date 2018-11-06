using AspNetCore.ReportingServices.ReportProcessing;
using System.IO;
using System.Xml;

namespace AspNetCore.ReportingServices.RdlObjectModel2008.Upgrade
{
	internal class Upgrader
	{
		internal static void Upgrade(string inputFile, string outputFile, bool throwUpgradeException)
		{
			using (FileStream inStream = File.OpenRead(inputFile))
			{
				using (FileStream outStream = File.Open(outputFile, FileMode.Create, FileAccess.ReadWrite))
				{
					Upgrader.Upgrade(inStream, outStream, throwUpgradeException);
				}
			}
		}

		internal static void Upgrade(Stream inStream, Stream outStream, bool throwUpgradeException)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(inStream);
			xmlTextReader.ProhibitDtd = true;
			Upgrader.Upgrade(xmlTextReader, outStream, throwUpgradeException);
		}

		internal static void Upgrade(XmlReader xmlReader, Stream outStream, bool throwUpgradeException)
		{
			UpgradeImpl2008 upgradeImpl = new UpgradeImpl2008();
			upgradeImpl.Upgrade(xmlReader, outStream);
		}

		internal static Stream UpgradeToCurrent(Stream inStream, bool throwUpgradeException)
		{
			return RDLUpgrader.UpgradeToCurrent(inStream, throwUpgradeException, true);
		}
	}
}
