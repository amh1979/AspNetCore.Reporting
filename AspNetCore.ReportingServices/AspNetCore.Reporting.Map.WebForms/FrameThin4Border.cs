namespace AspNetCore.Reporting.Map.WebForms
{
	internal class FrameThin4Border : FrameThin1Border
	{
		public override string Name
		{
			get
			{
				return "FrameThin4";
			}
		}

		public FrameThin4Border()
		{
			float[] array = base.cornerRadius = new float[8]
			{
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f
			};
		}
	}
}
