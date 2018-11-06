namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLTextRun : RPLElement
	{
		private RPLSizes m_contentSizes;

		public RPLSizes ContentSizes
		{
			get
			{
				return this.m_contentSizes;
			}
			set
			{
				this.m_contentSizes = value;
			}
		}

		internal RPLTextRun()
		{
			base.m_rplElementProps = new RPLTextRunProps();
			base.m_rplElementProps.Definition = new RPLTextRunPropsDef();
		}

		internal RPLTextRun(RPLTextRunProps rplElementProps)
			: base(rplElementProps)
		{
		}
	}
}
