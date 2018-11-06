namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportEnumProperty<EnumType> : ReportProperty where EnumType : struct
	{
		private EnumType m_value;

		public EnumType Value
		{
			get
			{
				return this.m_value;
			}
		}

		internal ReportEnumProperty()
		{
			this.m_value = default(EnumType);
		}

		internal ReportEnumProperty(EnumType value)
		{
			this.m_value = value;
		}

		internal ReportEnumProperty(bool isExpression, string expressionString, EnumType value)
			: this(isExpression, expressionString, value, default(EnumType))
		{
		}

		internal ReportEnumProperty(bool isExpression, string expressionString, EnumType value, EnumType defaultValue)
			: base(isExpression, expressionString)
		{
			if (!isExpression)
			{
				this.m_value = value;
			}
			else
			{
				this.m_value = defaultValue;
			}
		}
	}
}
