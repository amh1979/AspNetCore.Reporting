namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class EscherBoolProperty : EscherSimpleProperty
	{
		internal virtual bool True
		{
			get
			{
				return base.m_propertyValue != 0;
			}
		}

		internal virtual bool False
		{
			get
			{
				return base.m_propertyValue == 0;
			}
		}

		internal EscherBoolProperty(ushort propertyNumber, int value_Renamed)
			: base(propertyNumber, false, false, value_Renamed)
		{
		}
	}
}
