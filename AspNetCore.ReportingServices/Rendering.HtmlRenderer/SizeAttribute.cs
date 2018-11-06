namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal sealed class SizeAttribute
	{
		public ISize Width
		{
			get;
			set;
		}

		public ISize MinWidth
		{
			get;
			set;
		}

		public ISize MaxWidth
		{
			get;
			set;
		}

		public ISize Height
		{
			get;
			set;
		}

		public ISize MinHeight
		{
			get;
			set;
		}

		public ISize MaxHeight
		{
			get;
			set;
		}

		public void Render(IOutputStream outputStream)
		{
			if (this.MinWidth != null)
			{
				outputStream.Write(HTMLElements.m_space);
				outputStream.Write(HTMLElements.m_styleMinWidth);
				outputStream.Write(HTMLElements.m_space);
				this.MinWidth.Render(outputStream);
				outputStream.Write(HTMLElements.m_semiColon);
			}
			if (this.MinHeight != null)
			{
				outputStream.Write(HTMLElements.m_space);
				outputStream.Write(HTMLElements.m_styleMinHeight);
				outputStream.Write(HTMLElements.m_space);
				this.MinHeight.Render(outputStream);
				outputStream.Write(HTMLElements.m_semiColon);
			}
			if (this.MaxHeight != null)
			{
				outputStream.Write(HTMLElements.m_space);
				outputStream.Write(HTMLElements.m_styleMaxHeight);
				outputStream.Write(HTMLElements.m_space);
				this.MaxHeight.Render(outputStream);
				outputStream.Write(HTMLElements.m_semiColon);
			}
			if (this.MaxWidth != null)
			{
				outputStream.Write(HTMLElements.m_space);
				outputStream.Write(HTMLElements.m_styleMaxWidth);
				outputStream.Write(HTMLElements.m_space);
				this.MaxWidth.Render(outputStream);
				outputStream.Write(HTMLElements.m_semiColon);
			}
			if (this.Width != null)
			{
				outputStream.Write(HTMLElements.m_space);
				outputStream.Write(HTMLElements.m_styleWidth);
				outputStream.Write(HTMLElements.m_space);
				this.Width.Render(outputStream);
				outputStream.Write(HTMLElements.m_semiColon);
			}
			if (this.Height != null)
			{
				outputStream.Write(HTMLElements.m_space);
				outputStream.Write(HTMLElements.m_styleHeight);
				outputStream.Write(HTMLElements.m_space);
				this.Height.Render(outputStream);
				outputStream.Write(HTMLElements.m_semiColon);
			}
		}
	}
}
