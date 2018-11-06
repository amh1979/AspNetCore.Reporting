namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimDataValueInstance : DataValueInstance
	{
		private string m_name;

		private object m_value;

		public override string Name
		{
			get
			{
				return this.m_name;
			}
		}

		public override object Value
		{
			get
			{
				return this.m_value;
			}
		}

		internal ShimDataValueInstance(string name, object value)
			: base(null)
		{
			this.m_name = name;
			this.m_value = value;
		}

		internal void Update(string name, object value)
		{
			this.m_name = name;
			this.m_value = value;
		}

		protected override void ResetInstanceCache()
		{
		}
	}
}
