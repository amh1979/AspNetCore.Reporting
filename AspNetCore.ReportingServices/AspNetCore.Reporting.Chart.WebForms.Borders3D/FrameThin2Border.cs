namespace AspNetCore.Reporting.Chart.WebForms.Borders3D
{
	internal class FrameThin2Border : FrameThin1Border
	{
		public override string Name
		{
			get
			{
				return "FrameThin2";
			}
		}

		public override float Resolution
		{
			set
			{
				base.Resolution = value;
				float num = (float)(15.0 * base.resolution / 96.0);
				float num2 = (float)(1.0 * base.resolution / 96.0);
				base.innerCorners = (base.cornerRadius = new float[8]
				{
					num,
					num,
					num,
					num2,
					num2,
					num2,
					num2,
					num
				});
			}
		}

		public FrameThin2Border()
		{
			base.innerCorners = (base.cornerRadius = new float[8]
			{
				15f,
				15f,
				15f,
				1f,
				1f,
				1f,
				1f,
				15f
			});
		}
	}
}
