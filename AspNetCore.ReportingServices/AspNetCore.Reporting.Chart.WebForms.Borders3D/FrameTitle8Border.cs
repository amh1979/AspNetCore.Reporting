using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms.Borders3D
{
	internal class FrameTitle8Border : FrameTitle1Border
	{
		public override string Name
		{
			get
			{
				return "FrameTitle8";
			}
		}

		public override float Resolution
		{
			set
			{
				base.Resolution = value;
				base.sizeLeftTop = new SizeF(0f, base.sizeLeftTop.Height);
				base.sizeRightBottom = new SizeF(0f, base.sizeRightBottom.Height);
				float num = (float)(1.0 * base.resolution / 96.0);
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

		public FrameTitle8Border()
		{
			base.sizeLeftTop = new SizeF(0f, base.sizeLeftTop.Height);
			base.sizeRightBottom = new SizeF(0f, base.sizeRightBottom.Height);
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
