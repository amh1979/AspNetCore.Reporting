namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLImage : RPLItem
	{
		internal RPLImage()
		{
			base.m_rplElementProps = new RPLImageProps();
			base.m_rplElementProps.Definition = new RPLImagePropsDef();
		}

		internal RPLImage(long startOffset, RPLContext context)
			: base(startOffset, context)
		{
		}
	}
}
