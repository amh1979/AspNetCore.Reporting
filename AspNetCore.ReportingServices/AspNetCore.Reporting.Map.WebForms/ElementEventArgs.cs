namespace AspNetCore.Reporting.Map.WebForms
{
	internal class ElementEventArgs
	{
		private MapControl control;

		private NamedElement mapElement;

		internal MapControl MapControl
		{
			get
			{
				return this.control;
			}
		}

		public NamedElement MapElement
		{
			get
			{
				return this.mapElement;
			}
		}

		internal ElementEventArgs(MapControl control, NamedElement mapElement)
		{
			this.control = control;
			this.mapElement = mapElement;
		}
	}
}
