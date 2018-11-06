namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal abstract class RPLElementProps
	{
		protected RPLElementPropsDef m_definition;

		protected string m_uniqueName;

		protected RPLStyleProps m_nonSharedStyle;

		public virtual RPLElementPropsDef Definition
		{
			get
			{
				return this.m_definition;
			}
			set
			{
				this.m_definition = value;
			}
		}

		public RPLElementStyle Style
		{
			get
			{
				return new RPLElementStyle(this.m_nonSharedStyle, this.m_definition.SharedStyle);
			}
		}

		public string UniqueName
		{
			get
			{
				return this.m_uniqueName;
			}
			set
			{
				this.m_uniqueName = value;
			}
		}

		public RPLStyleProps NonSharedStyle
		{
			get
			{
				return this.m_nonSharedStyle;
			}
			set
			{
				this.m_nonSharedStyle = value;
			}
		}
	}
}
