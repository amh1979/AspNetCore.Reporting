using System.IO;
using System.Xml;

namespace AspNetCore.ReportingServices.RdlObjectModel2005.Upgrade
{
	internal class Upgrader
	{
		public static void Upgrade(string inputFile, string outputFile, bool throwUpgradeException)
		{
			using (FileStream inStream = File.OpenRead(inputFile))
			{
				using (FileStream outStream = File.Open(outputFile, FileMode.Create, FileAccess.ReadWrite))
				{
					Upgrader.Upgrade(inStream, outStream, throwUpgradeException);
				}
			}
		}

		public static void Upgrade(Stream inStream, Stream outStream, bool throwUpgradeException)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(inStream);
			xmlTextReader.ProhibitDtd = true;
			Upgrader.Upgrade(xmlTextReader, outStream, throwUpgradeException);
		}

		public static void Upgrade(XmlReader xmlReader, Stream outStream, bool throwUpgradeException)
		{
			UpgradeImpl2005 upgradeImpl = new UpgradeImpl2005(throwUpgradeException);
			upgradeImpl.Upgrade(xmlReader, outStream);
		}
	}
}
