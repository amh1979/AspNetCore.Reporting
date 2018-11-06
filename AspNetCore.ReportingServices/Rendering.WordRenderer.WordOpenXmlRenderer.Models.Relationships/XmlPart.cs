using AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models.Relationships
{
	internal class XmlPart : RelPart
	{
		private OoxmlPart _hydratedPart;

		public virtual OoxmlPart HydratedPart
		{
			get
			{
				return this._hydratedPart;
			}
			set
			{
				this._hydratedPart = value;
			}
		}
	}
}
