namespace AspNetCore.Reporting.Map.WebForms
{
	internal class MapPaintEventArgs
	{
		private MapControl control;

		private NamedElement mapElement;

		private MapGraphics graphics;

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

		public MapGraphics Graphics
		{
			get
			{
				return this.graphics;
			}
		}

		internal MapPaintEventArgs(MapControl control, NamedElement mapElement, MapGraphics graphics)
		{
			this.control = control;
			this.mapElement = mapElement;
			this.graphics = graphics;
		}
	}
}
