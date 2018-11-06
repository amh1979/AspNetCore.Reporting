namespace AspNetCore.Reporting.Map.WebForms
{
	internal class FrameThin6Border : FrameThin1Border
	{
		public override string Name
		{
			get
			{
				return "FrameThin6";
			}
		}

		public FrameThin6Border()
		{
			float[] array = base.innerCorners = new float[8]
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
