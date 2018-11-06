using System;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.ProgressivePackaging
{
	internal class ImageResponseMessageReader : ImageMessageReader<ImageResponseMessageElement>
	{
		private readonly IMessageReader m_reader;

		private readonly IEnumerator<MessageElement> m_enumerator;

		private bool m_hasCurrentElement;

		private bool m_isEnumeratorEmpty;

		public ImageResponseMessageReader(IMessageReader reader)
		{
			this.m_reader = reader;
			this.m_enumerator = reader.GetEnumerator();
		}

		public bool PeekIsErrorResponse(out MessageElement error)
		{
			error = null;
			MessageElement messageElement = this.Peek();
			if (ProgressiveTypeDictionary.IsErrorMessageElement(messageElement))
			{
				error = messageElement;
				return true;
			}
			return false;
		}

		private MessageElement Peek()
		{
			if (!this.m_hasCurrentElement && !this.m_isEnumeratorEmpty)
			{
				this.m_hasCurrentElement = this.m_enumerator.MoveNext();
			}
			if (this.m_hasCurrentElement)
			{
				return this.m_enumerator.Current;
			}
			this.m_isEnumeratorEmpty = true;
			return null;
		}

		public override IEnumerator<ImageResponseMessageElement> GetEnumerator()
		{
			while (true)
			{
				MessageElement messageElement = this.Peek();
				this.m_hasCurrentElement = false;
				if (messageElement == null)
				{
					break;
				}
				ImageResponseMessageElement imageMessageElement = this.ReadImageResponseFromMessageElement(messageElement);
				yield return imageMessageElement;
			}
		}

		private ImageResponseMessageElement ReadImageResponseFromMessageElement(MessageElement messageElement)
		{
			Stream stream = messageElement.Value as Stream;
			if ("getExternalImagesResponse".Equals(messageElement.Name) && stream != null)
			{
				ImageResponseMessageElement imageResponseMessageElement = new ImageResponseMessageElement();
				using (BinaryReader reader = new BinaryReader(stream, MessageUtil.StringEncoding))
				{
					imageResponseMessageElement.Read(reader);
					return imageResponseMessageElement;
				}
			}
			throw new InvalidOperationException("MessageElement is not an image response message element.");
		}

		public override void InternalDispose()
		{
			this.m_reader.Dispose();
			this.m_enumerator.Dispose();
		}
	}
}
