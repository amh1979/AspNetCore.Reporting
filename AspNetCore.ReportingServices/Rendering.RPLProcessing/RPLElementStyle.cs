namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLElementStyle : IRPLStyle
	{
		private RPLStyleProps m_sharedProperties;

		private RPLStyleProps m_nonSharedProperties;

		public object this[byte styleName]
		{
			get
			{
				object obj = null;
				if (this.m_nonSharedProperties != null)
				{
					obj = this.m_nonSharedProperties[styleName];
				}
				if (obj == null && this.m_sharedProperties != null)
				{
					obj = this.m_sharedProperties[styleName];
				}
				return obj;
			}
		}

		public RPLStyleProps SharedProperties
		{
			get
			{
				return this.m_sharedProperties;
			}
		}

		public RPLStyleProps NonSharedProperties
		{
			get
			{
				return this.m_nonSharedProperties;
			}
		}

		public RPLElementStyle(RPLStyleProps nonSharedProps, RPLStyleProps sharedProps)
		{
			this.m_nonSharedProperties = nonSharedProps;
			this.m_sharedProperties = sharedProps;
		}
	}
}
