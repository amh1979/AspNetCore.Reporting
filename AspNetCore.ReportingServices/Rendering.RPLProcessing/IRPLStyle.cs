namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal interface IRPLStyle
	{
		object this[byte styleName]
		{
			get;
		}
	}
}
