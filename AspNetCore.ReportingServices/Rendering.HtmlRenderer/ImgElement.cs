using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal sealed class ImgElement : HtmlElement
	{
		public string AltText
		{
			get;
			set;
		}

		public SizeAttribute Size
		{
			get;
			set;
		}

		public string Image
		{
			get;
			set;
		}

		public float? Opacity
		{
			get;
			set;
		}

		public Dictionary<string, string> CustomAttributes
		{
			get;
			set;
		}

		public void Render(IOutputStream outputStream)
		{
			outputStream.Write(HTMLElements.m_img);
			string text = string.Empty;
			if (this.Size != null && this.Size.Width != null && this.Size.Width.GetType() == typeof(AutoScaleTo100Percent))
			{
				text = HTMLElements.m_resize100WidthClassName;
			}
			if (this.Size != null && this.Size.Height != null && this.Size.Height.GetType() == typeof(AutoScaleTo100Percent))
			{
				text = text + HTMLElements.m_spaceString + HTMLElements.m_resize100HeightClassName;
			}
			if (!string.IsNullOrEmpty(text))
			{
				outputStream.Write(HTMLElements.m_classStyle);
				outputStream.Write(text);
				outputStream.Write(HTMLElements.m_quoteString);
			}
			if (!string.IsNullOrEmpty(this.AltText))
			{
				outputStream.Write(HTMLElements.m_alt);
				outputStream.Write(this.AltText);
				outputStream.Write(HTMLElements.m_quoteString);
				outputStream.Write(HTMLElements.m_title);
				outputStream.Write(this.AltText);
				outputStream.Write(HTMLElements.m_quoteString);
			}
			outputStream.Write(HTMLElements.m_openStyle);
			if (this.Size != null)
			{
				this.Size.Render(outputStream);
			}
			if (this.Opacity.HasValue)
			{
				outputStream.Write(HTMLElements.m_space);
				outputStream.Write(HTMLElements.m_opacity);
				outputStream.Write(this.Opacity.ToString());
			}
			outputStream.Write(HTMLElements.m_quoteString);
			if (this.CustomAttributes != null)
			{
				foreach (KeyValuePair<string, string> customAttribute in this.CustomAttributes)
				{
					outputStream.Write(HTMLElements.m_space);
					outputStream.Write(customAttribute.Key);
					outputStream.Write(HTMLElements.m_equal);
					outputStream.Write(HTMLElements.m_quoteString);
					outputStream.Write(customAttribute.Value);
					outputStream.Write(HTMLElements.m_quoteString);
				}
			}
			outputStream.Write(HTMLElements.m_src);
			if (this.Image != null)
			{
				outputStream.Write(this.Image);
			}
			outputStream.Write(HTMLElements.m_quoteString);
			outputStream.Write(HTMLElements.m_space);
			outputStream.Write(HTMLElements.m_closeSingleTag);
		}
	}
}
