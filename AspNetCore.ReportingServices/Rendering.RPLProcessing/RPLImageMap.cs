namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLImageMap
	{
		private RPLFormat.ShapeType m_shape;

		private float[] m_coordinates;

		private string m_tooltip;

		public RPLFormat.ShapeType Shape
		{
			get
			{
				return this.m_shape;
			}
			set
			{
				this.m_shape = value;
			}
		}

		public float[] Coordinates
		{
			get
			{
				return this.m_coordinates;
			}
			set
			{
				this.m_coordinates = value;
			}
		}

		public string ToolTip
		{
			get
			{
				return this.m_tooltip;
			}
			set
			{
				this.m_tooltip = value;
			}
		}
	}
}
