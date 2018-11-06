using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms.Borders3D
{
	internal class FrameTitle7Border : FrameTitle1Border
	{
		public override string Name
		{
			get
			{
				return "FrameTitle7";
			}
		}

		public override float Resolution
		{
			set
			{
				base.Resolution = value;
				base.sizeRightBottom = new SizeF(0f, base.sizeRightBottom.Height);
				float num = (float)(15.0 * base.resolution / 96.0);
				float num2 = (float)(1.0 * base.resolution / 96.0);
				float[] array = base.innerCorners = new float[8]
				{
					num,
					num2,
					num2,
					num2,
					num2,
					num,
					num,
					num
				};
			}
		}

		public FrameTitle7Border()
		{
			base.sizeRightBottom = new SizeF(0f, base.sizeRightBottom.Height);
			float[] array = base.innerCorners = new float[8]
			{
				15f,
				1f,
				1f,
				1f,
				1f,
				15f,
				15f,
				15f
			};
		}
	}
}
