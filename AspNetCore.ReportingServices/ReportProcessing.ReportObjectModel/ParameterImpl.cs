namespace AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel
{
	public sealed class ParameterImpl : Parameter
	{
		private object[] m_value;

		private string[] m_labels;

		private bool m_isMultiValue;

		public override object Value
		{
			get
			{
				if (this.m_value == null)
				{
					return null;
				}
				if (!this.m_isMultiValue)
				{
					return this.m_value[0];
				}
				return this.m_value;
			}
		}

		public override object Label
		{
			get
			{
				if (this.m_labels != null && this.m_labels.Length != 0)
				{
					if (!this.m_isMultiValue)
					{
						return this.m_labels[0];
					}
					return this.m_labels;
				}
				return null;
			}
		}

		public override int Count
		{
			get
			{
				if (this.m_value == null)
				{
					return 0;
				}
				return this.m_value.Length;
			}
		}

		public override bool IsMultiValue
		{
			get
			{
				return this.m_isMultiValue;
			}
		}

		internal ParameterImpl(object[] value, string[] labels, bool isMultiValue)
		{
			this.m_value = value;
			this.m_labels = labels;
			this.m_isMultiValue = isMultiValue;
		}
	}
}
