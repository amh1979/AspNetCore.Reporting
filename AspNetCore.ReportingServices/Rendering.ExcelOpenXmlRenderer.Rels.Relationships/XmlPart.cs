using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Rels.Relationships
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
