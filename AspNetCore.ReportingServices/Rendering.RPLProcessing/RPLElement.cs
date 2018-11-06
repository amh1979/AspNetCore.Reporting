namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal abstract class RPLElement
	{
		internal RPLContext m_context;

		protected RPLElementProps m_rplElementProps;

		public virtual RPLElementProps ElementProps
		{
			get
			{
				return this.m_rplElementProps;
			}
			set
			{
				this.m_rplElementProps = value;
			}
		}

		public virtual RPLElementPropsDef ElementPropsDef
		{
			get
			{
				if (this.m_rplElementProps != null)
				{
					return this.m_rplElementProps.Definition;
				}
				return null;
			}
		}

		protected RPLElement()
		{
		}

		internal RPLElement(RPLContext context)
		{
			this.m_context = context;
		}

		protected RPLElement(RPLElementProps rplElementProps)
		{
			this.m_rplElementProps = rplElementProps;
		}
	}
}
