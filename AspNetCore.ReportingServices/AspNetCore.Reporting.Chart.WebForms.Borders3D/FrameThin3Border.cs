namespace AspNetCore.Reporting.Chart.WebForms.Borders3D
{
	internal class FrameThin3Border : FrameThin1Border
	{
		public override string Name
		{
			get
			{
				return "FrameThin3";
			}
		}

		public override float Resolution
		{
			set
			{
				base.Resolution = value;
				float num = (float)(base.resolution / 96.0);
				base.innerCorners = (base.cornerRadius = new float[8]
				{
					num,
					num,
					num,
					num,
					num,
					num,
					num,
					num
				});
			}
		}

		public FrameThin3Border()
		{
			base.innerCorners = (base.cornerRadius = new float[8]
			{
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f
			});
		}
	}
}
