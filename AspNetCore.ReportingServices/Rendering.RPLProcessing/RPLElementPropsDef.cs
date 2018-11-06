namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal abstract class RPLElementPropsDef
	{
		protected string m_id;

		protected RPLStyleProps m_sharedStyle;

		public string ID
		{
			get
			{
				return this.m_id;
			}
			set
			{
				this.m_id = value;
			}
		}

		public RPLStyleProps SharedStyle
		{
			get
			{
				return this.m_sharedStyle;
			}
			set
			{
				this.m_sharedStyle = value;
			}
		}
	}
}
