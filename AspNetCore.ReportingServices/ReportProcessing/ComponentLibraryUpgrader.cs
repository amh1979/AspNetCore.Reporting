using System;
using System.IO;
using System.Text;
using System.Xml;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class ComponentLibraryUpgrader
	{
		private class ReportPartsUpgrader
		{
			private const string NamespaceURI201001 = "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition";

			private const string NamespaceURI201601 = "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition";

			private bool m_needsUpgrade;

			private string m_oldNamespace = string.Empty;

			private string m_oldNsPrefix = string.Empty;

			private XmlDocument m_document;

			private string[] m_knowNamespaces = new string[2]
			{
				"http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition",
				"http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition"
			};

			private bool NeedsUpgrade
			{
				get
				{
					return this.m_needsUpgrade;
				}
			}

			private string OldRdlNamespace
			{
				get
				{
					return this.m_oldNamespace;
				}
			}

			private string OldRdlNSPrefix
			{
				get
				{
					return this.m_oldNsPrefix;
				}
			}

			internal Stream UpgradeToCurrent(Stream stream, ref Stream outStream)
			{
				XmlDocument xmlDocument = this.LoadDefinition(stream);
				XmlElement documentElement = xmlDocument.DocumentElement;
				this.ScanRdlNamespace(documentElement);
				if (this.NeedsUpgrade)
				{
					switch (this.OldRdlNamespace)
					{
					case "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition":
						this.UpdateNamespaceURI("http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition", "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition");
						break;
					default:
						throw new RDLUpgradeException(RDLUpgradeStrings.rdlInvalidTargetNamespace(this.OldRdlNamespace));
					case "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition":
						break;
					}
				}
				return this.SaveDefinition(ref outStream);
			}

			private void ScanRdlNamespace(XmlElement root)
			{
				string empty = string.Empty;
				string[] knowNamespaces = this.m_knowNamespaces;
				int num = 0;
				string text;
				while (true)
				{
					if (num < knowNamespaces.Length)
					{
						text = knowNamespaces[num];
						empty = root.GetPrefixOfNamespace(text);
						if (string.IsNullOrEmpty(empty))
						{
							num++;
							continue;
						}
						break;
					}
					return;
				}
				this.m_oldNamespace = text;
				this.m_needsUpgrade = true;
				this.m_oldNsPrefix = empty;
			}

			private XmlDocument LoadDefinition(Stream stream)
			{
				this.m_document = new XmlDocument();
				this.m_document.PreserveWhitespace = true;
				this.m_document.Load(stream);
				return this.m_document;
			}

			private Stream SaveDefinition(ref Stream outStream)
			{
				this.m_document.Save(outStream);
				this.m_document = null;
				outStream.Seek(0L, SeekOrigin.Begin);
				return outStream;
			}

			private void UpdateNamespaceURI(string oldNamespaceURI, string newNamespaceURI)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(this.m_document.OuterXml);
				stringBuilder.Replace(oldNamespaceURI, newNamespaceURI);
				this.m_document.LoadXml(stringBuilder.ToString());
			}
		}

		internal static Stream UpgradeToCurrent(Stream stream, ref Stream outStream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			ReportPartsUpgrader reportPartsUpgrader = new ReportPartsUpgrader();
			return reportPartsUpgrader.UpgradeToCurrent(stream, ref outStream);
		}

		internal static byte[] UpgradeToCurrent(byte[] definition)
		{
			byte[] array = null;
			Stream stream = new MemoryStream(definition);
			Stream stream2 = new MemoryStream();
			ComponentLibraryUpgrader.UpgradeToCurrent(stream, ref stream2);
			array = new byte[stream2.Length];
			stream2.Position = 0L;
			stream2.Read(array, 0, (int)stream2.Length);
			stream2.Close();
			return array;
		}
	}
}
