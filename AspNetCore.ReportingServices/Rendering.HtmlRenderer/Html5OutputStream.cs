namespace AspNetCore.ReportingServices.Rendering.HtmlRenderer
{
	internal class Html5OutputStream : IOutputStream
	{
		private HTML5Renderer Renderer
		{
			get;
			set;
		}

		public Html5OutputStream(HTML5Renderer renderer)
		{
			this.Renderer = renderer;
		}

		public void Write(string text)
		{
			this.Renderer.WriteStream(text);
		}

		public void Write(byte[] text)
		{
			this.Renderer.WriteStream(text);
		}
	}
}
