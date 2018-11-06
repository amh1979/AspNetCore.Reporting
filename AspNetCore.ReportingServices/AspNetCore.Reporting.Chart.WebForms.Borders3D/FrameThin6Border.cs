namespace AspNetCore.Reporting.Chart.WebForms.Borders3D
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

		public override float Resolution
		{
			set
			{
				base.Resolution = value;
				float num = (float)(base.resolution / 96.0);
				float[] array = base.innerCorners = new float[8]
				{
					num,
					num,
					num,
					num,
					num,
					num,
					num,
					num
				};
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
