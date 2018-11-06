using System;
using System.Collections.Generic;
using System.Xml;

namespace AspNetCore.ReportingServices.ProgressivePackaging
{
	internal class ImageRequestMessageReader : ImageMessageReader<ImageRequestMessageElement>
	{
		private XmlReader m_xmlReader;

		public ImageRequestMessageReader(XmlReader xmlReader)
		{
			this.m_xmlReader = xmlReader;
		}

		public override IEnumerator<ImageRequestMessageElement> GetEnumerator()
		{
			while (this.m_xmlReader.Read())
			{
				if (this.m_xmlReader.NodeType == XmlNodeType.Element && "ExternalImage".Equals(this.m_xmlReader.Name, StringComparison.Ordinal))
				{
					using (XmlReader externalImageReader = this.m_xmlReader.ReadSubtree())
					{
						ImageRequestMessageElement element = new ImageRequestMessageElement();
						element.Read(externalImageReader);
						yield return element;
					}
				}
			}
		}

		public override void InternalDispose()
		{
			this.m_xmlReader.Close();
		}
	}
}
