namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal sealed class DivElement : HtmlElement
	{
		public SizeAttribute Size
		{
			get;
			set;
		}

		public string BackgroundImage
		{
			get;
			set;
		}

		public string BackgroundImageSize
		{
			get;
			set;
		}

		public string Role
		{
			get;
			set;
		}

		public string AriaLabel
		{
			get;
			set;
		}

		public IBackgroundRepeatAttribute BackgroundRepeat
		{
			get;
			set;
		}

		public HtmlElement ChildElement
		{
			get;
			set;
		}

		public string Overflow
		{
			get;
			set;
		}

		public void Render(IOutputStream outputStream)
		{
			outputStream.Write(HTMLElements.m_openDiv);
			if (!string.IsNullOrEmpty(this.Role))
			{
				outputStream.Write(HTMLElements.m_space);
				outputStream.Write(HTMLElements.m_role);
				outputStream.Write(HTMLElements.m_equal);
				outputStream.Write(HTMLElements.m_quote);
				outputStream.Write(this.Role);
				outputStream.Write(HTMLElements.m_quote);
			}
			if (!string.IsNullOrEmpty(this.AriaLabel))
			{
				outputStream.Write(HTMLElements.m_space);
				outputStream.Write(HTMLElements.m_ariaLabel);
				outputStream.Write(HTMLElements.m_equal);
				outputStream.Write(HTMLElements.m_quote);
				outputStream.Write(this.AriaLabel);
				outputStream.Write(HTMLElements.m_quote);
			}
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
			outputStream.Write(HTMLElements.m_openStyle);
			if (this.Size != null)
			{
				this.Size.Render(outputStream);
			}
			if (this.BackgroundImage != null)
			{
				outputStream.Write(HTMLElements.m_space);
				outputStream.Write(HTMLElements.m_backgroundImage);
				outputStream.Write(this.BackgroundImage);
				outputStream.Write(HTMLElements.m_closeParenthesis);
				outputStream.Write(HTMLElements.m_semiColon);
			}
			if (this.BackgroundImageSize != null)
			{
				outputStream.Write(HTMLElements.m_space);
				outputStream.Write(HTMLElements.m_backgroundSize);
				outputStream.Write(this.BackgroundImageSize);
				outputStream.Write(HTMLElements.m_semiColon);
			}
			if (this.BackgroundRepeat != null)
			{
				outputStream.Write(HTMLElements.m_space);
				outputStream.Write(HTMLElements.m_backgroundRepeat);
				this.BackgroundRepeat.Render(outputStream);
				outputStream.Write(HTMLElements.m_semiColon);
			}
			if (this.Overflow != null)
			{
				outputStream.Write(HTMLElements.m_overflow);
				outputStream.Write(this.Overflow);
				outputStream.Write(HTMLElements.m_semiColon);
			}
			outputStream.Write(HTMLElements.m_quoteString);
			outputStream.Write(HTMLElements.m_closeBracket);
			if (this.ChildElement != null)
			{
				this.ChildElement.Render(outputStream);
			}
			outputStream.Write(HTMLElements.m_closeDiv);
		}
	}
}
