namespace AspNetCore.Reporting.Chart.WebForms.Borders3D
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

		public override float Resolution
		{
			set
			{
				base.Resolution = value;
				float num = (float)(1.0 * base.resolution / 96.0);
				base.cornerRadius = new float[8]
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
